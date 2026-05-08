// =============================================================================
// MainMenuController.cs — VR Spatial Main Menu
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================

using UnityEngine;
using UnityEngine.UI;
using VirtualMuseumVR.Core;
using VirtualMuseumVR.Locomotion;

namespace VirtualMuseumVR.UI
{
    [RequireComponent(typeof(Canvas))]
    public class MainMenuController : MonoBehaviour
    {
        [Header("Menu Buttons")]
        [SerializeField] private Button freeRoamButton;
        [SerializeField] private Button guidedTourButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button exitButton;

        [Header("Panels")]
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject creditsPanel;

        [Header("Settings Controls")]
        [SerializeField] private Button snapAngleButton;
        [SerializeField] private TMPro.TextMeshProUGUI snapAngleLabel;
        [SerializeField] private Toggle seatedModeToggle;
        [SerializeField] private Toggle vignetteToggle;
        [SerializeField] private Button backFromSettingsButton;

        [Header("Credits")]
        [SerializeField] private TMPro.TextMeshProUGUI creditsText;
        [SerializeField] private Button backFromCreditsButton;

        [Header("Positioning")]
        [SerializeField] private float distanceFromPlayer = 2f;
        [SerializeField] private float heightOffset = 0.5f;

        private Canvas _canvas;
        private ComfortSettings _comfortSettings;
        private bool _isOpen;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.renderMode = RenderMode.WorldSpace;

            // Set up button listeners
            if (freeRoamButton != null) freeRoamButton.onClick.AddListener(OnFreeRoamClicked);
            if (guidedTourButton != null) guidedTourButton.onClick.AddListener(OnGuidedTourClicked);
            if (settingsButton != null) settingsButton.onClick.AddListener(OnSettingsClicked);
            if (creditsButton != null) creditsButton.onClick.AddListener(OnCreditsClicked);
            if (exitButton != null) exitButton.onClick.AddListener(OnExitClicked);
            if (snapAngleButton != null) snapAngleButton.onClick.AddListener(OnCycleSnapAngle);
            if (backFromSettingsButton != null) backFromSettingsButton.onClick.AddListener(ShowMainPanel);
            if (backFromCreditsButton != null) backFromCreditsButton.onClick.AddListener(ShowMainPanel);

            if (seatedModeToggle != null)
                seatedModeToggle.onValueChanged.AddListener(OnSeatedModeChanged);
            if (vignetteToggle != null)
                vignetteToggle.onValueChanged.AddListener(OnVignetteChanged);

            PopulateCredits();
        }

        private void Start()
        {
            _comfortSettings = FindFirstObjectByType<ComfortSettings>();
            ShowMainPanel();
        }

        private void OnEnable()
        {
            GameEvents.OnMainMenuOpened += HandleMenuOpened;
            GameEvents.OnMainMenuClosed += HandleMenuClosed;
        }

        private void OnDisable()
        {
            GameEvents.OnMainMenuOpened -= HandleMenuOpened;
            GameEvents.OnMainMenuClosed -= HandleMenuClosed;
        }

        public void Open()
        {
            _isOpen = true;
            gameObject.SetActive(true);
            PositionInFrontOfPlayer();
            ShowMainPanel();
        }

        public void Close()
        {
            _isOpen = false;
            gameObject.SetActive(false);
        }

        private void PositionInFrontOfPlayer()
        {
            var cam = Camera.main;
            if (cam == null) return;

            Vector3 forward = cam.transform.forward;
            forward.y = 0;
            forward.Normalize();

            transform.position = cam.transform.position +
                forward * distanceFromPlayer +
                Vector3.up * heightOffset;
            transform.rotation = Quaternion.LookRotation(forward);
        }

        private void ShowMainPanel()
        {
            if (mainPanel != null) mainPanel.SetActive(true);
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (creditsPanel != null) creditsPanel.SetActive(false);
        }

        private void OnFreeRoamClicked()
        {
            VirtualMuseumManager.Instance.StartFreeRoam();
            Close();
        }

        private void OnGuidedTourClicked()
        {
            VirtualMuseumManager.Instance.StartGuidedTour();
            Close();
        }

        private void OnSettingsClicked()
        {
            if (mainPanel != null) mainPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(true);
            UpdateSettingsUI();
        }

        private void OnCreditsClicked()
        {
            if (mainPanel != null) mainPanel.SetActive(false);
            if (creditsPanel != null) creditsPanel.SetActive(true);
        }

        private void OnExitClicked()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        private void OnCycleSnapAngle()
        {
            _comfortSettings?.CycleSnapAngle();
            UpdateSettingsUI();
        }

        private void OnSeatedModeChanged(bool seated)
        {
            _comfortSettings?.SetSeatedMode(seated);
        }

        private void OnVignetteChanged(bool enabled)
        {
            _comfortSettings?.SetVignetteEnabled(enabled);
        }

        private void UpdateSettingsUI()
        {
            if (_comfortSettings == null) return;
            if (snapAngleLabel != null)
                snapAngleLabel.text = $"Snap Turn: {_comfortSettings.GetSnapAngleLabel()}";
            if (seatedModeToggle != null)
                seatedModeToggle.isOn = _comfortSettings.IsSeatedMode;
            if (vignetteToggle != null)
                vignetteToggle.isOn = _comfortSettings.VignetteEnabled;
        }

        private void PopulateCredits()
        {
            if (creditsText == null) return;
            creditsText.text =
                "<b>Virtual Museum VR</b>\n\n" +
                "Based on the methodology from:\n" +
                "<i>\"Virtual Museums – The Future of Historical\n" +
                "Monuments Documentation and Visualization\"</i>\n\n" +
                "Pavelka, K., Jr. & Raeva, P. (2019)\n" +
                "ISPRS Archives, XLII-2/W15, 903-908\n\n" +
                "Built with Unity & XR Interaction Toolkit\n" +
                "Targeting Meta Quest\n\n" +
                "Photogrammetry models processed with\n" +
                "Agisoft Metashape / RealityCapture";
        }

        private void HandleMenuOpened() => Open();
        private void HandleMenuClosed() => Close();
    }
}
