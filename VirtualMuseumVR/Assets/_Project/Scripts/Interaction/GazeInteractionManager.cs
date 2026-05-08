using UnityEngine;
using VirtualMuseumVR.Core;

namespace VirtualMuseumVR.Interaction
{
    /// <summary>
    /// Tracks user gaze to trigger specific interactions (like contextual narration)
    /// when looking at specific parts of a monument for a certain duration.
    /// </summary>
    public class GazeInteractionManager : MonoBehaviour
    {
        [Header("Gaze Settings")]
        [Tooltip("The camera representing the user's head/eyes.")]
        public Transform headTransform;
        [Tooltip("Maximum distance the gaze raycast will travel.")]
        public float maxGazeDistance = 10f;
        [Tooltip("Layer mask for interactable gaze targets.")]
        public LayerMask gazeTargetLayerMask;
        
        [Tooltip("How long the user must look at a target to trigger the action.")]
        public float dwellTimeRequired = 2.0f;

        private Transform currentGazeTarget;
        private float currentDwellTimer = 0f;

        private void Awake()
        {
            if (headTransform == null)
            {
                if (Camera.main != null)
                    headTransform = Camera.main.transform;
            }
        }

        private void Update()
        {
            if (headTransform == null || VirtualMuseumManager.Instance == null || VirtualMuseumManager.Instance.CurrentState != MuseumState.FreeRoam)
            {
                ResetGaze();
                return;
            }

            PerformGazeRaycast();
        }

        private void PerformGazeRaycast()
        {
            Ray ray = new Ray(headTransform.position, headTransform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxGazeDistance, gazeTargetLayerMask))
            {
                Transform hitTransform = hit.transform;

                if (hitTransform != currentGazeTarget)
                {
                    // New target acquired
                    ResetGaze();
                    currentGazeTarget = hitTransform;
                }
                else
                {
                    // Still looking at the same target
                    currentDwellTimer += Time.deltaTime;

                    if (currentDwellTimer >= dwellTimeRequired)
                    {
                        TriggerGazeAction(currentGazeTarget);
                        ResetGaze(); // Reset so it doesn't trigger repeatedly immediately
                    }
                }
            }
            else
            {
                // Looking at nothing
                ResetGaze();
            }
        }

        private void ResetGaze()
        {
            currentGazeTarget = null;
            currentDwellTimer = 0f;
        }

        private void TriggerGazeAction(Transform target)
        {
            // Try to find a component on the target that handles what happens when gazed at
            var gazeTargetComponent = target.GetComponent<IGazeInteractable>();
            if (gazeTargetComponent != null)
            {
                gazeTargetComponent.OnGazeDwellCompleted();
            }
            else
            {
                Debug.Log($"[GazeInteraction] Triggered gaze on {target.name}, but no IGazeInteractable found.");
            }
        }
    }

    /// <summary>
    /// Interface for objects that can be interacted with via gaze.
    /// </summary>
    public interface IGazeInteractable
    {
        void OnGazeDwellCompleted();
    }
}
