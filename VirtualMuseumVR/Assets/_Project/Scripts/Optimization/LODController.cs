// =============================================================================
// LODController.cs — Dynamic LOD Switching Per Monument
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================
// Paper methodology: decimate from ~100k to ~10k polygons. This controller
// manages LOD groups for runtime distance-based switching.
// LOD0: Full (~10k) < 2m, LOD1: Medium (~5k) 2-5m, LOD2: Low (~1k) > 5m
// =============================================================================

using UnityEngine;
using VirtualMuseumVR.Core;

namespace VirtualMuseumVR.Optimization
{
    public class LODController : MonoBehaviour
    {
        [Header("LOD Configuration")]
        [SerializeField] private float lod0Distance = 2f;
        [SerializeField] private float lod1Distance = 5f;
        [SerializeField] private float cullDistance = 15f;

        [Header("References")]
        [SerializeField] private LODGroup lodGroup;
        [SerializeField] private Renderer[] lod0Renderers;
        [SerializeField] private Renderer[] lod1Renderers;
        [SerializeField] private Renderer[] lod2Renderers;

        [Header("Auto-Setup")]
        [Tooltip("If true, automatically creates LOD group from child renderers")]
        [SerializeField] private bool autoSetup = false;

        private Transform _playerCamera;

        private void Start()
        {
            _playerCamera = Camera.main?.transform;

            if (autoSetup && lodGroup == null)
                SetupLODGroup();
        }

        private void SetupLODGroup()
        {
            lodGroup = GetComponent<LODGroup>();
            if (lodGroup == null)
                lodGroup = gameObject.AddComponent<LODGroup>();

            // Calculate screen-relative transition heights
            // These are approximate screen heights where LOD transitions occur
            float lod0Height = 0.6f;  // High detail when filling 60%+ of screen
            float lod1Height = 0.3f;  // Medium detail at 30%
            float lod2Height = 0.1f;  // Low detail at 10%

            var lods = new LOD[3];

            lods[0] = new LOD(lod0Height,
                lod0Renderers != null && lod0Renderers.Length > 0
                    ? lod0Renderers
                    : GetComponentsInChildren<Renderer>());

            lods[1] = new LOD(lod1Height,
                lod1Renderers ?? new Renderer[0]);

            lods[2] = new LOD(lod2Height,
                lod2Renderers ?? new Renderer[0]);

            lodGroup.SetLODs(lods);
            lodGroup.RecalculateBounds();

            Debug.Log($"[LOD] Setup complete for {gameObject.name}: 3 LOD levels");
        }

        /// <summary>
        /// Force a specific LOD level (useful during inspection mode).
        /// Pass -1 to return to automatic LOD.
        /// </summary>
        public void ForceLOD(int level)
        {
            if (lodGroup == null) return;

            if (level < 0)
            {
                lodGroup.ForceLOD(-1); // Return to auto
            }
            else
            {
                lodGroup.ForceLOD(level);
            }
        }

        /// <summary>Configure LOD distances at runtime.</summary>
        public void SetLODDistances(float lod0Dist, float lod1Dist, float cullDist)
        {
            lod0Distance = lod0Dist;
            lod1Distance = lod1Dist;
            cullDistance = cullDist;

            if (lodGroup != null)
            {
                var lods = lodGroup.GetLODs();
                if (lods.Length >= 3)
                {
                    lods[0].screenRelativeTransitionHeight = DistanceToScreenHeight(lod0Dist);
                    lods[1].screenRelativeTransitionHeight = DistanceToScreenHeight(lod1Dist);
                    lods[2].screenRelativeTransitionHeight = DistanceToScreenHeight(cullDist);
                    lodGroup.SetLODs(lods);
                }
            }
        }

        private float DistanceToScreenHeight(float distance)
        {
            // Approximate conversion from world distance to screen height
            // Assumes a 1m object and standard VR FOV
            if (distance <= 0) return 1f;
            return Mathf.Clamp01(1f / distance * 0.5f);
        }
    }
}
