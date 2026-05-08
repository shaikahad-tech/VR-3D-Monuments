// =============================================================================
// AmbientAudioManager.cs — Per-Room Ambient Soundscapes
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualMuseumVR.Core;

namespace VirtualMuseumVR.Audio
{
    [System.Serializable]
    public class RoomAudioConfig
    {
        public string roomId;
        public AudioClip ambientLoop;
        [Range(0f, 1f)] public float volume = 0.3f;
    }

    public class AmbientAudioManager : MonoBehaviour
    {
        [Header("Room Audio")]
        [SerializeField] private List<RoomAudioConfig> roomAudioConfigs = new List<RoomAudioConfig>();

        [Header("Crossfade")]
        [SerializeField] private float crossfadeDuration = 1.5f;

        [Header("Narration Ducking")]
        [SerializeField] private float duckVolume = 0.1f;
        [SerializeField] private float duckFadeSpeed = 2f;

        private AudioSource _sourceA;
        private AudioSource _sourceB;
        private bool _useSourceA = true;
        private float _masterVolume = 1f;
        private bool _isDucking;
        private Dictionary<string, RoomAudioConfig> _configLookup;

        private void Awake()
        {
            _sourceA = gameObject.AddComponent<AudioSource>();
            _sourceB = gameObject.AddComponent<AudioSource>();
            ConfigureSource(_sourceA);
            ConfigureSource(_sourceB);

            _configLookup = new Dictionary<string, RoomAudioConfig>();
            foreach (var config in roomAudioConfigs)
                _configLookup[config.roomId] = config;
        }

        private void ConfigureSource(AudioSource source)
        {
            source.loop = true;
            source.playOnAwake = false;
            source.spatialBlend = 0f; // 2D (ambient)
            source.volume = 0f;
        }

        private void OnEnable()
        {
            GameEvents.OnRoomEntered += HandleRoomEntered;
            GameEvents.OnExhibitProximityEntered += HandleNarrationStart;
            GameEvents.OnExhibitProximityExited += HandleNarrationEnd;
        }

        private void OnDisable()
        {
            GameEvents.OnRoomEntered -= HandleRoomEntered;
            GameEvents.OnExhibitProximityEntered -= HandleNarrationStart;
            GameEvents.OnExhibitProximityExited -= HandleNarrationEnd;
        }

        private void HandleRoomEntered(string roomId)
        {
            if (_configLookup.TryGetValue(roomId, out var config))
            {
                CrossfadeTo(config.ambientLoop, config.volume);
            }
        }

        private void CrossfadeTo(AudioClip clip, float targetVolume)
        {
            if (clip == null) return;
            StartCoroutine(CrossfadeCoroutine(clip, targetVolume));
        }

        private IEnumerator CrossfadeCoroutine(AudioClip newClip, float targetVolume)
        {
            var outSource = _useSourceA ? _sourceA : _sourceB;
            var inSource = _useSourceA ? _sourceB : _sourceA;

            inSource.clip = newClip;
            inSource.volume = 0f;
            inSource.Play();

            float elapsed = 0f;
            float outStartVolume = outSource.volume;

            while (elapsed < crossfadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / crossfadeDuration;

                outSource.volume = Mathf.Lerp(outStartVolume, 0f, t);
                inSource.volume = Mathf.Lerp(0f, targetVolume * _masterVolume, t);

                // Apply ducking
                if (_isDucking)
                    inSource.volume *= duckVolume;

                yield return null;
            }

            outSource.Stop();
            outSource.volume = 0f;
            inSource.volume = targetVolume * _masterVolume;

            _useSourceA = !_useSourceA;
        }

        private void HandleNarrationStart(string exhibitId)
        {
            _isDucking = true;
            // Duck ambient audio during narration
            var active = _useSourceA ? _sourceA : _sourceB;
            StartCoroutine(DuckCoroutine(active, duckVolume));
        }

        private void HandleNarrationEnd(string exhibitId)
        {
            _isDucking = false;
            // Restore ambient audio
            var active = _useSourceA ? _sourceA : _sourceB;
            if (_configLookup.TryGetValue(
                VirtualMuseumManager.Instance?.CurrentRoomId ?? "", out var config))
            {
                StartCoroutine(DuckCoroutine(active, config.volume * _masterVolume));
            }
        }

        private IEnumerator DuckCoroutine(AudioSource source, float targetVolume)
        {
            float start = source.volume;
            float elapsed = 0f;
            float duration = 1f / duckFadeSpeed;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                source.volume = Mathf.Lerp(start, targetVolume, elapsed / duration);
                yield return null;
            }
            source.volume = targetVolume;
        }

        public void SetMasterVolume(float volume)
        {
            _masterVolume = Mathf.Clamp01(volume);
        }
    }
}
