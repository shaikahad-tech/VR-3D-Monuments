// =============================================================================
// ExhibitInfoPanel.cs — World-Space Info Panel for Each Exhibit
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================

using UnityEngine;
using VirtualMuseumVR.Core;

namespace VirtualMuseumVR.UI
{
    [RequireComponent(typeof(Canvas))]
    public class ExhibitInfoPanel : MonoBehaviour
    {
        [Header("Exhibit Reference")]
        [SerializeField] private ExhibitData exhibitData;
        [SerializeField] private string exhibitId;

        [Header("UI Elements")]
        [SerializeField] private TMPro.TextMeshProUGUI titleText;
        [SerializeField] private TMPro.TextMeshProUGUI descriptionText;
        [SerializeField] private TMPro.TextMeshProUGUI eraText;
        [SerializeField] private TMPro.TextMeshProUGUI locationText;
        [SerializeField] private TMPro.TextMeshProUGUI methodText;
        [SerializeField] private TMPro.TextMeshProUGUI polygonText;
        [SerializeField] private UnityEngine.UI.Image thumbnailImage;

        [Header("Behavior")]
        [SerializeField] private bool billboardToPlayer = true;
        [SerializeField] private float fadeInDuration = 0.5f;
        [SerializeField] private float fadeOutDuration = 0.3f;
        [SerializeField] private float displayDistance = 1.5f;

        private CanvasGroup _canvasGroup;
        private Transform _playerCamera;
        private bool _isVisible;
        private float _targetAlpha;
        private Canvas _canvas;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.renderMode = RenderMode.WorldSpace;

            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();

            _canvasGroup.alpha = 0f;
            _targetAlpha = 0f;

            if (string.IsNullOrEmpty(exhibitId) && exhibitData != null)
                exhibitId = exhibitData.exhibitId;
        }

        private void Start()
        {
            _playerCamera = Camera.main?.transform;
            PopulateData();
        }

        private void OnEnable()
        {
            GameEvents.OnExhibitProximityEntered += HandleProximityEntered;
            GameEvents.OnExhibitProximityExited += HandleProximityExited;
        }

        private void OnDisable()
        {
            GameEvents.OnExhibitProximityEntered -= HandleProximityEntered;
            GameEvents.OnExhibitProximityExited -= HandleProximityExited;
        }

        private void Update()
        {
            // Smooth alpha fade
            float fadeSpeed = _targetAlpha > _canvasGroup.alpha
                ? 1f / fadeInDuration
                : 1f / fadeOutDuration;
            _canvasGroup.alpha = Mathf.MoveTowards(
                _canvasGroup.alpha, _targetAlpha, Time.deltaTime * fadeSpeed);

            // Billboard to face player
            if (billboardToPlayer && _playerCamera != null && _canvasGroup.alpha > 0.01f)
            {
                Vector3 lookDir = transform.position - _playerCamera.position;
                lookDir.y = 0; // Keep upright
                if (lookDir.sqrMagnitude > 0.001f)
                    transform.rotation = Quaternion.LookRotation(lookDir);
            }
        }

        private void PopulateData()
        {
            if (exhibitData == null) return;

            if (titleText != null) titleText.text = exhibitData.title;
            if (descriptionText != null) descriptionText.text = exhibitData.description;
            if (eraText != null) eraText.text = exhibitData.era;
            if (locationText != null) locationText.text = $"{exhibitData.location}, {exhibitData.country}";
            if (methodText != null) methodText.text = exhibitData.AcquisitionMethodDisplay;
            if (polygonText != null) polygonText.text = exhibitData.PolygonSummary;
            if (thumbnailImage != null && exhibitData.thumbnail != null)
                thumbnailImage.sprite = exhibitData.thumbnail;
        }

        public void Show()
        {
            _isVisible = true;
            _targetAlpha = 1f;
        }

        public void Hide()
        {
            _isVisible = false;
            _targetAlpha = 0f;
        }

        private void HandleProximityEntered(string id)
        {
            if (id == exhibitId) Show();
        }

        private void HandleProximityExited(string id)
        {
            if (id == exhibitId) Hide();
        }
    }
}
