// =============================================================================
// MuseumHUD.cs — Wrist-Mounted HUD (Minimap, Room Name)
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================

using UnityEngine;
using UnityEngine.InputSystem;
using VirtualMuseumVR.Core;

namespace VirtualMuseumVR.UI
{
    public class MuseumHUD : MonoBehaviour
    {
        [Header("Attachment")]
        [SerializeField] private Transform leftWristAnchor;
        [SerializeField] private Vector3 wristOffset = new Vector3(0, 0.05f, 0.1f);
        [SerializeField] private Vector3 wristRotation = new Vector3(-30f, 0, 0);

        [Header("UI Elements")]
        [SerializeField] private TMPro.TextMeshProUGUI roomNameText;
        [SerializeField] private TMPro.TextMeshProUGUI tourProgressText;
        [SerializeField] private TMPro.TextMeshProUGUI sessionTimeText;
        [SerializeField] private GameObject tourProgressPanel;

        [Header("Visibility")]
        [SerializeField] private InputActionReference toggleHUDAction;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float lookAngleThreshold = 40f;

        private bool _isVisible = true;
        private bool _manuallyHidden;
        private Transform _headTransform;

        private void Start()
        {
            _headTransform = Camera.main?.transform;

            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();

            if (tourProgressPanel != null)
                tourProgressPanel.SetActive(false);
        }

        private void OnEnable()
        {
            GameEvents.OnRoomEntered += HandleRoomEntered;
            GameEvents.OnTourStepChanged += HandleTourStepChanged;
            GameEvents.OnTourStarted += HandleTourStarted;
            GameEvents.OnTourEnded += HandleTourEnded;

            if (toggleHUDAction?.action != null)
            {
                toggleHUDAction.action.Enable();
                toggleHUDAction.action.performed += OnToggleHUD;
            }
        }

        private void OnDisable()
        {
            GameEvents.OnRoomEntered -= HandleRoomEntered;
            GameEvents.OnTourStepChanged -= HandleTourStepChanged;
            GameEvents.OnTourStarted -= HandleTourStarted;
            GameEvents.OnTourEnded -= HandleTourEnded;

            if (toggleHUDAction?.action != null)
                toggleHUDAction.action.performed -= OnToggleHUD;
        }

        private void LateUpdate()
        {
            // Follow left wrist
            if (leftWristAnchor != null)
            {
                transform.position = leftWristAnchor.TransformPoint(wristOffset);
                transform.rotation = leftWristAnchor.rotation * Quaternion.Euler(wristRotation);
            }

            // Auto-show/hide based on wrist angle (look at watch gesture)
            if (!_manuallyHidden && _headTransform != null && leftWristAnchor != null)
            {
                Vector3 wristToHead = _headTransform.position - transform.position;
                float angle = Vector3.Angle(transform.up, wristToHead);
                bool shouldShow = angle < lookAngleThreshold;

                float targetAlpha = shouldShow ? 1f : 0f;
                if (canvasGroup != null)
                    canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * 5f);
            }

            // Update session time
            if (sessionTimeText != null && VirtualMuseumManager.Instance != null)
                sessionTimeText.text = VirtualMuseumManager.Instance.SessionTimeFormatted;
        }

        private void HandleRoomEntered(string roomId)
        {
            if (roomNameText != null)
            {
                string displayName = roomId switch
                {
                    "corridor" => "Main Corridor",
                    "iraq" => "Iraq Section",
                    "prague" => "Prague Monuments",
                    "shards" => "Archaeological Shards",
                    "aerial" => "Aerial Photogrammetry",
                    "india" => "Indian Architecture",
                    _ => roomId
                };
                roomNameText.text = displayName;
            }
        }

        private void HandleTourStepChanged(int current, int total)
        {
            if (tourProgressText != null)
                tourProgressText.text = $"Stop {current + 1} / {total}";
        }

        private void HandleTourStarted()
        {
            if (tourProgressPanel != null)
                tourProgressPanel.SetActive(true);
        }

        private void HandleTourEnded(bool completed)
        {
            if (tourProgressPanel != null)
                tourProgressPanel.SetActive(false);
        }

        private void OnToggleHUD(InputAction.CallbackContext ctx)
        {
            _manuallyHidden = !_manuallyHidden;
            if (canvasGroup != null)
                canvasGroup.alpha = _manuallyHidden ? 0f : 1f;
        }
    }
}
