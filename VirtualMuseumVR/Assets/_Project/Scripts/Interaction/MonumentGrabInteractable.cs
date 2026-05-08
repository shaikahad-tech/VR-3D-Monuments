// =============================================================================
// MonumentGrabInteractable.cs — XRGrabInteractable for Heritage Models
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================
// Paper Section 3.2: Models "able to be grabbed and you can rotate it in your
// hand". Extends XRGrabInteractable with rotation, scale override, haptic
// feedback, and smooth return-to-pedestal on release.
// =============================================================================

using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using VirtualMuseumVR.Core;

namespace VirtualMuseumVR.Interaction
{
    [RequireComponent(typeof(Rigidbody))]
    public class MonumentGrabInteractable : XRGrabInteractable
    {
        [Header("Exhibit Configuration")]
        [SerializeField] private ExhibitData exhibitData;

        [Header("Rotation Settings")]
        [Tooltip("Speed of thumbstick rotation while held")]
        [SerializeField] private float rotationSpeed = 50f;
        [Tooltip("Enable rotation on vertical axis (Y)")]
        [SerializeField] private bool allowYRotation = true;
        [Tooltip("Enable rotation on horizontal axis (X)")]
        [SerializeField] private bool allowXRotation = true;

        [Header("Scale Settings")]
        [Tooltip("Scale when grabbed for inspection")]
        [SerializeField] private float inspectionScale = 0.3f;

        [Header("Return Animation")]
        [Tooltip("Time to smoothly return to pedestal after release")]
        [SerializeField] private float returnDuration = 0.8f;
        [Tooltip("Animation curve for return movement")]
        [SerializeField] private AnimationCurve returnCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Haptics")]
        [SerializeField] private float grabHapticAmplitude = 0.5f;
        [SerializeField] private float grabHapticDuration = 0.1f;
        [SerializeField] private float holdHapticAmplitude = 0.05f;
        [SerializeField] private float holdHapticInterval = 0.5f;

        // Original pedestal state
        private Vector3 _originalPosition;
        private Quaternion _originalRotation;
        private Vector3 _originalScale;
        private Transform _pedestalTransform;

        // Runtime state
        private bool _isGrabbed;
        private bool _isReturning;
        private IXRSelectInteractor _currentInteractor;
        private float _nextHapticTime;

        public ExhibitData ExhibitInfo => exhibitData;
        public bool IsGrabbed => _isGrabbed;

        protected override void Awake()
        {
            base.Awake();

            // Store original transform for return-to-pedestal
            _originalPosition = transform.position;
            _originalRotation = transform.rotation;
            _originalScale = transform.localScale;
            _pedestalTransform = transform.parent;

            // Apply ExhibitData settings if available
            if (exhibitData != null)
            {
                rotationSpeed = exhibitData.rotationSpeed;
                inspectionScale = exhibitData.inspectionScale;
            }

            // Configure Rigidbody for VR interaction
            var rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;

            // XRGrabInteractable settings
            movementType = MovementType.VelocityTracking;
            throwOnDetach = false;
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);

            _isGrabbed = true;
            _isReturning = false;
            _currentInteractor = args.interactorObject;

            // Scale up for comfortable inspection
            transform.localScale = _originalScale * (inspectionScale / Mathf.Max(_originalScale.x, 0.001f));

            // Send haptic pulse on grab
            if (_currentInteractor is XRBaseInputInteractor inputInteractor)
            {
                inputInteractor.SendHapticImpulse(grabHapticAmplitude, grabHapticDuration);
            }

            // Fire event
            string id = exhibitData != null ? exhibitData.exhibitId : gameObject.name;
            GameEvents.RaiseExhibitGrabbed(id);

            Debug.Log($"[Monument] Grabbed: {(exhibitData != null ? exhibitData.title : gameObject.name)}");
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);

            _isGrabbed = false;
            _currentInteractor = null;

            // Fire event
            string id = exhibitData != null ? exhibitData.exhibitId : gameObject.name;
            GameEvents.RaiseExhibitReleased(id);

            // Smoothly return to pedestal
            StartCoroutine(ReturnToPedestal());

            Debug.Log($"[Monument] Released: {(exhibitData != null ? exhibitData.title : gameObject.name)}");
        }

        private void Update()
        {
            if (!_isGrabbed) return;

            HandleRotationInput();
            HandleContinuousHaptics();
        }

        private void HandleRotationInput()
        {
            // Read thumbstick input for rotation while held
            // Uses Unity's Input System via XRI action maps
            float horizontalInput = 0f;
            float verticalInput = 0f;

            // Try to get input from the interacting controller
            if (_currentInteractor is XRBaseInputInteractor)
            {
                // Fallback to legacy input for broader compatibility
                horizontalInput = Input.GetAxis("Horizontal");
                verticalInput = Input.GetAxis("Vertical");
            }

            // Apply rotation
            if (allowYRotation && Mathf.Abs(horizontalInput) > 0.1f)
            {
                transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime, Space.World);
            }

            if (allowXRotation && Mathf.Abs(verticalInput) > 0.1f)
            {
                transform.Rotate(Vector3.right, verticalInput * rotationSpeed * Time.deltaTime, Space.World);
            }
        }

        private void HandleContinuousHaptics()
        {
            if (Time.time < _nextHapticTime) return;
            _nextHapticTime = Time.time + holdHapticInterval;

            if (_currentInteractor is XRBaseInputInteractor inputInteractor)
            {
                inputInteractor.SendHapticImpulse(holdHapticAmplitude, 0.05f);
            }
        }

        private IEnumerator ReturnToPedestal()
        {
            _isReturning = true;

            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;
            Vector3 startScale = transform.localScale;
            float elapsed = 0f;

            // Make kinematic during return
            var rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            while (elapsed < returnDuration)
            {
                elapsed += Time.deltaTime;
                float t = returnCurve.Evaluate(elapsed / returnDuration);

                transform.position = Vector3.Lerp(startPos, _originalPosition, t);
                transform.rotation = Quaternion.Slerp(startRot, _originalRotation, t);
                transform.localScale = Vector3.Lerp(startScale, _originalScale, t);

                yield return null;
            }

            // Snap to exact position
            transform.position = _originalPosition;
            transform.rotation = _originalRotation;
            transform.localScale = _originalScale;

            _isReturning = false;
        }

        /// <summary>
        /// Update the stored home position (called when pedestal moves).
        /// </summary>
        public void SetHomePosition(Vector3 position, Quaternion rotation)
        {
            _originalPosition = position;
            _originalRotation = rotation;
        }
    }
}
