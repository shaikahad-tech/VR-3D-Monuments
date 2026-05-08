using UnityEngine;
using VirtualMuseumVR.Core;

namespace VirtualMuseumVR.Optimization
{
    /// <summary>
    /// Monitors performance (FPS) and triggers automatic LOD and quality adjustments
    /// to ensure a smooth framerate (e.g., 72Hz or 90Hz on Quest).
    /// </summary>
    public class PerformanceAnalytics : MonoBehaviour
    {
        [Header("Settings")]
        public float targetFPS = 72f;
        public float warningThresholdFPS = 65f;
        public float criticalThresholdFPS = 55f;
        
        [Tooltip("How often to calculate average FPS (in seconds)")]
        public float updateInterval = 1.0f;

        private float accumulatedDeltaTime = 0f;
        private int framesCount = 0;
        private float currentFPS = 0f;
        private float nextUpdateTime = 0f;

        private void Update()
        {
            accumulatedDeltaTime += Time.unscaledDeltaTime;
            framesCount++;

            if (Time.unscaledTime >= nextUpdateTime)
            {
                currentFPS = framesCount / accumulatedDeltaTime;
                
                AnalyzePerformance(currentFPS);

                accumulatedDeltaTime = 0f;
                framesCount = 0;
                nextUpdateTime = Time.unscaledTime + updateInterval;
            }
        }

        private void AnalyzePerformance(float fps)
        {
            if (fps < criticalThresholdFPS)
            {
                Debug.LogWarning($"[PerformanceAnalytics] CRITICAL FPS Drop: {fps:F1}. Triggering aggressive optimizations.");
                GameEvents.RaisePerformanceWarning(fps);
                TriggerAggressiveOptimization();
            }
            else if (fps < warningThresholdFPS)
            {
                // Minor warning, maybe drop texture quality
                GameEvents.RaisePerformanceWarning(fps);
            }
        }

        private void TriggerAggressiveOptimization()
        {
            // Example: Decrease QualitySettings
            if (QualitySettings.GetQualityLevel() > 0)
            {
                QualitySettings.DecreaseLevel(true);
                GameEvents.RaiseQualityLevelChanged(QualitySettings.GetQualityLevel());
                Debug.Log($"[PerformanceAnalytics] Decreased Quality Level to: {QualitySettings.GetQualityLevel()}");
            }
            
            // Note: In a full project, this would also tell the LODController to bias towards lower LODs,
            // or disable distant particle systems.
        }
        
        public float GetCurrentFPS()
        {
            return currentFPS;
        }
    }
}
