// =============================================================================
// ComfortSettings.cs — VR Comfort Options
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
using VirtualMuseumVR.Core;

namespace VirtualMuseumVR.Locomotion
{
    public class ComfortSettings : MonoBehaviour
    {
        [Header("Snap Turn")]
        [SerializeField] private SnapTurnProvider snapTurnProvider;
        [SerializeField] private float[] snapAngleOptions = { 30f, 45f, 60f, 90f };
        [SerializeField] private int defaultSnapAngleIndex = 1; // 45°

        [Header("Vignette")]
        [SerializeField] private GameObject vignetteOverlay;
        [SerializeField] private bool enableVignetteDuringTeleport = true;
        [SerializeField] private float vignetteIntensity = 0.6f;

        [Header("Height")]
        [SerializeField] private float seatedHeightOffset = 0.5f;
        [SerializeField] private bool isSeatedMode = false;

        [Header("Movement")]
        [SerializeField] private bool enableContinuousMovement = false;
        [SerializeField] private float moveSpeed = 2f;

        private int _currentSnapAngleIndex;
        private Unity.XR.CoreUtils.XROrigin _xrOrigin;

        public bool IsSeatedMode => isSeatedMode;
        public float CurrentSnapAngle => snapAngleOptions[_currentSnapAngleIndex];
        public bool VignetteEnabled => enableVignetteDuringTeleport;

        private void Start()
        {
            _xrOrigin = FindFirstObjectByType<Unity.XR.CoreUtils.XROrigin>();
            _currentSnapAngleIndex = defaultSnapAngleIndex;
            ApplySettings();
        }

        public void SetSnapTurnAngle(int optionIndex)
        {
            _currentSnapAngleIndex = Mathf.Clamp(optionIndex, 0, snapAngleOptions.Length - 1);
            if (snapTurnProvider != null)
                snapTurnProvider.turnAmount = snapAngleOptions[_currentSnapAngleIndex];
            GameEvents.RaiseComfortSettingsChanged();
        }

        public void CycleSnapAngle()
        {
            _currentSnapAngleIndex = (_currentSnapAngleIndex + 1) % snapAngleOptions.Length;
            SetSnapTurnAngle(_currentSnapAngleIndex);
        }

        public void SetSeatedMode(bool seated)
        {
            isSeatedMode = seated;
            if (_xrOrigin != null)
            {
                // Adjust camera Y offset for seated play
                _xrOrigin.CameraYOffset = seated ? seatedHeightOffset : 0f;
            }
            GameEvents.RaiseComfortSettingsChanged();
        }

        public void ToggleSeatedMode() => SetSeatedMode(!isSeatedMode);

        public void SetVignetteEnabled(bool enabled)
        {
            enableVignetteDuringTeleport = enabled;
            GameEvents.RaiseComfortSettingsChanged();
        }

        public void ShowVignette()
        {
            if (vignetteOverlay != null && enableVignetteDuringTeleport)
                vignetteOverlay.SetActive(true);
        }

        public void HideVignette()
        {
            if (vignetteOverlay != null)
                vignetteOverlay.SetActive(false);
        }

        public void CalibrateHeight()
        {
            if (_xrOrigin != null)
            {
                var cam = Camera.main;
                if (cam != null)
                {
                    float currentHeight = cam.transform.localPosition.y;
                    _xrOrigin.CameraYOffset = -currentHeight + 1.6f; // Normalize to ~1.6m
                    Debug.Log($"[Comfort] Height calibrated. Offset: {_xrOrigin.CameraYOffset:F2}m");
                }
            }
        }

        private void ApplySettings()
        {
            SetSnapTurnAngle(_currentSnapAngleIndex);
            SetSeatedMode(isSeatedMode);
            HideVignette();
        }

        /// <summary>Get human-readable string for current snap angle.</summary>
        public string GetSnapAngleLabel() => $"{snapAngleOptions[_currentSnapAngleIndex]}°";

        /// <summary>Get all available snap angle labels.</summary>
        public string[] GetAllSnapAngleLabels()
        {
            var labels = new string[snapAngleOptions.Length];
            for (int i = 0; i < snapAngleOptions.Length; i++)
                labels[i] = $"{snapAngleOptions[i]}°";
            return labels;
        }
    }
}
