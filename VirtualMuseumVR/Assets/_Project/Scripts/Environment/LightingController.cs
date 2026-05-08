// =============================================================================
// LightingController.cs — Per-Room Baked + Accent Lighting
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================
// Paper recommendation: use baked lighting for mobile VR performance,
// with dynamic accent spotlights only for interactive highlighting.
// =============================================================================

using System;
using System.Collections.Generic;
using UnityEngine;
using VirtualMuseumVR.Core;

namespace VirtualMuseumVR.Environment
{
    [Serializable]
    public class RoomLightingPreset
    {
        public string roomId;
        public Color ambientColor = new Color(0.15f, 0.15f, 0.2f);
        [Range(0f, 2f)] public float ambientIntensity = 0.8f;
        public Color fogColor = new Color(0.1f, 0.1f, 0.15f);
        [Range(0f, 0.1f)] public float fogDensity = 0.02f;
        public bool enableFog = true;
    }

    public class LightingController : MonoBehaviour
    {
        [Header("Room Lighting Presets")]
        [SerializeField] private List<RoomLightingPreset> roomPresets = new List<RoomLightingPreset>()
        {
            new RoomLightingPreset
            {
                roomId = "corridor",
                ambientColor = new Color(0.12f, 0.12f, 0.18f),
                ambientIntensity = 0.6f
            },
            new RoomLightingPreset
            {
                roomId = "iraq",
                ambientColor = new Color(0.2f, 0.15f, 0.1f), // warm sandstone
                ambientIntensity = 0.7f
            },
            new RoomLightingPreset
            {
                roomId = "prague",
                ambientColor = new Color(0.1f, 0.12f, 0.2f), // cool blue
                ambientIntensity = 0.8f
            },
            new RoomLightingPreset
            {
                roomId = "shards",
                ambientColor = new Color(0.18f, 0.18f, 0.16f), // neutral
                ambientIntensity = 0.9f
            },
            new RoomLightingPreset
            {
                roomId = "aerial",
                ambientColor = new Color(0.15f, 0.17f, 0.2f), // sky tint
                ambientIntensity = 0.75f
            },
            new RoomLightingPreset
            {
                roomId = "india",
                ambientColor = new Color(0.25f, 0.18f, 0.08f), // warm saffron/amber
                ambientIntensity = 0.85f,
                fogColor = new Color(0.15f, 0.1f, 0.05f),
                fogDensity = 0.015f
            }
        };

        [Header("Exhibit Spotlights")]
        [SerializeField] private float spotlightFadeSpeed = 3f;
        [SerializeField] private float defaultSpotlightAngle = 45f;
        [SerializeField] private float defaultSpotlightRange = 5f;

        [Header("Transition")]
        [SerializeField] private float lightTransitionSpeed = 2f;

        private Dictionary<string, RoomLightingPreset> _presetLookup;
        private RoomLightingPreset _currentPreset;
        private RoomLightingPreset _targetPreset;
        private float _transitionProgress = 1f;

        private void Awake()
        {
            _presetLookup = new Dictionary<string, RoomLightingPreset>();
            foreach (var preset in roomPresets)
                _presetLookup[preset.roomId] = preset;
        }

        private void OnEnable()
        {
            GameEvents.OnRoomEntered += HandleRoomEntered;
        }

        private void OnDisable()
        {
            GameEvents.OnRoomEntered -= HandleRoomEntered;
        }

        private void Update()
        {
            if (_transitionProgress < 1f && _currentPreset != null && _targetPreset != null)
            {
                _transitionProgress += Time.deltaTime * lightTransitionSpeed;
                _transitionProgress = Mathf.Clamp01(_transitionProgress);

                // Lerp ambient lighting
                RenderSettings.ambientLight = Color.Lerp(
                    _currentPreset.ambientColor,
                    _targetPreset.ambientColor,
                    _transitionProgress
                );

                RenderSettings.ambientIntensity = Mathf.Lerp(
                    _currentPreset.ambientIntensity,
                    _targetPreset.ambientIntensity,
                    _transitionProgress
                );

                // Lerp fog
                if (_targetPreset.enableFog)
                {
                    RenderSettings.fog = true;
                    RenderSettings.fogColor = Color.Lerp(
                        _currentPreset.fogColor,
                        _targetPreset.fogColor,
                        _transitionProgress
                    );
                    RenderSettings.fogDensity = Mathf.Lerp(
                        _currentPreset.fogDensity,
                        _targetPreset.fogDensity,
                        _transitionProgress
                    );
                }

                if (_transitionProgress >= 1f)
                    _currentPreset = _targetPreset;
            }
        }

        private void HandleRoomEntered(string roomId)
        {
            if (_presetLookup.TryGetValue(roomId, out var preset))
            {
                TransitionToPreset(preset);
            }
        }

        private void TransitionToPreset(RoomLightingPreset target)
        {
            if (_currentPreset == null)
            {
                // First load — apply immediately
                _currentPreset = target;
                _targetPreset = target;
                ApplyPresetImmediate(target);
                return;
            }

            _targetPreset = target;
            _transitionProgress = 0f;
        }

        private void ApplyPresetImmediate(RoomLightingPreset preset)
        {
            RenderSettings.ambientLight = preset.ambientColor;
            RenderSettings.ambientIntensity = preset.ambientIntensity;
            RenderSettings.fog = preset.enableFog;
            RenderSettings.fogColor = preset.fogColor;
            RenderSettings.fogDensity = preset.fogDensity;
        }

        /// <summary>Create a dynamic spotlight for an exhibit.</summary>
        public Light CreateExhibitSpotlight(Transform exhibitTransform, ExhibitData data)
        {
            var spotObj = new GameObject($"Spotlight_{data.exhibitId}");
            spotObj.transform.SetParent(exhibitTransform);
            spotObj.transform.localPosition = new Vector3(0, 3f, 0);
            spotObj.transform.LookAt(exhibitTransform);

            var light = spotObj.AddComponent<Light>();
            light.type = LightType.Spot;
            light.color = data.spotlightColor;
            light.intensity = 0f; // Starts off, faded in by trigger zone
            light.spotAngle = defaultSpotlightAngle;
            light.range = defaultSpotlightRange;
            light.renderMode = LightRenderMode.ForcePixel;

            return light;
        }
    }
}
