// =============================================================================
// OcclusionCullingManager.cs — Room-Based Manual Occlusion
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================

using System.Collections.Generic;
using UnityEngine;
using VirtualMuseumVR.Core;

namespace VirtualMuseumVR.Optimization
{
    public class OcclusionCullingManager : MonoBehaviour
    {
        [Header("Room References")]
        [SerializeField] private List<RoomRendererGroup> roomGroups = new List<RoomRendererGroup>();

        [Header("Settings")]
        [Tooltip("Keep corridor always visible (it's the hub)")]
        [SerializeField] private bool alwaysShowCorridor = true;
        [SerializeField] private string corridorRoomId = "corridor";

        private string _currentRoomId;
        private Dictionary<string, RoomRendererGroup> _groupLookup;

        [System.Serializable]
        public class RoomRendererGroup
        {
            public string roomId;
            public Renderer[] renderers;
            public ParticleSystem[] particles;
            public Light[] lights;
        }

        private void Awake()
        {
            _groupLookup = new Dictionary<string, RoomRendererGroup>();
            foreach (var group in roomGroups)
                _groupLookup[group.roomId] = group;
        }

        private void OnEnable()
        {
            GameEvents.OnRoomEntered += HandleRoomEntered;
        }

        private void OnDisable()
        {
            GameEvents.OnRoomEntered -= HandleRoomEntered;
        }

        private void HandleRoomEntered(string roomId)
        {
            _currentRoomId = roomId;
            UpdateOcclusion();
        }

        private void UpdateOcclusion()
        {
            foreach (var group in roomGroups)
            {
                bool shouldBeVisible = group.roomId == _currentRoomId ||
                    (alwaysShowCorridor && group.roomId == corridorRoomId);

                SetGroupVisible(group, shouldBeVisible);
            }
        }

        private void SetGroupVisible(RoomRendererGroup group, bool visible)
        {
            if (group.renderers != null)
            {
                foreach (var r in group.renderers)
                {
                    if (r != null) r.enabled = visible;
                }
            }

            if (group.particles != null)
            {
                foreach (var ps in group.particles)
                {
                    if (ps == null) continue;
                    if (visible && !ps.isPlaying) ps.Play();
                    else if (!visible && ps.isPlaying) ps.Stop();
                }
            }

            if (group.lights != null)
            {
                foreach (var l in group.lights)
                {
                    if (l != null) l.enabled = visible;
                }
            }
        }

        /// <summary>Force all rooms to be visible (for debugging).</summary>
        public void ShowAllRooms()
        {
            foreach (var group in roomGroups)
                SetGroupVisible(group, true);
        }

        /// <summary>Get count of currently active renderers.</summary>
        public int GetActiveRendererCount()
        {
            int count = 0;
            foreach (var group in roomGroups)
            {
                if (group.renderers == null) continue;
                foreach (var r in group.renderers)
                {
                    if (r != null && r.enabled) count++;
                }
            }
            return count;
        }
    }
}
