// =============================================================================
// SpatialAudioTrigger.cs — 3D Positional Audio for Exhibits
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================

using UnityEngine;
using VirtualMuseumVR.Core;

namespace VirtualMuseumVR.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SpatialAudioTrigger : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private string exhibitId;
        [SerializeField] private ExhibitData exhibitData;
        [SerializeField] private AudioClip contextualSound;

        [Header("Spatial Settings")]
        [SerializeField] private float maxDistance = 5f;
        [SerializeField] private float minDistance = 0.5f;
        [SerializeField] private AnimationCurve falloffCurve = AnimationCurve.Linear(0, 1, 1, 0);

        [Header("Playback")]
        [SerializeField] private bool playOnProximity = true;
        [SerializeField] private bool loopAudio = false;
        [SerializeField] private float fadeInDuration = 0.5f;
        [SerializeField] private float fadeOutDuration = 0.3f;

        private AudioSource _audioSource;
        private float _targetVolume;
        private bool _isPlaying;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.spatialBlend = 1f; // Fully 3D
            _audioSource.rolloffMode = AudioRolloffMode.Custom;
            _audioSource.maxDistance = maxDistance;
            _audioSource.minDistance = minDistance;
            _audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, falloffCurve);
            _audioSource.loop = loopAudio;
            _audioSource.playOnAwake = false;

            // Use narration from exhibit data if available
            if (exhibitData?.narrationClip != null)
                _audioSource.clip = exhibitData.narrationClip;
            else if (contextualSound != null)
                _audioSource.clip = contextualSound;

            if (string.IsNullOrEmpty(exhibitId) && exhibitData != null)
                exhibitId = exhibitData.exhibitId;
        }

        private void OnEnable()
        {
            if (playOnProximity)
            {
                GameEvents.OnExhibitProximityEntered += HandleProximityEntered;
                GameEvents.OnExhibitProximityExited += HandleProximityExited;
            }
        }

        private void OnDisable()
        {
            GameEvents.OnExhibitProximityEntered -= HandleProximityEntered;
            GameEvents.OnExhibitProximityExited -= HandleProximityExited;
        }

        private void Update()
        {
            // Smooth volume transitions
            if (_audioSource.isPlaying)
            {
                float speed = _targetVolume > _audioSource.volume
                    ? 1f / fadeInDuration
                    : 1f / fadeOutDuration;
                _audioSource.volume = Mathf.MoveTowards(
                    _audioSource.volume, _targetVolume, Time.deltaTime * speed);

                // Stop when fully faded out
                if (_audioSource.volume < 0.01f && _targetVolume == 0f)
                {
                    _audioSource.Stop();
                    _isPlaying = false;
                }
            }
        }

        public void Play()
        {
            if (_audioSource.clip == null) return;
            _audioSource.volume = 0f;
            _targetVolume = 1f;
            _audioSource.Play();
            _isPlaying = true;
        }

        public void Stop()
        {
            _targetVolume = 0f;
        }

        private void HandleProximityEntered(string id)
        {
            if (id == exhibitId) Play();
        }

        private void HandleProximityExited(string id)
        {
            if (id == exhibitId) Stop();
        }
    }
}
