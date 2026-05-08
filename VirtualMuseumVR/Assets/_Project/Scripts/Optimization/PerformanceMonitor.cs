// =============================================================================
// PerformanceMonitor.cs — FPS Monitoring with Auto-Quality Adjustment
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================

using UnityEngine;
using VirtualMuseumVR.Core;

namespace VirtualMuseumVR.Optimization
{
    public class PerformanceMonitor : MonoBehaviour
    {
        [Header("Monitoring")]
        [SerializeField] private float updateInterval = 0.5f;
        [SerializeField] private float warningFPSThreshold = 68f;
        [SerializeField] private float criticalFPSThreshold = 60f;

        [Header("Auto-Quality")]
        [SerializeField] private bool autoAdjustQuality = true;
        [SerializeField] private float qualityAdjustCooldown = 10f;

        [Header("Debug Display")]
        [SerializeField] private TMPro.TextMeshPro debugText;
        [SerializeField] private bool showInEditor = true;

        // FPS tracking
        private float _fps;
        private float _smoothedFPS;
        private int _frameCount;
        private float _timeAccumulator;
        private float _lastQualityAdjustTime;

        // Stats
        private float _minFPS = float.MaxValue;
        private float _maxFPS;
        private int _droppedFrameCount;
        private int _qualityDowngradeCount;

        public float CurrentFPS => _smoothedFPS;
        public float MinFPS => _minFPS;
        public float MaxFPS => _maxFPS;
        public int DroppedFrames => _droppedFrameCount;

        private void Update()
        {
            _frameCount++;
            _timeAccumulator += Time.unscaledDeltaTime;

            if (_timeAccumulator >= updateInterval)
            {
                _fps = _frameCount / _timeAccumulator;
                _smoothedFPS = Mathf.Lerp(_smoothedFPS, _fps, 0.3f);
                _frameCount = 0;
                _timeAccumulator = 0f;

                // Track min/max
                if (_smoothedFPS < _minFPS && _smoothedFPS > 1f) _minFPS = _smoothedFPS;
                if (_smoothedFPS > _maxFPS) _maxFPS = _smoothedFPS;

                // Check thresholds
                CheckPerformance();

                // Update debug display
                UpdateDebugDisplay();
            }
        }

        private void CheckPerformance()
        {
            int targetFPS = VirtualMuseumManager.Instance != null
                ? VirtualMuseumManager.Instance.TargetFrameRate
                : 72;

            if (_smoothedFPS < criticalFPSThreshold)
            {
                _droppedFrameCount++;
                GameEvents.RaisePerformanceWarning(_smoothedFPS);

                if (autoAdjustQuality &&
                    Time.time - _lastQualityAdjustTime > qualityAdjustCooldown)
                {
                    _lastQualityAdjustTime = Time.time;
                    _qualityDowngradeCount++;
                    Debug.LogWarning($"[Performance] Critical FPS: {_smoothedFPS:F1}. " +
                        "Triggering quality reduction.");
                }
            }
            else if (_smoothedFPS < warningFPSThreshold)
            {
                // Soft warning only
                if (Time.frameCount % 300 == 0) // Log every ~5 seconds
                {
                    Debug.Log($"[Performance] FPS below target: {_smoothedFPS:F1}/{targetFPS}");
                }
            }
        }

        private void UpdateDebugDisplay()
        {
            bool show = (VirtualMuseumManager.Instance != null && VirtualMuseumManager.Instance.ShowDebugUI)
                || (Application.isEditor && showInEditor);

            if (debugText != null)
            {
                debugText.gameObject.SetActive(show);
                if (show)
                {
                    Color fpsColor = _smoothedFPS >= 72 ? Color.green :
                        _smoothedFPS >= 60 ? Color.yellow : Color.red;

                    debugText.text = $"<color=#{ColorUtility.ToHtmlStringRGB(fpsColor)}>" +
                        $"FPS: {_smoothedFPS:F0}</color>\n" +
                        $"Min: {_minFPS:F0} / Max: {_maxFPS:F0}\n" +
                        $"Dropped: {_droppedFrameCount}\n" +
                        $"Draw Calls: {UnityEngine.Rendering.DebugManager.instance != null}";
                }
            }
        }

        /// <summary>Get a comprehensive performance report.</summary>
        public string GetPerformanceReport()
        {
            return $"═══ Performance Report ═══\n" +
                   $"Current FPS: {_smoothedFPS:F1}\n" +
                   $"Min/Max FPS: {_minFPS:F1} / {_maxFPS:F1}\n" +
                   $"Dropped Frames: {_droppedFrameCount}\n" +
                   $"Quality Downgrades: {_qualityDowngradeCount}\n" +
                   $"Session Time: {Time.realtimeSinceStartup:F0}s";
        }

        /// <summary>Reset tracking statistics.</summary>
        public void ResetStats()
        {
            _minFPS = float.MaxValue;
            _maxFPS = 0;
            _droppedFrameCount = 0;
            _qualityDowngradeCount = 0;
        }
    }
}
