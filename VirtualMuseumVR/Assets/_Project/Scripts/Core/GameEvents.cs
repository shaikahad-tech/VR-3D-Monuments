// =============================================================================
// GameEvents.cs — Static Event Bus for Decoupled Messaging
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================
// Provides a centralized, static event system so that scripts can communicate
// without direct references to each other. This follows the Observer pattern
// and keeps the codebase modular and testable.
// =============================================================================

using System;
using UnityEngine;

namespace VirtualMuseumVR.Core
{
    /// <summary>
    /// Static event bus for all museum-wide events.
    /// Subscribe in OnEnable, unsubscribe in OnDisable to avoid memory leaks.
    /// </summary>
    public static class GameEvents
    {
        // =====================================================================
        // Room Events
        // =====================================================================

        /// <summary>
        /// Fired when the player physically enters a new room trigger zone.
        /// Parameter: RoomId (string identifier matching RoomConfigSO).
        /// </summary>
        public static event Action<string> OnRoomEntered;

        /// <summary>
        /// Fired when the player exits a room trigger zone.
        /// </summary>
        public static event Action<string> OnRoomExited;

        /// <summary>
        /// Fired when a room transition animation begins (fade-to-black).
        /// </summary>
        public static event Action<string, string> OnRoomTransitionStarted; // fromRoomId, toRoomId

        /// <summary>
        /// Fired when a room transition animation completes.
        /// </summary>
        public static event Action<string> OnRoomTransitionCompleted; // newRoomId

        // =====================================================================
        // Exhibit Events
        // =====================================================================

        /// <summary>
        /// Fired when a monument/exhibit is grabbed by the player.
        /// </summary>
        public static event Action<string> OnExhibitGrabbed; // exhibitId

        /// <summary>
        /// Fired when a monument/exhibit is released by the player.
        /// </summary>
        public static event Action<string> OnExhibitReleased; // exhibitId

        /// <summary>
        /// Fired when the player enters detailed inspection mode on an exhibit.
        /// </summary>
        public static event Action<string> OnInspectionStarted; // exhibitId

        /// <summary>
        /// Fired when the player exits detailed inspection mode.
        /// </summary>
        public static event Action<string> OnInspectionEnded; // exhibitId

        /// <summary>
        /// Fired when the player enters an exhibit's proximity trigger zone.
        /// </summary>
        public static event Action<string> OnExhibitProximityEntered; // exhibitId

        /// <summary>
        /// Fired when the player leaves an exhibit's proximity trigger zone.
        /// </summary>
        public static event Action<string> OnExhibitProximityExited; // exhibitId

        // =====================================================================
        // Tour Events
        // =====================================================================

        /// <summary>
        /// Fired when the guided tour advances to a new step/waypoint.
        /// </summary>
        public static event Action<int, int> OnTourStepChanged; // currentStep, totalSteps

        /// <summary>
        /// Fired when the guided tour starts.
        /// </summary>
        public static event Action OnTourStarted;

        /// <summary>
        /// Fired when the guided tour ends (completed or cancelled).
        /// </summary>
        public static event Action<bool> OnTourEnded; // wasCompleted

        // =====================================================================
        // UI Events
        // =====================================================================

        /// <summary>
        /// Fired when the main menu is opened.
        /// </summary>
        public static event Action OnMainMenuOpened;

        /// <summary>
        /// Fired when the main menu is closed.
        /// </summary>
        public static event Action OnMainMenuClosed;

        /// <summary>
        /// Fired when comfort settings are changed.
        /// </summary>
        public static event Action OnComfortSettingsChanged;

        // =====================================================================
        // Performance Events
        // =====================================================================

        /// <summary>
        /// Fired when performance drops below acceptable threshold.
        /// Parameter: current FPS.
        /// </summary>
        public static event Action<float> OnPerformanceWarning;

        /// <summary>
        /// Fired when quality level is auto-adjusted.
        /// </summary>
        public static event Action<int> OnQualityLevelChanged; // newQualityLevel
        
        // =====================================================================
        // Locomotion Events
        // =====================================================================

        /// <summary>
        /// Fired when the locomotion mode changes between continuous and teleport.
        /// Parameter: true if continuous movement is enabled, false if teleportation is enabled.
        /// </summary>
        public static event Action<bool> OnMovementModeChanged;

        // =====================================================================
        // Path Graph Events
        // =====================================================================

        /// <summary>
        /// Fired when a previously locked path edge becomes traversable.
        /// Parameter: edgeId from MultiPathNavigationSystem.
        /// </summary>
        public static event Action<string> OnPathUnlocked;

        /// <summary>
        /// Fired when the player traverses an edge in the path graph.
        /// Parameters: fromNodeId, toNodeId, edgeId.
        /// </summary>
        public static event Action<string, string, string> OnPathTraversed;

        // =====================================================================
        // Event Invocation Methods (Thread-safe null checks)
        // =====================================================================

        // --- Room ---
        public static void RaiseRoomEntered(string roomId) =>
            OnRoomEntered?.Invoke(roomId);

        public static void RaiseRoomExited(string roomId) =>
            OnRoomExited?.Invoke(roomId);

        public static void RaiseRoomTransitionStarted(string fromRoomId, string toRoomId) =>
            OnRoomTransitionStarted?.Invoke(fromRoomId, toRoomId);

        public static void RaiseRoomTransitionCompleted(string newRoomId) =>
            OnRoomTransitionCompleted?.Invoke(newRoomId);

        // --- Exhibit ---
        public static void RaiseExhibitGrabbed(string exhibitId) =>
            OnExhibitGrabbed?.Invoke(exhibitId);

        public static void RaiseExhibitReleased(string exhibitId) =>
            OnExhibitReleased?.Invoke(exhibitId);

        public static void RaiseInspectionStarted(string exhibitId) =>
            OnInspectionStarted?.Invoke(exhibitId);

        public static void RaiseInspectionEnded(string exhibitId) =>
            OnInspectionEnded?.Invoke(exhibitId);

        public static void RaiseExhibitProximityEntered(string exhibitId) =>
            OnExhibitProximityEntered?.Invoke(exhibitId);

        public static void RaiseExhibitProximityExited(string exhibitId) =>
            OnExhibitProximityExited?.Invoke(exhibitId);

        // --- Tour ---
        public static void RaiseTourStepChanged(int currentStep, int totalSteps) =>
            OnTourStepChanged?.Invoke(currentStep, totalSteps);

        public static void RaiseTourStarted() =>
            OnTourStarted?.Invoke();

        public static void RaiseTourEnded(bool wasCompleted) =>
            OnTourEnded?.Invoke(wasCompleted);

        // --- UI ---
        public static void RaiseMainMenuOpened() =>
            OnMainMenuOpened?.Invoke();

        public static void RaiseMainMenuClosed() =>
            OnMainMenuClosed?.Invoke();

        public static void RaiseComfortSettingsChanged() =>
            OnComfortSettingsChanged?.Invoke();

        // --- Performance ---
        public static void RaisePerformanceWarning(float fps) =>
            OnPerformanceWarning?.Invoke(fps);

        public static void RaiseQualityLevelChanged(int newLevel) =>
            OnQualityLevelChanged?.Invoke(newLevel);
            
        // --- Locomotion ---
        public static void RaiseMovementModeChanged(bool useContinuous) =>
            OnMovementModeChanged?.Invoke(useContinuous);

        // --- Path Graph ---
        public static void RaisePathUnlocked(string edgeId) =>
            OnPathUnlocked?.Invoke(edgeId);

        public static void RaisePathTraversed(string fromNodeId, string toNodeId, string edgeId) =>
            OnPathTraversed?.Invoke(fromNodeId, toNodeId, edgeId);
    }
}
