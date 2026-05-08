using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;
using VirtualMuseumVR.Core;

namespace VirtualMuseumVR.Interaction
{
    /// <summary>
    /// A complex interaction system where an exhibit is broken into multiple shards.
    /// The user must place the shards in correct placeholder positions to restore the monument.
    /// </summary>
    public class MonumentRestorationPuzzle : MonoBehaviour
    {
        [System.Serializable]
        public class PuzzlePiece
        {
            public string pieceId;
            public XRGrabInteractable interactable;
            public Transform correctPositionTarget;
            public float snapDistanceThreshold = 0.1f;
            public float snapAngleThreshold = 15f;
            [HideInInspector] public bool isPlacedCorrectly = false;
        }

        [Header("Puzzle Settings")]
        [Tooltip("The unique ID of the exhibit this puzzle belongs to.")]
        public string exhibitId;
        public List<PuzzlePiece> pieces = new List<PuzzlePiece>();
        
        [Header("Feedback")]
        public AudioSource audioSource;
        public AudioClip pieceSnappedSound;
        public AudioClip puzzleCompleteSound;
        public ParticleSystem puzzleCompleteParticles;

        private bool isPuzzleComplete = false;
        
        // Caching for performance
        private WaitForSeconds checkInterval = new WaitForSeconds(0.5f);

        private void OnEnable()
        {
            foreach (var piece in pieces)
            {
                if (piece.interactable != null)
                {
                    piece.interactable.selectExited.AddListener(OnPieceReleased);
                }
            }
        }

        private void OnDisable()
        {
            foreach (var piece in pieces)
            {
                if (piece.interactable != null)
                {
                    piece.interactable.selectExited.RemoveListener(OnPieceReleased);
                }
            }
        }

        private void OnPieceReleased(SelectExitEventArgs args)
        {
            if (isPuzzleComplete) return;

            XRGrabInteractable releasedInteractable = args.interactableObject as XRGrabInteractable;
            if (releasedInteractable != null)
            {
                CheckPiecePlacement(releasedInteractable);
            }
        }

        private void CheckPiecePlacement(XRGrabInteractable interactable)
        {
            foreach (var piece in pieces)
            {
                if (piece.interactable == interactable && !piece.isPlacedCorrectly)
                {
                    float distance = Vector3.Distance(interactable.transform.position, piece.correctPositionTarget.position);
                    float angle = Quaternion.Angle(interactable.transform.rotation, piece.correctPositionTarget.rotation);

                    if (distance <= piece.snapDistanceThreshold && angle <= piece.snapAngleThreshold)
                    {
                        SnapPiece(piece);
                    }
                    break;
                }
            }
        }

        private void SnapPiece(PuzzlePiece piece)
        {
            piece.isPlacedCorrectly = true;
            
            // Snap position and rotation exactly
            piece.interactable.transform.position = piece.correctPositionTarget.position;
            piece.interactable.transform.rotation = piece.correctPositionTarget.rotation;
            
            // Disable interaction
            piece.interactable.enabled = false;
            
            // Disable physics
            Rigidbody rb = piece.interactable.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }

            // Feedback
            if (audioSource != null && pieceSnappedSound != null)
            {
                audioSource.PlayOneShot(pieceSnappedSound);
            }

            CheckPuzzleCompletion();
        }

        private void CheckPuzzleCompletion()
        {
            foreach (var piece in pieces)
            {
                if (!piece.isPlacedCorrectly)
                {
                    return; // Not complete yet
                }
            }

            isPuzzleComplete = true;
            OnPuzzleCompleted();
        }

        private void OnPuzzleCompleted()
        {
            Debug.Log($"[Restoration] Puzzle completed for exhibit: {exhibitId}");
            
            if (audioSource != null && puzzleCompleteSound != null)
            {
                audioSource.PlayOneShot(puzzleCompleteSound);
            }

            if (puzzleCompleteParticles != null)
            {
                puzzleCompleteParticles.Play();
            }
            
            // Allow this completed exhibit to be inspected/grabbed as a whole now
            // GameEvents.RaiseRestorationCompleted(exhibitId); // E.g., unlock the next interaction tier
        }
    }
}
