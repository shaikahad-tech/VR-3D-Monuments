// =============================================================================
// WaypointIndicator.cs — 3D Arrow Pointing to Next Exhibit
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================

using UnityEngine;
using VirtualMuseumVR.Core;

namespace VirtualMuseumVR.UI
{
    public class WaypointIndicator : MonoBehaviour
    {
        [Header("Visual")]
        [SerializeField] private GameObject arrowMesh;
        [SerializeField] private Renderer arrowRenderer;
        [SerializeField] private Color arrowColor = new Color(0.3f, 0.7f, 1f);
        [SerializeField] private float pulseSpeed = 2f;
        [SerializeField] private float pulseMinScale = 0.8f;
        [SerializeField] private float pulseMaxScale = 1.2f;

        [Header("Behavior")]
        [SerializeField] private Transform targetTransform;
        [SerializeField] private float floatHeight = 2.5f;
        [SerializeField] private float floatBobSpeed = 1f;
        [SerializeField] private float floatBobAmount = 0.1f;
        [SerializeField] private float rotationSmoothing = 5f;

        [Header("Visibility")]
        [SerializeField] private float showDistance = 15f;
        [SerializeField] private float hideDistance = 2f;

        private Transform _playerCamera;
        private bool _isActive;
        private Vector3 _basePosition;
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        private void Start()
        {
            _playerCamera = Camera.main?.transform;
            if (arrowMesh != null) arrowMesh.SetActive(false);
        }

        private void OnEnable()
        {
            GameEvents.OnTourStepChanged += HandleTourStepChanged;
            GameEvents.OnTourEnded += HandleTourEnded;
        }

        private void OnDisable()
        {
            GameEvents.OnTourStepChanged -= HandleTourStepChanged;
            GameEvents.OnTourEnded -= HandleTourEnded;
        }

        private void Update()
        {
            if (!_isActive || targetTransform == null || _playerCamera == null) return;

            // Position above player
            _basePosition = _playerCamera.position + Vector3.up * floatHeight;
            float bob = Mathf.Sin(Time.time * floatBobSpeed) * floatBobAmount;
            transform.position = _basePosition + Vector3.up * bob;

            // Point toward target
            Vector3 direction = targetTransform.position - transform.position;
            direction.y = 0; // Keep level
            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, targetRot, Time.deltaTime * rotationSmoothing);
            }

            // Pulse animation
            float scale = Mathf.Lerp(pulseMinScale, pulseMaxScale,
                (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f);
            if (arrowMesh != null)
                arrowMesh.transform.localScale = Vector3.one * scale;

            // Emission pulse
            if (arrowRenderer != null)
            {
                float emission = (Mathf.Sin(Time.time * pulseSpeed * 2f) + 1f) * 0.5f;
                arrowRenderer.material.SetColor(EmissionColor, arrowColor * (1f + emission));
            }

            // Auto-hide when close to target
            float distToTarget = Vector3.Distance(_playerCamera.position, targetTransform.position);
            if (arrowMesh != null)
                arrowMesh.SetActive(distToTarget > hideDistance && distToTarget < showDistance);
        }

        public void SetTarget(Transform target)
        {
            targetTransform = target;
            _isActive = target != null;
            if (arrowMesh != null) arrowMesh.SetActive(_isActive);
        }

        public void Hide()
        {
            _isActive = false;
            if (arrowMesh != null) arrowMesh.SetActive(false);
        }

        private void HandleTourStepChanged(int current, int total)
        {
            // Tour system should call SetTarget with the next waypoint
        }

        private void HandleTourEnded(bool completed) => Hide();
    }
}
