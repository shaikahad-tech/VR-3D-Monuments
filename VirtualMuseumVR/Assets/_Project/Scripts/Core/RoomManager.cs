// =============================================================================
// RoomManager.cs — Room Activation/Loading Controller
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================
// Manages the 4-room + corridor museum layout described in the paper.
// Controls room activation/deactivation, transitions with fade effects,
// and room-specific configuration loading.
// =============================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualMuseumVR.Core
{
    [Serializable]
    public class RoomDefinition
    {
        public string roomId;
        public string displayName;
        public MuseumRoom roomType;
        public GameObject roomGameObject;
        public Transform playerSpawnPoint;
        public Color ambientLightColor = new Color(0.2f, 0.2f, 0.25f);
        [Range(0f, 2f)] public float ambientIntensity = 1f;
        public AudioClip ambientAudioClip;
        [TextArea(2, 4)] public string roomDescription;
    }

    public class RoomManager : MonoBehaviour
    {
        [Header("Room Definitions")]
        [Tooltip("All rooms in the museum, matching the paper's 4-room + corridor layout")]
        [SerializeField] private List<RoomDefinition> rooms = new List<RoomDefinition>();

        [Header("Transition")]
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private float fadeDuration = 0.75f;

        [Header("Settings")]
        [SerializeField] private string startingRoomId = "corridor";
        [SerializeField] private bool deactivateHiddenRooms = true;

        private string _activeRoomId;
        private Dictionary<string, RoomDefinition> _roomLookup;
        private bool _isTransitioning;

        public string ActiveRoomId => _activeRoomId;
        public RoomDefinition ActiveRoom => GetRoom(_activeRoomId);
        public bool IsTransitioning => _isTransitioning;

        public event Action<RoomDefinition> OnRoomActivated;
        public event Action<RoomDefinition> OnRoomDeactivated;

        private void Awake()
        {
            _roomLookup = new Dictionary<string, RoomDefinition>();
            foreach (var room in rooms)
            {
                if (!string.IsNullOrEmpty(room.roomId))
                    _roomLookup[room.roomId] = room;
            }
        }

        private void Start()
        {
            // Start with all rooms deactivated, then activate the starting room
            if (deactivateHiddenRooms)
            {
                foreach (var room in rooms)
                {
                    if (room.roomGameObject != null)
                        room.roomGameObject.SetActive(false);
                }
            }
            ActivateRoom(startingRoomId);
        }

        private void OnEnable()
        {
            GameEvents.OnRoomTransitionStarted += HandleRoomTransitionStarted;
        }

        private void OnDisable()
        {
            GameEvents.OnRoomTransitionStarted -= HandleRoomTransitionStarted;
        }

        public RoomDefinition GetRoom(string roomId)
        {
            if (string.IsNullOrEmpty(roomId)) return null;
            _roomLookup.TryGetValue(roomId, out var room);
            return room;
        }

        public void ActivateRoom(string roomId)
        {
            var room = GetRoom(roomId);
            if (room == null)
            {
                Debug.LogError($"[RoomManager] Room not found: {roomId}");
                return;
            }

            // Deactivate previous room
            if (!string.IsNullOrEmpty(_activeRoomId) && _activeRoomId != roomId)
            {
                var prevRoom = GetRoom(_activeRoomId);
                if (prevRoom?.roomGameObject != null && deactivateHiddenRooms)
                {
                    prevRoom.roomGameObject.SetActive(false);
                    OnRoomDeactivated?.Invoke(prevRoom);
                }
            }

            // Activate new room
            if (room.roomGameObject != null)
                room.roomGameObject.SetActive(true);

            _activeRoomId = roomId;

            // Apply room ambient settings
            RenderSettings.ambientLight = room.ambientLightColor;
            RenderSettings.ambientIntensity = room.ambientIntensity;

            OnRoomActivated?.Invoke(room);
            GameEvents.RaiseRoomEntered(roomId);

            Debug.Log($"[RoomManager] Activated room: {room.displayName} ({roomId})");
        }

        public void TransitionToRoom(string targetRoomId)
        {
            if (_isTransitioning) return;
            StartCoroutine(TransitionCoroutine(targetRoomId));
        }

        private IEnumerator TransitionCoroutine(string targetRoomId)
        {
            _isTransitioning = true;

            // Fade to black
            if (fadeCanvasGroup != null)
            {
                yield return StartCoroutine(FadeCoroutine(0f, 1f, fadeDuration));
            }

            // Switch room
            ActivateRoom(targetRoomId);

            // Teleport player to spawn point
            var room = GetRoom(targetRoomId);
            if (room?.playerSpawnPoint != null)
            {
                var xrOrigin = FindFirstObjectByType<Unity.XR.CoreUtils.XROrigin>();
                if (xrOrigin != null)
                {
                    xrOrigin.transform.position = room.playerSpawnPoint.position;
                    xrOrigin.transform.rotation = room.playerSpawnPoint.rotation;
                }
            }

            // Wait a frame for room to load
            yield return null;

            // Fade back in
            if (fadeCanvasGroup != null)
            {
                yield return StartCoroutine(FadeCoroutine(1f, 0f, fadeDuration));
            }

            _isTransitioning = false;
            GameEvents.RaiseRoomTransitionCompleted(targetRoomId);
        }

        private IEnumerator FadeCoroutine(float from, float to, float duration)
        {
            float elapsed = 0f;
            fadeCanvasGroup.alpha = from;
            fadeCanvasGroup.gameObject.SetActive(true);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
                yield return null;
            }

            fadeCanvasGroup.alpha = to;
            if (to <= 0f) fadeCanvasGroup.gameObject.SetActive(false);
        }

        private void HandleRoomTransitionStarted(string fromRoomId, string toRoomId)
        {
            TransitionToRoom(toRoomId);
        }

        /// <summary>Get the names of all rooms for UI display.</summary>
        public List<string> GetAllRoomNames()
        {
            var names = new List<string>();
            foreach (var room in rooms)
                names.Add(room.displayName);
            return names;
        }

        /// <summary>Get spawn position for a specific room.</summary>
        public Vector3 GetRoomSpawnPosition(string roomId)
        {
            var room = GetRoom(roomId);
            if (room?.playerSpawnPoint != null)
                return room.playerSpawnPoint.position;
            return Vector3.zero;
        }
    }
}
