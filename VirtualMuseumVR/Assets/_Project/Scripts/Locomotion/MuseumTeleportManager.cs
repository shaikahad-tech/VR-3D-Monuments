// =============================================================================
// MuseumTeleportManager.cs — Teleportation with Waypoint System
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using VirtualMuseumVR.Core;

namespace VirtualMuseumVR.Locomotion
{
    [System.Serializable]
    public class TeleportWaypoint
    {
        public string waypointId;
        public string displayName;
        public Transform anchorTransform;
        public string targetRoomId;
        public bool isRoomDoorway;
        [Tooltip("Order in guided tour sequence (0 = not part of tour)")]
        public int tourOrder;
    }

    public class MuseumTeleportManager : MonoBehaviour
    {
        [Header("Waypoints")]
        [SerializeField] private List<TeleportWaypoint> waypoints = new List<TeleportWaypoint>();

        [Header("Teleport Visual")]
        [SerializeField] private Material teleportArcMaterial;
        [SerializeField] private Material validTargetMaterial;
        [SerializeField] private Material invalidTargetMaterial;
        [SerializeField] private Color arcColor = new Color(0.3f, 0.6f, 1f, 0.8f);

        [Header("Settings")]
        [SerializeField] private float teleportFadeDuration = 0.3f;
        [SerializeField] private bool hapticOnTeleport = true;
        [SerializeField] private float hapticAmplitude = 0.3f;

        [Header("References")]
        [SerializeField] private TeleportationProvider teleportationProvider;

        private Dictionary<string, TeleportWaypoint> _waypointLookup;

        private void Awake()
        {
            _waypointLookup = new Dictionary<string, TeleportWaypoint>();
            foreach (var wp in waypoints)
            {
                if (!string.IsNullOrEmpty(wp.waypointId))
                    _waypointLookup[wp.waypointId] = wp;
            }
        }

        private void OnEnable()
        {
            // Subscribe to teleport events
            var anchors = FindObjectsByType<TeleportationAnchor>(FindObjectsSortMode.None);
            foreach (var anchor in anchors)
            {
                anchor.teleporting.AddListener(OnTeleportPerformed);
            }
            GameEvents.OnMovementModeChanged += HandleMovementModeChanged;
        }

        private void OnDisable()
        {
            var anchors = FindObjectsByType<TeleportationAnchor>(FindObjectsSortMode.None);
            foreach (var anchor in anchors)
            {
                anchor.teleporting.RemoveListener(OnTeleportPerformed);
            }
            GameEvents.OnMovementModeChanged -= HandleMovementModeChanged;
        }

        private void HandleMovementModeChanged(bool useContinuousMovement)
        {
            if (teleportationProvider != null)
            {
                teleportationProvider.enabled = !useContinuousMovement;
            }
            
            ContinuousMovementProvider cmp = FindAnyObjectByType<ContinuousMovementProvider>();
            if (cmp != null)
            {
                cmp.SetMovementEnabled(useContinuousMovement);
            }
            
            SnapTurnProvider stp = FindAnyObjectByType<SnapTurnProvider>();
            if (stp != null)
            {
                stp.SetTurningEnabled(useContinuousMovement); // Turn snap turning on when continuous is on
            }
        }

        private void OnTeleportPerformed(TeleportingEventArgs args)
        {
            // Check if this teleport target is a room doorway
            foreach (var wp in waypoints)
            {
                if (wp.isRoomDoorway && wp.anchorTransform != null)
                {
                    float dist = Vector3.Distance(
                        args.teleportRequest.destinationPosition,
                        wp.anchorTransform.position
                    );

                    if (dist < 1f)
                    {
                        // Trigger room transition
                        VirtualMuseumManager.Instance.TransitionToRoom(wp.targetRoomId);
                        Debug.Log($"[Teleport] Room doorway reached: {wp.displayName} → {wp.targetRoomId}");
                        break;
                    }
                }
            }
        }

        /// <summary>Teleport player to a specific waypoint by ID.</summary>
        public void TeleportToWaypoint(string waypointId)
        {
            if (!_waypointLookup.TryGetValue(waypointId, out var wp))
            {
                Debug.LogWarning($"[Teleport] Waypoint not found: {waypointId}");
                return;
            }

            if (wp.anchorTransform == null)
            {
                Debug.LogWarning($"[Teleport] Waypoint has no anchor: {waypointId}");
                return;
            }

            // If it's a room doorway, trigger room transition
            if (wp.isRoomDoorway && !string.IsNullOrEmpty(wp.targetRoomId))
            {
                VirtualMuseumManager.Instance.TransitionToRoom(wp.targetRoomId);
                return;
            }

            // Direct teleport within current room
            if (teleportationProvider != null)
            {
                var request = new TeleportRequest
                {
                    destinationPosition = wp.anchorTransform.position,
                    destinationRotation = wp.anchorTransform.rotation,
                    matchOrientation = MatchOrientation.TargetUpAndForward
                };
                teleportationProvider.QueueTeleportRequest(request);
            }

            Debug.Log($"[Teleport] Teleported to: {wp.displayName}");
        }

        /// <summary>Get the tour-ordered list of waypoints.</summary>
        public List<TeleportWaypoint> GetTourWaypoints()
        {
            var tourWaypoints = new List<TeleportWaypoint>();
            foreach (var wp in waypoints)
            {
                if (wp.tourOrder > 0)
                    tourWaypoints.Add(wp);
            }
            tourWaypoints.Sort((a, b) => a.tourOrder.CompareTo(b.tourOrder));
            return tourWaypoints;
        }

        /// <summary>Get the nearest waypoint to a position.</summary>
        public TeleportWaypoint GetNearestWaypoint(Vector3 position)
        {
            TeleportWaypoint nearest = null;
            float nearestDist = float.MaxValue;

            foreach (var wp in waypoints)
            {
                if (wp.anchorTransform == null) continue;
                float dist = Vector3.Distance(position, wp.anchorTransform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = wp;
                }
            }
            return nearest;
        }
    }
}
