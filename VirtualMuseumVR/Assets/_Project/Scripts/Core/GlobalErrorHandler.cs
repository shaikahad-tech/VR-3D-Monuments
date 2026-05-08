using System;
using UnityEngine;

namespace VirtualMuseumVR.Core
{
    /// <summary>
    /// Global exception and error handler to prevent the VR experience from crashing
    /// or getting stuck in an invalid state.
    /// </summary>
    public class GlobalErrorHandler : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("If true, handled errors will attempt to reset the museum to the FreeRoam state.")]
        public bool autoResetOnSevereError = true;
        
        [Header("UI Feedback")]
        [Tooltip("Canvas or UI object to show an error message to the user.")]
        public GameObject errorUI;
        public TMPro.TextMeshProUGUI errorText;

        private void OnEnable()
        {
            Application.logMessageReceived += HandleLogMessageReceived;
            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLogMessageReceived;
            AppDomain.CurrentDomain.UnhandledException -= HandleUnhandledException;
        }

        private void HandleLogMessageReceived(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Exception || type == LogType.Error)
            {
                HandleError($"Runtime Error: {logString}", stackTrace, type == LogType.Exception);
            }
        }

        private void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleError($"Unhandled Exception: {e.ExceptionObject}", "Stack trace unavailable here", true);
        }

        private void HandleError(string message, string stackTrace, bool isSevere)
        {
            Debug.LogError($"[GlobalErrorHandler] Caught error: {message}\nTrace: {stackTrace}");

            // Show UI to the user
            if (errorUI != null && errorText != null)
            {
                errorText.text = "An unexpected error occurred. The experience has been reset to prevent crashing.";
                errorUI.SetActive(true);
                Invoke(nameof(HideErrorUI), 5f); // Hide after 5 seconds
            }

            if (isSevere && autoResetOnSevereError)
            {
                AttemptRecovery();
            }
        }

        private void HideErrorUI()
        {
            if (errorUI != null)
            {
                errorUI.SetActive(false);
            }
        }

        private void AttemptRecovery()
        {
            // If we are stuck in an inspection or transition state, force back to free roam
            if (VirtualMuseumManager.Instance != null && VirtualMuseumManager.Instance.CurrentState != MuseumState.FreeRoam)
            {
                Debug.Log("[GlobalErrorHandler] Attempting to force state back to FreeRoam...");
                // Force state reset (would require a public method on VirtualMuseumManager, or we simulate it)
                // For now, we simulate releasing all objects.
                
                // Unparent anything grabbed
                var rightHand = GameObject.Find("RightHand Controller");
                var leftHand = GameObject.Find("LeftHand Controller");
                
                if (rightHand) rightHand.GetComponentInChildren<UnityEngine.XR.Interaction.Toolkit.XRDirectInteractor>()?.interactionManager.CancelInteractorSelection(rightHand.GetComponentInChildren<UnityEngine.XR.Interaction.Toolkit.XRDirectInteractor>());
                if (leftHand) leftHand.GetComponentInChildren<UnityEngine.XR.Interaction.Toolkit.XRDirectInteractor>()?.interactionManager.CancelInteractorSelection(leftHand.GetComponentInChildren<UnityEngine.XR.Interaction.Toolkit.XRDirectInteractor>());
            }
        }
    }
}
