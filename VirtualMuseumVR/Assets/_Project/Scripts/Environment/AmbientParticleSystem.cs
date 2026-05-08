// =============================================================================
// AmbientParticleSystem.cs — Atmospheric Dust Motes / Particles
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================

using UnityEngine;
using VirtualMuseumVR.Core;

namespace VirtualMuseumVR.Environment
{
    public class AmbientParticleSystem : MonoBehaviour
    {
        [Header("Particle System")]
        [SerializeField] private ParticleSystem dustParticles;

        [Header("Per-Room Configuration")]
        [SerializeField] private Color corridorParticleColor = new Color(1f, 1f, 1f, 0.3f);
        [SerializeField] private Color iraqParticleColor = new Color(0.9f, 0.8f, 0.6f, 0.25f); // sand
        [SerializeField] private Color pragueParticleColor = new Color(0.7f, 0.8f, 1f, 0.2f); // cool
        [SerializeField] private Color shardsParticleColor = new Color(0.8f, 0.8f, 0.7f, 0.15f); // neutral
        [SerializeField] private Color aerialParticleColor = new Color(0.6f, 0.7f, 0.9f, 0.2f); // sky
        [SerializeField] private Color indiaParticleColor = new Color(1f, 0.85f, 0.5f, 0.2f); // warm gold

        [Header("Settings")]
        [SerializeField] private float particleSize = 0.02f;
        [SerializeField] private int maxParticles = 200;
        [SerializeField] private float emissionRate = 20f;

        private void Start()
        {
            if (dustParticles == null)
            {
                dustParticles = CreateDefaultParticleSystem();
            }
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
            Color targetColor = roomId switch
            {
                "iraq" => iraqParticleColor,
                "prague" => pragueParticleColor,
                "shards" => shardsParticleColor,
                "aerial" => aerialParticleColor,
                "india" => indiaParticleColor,
                _ => corridorParticleColor
            };

            ApplyParticleColor(targetColor);
        }

        private void ApplyParticleColor(Color color)
        {
            if (dustParticles == null) return;

            var main = dustParticles.main;
            main.startColor = color;
        }

        private ParticleSystem CreateDefaultParticleSystem()
        {
            var ps = gameObject.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startSize = particleSize;
            main.startSpeed = 0.05f;
            main.startLifetime = 10f;
            main.maxParticles = maxParticles;
            main.startColor = corridorParticleColor;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.gravityModifier = -0.01f; // Slight upward drift

            var emission = ps.emission;
            emission.rateOverTime = emissionRate;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(8f, 4f, 8f);

            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            renderer.maxParticleSize = 0.05f;

            return ps;
        }

        /// <summary>Enable or disable particles (for performance).</summary>
        public void SetParticlesEnabled(bool enabled)
        {
            if (dustParticles == null) return;

            if (enabled && !dustParticles.isPlaying)
                dustParticles.Play();
            else if (!enabled && dustParticles.isPlaying)
                dustParticles.Stop();
        }
    }
}
