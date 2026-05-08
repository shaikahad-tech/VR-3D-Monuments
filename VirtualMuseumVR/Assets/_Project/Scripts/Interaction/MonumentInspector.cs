// =============================================================================
// MonumentInspector.cs — Detailed Inspection Mode
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================
// Activated by secondary button press while holding a monument. Enters
// inspection mode: freezes position, enables free orbit, displays info
// panel with exhibit metadata, and highlights surface detail.
// =============================================================================

using UnityEngine;
using UnityEngine.InputSystem;
using VirtualMuseumVR.Core;

namespace VirtualMuseumVR.Interaction
{
    public class MonumentInspector : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private InputActionReference inspectAction;

        [Header("Inspection Settings")]
        [SerializeField] private float inspectionDistance = 0.5f;
        [SerializeField] private float orbitSpeed = 100f;
        [SerializeField] private float zoomSpeed = 0.3f;
        [SerializeField] private float minZoomDistance = 0.2f;
        [SerializeField] private float maxZoomDistance = 1.5f;

        [Header("Spotlight")]
        [SerializeField] private Light inspectionSpotlight;
        [SerializeField] private float spotlightIntensity = 3f;
        [SerializeField] private Color spotlightColor = new Color(1f, 0.95f, 0.9f);

        [Header("References")]
        [SerializeField] private Transform headTransform;

        private MonumentGrabInteractable _currentMonument;
        private bool _isInspecting;
        private Vector3 _inspectionPosition;
        private float _currentDistance;

        public bool IsInspecting => _isInspecting;
        public MonumentGrabInteractable CurrentMonument => _currentMonument;

        private void Start()
        {
            if (inspectionSpotlight != null)
            {
                inspectionSpotlight.enabled = false;
                inspectionSpotlight.intensity = spotlightIntensity;
                inspectionSpotlight.color = spotlightColor;
            }

            // Auto-find head transform if not assigned
            if (headTransform == null)
            {
                var camera = Camera.main;
                if (camera != null) headTransform = camera.transform;
            }
        }

        private void OnEnable()
        {
            if (inspectAction != null && inspectAction.action != null)
            {
                inspectAction.action.Enable();
                inspectAction.action.performed += OnInspectPressed;
            }

            GameEvents.OnExhibitGrabbed += HandleExhibitGrabbed;
            GameEvents.OnExhibitReleased += HandleExhibitReleased;
        }

        private void OnDisable()
        {
            if (inspectAction != null && inspectAction.action != null)
            {
                inspectAction.action.performed -= OnInspectPressed;
            }

            GameEvents.OnExhibitGrabbed -= HandleExhibitGrabbed;
            GameEvents.OnExhibitReleased -= HandleExhibitReleased;
        }

        private void Update()
        {
            if (!_isInspecting || _currentMonument == null) return;

            // Keep the monument at inspection distance in front of the player
            if (headTransform != null)
            {
                _inspectionPosition = headTransform.position + headTransform.forward * _currentDistance;
                _currentMonument.transform.position = Vector3.Lerp(
                    _currentMonument.transform.position,
                    _inspectionPosition,
                    Time.deltaTime * 10f
                );
            }

            // Update spotlight position
            if (inspectionSpotlight != null)
            {
                inspectionSpotlight.transform.position = headTransform.position;
                inspectionSpotlight.transform.LookAt(_currentMonument.transform);
            }

            HandleOrbitInput();
        }

        private void HandleOrbitInput()
        {
            // Thumbstick orbit
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if (Mathf.Abs(horizontal) > 0.1f)
            {
                _currentMonument.transform.Rotate(Vector3.up, horizontal * orbitSpeed * Time.deltaTime, Space.World);
            }
            if (Mathf.Abs(vertical) > 0.1f)
            {
                _currentMonument.transform.Rotate(Vector3.right, vertical * orbitSpeed * Time.deltaTime, Space.World);
            }
        }

        public void EnterInspection(MonumentGrabInteractable monument)
        {
            if (_isInspecting) ExitInspection();

            _currentMonument = monument;
            _isInspecting = true;
            _currentDistance = inspectionDistance;

            // Enable spotlight
            if (inspectionSpotlight != null)
                inspectionSpotlight.enabled = true;

            // Apply exhibit-specific spotlight settings
            if (monument.ExhibitInfo != null)
            {
                if (inspectionSpotlight != null)
                {
                    inspectionSpotlight.color = monument.ExhibitInfo.spotlightColor;
                    inspectionSpotlight.intensity = monument.ExhibitInfo.spotlightIntensity;
                }
            }

            string id = monument.ExhibitInfo != null ? monument.ExhibitInfo.exhibitId : monument.gameObject.name;
            GameEvents.RaiseInspectionStarted(id);

            Debug.Log($"[Inspector] Entered inspection: {(monument.ExhibitInfo != null ? monument.ExhibitInfo.title : monument.name)}");
        }

        public void ExitInspection()
        {
            if (!_isInspecting) return;

            if (inspectionSpotlight != null)
                inspectionSpotlight.enabled = false;

            if (_currentMonument != null)
            {
                string id = _currentMonument.ExhibitInfo != null
                    ? _currentMonument.ExhibitInfo.exhibitId
                    : _currentMonument.gameObject.name;
                GameEvents.RaiseInspectionEnded(id);
            }

            _isInspecting = false;
            _currentMonument = null;

            Debug.Log("[Inspector] Exited inspection mode.");
        }

        public void AdjustZoom(float delta)
        {
            _currentDistance = Mathf.Clamp(
                _currentDistance + delta * zoomSpeed,
                minZoomDistance,
                maxZoomDistance
            );
        }

        private void OnInspectPressed(InputAction.CallbackContext ctx)
        {
            if (_isInspecting)
            {
                ExitInspection();
            }
            else
            {
                // Find grabbed monument
                var monuments = FindObjectsByType<MonumentGrabInteractable>(FindObjectsSortMode.None);
                foreach (var m in monuments)
                {
                    if (m.IsGrabbed)
                    {
                        EnterInspection(m);
                        break;
                    }
                }
            }
        }

        private void HandleExhibitGrabbed(string exhibitId) { /* Ready for inspection */ }

        private void HandleExhibitReleased(string exhibitId)
        {
            if (_isInspecting) ExitInspection();
        }
    }
}
