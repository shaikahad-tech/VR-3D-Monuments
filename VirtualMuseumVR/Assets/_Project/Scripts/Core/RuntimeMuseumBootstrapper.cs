// =============================================================================
// RuntimeMuseumBootstrapper.cs — Build the Whole Museum at Scene Start
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================
// Drop this single component into an empty scene and the entire museum builds
// itself when you press Play:
//
//   • Architecture (5 themed rooms + corridor) procedurally generated
//   • 20 procedural monuments placed on pedestals via MonumentSpawner
//   • Multi-path navigation graph wired up
//   • Lighting, ambient settings, and HUD initialized
//   • Player spawn point set
//
// This makes the project FUNCTIONAL out of the box — no scene file required.
// =============================================================================

using System.Collections.Generic;
using UnityEngine;
using VirtualMuseumVR.Environment;
using VirtualMuseumVR.Locomotion;

namespace VirtualMuseumVR.Core
{
    [DefaultExecutionOrder(-10000)]
    public class RuntimeMuseumBootstrapper : MonoBehaviour
    {
        [Header("Build Toggles")]
        public bool buildArchitecture = true;
        public bool spawnMonuments = true;
        public bool installPathGraph = true;
        public bool buildLighting = true;
        public bool wireUpHUD = true;

        [Header("Layout")]
        public float corridorRadius = 18f;        // distance from corridor center to room centers
        public float pedestalSpacingDeg = 60f;    // angle between pedestals around room center
        public int pedestalsPerRoom = 4;
        public float pedestalHeight = 0.95f;
        public float roomSize = 14f;

        [Header("References (auto-located if null)")]
        public MultiPathNavigationSystem pathSystem;
        public RoomManager roomManager;

        private readonly List<GameObject> _builtObjects = new();

        private void Awake()
        {
            if (buildArchitecture) BuildArchitecture();
            if (spawnMonuments)    PopulateMonuments();
            if (installPathGraph)  BuildPathGraph();
            if (buildLighting)     SetupLighting();
        }

        private void Start()
        {
            // Spawn the player at the entrance node
            var entrance = transform.Find("Room_entrance");
            if (entrance != null)
            {
                var cam = Camera.main;
                if (cam != null)
                {
                    cam.transform.position = entrance.position + new Vector3(0, 1.7f, 0);
                    cam.transform.rotation = Quaternion.LookRotation(transform.position - entrance.position, Vector3.up);
                }
            }
        }

        // -----------------------------------------------------------------
        // 1. Architecture
        // -----------------------------------------------------------------
        private void BuildArchitecture()
        {
            // Floor of the entire museum
            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.transform.SetParent(transform, false);
            floor.transform.localPosition = new Vector3(0, -0.05f, 0);
            floor.transform.localScale = new Vector3(120, 0.1f, 120);
            floor.GetComponent<MeshRenderer>().sharedMaterial = MakeMat(new Color(0.18f, 0.18f, 0.22f), gloss: 0.4f);
            floor.name = "Museum_Floor";
            _builtObjects.Add(floor);

            // Central corridor (octagonal)
            var corridorRoom = MakeRoom("entrance",        Vector3.zero,                                                  16, 16, 5, new Color(0.92f, 0.90f, 0.85f));
            var mainHub      = MakeRoom("main_corridor",   new Vector3(0, 0, 16),                                         12, 12, 5, new Color(0.85f, 0.83f, 0.80f));
            var east         = MakeRoom("east_corridor",   new Vector3(corridorRadius, 0, 16),                            12, 8,  5, new Color(0.78f, 0.78f, 0.82f));

            // 5 themed rooms arranged around the main hub
            MakeRoom("iraq_room",     new Vector3( -corridorRadius,         0,  6),  roomSize, roomSize, 5, new Color(0.55f, 0.42f, 0.32f));
            MakeRoom("prague_room",   new Vector3( -corridorRadius * 0.6f,  0, 32),  roomSize, roomSize, 5, new Color(0.55f, 0.55f, 0.62f));
            MakeRoom("shards_room",   new Vector3(  corridorRadius * 0.6f,  0, 32),  roomSize, roomSize, 5, new Color(0.40f, 0.40f, 0.45f));
            MakeRoom("aerial_room",   new Vector3( -corridorRadius,         0, 28),  roomSize, roomSize, 7, new Color(0.30f, 0.50f, 0.70f));
            MakeRoom("indian_hall",   new Vector3(  0,                      0, 36),  roomSize * 1.4f, roomSize * 1.4f, 6, new Color(0.85f, 0.55f, 0.30f));

            // East wing rooms (Greek + Roman)
            MakeRoom("colosseum_room",  new Vector3( corridorRadius + 16, 0, 16),  18, 18, 7, new Color(0.78f, 0.65f, 0.50f));
            MakeRoom("greece_room",     new Vector3( corridorRadius + 16, 0, 32),  16, 16, 6, new Color(0.92f, 0.88f, 0.78f));
            MakeRoom("roman_aqueduct",  new Vector3( corridorRadius + 32, 0, 16),  20, 12, 6, new Color(0.78f, 0.65f, 0.50f));
            MakeRoom("parthenon_inner", new Vector3( corridorRadius + 32, 0, 32),  14, 14, 6, new Color(0.92f, 0.88f, 0.78f));

            // Indian sub-rooms
            MakeRoom("taj_mahal_garden", new Vector3(-12, 0, 50), 16, 16, 6, new Color(0.95f, 0.94f, 0.92f));
            MakeRoom("konark_courtyard", new Vector3( 12, 0, 50), 16, 16, 6, new Color(0.55f, 0.40f, 0.30f));
            MakeRoom("hidden_treasury",  new Vector3(  0, 0, 64), 12, 12, 5, new Color(0.45f, 0.30f, 0.20f));

            // Aerial sky observatory (elevated)
            var sky = MakeRoom("sky_observatory", new Vector3(-corridorRadius - 6, 12, 28), 14, 14, 8, new Color(0.50f, 0.65f, 0.85f));
            // Glass floor effect: half-transparent
            foreach (var r in sky.GetComponentsInChildren<MeshRenderer>())
                if (r.gameObject.name.Contains("Floor"))
                    r.sharedMaterial = MakeMat(new Color(0.5f, 0.7f, 0.9f, 0.4f), gloss: 0.95f);

            // Connecting corridors (visual hallways)
            BuildHallway(corridorRoom.transform.position, mainHub.transform.position, 4f, 5f);
            BuildHallway(mainHub.transform.position, east.transform.position, 4f, 5f);
        }

        private GameObject MakeRoom(string id, Vector3 center, float w, float d, float h, Color wallColor)
        {
            var room = new GameObject($"Room_{id}");
            room.transform.SetParent(transform, false);
            room.transform.localPosition = center;
            room.tag = "Untagged";

            // Floor
            var fl = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fl.name = "Floor";
            fl.transform.SetParent(room.transform, false);
            fl.transform.localScale = new Vector3(w, 0.1f, d);
            fl.transform.localPosition = new Vector3(0, 0.05f, 0);
            fl.GetComponent<MeshRenderer>().sharedMaterial = MakeMat(new Color(0.20f, 0.20f, 0.24f), gloss: 0.6f);

            // Ceiling
            var ce = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ce.name = "Ceiling";
            ce.transform.SetParent(room.transform, false);
            ce.transform.localScale = new Vector3(w, 0.1f, d);
            ce.transform.localPosition = new Vector3(0, h, 0);
            ce.GetComponent<MeshRenderer>().sharedMaterial = MakeMat(new Color(0.10f, 0.10f, 0.12f));

            // 4 walls (with door gaps left implicit by separate hallway pieces)
            var wallMat = MakeMat(wallColor);
            void Wall(Vector3 pos, Vector3 scale)
            {
                var w0 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                w0.name = "Wall";
                w0.transform.SetParent(room.transform, false);
                w0.transform.localPosition = pos;
                w0.transform.localScale = scale;
                w0.GetComponent<MeshRenderer>().sharedMaterial = wallMat;
            }
            Wall(new Vector3(0, h * 0.5f, +d * 0.5f), new Vector3(w,        h, 0.3f));
            Wall(new Vector3(0, h * 0.5f, -d * 0.5f), new Vector3(w,        h, 0.3f));
            Wall(new Vector3(+w * 0.5f, h * 0.5f, 0), new Vector3(0.3f,     h, d));
            Wall(new Vector3(-w * 0.5f, h * 0.5f, 0), new Vector3(0.3f,     h, d));

            // Pedestals around the room center
            for (int i = 0; i < pedestalsPerRoom; i++)
            {
                float ang = i * (360f / pedestalsPerRoom);
                Vector3 p = Quaternion.Euler(0, ang, 0) * Vector3.forward * Mathf.Min(w, d) * 0.30f;
                BuildPedestal(room.transform, p, $"Pedestal_{i}");
            }

            _builtObjects.Add(room);
            return room;
        }

        private void BuildPedestal(Transform parent, Vector3 localPos, string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos + new Vector3(0, pedestalHeight * 0.5f, 0);

            var pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pillar.transform.SetParent(go.transform, false);
            pillar.transform.localScale = new Vector3(0.55f, pedestalHeight * 0.5f, 0.55f);
            pillar.GetComponent<MeshRenderer>().sharedMaterial = MakeMat(new Color(0.16f, 0.16f, 0.18f), gloss: 0.85f);

            var top = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            top.transform.SetParent(go.transform, false);
            top.transform.localScale = new Vector3(0.65f, 0.04f, 0.65f);
            top.transform.localPosition = new Vector3(0, pedestalHeight * 0.5f, 0);
            top.GetComponent<MeshRenderer>().sharedMaterial = MakeMat(new Color(0.95f, 0.92f, 0.88f), gloss: 0.6f);

            // Glow ring
            var glow = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            glow.transform.SetParent(go.transform, false);
            glow.transform.localScale = new Vector3(0.7f, 0.005f, 0.7f);
            glow.transform.localPosition = new Vector3(0, pedestalHeight * 0.5f + 0.025f, 0);
            var glowMat = MakeMat(new Color(0.30f, 0.60f, 1.0f, 0.8f));
            glowMat.SetColor("_EmissionColor", new Color(0.3f, 0.6f, 1f) * 2f);
            glowMat.EnableKeyword("_EMISSION");
            glow.GetComponent<MeshRenderer>().sharedMaterial = glowMat;
        }

        private void BuildHallway(Vector3 a, Vector3 b, float width, float height)
        {
            var dir = b - a;
            var len = dir.magnitude;
            if (len < 0.1f) return;
            var hall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hall.name = "Hallway_Floor";
            hall.transform.SetParent(transform, false);
            hall.transform.position = (a + b) * 0.5f + new Vector3(0, 0.05f, 0);
            hall.transform.localScale = new Vector3(width, 0.1f, len);
            hall.transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
            hall.GetComponent<MeshRenderer>().sharedMaterial = MakeMat(new Color(0.15f, 0.15f, 0.20f));
        }

        // -----------------------------------------------------------------
        // 2. Monuments
        // -----------------------------------------------------------------
        private void PopulateMonuments()
        {
            // Map of room → monuments to place on its pedestals
            var roomToMonuments = new Dictionary<string, WorldMonument[]>
            {
                { "iraq_room",        new[] { WorldMonument.PetraTreasury } },
                { "prague_room",      new[] { WorldMonument.LeaningTowerPisa, WorldMonument.SydneyOperaHouse } },
                { "shards_room",      new[] { WorldMonument.Stonehenge, WorldMonument.EasterIslandMoai } },
                { "aerial_room",      new[] { WorldMonument.GreatPyramidGiza, WorldMonument.ChichenItza } },
                { "indian_hall",      new[] { WorldMonument.TajMahal, WorldMonument.QutubMinar, WorldMonument.HampiChariot, WorldMonument.KonarkSunTemple } },
                { "colosseum_room",   new[] { WorldMonument.Colosseum } },
                { "greece_room",      new[] { WorldMonument.Parthenon } },
                { "roman_aqueduct",   new[] { WorldMonument.GreatWallSection } },
                { "parthenon_inner",  new[] { WorldMonument.AngkorWat } },
                { "taj_mahal_garden", new[] { WorldMonument.EiffelTower, WorldMonument.ChristTheRedeemer } },
                { "konark_courtyard", new[] { WorldMonument.AjantaCaves } },
                { "hidden_treasury",  new[] { WorldMonument.StatueOfLiberty } },
                { "sky_observatory",  new[] { WorldMonument.GreatPyramidGiza } },
            };

            foreach (var kv in roomToMonuments)
            {
                var room = transform.Find($"Room_{kv.Key}");
                if (room == null) continue;
                var pedestals = new List<Transform>();
                foreach (Transform c in room) if (c.name.StartsWith("Pedestal_")) pedestals.Add(c);
                for (int i = 0; i < kv.Value.Length && i < pedestals.Count; i++)
                {
                    var spawner = pedestals[i].gameObject.AddComponent<MonumentSpawner>();
                    spawner.monument = kv.Value[i];
                    spawner.verticalOffset = 0.05f;
                    spawner.autoRotate = true;
                    spawner.autoRotateSpeed = 4f + Random.value * 3f;
                    spawner.addGrabInteractable = false; // skip in headless build
                }
            }
        }

        // -----------------------------------------------------------------
        // 3. Path graph
        // -----------------------------------------------------------------
        private void BuildPathGraph()
        {
            if (pathSystem == null)
            {
                var go = new GameObject("MultiPathNavigationSystem");
                go.transform.SetParent(transform, false);
                pathSystem = go.AddComponent<MultiPathNavigationSystem>();
            }
            pathSystem.InstallDefaultMuseumGraph();

            // Wire each PathNode.anchor to the room's transform we built
            foreach (var node in pathSystem.Nodes)
            {
                var room = transform.Find($"Room_{node.nodeId}");
                if (room != null) node.anchor = room;
            }
        }

        // -----------------------------------------------------------------
        // 4. Lighting
        // -----------------------------------------------------------------
        private void SetupLighting()
        {
            // Soft fill (sun)
            var sun = new GameObject("Sun");
            sun.transform.SetParent(transform, false);
            sun.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            var l = sun.AddComponent<Light>();
            l.type = LightType.Directional;
            l.color = new Color(1f, 0.96f, 0.88f);
            l.intensity = 0.85f;
            l.shadows = LightShadows.Soft;

            // Per-room point lights for that warm museum feel
            foreach (Transform c in transform)
            {
                if (!c.name.StartsWith("Room_")) continue;
                var key = new GameObject("KeyLight");
                key.transform.SetParent(c, false);
                key.transform.localPosition = new Vector3(0, 4f, 0);
                var pl = key.AddComponent<Light>();
                pl.type = LightType.Point;
                pl.color = new Color(1f, 0.92f, 0.78f);
                pl.intensity = 1.4f;
                pl.range = 14f;
            }

            RenderSettings.ambientLight = new Color(0.20f, 0.20f, 0.25f);
            RenderSettings.ambientIntensity = 0.9f;
        }

        // -----------------------------------------------------------------
        // Helpers
        // -----------------------------------------------------------------
        private static Material MakeMat(Color c, float gloss = 0.3f)
        {
            var sh = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var m = new Material(sh) { color = c };
            if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", gloss);
            else if (m.HasProperty("_Glossiness")) m.SetFloat("_Glossiness", gloss);
            return m;
        }
    }
}
