// =============================================================================
// PedestalController.cs — Animated Display Pedestals for Monuments
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================

using UnityEngine;
using VirtualMuseumVR.Core;

namespace VirtualMuseumVR.Environment
{
    public class PedestalController : MonoBehaviour
    {
        [Header("Exhibit")]
        [SerializeField] private ExhibitData exhibitData;
        [SerializeField] private Transform exhibitSpawnPoint;
        [SerializeField] private Interaction.MonumentGrabInteractable monumentInteractable;

        [Header("Auto-Rotation")]
        [SerializeField] private bool autoRotate = true;
        [SerializeField] private float rotationSpeed = 5f;

        [Header("Glow Ring")]
        [SerializeField] private Renderer glowRingRenderer;
        [SerializeField] private Color glowColor = new Color(0.3f, 0.6f, 1f);
        [SerializeField] private float glowPulseSpeed = 1f;
        [SerializeField] private float glowMinIntensity = 0.5f;
        [SerializeField] private float glowMaxIntensity = 2f;

        [Header("Label")]
        [SerializeField] private TMPro.TextMeshPro labelText;
        [SerializeField] private TMPro.TextMeshPro subLabelText;

        private bool _isExhibitGrabbed;
        private Material _glowMaterial;
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        private void Start()
        {
            // Apply exhibit data settings
            if (exhibitData != null)
            {
                autoRotate = exhibitData.autoRotateOnPedestal;
                rotationSpeed = exhibitData.autoRotationSpeed;
                glowColor = exhibitData.pedestalGlowColor;

                if (labelText != null) labelText.text = exhibitData.title;
                if (subLabelText != null) subLabelText.text = exhibitData.era;
            }

            // Cache glow material
            if (glowRingRenderer != null)
            {
                _glowMaterial = glowRingRenderer.material;
                _glowMaterial.EnableKeyword("_EMISSION");
            }
        }

        private void OnEnable()
        {
            GameEvents.OnExhibitGrabbed += HandleExhibitGrabbed;
            GameEvents.OnExhibitReleased += HandleExhibitReleased;
        }

        private void OnDisable()
        {
            GameEvents.OnExhibitGrabbed -= HandleExhibitGrabbed;
            GameEvents.OnExhibitReleased -= HandleExhibitReleased;
        }

        private void Update()
        {
            // Auto-rotate exhibit on pedestal when not grabbed
            if (autoRotate && !_isExhibitGrabbed && exhibitSpawnPoint != null)
            {
                exhibitSpawnPoint.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            }

            // Pulse glow ring
            if (_glowMaterial != null)
            {
                float pulse = Mathf.Lerp(glowMinIntensity, glowMaxIntensity,
                    (Mathf.Sin(Time.time * glowPulseSpeed) + 1f) * 0.5f);
                _glowMaterial.SetColor(EmissionColor, glowColor * pulse);
            }
        }

        private void HandleExhibitGrabbed(string exhibitId)
        {
            if (exhibitData != null && exhibitData.exhibitId == exhibitId)
            {
                _isExhibitGrabbed = true;
            }
        }

        private void HandleExhibitReleased(string exhibitId)
        {
            if (exhibitData != null && exhibitData.exhibitId == exhibitId)
            {
                _isExhibitGrabbed = false;
            }
        }

        /// <summary>Set the exhibit displayed on this pedestal.</summary>
        public void SetExhibit(ExhibitData data, GameObject monumentPrefab)
        {
            exhibitData = data;

            if (exhibitSpawnPoint != null && monumentPrefab != null)
            {
                // Clear previous
                foreach (Transform child in exhibitSpawnPoint)
                    Destroy(child.gameObject);

                // Instantiate new
                var instance = Instantiate(monumentPrefab, exhibitSpawnPoint);
                instance.transform.localPosition = Vector3.zero;
                instance.transform.localScale = Vector3.one * data.pedestalDisplayScale;

                monumentInteractable = instance.GetComponent<Interaction.MonumentGrabInteractable>();
            }

            // Update labels
            if (labelText != null) labelText.text = data.title;
            if (subLabelText != null) subLabelText.text = data.era;
            glowColor = data.pedestalGlowColor;
        }
    }
}
