using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using VirtualMuseumVR.Core;

namespace VirtualMuseumVR.Locomotion
{
    /// <summary>
    /// Provides continuous joystick movement as an alternative to teleportation.
    /// Integrates with the VirtualMuseumManager for centralized control.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class ContinuousMovementProvider : MonoBehaviour
    {
        [Header("Movement Settings")]
        [Tooltip("The speed at which the player moves.")]
        public float moveSpeed = 1.5f;
        [Tooltip("The action reference for the left joystick.")]
        public UnityEngine.InputSystem.InputActionReference moveAction;
        [Tooltip("The transform representing the player's head (camera).")]
        public Transform headTransform;
        
        [Header("Comfort Settings (Optional Integration)")]
        [Tooltip("Reference to comfort settings if vignette effects are needed during movement.")]
        public ComfortSettings comfortSettings;

        private CharacterController characterController;
        private float gravity = -9.81f;
        private float verticalVelocity = 0f;

        private bool isMovementEnabled = true;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            if (headTransform == null)
            {
                Debug.LogWarning("[ContinuousMovementProvider] HeadTransform not assigned. Attempting to find Camera.main.");
                if (Camera.main != null)
                    headTransform = Camera.main.transform;
            }
        }

        private void OnEnable()
        {
            moveAction.action.Enable();
            // Optional: Subscribe to GameEvents if you want movement to be disabled during specific museum states (e.g., inspecting a UI closely).
            // GameEvents.OnStateChanged += HandleMuseumStateChanged;
        }

        private void OnDisable()
        {
            moveAction.action.Disable();
            // GameEvents.OnStateChanged -= HandleMuseumStateChanged;
        }

        private void Update()
        {
            if (!isMovementEnabled || VirtualMuseumManager.Instance == null || VirtualMuseumManager.Instance.CurrentState != MuseumState.FreeRoam)
                return;

            HandleMovement();
            HandleGravity();
            UpdateCharacterControllerHeight();
        }

        private void HandleMovement()
        {
            Vector2 input = moveAction.action.ReadValue<Vector2>();
            
            if (input.sqrMagnitude < 0.01f)
            {
                // Trigger vignette off if comfort settings are used
                if (comfortSettings != null) comfortSettings.ApplyVignetteIntensity(0f);
                return;
            }

            // Trigger vignette on for comfort
            if (comfortSettings != null) comfortSettings.ApplyVignetteIntensity(comfortSettings.movementVignetteIntensity);

            // Move relative to head rotation
            Vector3 direction = new Vector3(input.x, 0, input.y);
            Vector3 headRotation = new Vector3(0, headTransform.eulerAngles.y, 0);
            direction = Quaternion.Euler(headRotation) * direction;

            Vector3 movement = direction * moveSpeed * Time.deltaTime;
            characterController.Move(movement);
        }

        private void HandleGravity()
        {
            if (characterController.isGrounded)
            {
                verticalVelocity = -0.5f; // Small constant downward force to stay grounded
            }
            else
            {
                verticalVelocity += gravity * Time.deltaTime;
            }

            Vector3 gravityMovement = new Vector3(0, verticalVelocity, 0) * Time.deltaTime;
            characterController.Move(gravityMovement);
        }

        private void UpdateCharacterControllerHeight()
        {
            // Adjust the capsule collider based on the HMD's current height
            if (headTransform != null)
            {
                characterController.height = headTransform.localPosition.y;
                // Keep the center of the capsule at half its height
                characterController.center = new Vector3(0, characterController.height / 2f, 0); 
            }
        }
        
        public void SetMovementEnabled(bool enabled)
        {
            isMovementEnabled = enabled;
        }
    }
}
