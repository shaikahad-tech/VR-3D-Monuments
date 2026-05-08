using UnityEngine;
using UnityEngine.InputSystem;
using VirtualMuseumVR.Core;

namespace VirtualMuseumVR.Locomotion
{
    /// <summary>
    /// Provides snap turning to allow users to rotate their view without physical rotation, maximizing comfort.
    /// </summary>
    public class SnapTurnProvider : MonoBehaviour
    {
        [Header("Turn Settings")]
        [Tooltip("Degrees to turn per snap.")]
        public float turnAmount = 45f;
        [Tooltip("The action reference for the right joystick (X-axis).")]
        public InputActionReference turnAction;
        [Tooltip("The XR Rig or root object to rotate.")]
        public Transform xrRigTransform;
        
        [Header("Debounce Settings")]
        [Tooltip("Threshold the joystick must cross to trigger a turn.")]
        public float inputThreshold = 0.5f;
        [Tooltip("Time in seconds before another turn can be triggered.")]
        public float debounceTime = 0.3f;

        private float nextTurnTime = 0f;
        private bool isTurningEnabled = true;

        private void OnEnable()
        {
            turnAction.action.Enable();
        }

        private void OnDisable()
        {
            turnAction.action.Disable();
        }

        private void Update()
        {
            if (!isTurningEnabled || Time.time < nextTurnTime)
                return;

            Vector2 input = turnAction.action.ReadValue<Vector2>();

            if (Mathf.Abs(input.x) > inputThreshold)
            {
                PerformSnapTurn(Mathf.Sign(input.x));
            }
        }

        private void PerformSnapTurn(float direction)
        {
            if (xrRigTransform != null)
            {
                xrRigTransform.RotateAround(Camera.main.transform.position, Vector3.up, direction * turnAmount);
                nextTurnTime = Time.time + debounceTime;
                
                // Note: In a full XR Interaction Toolkit setup, we'd ideally use the existing SnapTurnProviderBase,
                // but this custom script gives us tight integration with our custom MuseumManager state if needed.
            }
        }
        
        public void SetTurningEnabled(bool enabled)
        {
            isTurningEnabled = enabled;
        }
    }
}
