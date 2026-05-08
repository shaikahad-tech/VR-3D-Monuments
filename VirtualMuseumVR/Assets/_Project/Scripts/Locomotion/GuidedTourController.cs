// =============================================================================
// GuidedTourController.cs — Auto-Guided Tour Through All 4 Rooms
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualMuseumVR.Core;

namespace VirtualMuseumVR.Locomotion
{
    public class GuidedTourController : MonoBehaviour
    {
        [Header("Tour Configuration")]
        [SerializeField] private MuseumTeleportManager teleportManager;
        [SerializeField] private float dwellTimePerExhibit = 15f;
        [SerializeField] private float transitionPause = 2f;
        [SerializeField] private bool autoAdvance = true;

        [Header("Audio")]
        [SerializeField] private AudioSource narrationSource;

        private List<TeleportWaypoint> _tourWaypoints;
        private int _currentStepIndex;
        private bool _isTourActive;
        private bool _isPaused;
        private Coroutine _tourCoroutine;

        public bool IsTourActive => _isTourActive;
        public bool IsPaused => _isPaused;
        public int CurrentStep => _currentStepIndex;
        public int TotalSteps => _tourWaypoints?.Count ?? 0;

        public float Progress => TotalSteps > 0
            ? (float)_currentStepIndex / TotalSteps
            : 0f;

        private void OnEnable()
        {
            GameEvents.OnTourStarted += HandleTourStarted;
            GameEvents.OnTourEnded += HandleTourEnded;
        }

        private void OnDisable()
        {
            GameEvents.OnTourStarted -= HandleTourStarted;
            GameEvents.OnTourEnded -= HandleTourEnded;
        }

        public void StartTour()
        {
            if (_isTourActive) StopTour(false);

            if (teleportManager == null)
            {
                Debug.LogError("[GuidedTour] No TeleportManager assigned!");
                return;
            }

            _tourWaypoints = teleportManager.GetTourWaypoints();
            if (_tourWaypoints.Count == 0)
            {
                Debug.LogWarning("[GuidedTour] No tour waypoints configured!");
                return;
            }

            _currentStepIndex = 0;
            _isTourActive = true;
            _isPaused = false;

            _tourCoroutine = StartCoroutine(TourSequence());
            GameEvents.RaiseTourStarted();

            Debug.Log($"[GuidedTour] Tour started with {_tourWaypoints.Count} stops.");
        }

        public void StopTour(bool completed)
        {
            _isTourActive = false;
            _isPaused = false;

            if (_tourCoroutine != null)
            {
                StopCoroutine(_tourCoroutine);
                _tourCoroutine = null;
            }

            if (narrationSource != null && narrationSource.isPlaying)
                narrationSource.Stop();

            GameEvents.RaiseTourEnded(completed);
            Debug.Log($"[GuidedTour] Tour ended. Completed: {completed}");
        }

        public void PauseTour() { _isPaused = true; }
        public void ResumeTour() { _isPaused = false; }

        public void SkipToNextStop()
        {
            if (!_isTourActive) return;
            _currentStepIndex++;
            // The coroutine will handle the skip
        }

        private IEnumerator TourSequence()
        {
            for (int i = 0; i < _tourWaypoints.Count; i++)
            {
                _currentStepIndex = i;
                var waypoint = _tourWaypoints[i];

                GameEvents.RaiseTourStepChanged(_currentStepIndex, _tourWaypoints.Count);

                // Teleport to waypoint
                teleportManager.TeleportToWaypoint(waypoint.waypointId);

                // Wait for transition
                yield return new WaitForSeconds(transitionPause);

                // Dwell at exhibit
                if (autoAdvance)
                {
                    float dwellTimer = 0f;
                    while (dwellTimer < dwellTimePerExhibit)
                    {
                        // Check for pause
                        while (_isPaused)
                            yield return null;

                        // Check for skip
                        if (_currentStepIndex != i) break;

                        dwellTimer += Time.deltaTime;
                        yield return null;
                    }
                }
                else
                {
                    // Wait for manual advance
                    int savedStep = _currentStepIndex;
                    while (_currentStepIndex == savedStep && _isTourActive)
                    {
                        while (_isPaused) yield return null;
                        yield return null;
                    }
                }
            }

            // Tour complete
            StopTour(true);
        }

        private void HandleTourStarted()
        {
            if (!_isTourActive) StartTour();
        }

        private void HandleTourEnded(bool completed)
        {
            if (_isTourActive && !completed) StopTour(false);
        }
    }
}
