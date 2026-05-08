// =============================================================================
// ExhibitTriggerZone.cs — Proximity Trigger for Exhibit Activation
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================

using UnityEngine;
using VirtualMuseumVR.Core;

namespace VirtualMuseumVR.Interaction
{
    [RequireComponent(typeof(SphereCollider))]
    public class ExhibitTriggerZone : MonoBehaviour
    {
        [Header("Exhibit Reference")]
        [SerializeField] private ExhibitData exhibitData;
        [SerializeField] private string exhibitId;

        [Header("Trigger Settings")]
        [SerializeField] private float triggerRadius = 2.5f;
        [SerializeField] private LayerMask playerLayer;

        [Header("Visual Feedback")]
        [SerializeField] private Light accentSpotlight;
        [SerializeField] private GameObject infoPanel;
        [SerializeField] private float spotlightFadeSpeed = 3f;

        [Header("Audio")]
        [SerializeField] private AudioSource narrationSource;
        [SerializeField] private AudioSource ambientSource;
        [SerializeField] private float audioFadeSpeed = 2f;

        private SphereCollider _triggerCollider;
        private bool _playerInZone;
        private float _spotlightTargetIntensity;
        private float _currentSpotlightIntensity;

        public bool IsPlayerInZone => _playerInZone;

        private void Awake()
        {
            _triggerCollider = GetComponent<SphereCollider>();
            _triggerCollider.isTrigger = true;
            _triggerCollider.radius = triggerRadius;

            if (string.IsNullOrEmpty(exhibitId) && exhibitData != null)
                exhibitId = exhibitData.exhibitId;

            // Initialize visual elements as hidden
            if (accentSpotlight != null)
            {
                _spotlightTargetIntensity = accentSpotlight.intensity;
                accentSpotlight.intensity = 0f;
            }

            if (infoPanel != null)
                infoPanel.SetActive(false);
        }

        private void Update()
        {
            // Smooth spotlight fade
            if (accentSpotlight != null)
            {
                float target = _playerInZone ? _spotlightTargetIntensity : 0f;
                _currentSpotlightIntensity = Mathf.Lerp(
                    _currentSpotlightIntensity, target,
                    Time.deltaTime * spotlightFadeSpeed
                );
                accentSpotlight.intensity = _currentSpotlightIntensity;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsPlayer(other)) return;

            _playerInZone = true;

            // Show info panel
            if (infoPanel != null)
                infoPanel.SetActive(true);

            // Start narration
            if (narrationSource != null && exhibitData?.narrationClip != null)
            {
                narrationSource.clip = exhibitData.narrationClip;
                narrationSource.Play();
            }

            GameEvents.RaiseExhibitProximityEntered(exhibitId);
            Debug.Log($"[ExhibitTrigger] Player entered zone: {exhibitId}");
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsPlayer(other)) return;

            _playerInZone = false;

            // Hide info panel
            if (infoPanel != null)
                infoPanel.SetActive(false);

            // Stop narration
            if (narrationSource != null && narrationSource.isPlaying)
                narrationSource.Stop();

            GameEvents.RaiseExhibitProximityExited(exhibitId);
            Debug.Log($"[ExhibitTrigger] Player exited zone: {exhibitId}");
        }

        private bool IsPlayer(Collider other)
        {
            if (playerLayer.value != 0)
                return (playerLayer.value & (1 << other.gameObject.layer)) != 0;

            // Fallback: check for XR Origin or MainCamera tag
            return other.CompareTag("Player") ||
                   other.GetComponentInParent<Unity.XR.CoreUtils.XROrigin>() != null;
        }

        private void OnValidate()
        {
            if (_triggerCollider == null)
                _triggerCollider = GetComponent<SphereCollider>();
            if (_triggerCollider != null)
            {
                _triggerCollider.isTrigger = true;
                _triggerCollider.radius = triggerRadius;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.3f, 0.6f, 1f, 0.2f);
            Gizmos.DrawSphere(transform.position, triggerRadius);
            Gizmos.color = new Color(0.3f, 0.6f, 1f, 0.5f);
            Gizmos.DrawWireSphere(transform.position, triggerRadius);
        }
    }
}
