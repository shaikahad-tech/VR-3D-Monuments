// =============================================================================
// TextureQualityManager.cs — Runtime Texture Resolution Management
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================
// Paper note: large models split into parts to maintain 4K/8K resolution.
// This manager handles runtime mipmap streaming and memory monitoring.
// =============================================================================

using UnityEngine;
using VirtualMuseumVR.Core;

namespace VirtualMuseumVR.Optimization
{
    public class TextureQualityManager : MonoBehaviour
    {
        [Header("Memory Budget")]
        [Tooltip("Maximum texture memory budget in MB (Quest 2 ≈ 3500MB total)")]
        [SerializeField] private int textureMemoryBudgetMB = 512;
        [SerializeField] private float memoryWarningThreshold = 0.85f;

        [Header("Mipmap Streaming")]
        [SerializeField] private bool enableMipmapStreaming = true;
        [SerializeField] private int maxMipmapLevel = 2; // 0=full, higher=lower quality

        [Header("Quality Levels")]
        [SerializeField] private int highQualityMaxSize = 4096;
        [SerializeField] private int mediumQualityMaxSize = 2048;
        [SerializeField] private int lowQualityMaxSize = 1024;

        private int _currentQualityLevel = 0; // 0=high, 1=medium, 2=low
        private float _lastMemoryCheck;
        private const float MEMORY_CHECK_INTERVAL = 5f;

        public int CurrentQualityLevel => _currentQualityLevel;
        public int CurrentMaxTextureSize => _currentQualityLevel switch
        {
            0 => highQualityMaxSize,
            1 => mediumQualityMaxSize,
            _ => lowQualityMaxSize
        };

        private void Start()
        {
            if (enableMipmapStreaming)
            {
                QualitySettings.streamingMipmapsActive = true;
                QualitySettings.streamingMipmapsMemoryBudget = textureMemoryBudgetMB;
            }

            ApplyQualityLevel(_currentQualityLevel);
        }

        private void OnEnable()
        {
            GameEvents.OnPerformanceWarning += HandlePerformanceWarning;
        }

        private void OnDisable()
        {
            GameEvents.OnPerformanceWarning -= HandlePerformanceWarning;
        }

        private void Update()
        {
            // Periodic memory check
            if (Time.time - _lastMemoryCheck > MEMORY_CHECK_INTERVAL)
            {
                _lastMemoryCheck = Time.time;
                CheckTextureMemory();
            }
        }

        private void CheckTextureMemory()
        {
            // Check if we're approaching the memory budget
            long usedMemory = Texture.currentTextureMemory;
            long budgetBytes = (long)textureMemoryBudgetMB * 1024 * 1024;
            float usage = (float)usedMemory / budgetBytes;

            if (usage > memoryWarningThreshold)
            {
                Debug.LogWarning($"[TextureQuality] Texture memory at {usage * 100:F1}% " +
                    $"({usedMemory / (1024 * 1024)}MB / {textureMemoryBudgetMB}MB)");

                // Auto-reduce quality if over budget
                if (_currentQualityLevel < 2)
                {
                    SetQualityLevel(_currentQualityLevel + 1);
                }
            }
        }

        /// <summary>Set texture quality level (0=high, 1=medium, 2=low).</summary>
        public void SetQualityLevel(int level)
        {
            _currentQualityLevel = Mathf.Clamp(level, 0, 2);
            ApplyQualityLevel(_currentQualityLevel);
            GameEvents.RaiseQualityLevelChanged(_currentQualityLevel);
        }

        private void ApplyQualityLevel(int level)
        {
            int maxSize = level switch
            {
                0 => highQualityMaxSize,
                1 => mediumQualityMaxSize,
                _ => lowQualityMaxSize
            };

            // Global mipmap bias based on quality
            QualitySettings.globalTextureMipmapLimit = level;

            Debug.Log($"[TextureQuality] Set to level {level} " +
                $"(max {maxSize}px, mipmap bias {level})");
        }

        private void HandlePerformanceWarning(float fps)
        {
            // If FPS is critically low and we're at high quality, reduce
            if (fps < 65f && _currentQualityLevel < 2)
            {
                SetQualityLevel(_currentQualityLevel + 1);
                Debug.Log($"[TextureQuality] Auto-reduced quality due to low FPS ({fps:F0})");
            }
        }

        /// <summary>Get a report of current texture memory usage.</summary>
        public string GetMemoryReport()
        {
            long used = Texture.currentTextureMemory;
            long desired = Texture.desiredTextureMemory;
            long total = Texture.totalTextureMemory;

            return $"Textures — Used: {used / (1024 * 1024)}MB, " +
                   $"Desired: {desired / (1024 * 1024)}MB, " +
                   $"Total: {total / (1024 * 1024)}MB, " +
                   $"Quality: {_currentQualityLevel}";
        }
    }
}
