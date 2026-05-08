// =============================================================================
// VirtualMuseumManager.cs — Singleton App Lifecycle Manager
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================

using System;
using System.Collections;
using UnityEngine;

namespace VirtualMuseumVR.Core
{
    public enum AppState
    {
        Initializing, MainMenu, FreeRoam, GuidedTour,
        Inspecting, RoomTransition, Paused
    }

    public class VirtualMuseumManager : MonoBehaviour
    {
        private static VirtualMuseumManager _instance;
        public static VirtualMuseumManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<VirtualMuseumManager>();
                    if (_instance == null)
                    {
                        var go = new GameObject("[VirtualMuseumManager]");
                        _instance = go.AddComponent<VirtualMuseumManager>();
                    }
                }
                return _instance;
            }
        }

        [Header("Museum Configuration")]
        [SerializeField] private int targetFrameRate = 72;
        [SerializeField] private float roomTransitionDuration = 1.5f;
        [SerializeField] private string defaultRoomId = "corridor";

        [Header("Debug")]
        [SerializeField] private bool showDebugUI = false;
        [SerializeField] private bool logEvents = true;

        private AppState _currentState = AppState.Initializing;
        private AppState _previousState;

        public AppState CurrentState
        {
            get => _currentState;
            private set
            {
                if (_currentState == value) return;
                _previousState = _currentState;
                _currentState = value;
                OnStateChanged?.Invoke(_previousState, _currentState);
                if (logEvents) Debug.Log($"[VirtualMuseum] State: {_previousState} → {_currentState}");
            }
        }

        public string CurrentRoomId { get; private set; }
        public string CurrentExhibitId { get; private set; }
        public bool IsInspecting => _currentState == AppState.Inspecting;
        public bool IsOnTour => _currentState == AppState.GuidedTour;
        public int TargetFrameRate => targetFrameRate;
        public float RoomTransitionDuration => roomTransitionDuration;
        public bool ShowDebugUI => showDebugUI;

        public event Action<AppState, AppState> OnStateChanged;

        private void Awake()
        {
            if (_instance != null && _instance != this) { Destroy(gameObject); return; }
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeApplication();
        }

        private void OnEnable() => SubscribeToEvents();
        private void OnDisable() => UnsubscribeFromEvents();
        private void OnDestroy() { if (_instance == this) _instance = null; }

        private void InitializeApplication()
        {
            Debug.Log("══════════════════════════════════════════════════");
            Debug.Log("  VIRTUAL MUSEUM VR — Initializing");
            Debug.Log("  Based on Pavelka & Raeva (2019), ISPRS Archives");
            Debug.Log("══════════════════════════════════════════════════");

            Application.targetFrameRate = targetFrameRate;
            QualitySettings.vSyncCount = 0;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            QualitySettings.maxQueuedFrames = 0;
            Physics.defaultSolverIterations = 4;
            Physics.defaultSolverVelocityIterations = 1;

            CurrentState = AppState.MainMenu;
            CurrentRoomId = defaultRoomId;
        }

        private void SubscribeToEvents()
        {
            GameEvents.OnRoomEntered += HandleRoomEntered;
            GameEvents.OnExhibitGrabbed += HandleExhibitGrabbed;
            GameEvents.OnExhibitReleased += HandleExhibitReleased;
            GameEvents.OnInspectionStarted += HandleInspectionStarted;
            GameEvents.OnInspectionEnded += HandleInspectionEnded;
            GameEvents.OnTourEnded += HandleTourEnded;
        }

        private void UnsubscribeFromEvents()
        {
            GameEvents.OnRoomEntered -= HandleRoomEntered;
            GameEvents.OnExhibitGrabbed -= HandleExhibitGrabbed;
            GameEvents.OnExhibitReleased -= HandleExhibitReleased;
            GameEvents.OnInspectionStarted -= HandleInspectionStarted;
            GameEvents.OnInspectionEnded -= HandleInspectionEnded;
            GameEvents.OnTourEnded -= HandleTourEnded;
        }

        public void StartFreeRoam()
        {
            CurrentState = AppState.FreeRoam;
            GameEvents.RaiseMainMenuClosed();
        }

        public void StartGuidedTour()
        {
            CurrentState = AppState.GuidedTour;
            GameEvents.RaiseTourStarted();
            GameEvents.RaiseMainMenuClosed();
        }

        public void ReturnToMainMenu()
        {
            if (IsOnTour) GameEvents.RaiseTourEnded(false);
            CurrentState = AppState.MainMenu;
            GameEvents.RaiseMainMenuOpened();
        }

        public void TransitionToRoom(string targetRoomId)
        {
            if (_currentState == AppState.RoomTransition) return;
            StartCoroutine(RoomTransitionCoroutine(targetRoomId));
        }

        private IEnumerator RoomTransitionCoroutine(string targetRoomId)
        {
            string previousRoom = CurrentRoomId;
            CurrentState = AppState.RoomTransition;
            GameEvents.RaiseRoomTransitionStarted(previousRoom, targetRoomId);

            yield return new WaitForSeconds(roomTransitionDuration * 0.5f);
            CurrentRoomId = targetRoomId;
            GameEvents.RaiseRoomEntered(targetRoomId);

            yield return new WaitForSeconds(roomTransitionDuration * 0.5f);
            CurrentState = IsOnTour ? AppState.GuidedTour : AppState.FreeRoam;
            GameEvents.RaiseRoomTransitionCompleted(targetRoomId);
        }

        private void HandleRoomEntered(string roomId) => CurrentRoomId = roomId;
        private void HandleExhibitGrabbed(string id) => CurrentExhibitId = id;
        private void HandleExhibitReleased(string id) => CurrentExhibitId = null;

        private void HandleInspectionStarted(string id)
        {
            _previousState = _currentState;
            CurrentState = AppState.Inspecting;
        }

        private void HandleInspectionEnded(string id) => CurrentState = _previousState;
        private void HandleTourEnded(bool completed) => CurrentState = AppState.FreeRoam;

        public float SessionTime => Time.realtimeSinceStartup;
        public string SessionTimeFormatted
        {
            get
            {
                var ts = TimeSpan.FromSeconds(SessionTime);
                return $"{ts.Hours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}";
            }
        }
    }
}
