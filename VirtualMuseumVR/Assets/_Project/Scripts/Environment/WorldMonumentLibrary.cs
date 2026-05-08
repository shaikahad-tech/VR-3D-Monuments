// =============================================================================
// WorldMonumentLibrary.cs — Runtime Procedural World Monument Builder
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================
// Generates detailed procedural 3D meshes for world heritage monuments at
// runtime (no Editor dependency). Each monument is composed of stacked,
// rotated, and scaled primitives plus parameterized geometry to approximate
// the silhouette and proportions of the real structure. The output is a
// GameObject hierarchy with MeshRenderers using a shared material so that
// LOD, occlusion culling, and grab interaction can be wired up afterwards.
//
// Used by MonumentSpawner and RuntimeMuseumBootstrapper to populate exhibit
// pedestals when no FBX/OBJ photogrammetry asset has been imported yet.
// =============================================================================

using System.Collections.Generic;
using UnityEngine;

namespace VirtualMuseumVR.Environment
{
    public enum WorldMonument
    {
        // ---- Indian heritage (matches MonumentAssetBuilder) ----
        TajMahal,
        QutubMinar,
        HampiChariot,
        KonarkSunTemple,
        AjantaCaves,

        // ---- Global monuments added for the world tour path ----
        GreatPyramidGiza,
        Colosseum,
        Stonehenge,
        Parthenon,
        EiffelTower,
        ChristTheRedeemer,
        StatueOfLiberty,
        EasterIslandMoai,
        AngkorWat,
        GreatWallSection,
        PetraTreasury,
        ChichenItza,
        LeaningTowerPisa,
        SydneyOperaHouse
    }

    [System.Serializable]
    public struct MonumentDefinition
    {
        public WorldMonument id;
        public string title;
        public string era;
        public string location;
        public string region;        // "Asia", "Europe", "Americas", "Africa", "Oceania"
        public Color stoneColor;
        public float scaleMultiplier;
    }

    /// <summary>
    /// Static factory for procedural monument meshes. Pure runtime: no Editor APIs.
    /// </summary>
    public static class WorldMonumentLibrary
    {
        // -----------------------------------------------------------------
        // Catalog
        // -----------------------------------------------------------------
        private static readonly MonumentDefinition[] _catalog =
        {
            new MonumentDefinition { id = WorldMonument.TajMahal,           title = "Taj Mahal",            era = "17th C. (Mughal)",       location = "Agra, India",            region = "Asia",     stoneColor = new Color(0.95f, 0.94f, 0.92f), scaleMultiplier = 1.0f },
            new MonumentDefinition { id = WorldMonument.QutubMinar,         title = "Qutub Minar",          era = "12th C. (Sultanate)",    location = "Delhi, India",           region = "Asia",     stoneColor = new Color(0.78f, 0.42f, 0.28f), scaleMultiplier = 1.0f },
            new MonumentDefinition { id = WorldMonument.HampiChariot,       title = "Stone Chariot, Hampi", era = "16th C. (Vijayanagara)", location = "Hampi, India",           region = "Asia",     stoneColor = new Color(0.65f, 0.55f, 0.40f), scaleMultiplier = 1.0f },
            new MonumentDefinition { id = WorldMonument.KonarkSunTemple,    title = "Konark Sun Temple",    era = "13th C. (Eastern Ganga)",location = "Odisha, India",          region = "Asia",     stoneColor = new Color(0.55f, 0.40f, 0.30f), scaleMultiplier = 1.0f },
            new MonumentDefinition { id = WorldMonument.AjantaCaves,        title = "Ajanta Caves",         era = "2nd C. BCE – 5th C. CE", location = "Maharashtra, India",     region = "Asia",     stoneColor = new Color(0.50f, 0.42f, 0.36f), scaleMultiplier = 1.0f },

            new MonumentDefinition { id = WorldMonument.GreatPyramidGiza,   title = "Great Pyramid of Giza",era = "c. 2560 BCE",            location = "Giza, Egypt",            region = "Africa",   stoneColor = new Color(0.85f, 0.75f, 0.55f), scaleMultiplier = 1.0f },
            new MonumentDefinition { id = WorldMonument.Colosseum,          title = "Roman Colosseum",      era = "80 CE",                  location = "Rome, Italy",            region = "Europe",   stoneColor = new Color(0.78f, 0.65f, 0.50f), scaleMultiplier = 1.0f },
            new MonumentDefinition { id = WorldMonument.Stonehenge,         title = "Stonehenge",           era = "c. 3000 BCE",            location = "Wiltshire, England",     region = "Europe",   stoneColor = new Color(0.55f, 0.55f, 0.55f), scaleMultiplier = 1.0f },
            new MonumentDefinition { id = WorldMonument.Parthenon,          title = "Parthenon",            era = "447 BCE",                location = "Athens, Greece",         region = "Europe",   stoneColor = new Color(0.92f, 0.88f, 0.78f), scaleMultiplier = 1.0f },
            new MonumentDefinition { id = WorldMonument.EiffelTower,        title = "Eiffel Tower",         era = "1889 CE",                location = "Paris, France",          region = "Europe",   stoneColor = new Color(0.30f, 0.27f, 0.25f), scaleMultiplier = 1.0f },
            new MonumentDefinition { id = WorldMonument.ChristTheRedeemer,  title = "Christ the Redeemer",  era = "1931 CE",                location = "Rio de Janeiro, Brazil", region = "Americas", stoneColor = new Color(0.86f, 0.84f, 0.80f), scaleMultiplier = 1.0f },
            new MonumentDefinition { id = WorldMonument.StatueOfLiberty,    title = "Statue of Liberty",    era = "1886 CE",                location = "New York, USA",          region = "Americas", stoneColor = new Color(0.40f, 0.78f, 0.65f), scaleMultiplier = 1.0f },
            new MonumentDefinition { id = WorldMonument.EasterIslandMoai,   title = "Moai (Easter Island)", era = "1250–1500 CE",           location = "Rapa Nui, Chile",        region = "Oceania",  stoneColor = new Color(0.45f, 0.40f, 0.35f), scaleMultiplier = 1.0f },
            new MonumentDefinition { id = WorldMonument.AngkorWat,          title = "Angkor Wat",           era = "12th C. (Khmer)",        location = "Siem Reap, Cambodia",    region = "Asia",     stoneColor = new Color(0.55f, 0.50f, 0.40f), scaleMultiplier = 1.0f },
            new MonumentDefinition { id = WorldMonument.GreatWallSection,   title = "Great Wall (section)", era = "7th C. BCE – 17th C. CE",location = "Northern China",         region = "Asia",     stoneColor = new Color(0.60f, 0.55f, 0.48f), scaleMultiplier = 1.0f },
            new MonumentDefinition { id = WorldMonument.PetraTreasury,      title = "Al-Khazneh, Petra",    era = "1st C. CE (Nabataean)",  location = "Petra, Jordan",          region = "Asia",     stoneColor = new Color(0.82f, 0.50f, 0.35f), scaleMultiplier = 1.0f },
            new MonumentDefinition { id = WorldMonument.ChichenItza,        title = "El Castillo, Chichén Itzá", era = "9th–12th C. (Maya)",location = "Yucatán, Mexico",        region = "Americas", stoneColor = new Color(0.62f, 0.58f, 0.50f), scaleMultiplier = 1.0f },
            new MonumentDefinition { id = WorldMonument.LeaningTowerPisa,   title = "Leaning Tower of Pisa",era = "1372 CE",                location = "Pisa, Italy",            region = "Europe",   stoneColor = new Color(0.94f, 0.92f, 0.86f), scaleMultiplier = 1.0f },
            new MonumentDefinition { id = WorldMonument.SydneyOperaHouse,   title = "Sydney Opera House",   era = "1973 CE",                location = "Sydney, Australia",      region = "Oceania",  stoneColor = new Color(0.96f, 0.96f, 0.95f), scaleMultiplier = 1.0f },
        };

        public static IReadOnlyList<MonumentDefinition> Catalog => _catalog;

        public static MonumentDefinition GetDefinition(WorldMonument id)
        {
            for (int i = 0; i < _catalog.Length; i++)
                if (_catalog[i].id == id) return _catalog[i];
            return _catalog[0];
        }

        // -----------------------------------------------------------------
        // Build entry point
        // -----------------------------------------------------------------
        public static GameObject Build(WorldMonument monument, Material baseMaterial = null, Transform parent = null)
        {
            var def = GetDefinition(monument);
            var root = new GameObject($"Monument_{monument}");
            if (parent != null) root.transform.SetParent(parent, false);

            var mat = baseMaterial != null ? baseMaterial : CreateDefaultMaterial(def.stoneColor);

            switch (monument)
            {
                case WorldMonument.TajMahal:          BuildTajMahal(root, mat); break;
                case WorldMonument.QutubMinar:        BuildQutubMinar(root, mat); break;
                case WorldMonument.HampiChariot:      BuildHampiChariot(root, mat); break;
                case WorldMonument.KonarkSunTemple:   BuildKonarkTemple(root, mat); break;
                case WorldMonument.AjantaCaves:       BuildAjantaCaves(root, mat); break;
                case WorldMonument.GreatPyramidGiza:  BuildGreatPyramid(root, mat); break;
                case WorldMonument.Colosseum:         BuildColosseum(root, mat); break;
                case WorldMonument.Stonehenge:        BuildStonehenge(root, mat); break;
                case WorldMonument.Parthenon:         BuildParthenon(root, mat); break;
                case WorldMonument.EiffelTower:       BuildEiffelTower(root, mat); break;
                case WorldMonument.ChristTheRedeemer: BuildChristTheRedeemer(root, mat); break;
                case WorldMonument.StatueOfLiberty:   BuildStatueOfLiberty(root, mat); break;
                case WorldMonument.EasterIslandMoai:  BuildMoai(root, mat); break;
                case WorldMonument.AngkorWat:         BuildAngkorWat(root, mat); break;
                case WorldMonument.GreatWallSection:  BuildGreatWall(root, mat); break;
                case WorldMonument.PetraTreasury:     BuildPetraTreasury(root, mat); break;
                case WorldMonument.ChichenItza:       BuildChichenItza(root, mat); break;
                case WorldMonument.LeaningTowerPisa:  BuildLeaningTower(root, mat); break;
                case WorldMonument.SydneyOperaHouse:  BuildSydneyOperaHouse(root, mat); break;
            }

            // Apply material to all renderers that don't have an override
            foreach (var r in root.GetComponentsInChildren<MeshRenderer>())
            {
                if (r.sharedMaterial == null) r.sharedMaterial = mat;
            }

            // Uniform scale so monuments fit on a 1m pedestal nicely
            root.transform.localScale = Vector3.one * 0.05f * def.scaleMultiplier;

            return root;
        }

        // -----------------------------------------------------------------
        // Helpers
        // -----------------------------------------------------------------
        private static Material CreateDefaultMaterial(Color color)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var mat = new Material(shader) { color = color };
            return mat;
        }

        private static GameObject Prim(PrimitiveType type, Transform parent, Vector3 pos, Vector3 scale, Vector3 eulerRot = default, Material overrideMat = null)
        {
            var go = GameObject.CreatePrimitive(type);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localScale = scale;
            go.transform.localRotation = Quaternion.Euler(eulerRot);
            if (overrideMat != null) go.GetComponent<MeshRenderer>().sharedMaterial = overrideMat;
            // Drop colliders by default — they're added by MonumentGrabInteractable later.
            var col = go.GetComponent<Collider>();
            if (col != null) Object.DestroyImmediate(col);
            return go;
        }

        // =================================================================
        // Indian heritage builders (more detailed than the Editor stubs)
        // =================================================================
        private static void BuildTajMahal(GameObject root, Material mat)
        {
            var t = root.transform;

            // Plinth (large red sandstone base)
            var plinthMat = CreateDefaultMaterial(new Color(0.55f, 0.27f, 0.20f));
            Prim(PrimitiveType.Cube,     t, new Vector3(0, 0.5f, 0),    new Vector3(34, 1f, 34), default, plinthMat);

            // Main marble platform
            Prim(PrimitiveType.Cube,     t, new Vector3(0, 1.5f, 0),    new Vector3(28, 1f, 28));

            // Main building cube
            Prim(PrimitiveType.Cube,     t, new Vector3(0, 5.5f, 0),    new Vector3(20, 7f, 20));

            // Iwan arches (recessed)
            for (int s = 0; s < 4; s++)
            {
                float a = s * 90f;
                Vector3 dir = Quaternion.Euler(0, a, 0) * Vector3.forward;
                Prim(PrimitiveType.Cube, t, dir * 10.05f + new Vector3(0, 5.5f, 0), new Vector3(8, 6, 0.5f), new Vector3(0, a, 0), CreateDefaultMaterial(new Color(0.30f, 0.20f, 0.15f)));
            }

            // Main dome (onion shape — sphere + cap)
            Prim(PrimitiveType.Sphere,   t, new Vector3(0, 12f, 0),     new Vector3(11, 11, 11));
            Prim(PrimitiveType.Cylinder, t, new Vector3(0, 17.5f, 0),   new Vector3(0.5f, 1.5f, 0.5f));
            Prim(PrimitiveType.Sphere,   t, new Vector3(0, 19.5f, 0),   new Vector3(1.4f, 1.4f, 1.4f));

            // Drum below dome
            Prim(PrimitiveType.Cylinder, t, new Vector3(0, 9f, 0),      new Vector3(11, 0.5f, 11));

            // 4 chhatris (small domes at corners)
            Vector3[] chhatris = { new Vector3(7,10,7), new Vector3(-7,10,7), new Vector3(7,10,-7), new Vector3(-7,10,-7) };
            foreach (var p in chhatris)
            {
                Prim(PrimitiveType.Cylinder, t, p + new Vector3(0,-1.5f,0), new Vector3(2, 1.2f, 2));
                Prim(PrimitiveType.Sphere,   t, p,                          new Vector3(2.5f, 2.0f, 2.5f));
            }

            // 4 minarets at platform corners
            Vector3[] minarets = { new Vector3(13, 8, 13), new Vector3(-13, 8, 13), new Vector3(13, 8, -13), new Vector3(-13, 8, -13) };
            foreach (var p in minarets)
            {
                Prim(PrimitiveType.Cylinder, t, p, new Vector3(1.4f, 9f, 1.4f));
                // gallery rings
                for (int g = 0; g < 3; g++)
                {
                    Prim(PrimitiveType.Cylinder, t, p + new Vector3(0, -6 + g*5, 0), new Vector3(1.7f, 0.2f, 1.7f));
                }
                // capstone dome
                Prim(PrimitiveType.Sphere, t, p + new Vector3(0, 9.2f, 0), new Vector3(2, 1.5f, 2));
            }

            // Reflecting pool (long thin water-blue plane)
            Prim(PrimitiveType.Cube, t, new Vector3(0, 1.55f, 22), new Vector3(3, 0.05f, 30), default, CreateDefaultMaterial(new Color(0.25f, 0.45f, 0.65f)));
        }

        private static void BuildQutubMinar(GameObject root, Material mat)
        {
            var t = root.transform;
            float h = 0f;
            float r = 5f;
            // 5 fluted tiers, tapering, with balcony rings between
            for (int i = 0; i < 5; i++)
            {
                Prim(PrimitiveType.Cylinder, t, new Vector3(0, h + 4, 0), new Vector3(r, 4, r));

                // simulate fluting: 16 thin half-cylinders around the perimeter
                for (int f = 0; f < 16; f++)
                {
                    float ang = f * (360f / 16f);
                    Vector3 dir = Quaternion.Euler(0, ang, 0) * Vector3.forward;
                    Prim(PrimitiveType.Cylinder, t, dir * (r * 0.5f) + new Vector3(0, h + 4, 0), new Vector3(0.6f, 4, 0.6f));
                }

                // balcony
                if (i < 4)
                    Prim(PrimitiveType.Cylinder, t, new Vector3(0, h + 8.1f, 0), new Vector3(r * 1.15f, 0.4f, r * 1.15f));

                h += 8f;
                r *= 0.78f;
            }

            // Cap
            Prim(PrimitiveType.Cylinder, t, new Vector3(0, h + 2, 0), new Vector3(r * 0.9f, 1.5f, r * 0.9f));
        }

        private static void BuildHampiChariot(GameObject root, Material mat)
        {
            var t = root.transform;
            // Base
            Prim(PrimitiveType.Cube, t, new Vector3(0, 1f, 0), new Vector3(6, 2, 8));

            // Wheels
            Vector3[] wheels = { new Vector3(3.5f, 1, 3), new Vector3(-3.5f, 1, 3), new Vector3(3.5f, 1, -3), new Vector3(-3.5f, 1, -3) };
            foreach (var p in wheels)
            {
                Prim(PrimitiveType.Cylinder, t, p, new Vector3(2, 0.5f, 2), new Vector3(0, 0, 90));
                // wheel hub
                Prim(PrimitiveType.Sphere, t, p, new Vector3(0.9f, 0.9f, 0.9f));
                // 12 spokes
                for (int s = 0; s < 12; s++)
                {
                    float ang = s * 30f;
                    var spoke = Prim(PrimitiveType.Cube, t, p, new Vector3(0.15f, 1.6f, 0.15f), new Vector3(ang, 0, 90));
                    spoke.transform.localPosition = p;
                }
            }

            // Pillars at corners of upper deck
            Vector3[] pillars = { new Vector3(2, 2.5f, 3), new Vector3(-2, 2.5f, 3), new Vector3(2, 2.5f, -3), new Vector3(-2, 2.5f, -3) };
            foreach (var p in pillars)
                Prim(PrimitiveType.Cylinder, t, p, new Vector3(0.4f, 1.5f, 0.4f));

            // Top shrine
            Prim(PrimitiveType.Cube, t, new Vector3(0, 4.5f, 0), new Vector3(4, 1.8f, 6));
            // Tiered roof (gopuram-style stepped pyramid)
            for (int i = 0; i < 4; i++)
            {
                float s = 4f - i * 0.7f;
                Prim(PrimitiveType.Cube, t, new Vector3(0, 5.5f + i * 0.7f, 0), new Vector3(s, 0.6f, s + 1f));
            }
            Prim(PrimitiveType.Sphere, t, new Vector3(0, 8.5f, 0), new Vector3(1.2f, 1.2f, 1.2f));
        }

        private static void BuildKonarkTemple(GameObject root, Material mat)
        {
            var t = root.transform;

            // Base platform on chariot wheels
            Prim(PrimitiveType.Cube, t, new Vector3(0, 1, 0), new Vector3(20, 2, 14));

            // 12 chariot wheels along the side
            for (int i = -3; i <= 3; i++)
            {
                if (i == 0) continue;
                Prim(PrimitiveType.Cylinder, t, new Vector3(i * 2.5f, 1, 7.05f), new Vector3(2.0f, 0.4f, 2.0f), new Vector3(90, 0, 0));
                Prim(PrimitiveType.Cylinder, t, new Vector3(i * 2.5f, 1, -7.05f), new Vector3(2.0f, 0.4f, 2.0f), new Vector3(90, 0, 0));
            }

            // Stepped pyramidal jagamohan
            float size = 14f, height = 2f;
            for (int i = 0; i < 12; i++)
            {
                Prim(PrimitiveType.Cube, t, new Vector3(0, height, 0), new Vector3(size, 1f, size));
                size -= 1.0f;
                height += 1.0f;
            }
            // Crowning amla & kalash
            Prim(PrimitiveType.Sphere, t, new Vector3(0, height + 0.5f, 0), new Vector3(2.5f, 0.8f, 2.5f));
            Prim(PrimitiveType.Cylinder, t, new Vector3(0, height + 1.4f, 0), new Vector3(0.6f, 0.6f, 0.6f));

            // 7 horse statues at the eastern end (simplified)
            for (int i = -3; i <= 3; i++)
            {
                Prim(PrimitiveType.Cube, t, new Vector3(i * 1.4f, 0.8f, -10), new Vector3(0.6f, 1.4f, 1.6f));
                Prim(PrimitiveType.Cube, t, new Vector3(i * 1.4f, 1.6f, -9.5f), new Vector3(0.5f, 0.5f, 0.6f));
            }
        }

        private static void BuildAjantaCaves(GameObject root, Material mat)
        {
            var t = root.transform;

            // Carved cliff face — one big curved cube
            Prim(PrimitiveType.Cube, t, new Vector3(0, 7, 0), new Vector3(40, 14, 6));

            // 6 cave entrances
            var darkMat = CreateDefaultMaterial(new Color(0.05f, 0.04f, 0.04f));
            for (int i = -2; i <= 3; i++)
            {
                Vector3 c = new Vector3(i * 6 - 3, 4, -3.05f);
                // arched entrance
                Prim(PrimitiveType.Cube,    t, c, new Vector3(3.5f, 4, 0.6f), default, darkMat);
                Prim(PrimitiveType.Cylinder,t, c + new Vector3(0, 2, 0), new Vector3(3.5f, 0.6f, 1.2f), new Vector3(90, 0, 0), darkMat);

                // Pillars on either side
                Prim(PrimitiveType.Cylinder, t, c + new Vector3(-1.4f, -1.5f, 1.5f), new Vector3(0.5f, 2.5f, 0.5f));
                Prim(PrimitiveType.Cylinder, t, c + new Vector3( 1.4f, -1.5f, 1.5f), new Vector3(0.5f, 2.5f, 0.5f));

                // Carved relief above
                Prim(PrimitiveType.Cube, t, c + new Vector3(0, 3, 0), new Vector3(3.5f, 1.5f, 0.4f));
            }

            // Path / river at base
            Prim(PrimitiveType.Cube, t, new Vector3(0, 0.05f, -8), new Vector3(50, 0.1f, 4), default, CreateDefaultMaterial(new Color(0.25f, 0.35f, 0.45f)));
        }

        // =================================================================
        // Global monument builders
        // =================================================================
        private static void BuildGreatPyramid(GameObject root, Material mat)
        {
            var t = root.transform;
            // Stepped pyramid using shrinking cubes (approximates the casing-stone profile)
            int steps = 40;
            float baseSize = 30f;
            float stepHeight = 0.6f;
            for (int i = 0; i < steps; i++)
            {
                float frac = (float)i / steps;
                float s = baseSize * (1f - frac);
                Prim(PrimitiveType.Cube, t, new Vector3(0, i * stepHeight + stepHeight * 0.5f, 0), new Vector3(s, stepHeight, s));
            }

            // Sphinx in front (very stylized)
            Vector3 sphinxBase = new Vector3(0, 0.5f, -22);
            Prim(PrimitiveType.Cube,     t, sphinxBase + new Vector3(0, 1, 0),   new Vector3(3, 2, 8));   // body
            Prim(PrimitiveType.Cube,     t, sphinxBase + new Vector3(0, 3.2f, 4),new Vector3(2.5f, 2.4f, 2.4f)); // head
            Prim(PrimitiveType.Cube,     t, sphinxBase + new Vector3(0, 1.5f, 5),new Vector3(2.5f, 1, 1));        // beard
            Prim(PrimitiveType.Cylinder, t, sphinxBase + new Vector3(-1.0f, 0.5f, -3), new Vector3(0.5f, 0.6f, 0.5f));
            Prim(PrimitiveType.Cylinder, t, sphinxBase + new Vector3( 1.0f, 0.5f, -3), new Vector3(0.5f, 0.6f, 0.5f));
        }

        private static void BuildColosseum(GameObject root, Material mat)
        {
            var t = root.transform;
            int rings = 4;
            int arches = 60;
            float radius = 18f;
            float archHeight = 4.2f;

            for (int level = 0; level < rings; level++)
            {
                float y = 0.5f + level * archHeight;
                // Ring base
                Prim(PrimitiveType.Cylinder, t, new Vector3(0, y, 0), new Vector3(radius * 2, 0.3f, radius * 2));

                // Arches around
                for (int i = 0; i < arches; i++)
                {
                    float ang = i * (360f / arches);
                    Vector3 dir = Quaternion.Euler(0, ang, 0) * Vector3.forward;
                    Vector3 pos = dir * radius + new Vector3(0, y + archHeight * 0.5f, 0);
                    // pillar
                    Prim(PrimitiveType.Cube, t, pos, new Vector3(0.6f, archHeight, 0.6f), new Vector3(0, ang, 0));
                    // decorative half-column on facade
                    Prim(PrimitiveType.Cylinder, t, dir * (radius - 0.3f) + new Vector3(0, y + archHeight * 0.5f, 0), new Vector3(0.5f, archHeight * 0.5f, 0.5f), new Vector3(0, ang, 0));
                }

                // Cornice
                Prim(PrimitiveType.Cylinder, t, new Vector3(0, y + archHeight, 0), new Vector3(radius * 2.05f, 0.4f, radius * 2.05f));

                radius *= 0.97f;
            }

            // Inner arena floor
            Prim(PrimitiveType.Cylinder, t, new Vector3(0, 0.6f, 0), new Vector3(28, 0.2f, 22), default, CreateDefaultMaterial(new Color(0.65f, 0.55f, 0.40f)));

            // Partially-collapsed wall (one side missing the upper rings — simulate with a clipping wall)
            Prim(PrimitiveType.Cube, t, new Vector3(15, 8, 0), new Vector3(0.1f, 12, 24), default, mat);
        }

        private static void BuildStonehenge(GameObject root, Material mat)
        {
            var t = root.transform;
            int sarsen = 16;
            float r = 6f;
            for (int i = 0; i < sarsen; i++)
            {
                float ang = i * (360f / sarsen);
                Vector3 dir = Quaternion.Euler(0, ang, 0) * Vector3.forward;
                Vector3 p = dir * r;
                // Standing stone
                Prim(PrimitiveType.Cube, t, p + new Vector3(0, 2, 0), new Vector3(1.4f, 4, 0.9f), new Vector3(0, ang, 0));
                // Lintel between every two stones
                if (i % 2 == 0)
                {
                    float midAng = ang + (180f / sarsen);
                    Vector3 midDir = Quaternion.Euler(0, midAng, 0) * Vector3.forward;
                    Prim(PrimitiveType.Cube, t, midDir * r + new Vector3(0, 4.3f, 0), new Vector3(2.4f, 0.5f, 0.9f), new Vector3(0, midAng, 0));
                }
            }
            // Inner trilithon horseshoe
            for (int i = 0; i < 5; i++)
            {
                float ang = -90f + i * 45f;
                Vector3 dir = Quaternion.Euler(0, ang, 0) * Vector3.forward;
                Vector3 p = dir * 3.2f;
                Prim(PrimitiveType.Cube, t, p + new Vector3(0, 3, 0), new Vector3(1.5f, 6, 1.0f), new Vector3(0, ang, 0));
            }
            // Heel stone
            Prim(PrimitiveType.Cube, t, new Vector3(0, 1.8f, 12), new Vector3(1, 3.6f, 0.9f), new Vector3(8, 10, 0));
            // Grass (dark-green disc)
            Prim(PrimitiveType.Cylinder, t, new Vector3(0, 0.05f, 0), new Vector3(20, 0.05f, 20), default, CreateDefaultMaterial(new Color(0.25f, 0.40f, 0.20f)));
        }

        private static void BuildParthenon(GameObject root, Material mat)
        {
            var t = root.transform;
            // Stylobate (3-step base)
            for (int i = 0; i < 3; i++)
                Prim(PrimitiveType.Cube, t, new Vector3(0, 0.4f + i * 0.4f, 0), new Vector3(20 - i * 0.5f, 0.4f, 12 - i * 0.5f));

            // Doric columns (8 across, 17 deep)
            int cx = 8; int cz = 17;
            float spacingX = 2.5f; float spacingZ = 0.65f;
            for (int x = 0; x < cx; x++)
            for (int z = 0; z < cz; z++)
            {
                bool perimeter = (x == 0 || x == cx - 1 || z == 0 || z == cz - 1);
                if (!perimeter) continue;
                Vector3 pos = new Vector3((x - (cx - 1) / 2f) * spacingX, 4.6f, (z - (cz - 1) / 2f) * spacingZ);
                // shaft
                Prim(PrimitiveType.Cylinder, t, pos, new Vector3(0.7f, 4f, 0.7f));
                // capital
                Prim(PrimitiveType.Cube, t, pos + new Vector3(0, 4.0f, 0), new Vector3(1.0f, 0.3f, 1.0f));
                // base
                Prim(PrimitiveType.Cylinder, t, pos + new Vector3(0, -3.95f, 0), new Vector3(0.85f, 0.1f, 0.85f));
            }

            // Entablature
            Prim(PrimitiveType.Cube, t, new Vector3(0, 9.0f, 0), new Vector3(20, 0.6f, 12));
            // Pediment (triangular, approximated with flat triangle prism)
            Prim(PrimitiveType.Cube, t, new Vector3(0, 9.7f, 4.5f), new Vector3(20, 1.4f, 0.5f), new Vector3(20, 0, 0));
            Prim(PrimitiveType.Cube, t, new Vector3(0, 9.7f, -4.5f), new Vector3(20, 1.4f, 0.5f), new Vector3(-20, 0, 0));
            // Roof
            Prim(PrimitiveType.Cube, t, new Vector3(0, 10.5f, 0), new Vector3(20.4f, 0.4f, 12.4f));
        }

        private static void BuildEiffelTower(GameObject root, Material mat)
        {
            var t = root.transform;
            // 4 legs converging from base to first platform
            float legBase = 8f;
            float legTop = 3f;
            float legHeight = 12f;
            for (int s = 0; s < 4; s++)
            {
                float a = s * 90f + 45f;
                Vector3 dir = Quaternion.Euler(0, a, 0) * Vector3.forward;
                Vector3 b = dir * legBase + new Vector3(0, 6f, 0);
                Prim(PrimitiveType.Cube, t, b, new Vector3(0.6f, legHeight, 0.6f), new Vector3(15, a, 0));
                // cross-bracing X
                Prim(PrimitiveType.Cube, t, b, new Vector3(0.2f, legHeight * 0.9f, 0.2f), new Vector3(-15, a, 30));
                Prim(PrimitiveType.Cube, t, b, new Vector3(0.2f, legHeight * 0.9f, 0.2f), new Vector3(-15, a, -30));
            }

            // First platform
            Prim(PrimitiveType.Cube, t, new Vector3(0, 12, 0), new Vector3(7, 0.5f, 7));

            // Middle section
            for (int i = 0; i < 4; i++)
            {
                float a = i * 90f + 45f;
                Vector3 dir = Quaternion.Euler(0, a, 0) * Vector3.forward;
                Prim(PrimitiveType.Cube, t, dir * 2f + new Vector3(0, 16, 0), new Vector3(0.4f, 8, 0.4f), new Vector3(8, a, 0));
            }

            // Second platform
            Prim(PrimitiveType.Cube, t, new Vector3(0, 20, 0), new Vector3(4, 0.4f, 4));

            // Tapered top
            for (int i = 0; i < 12; i++)
            {
                float h = 20 + i * 0.9f;
                float s = 1.5f - i * 0.1f;
                Prim(PrimitiveType.Cube, t, new Vector3(0, h, 0), new Vector3(s, 0.9f, s));
            }
            // Antenna
            Prim(PrimitiveType.Cylinder, t, new Vector3(0, 33, 0), new Vector3(0.2f, 3, 0.2f));
            Prim(PrimitiveType.Sphere,   t, new Vector3(0, 36, 0), new Vector3(0.4f, 0.4f, 0.4f), default, CreateDefaultMaterial(Color.red));
        }

        private static void BuildChristTheRedeemer(GameObject root, Material mat)
        {
            var t = root.transform;
            // Pedestal mountain top
            Prim(PrimitiveType.Cube, t, new Vector3(0, 0.5f, 0), new Vector3(8, 1, 8), default, CreateDefaultMaterial(new Color(0.4f, 0.35f, 0.30f)));
            Prim(PrimitiveType.Cube, t, new Vector3(0, 1.5f, 0), new Vector3(4, 1, 4));

            // Body (long robe)
            Prim(PrimitiveType.Cube,     t, new Vector3(0, 6, 0),    new Vector3(2.6f, 8, 1.6f));
            // Lower flare of robe
            Prim(PrimitiveType.Cube,     t, new Vector3(0, 2.5f, 0), new Vector3(3.4f, 2, 2));

            // Shoulders (outstretched arms)
            Prim(PrimitiveType.Cube, t, new Vector3(0, 9.5f, 0), new Vector3(8.5f, 0.8f, 0.9f));
            // Arm extensions (down to fingertips)
            Prim(PrimitiveType.Cube, t, new Vector3( 4.25f, 9.5f, 0), new Vector3(0.7f, 0.7f, 0.7f));
            Prim(PrimitiveType.Cube, t, new Vector3(-4.25f, 9.5f, 0), new Vector3(0.7f, 0.7f, 0.7f));

            // Head + halo
            Prim(PrimitiveType.Sphere, t, new Vector3(0, 11, 0), new Vector3(1.4f, 1.6f, 1.4f));
            // Beard
            Prim(PrimitiveType.Cube,   t, new Vector3(0, 10.4f, 0.3f), new Vector3(0.6f, 0.5f, 0.4f));
            // Hair
            Prim(PrimitiveType.Sphere, t, new Vector3(0, 11.2f, -0.2f), new Vector3(1.6f, 1.0f, 1.4f));
        }

        private static void BuildStatueOfLiberty(GameObject root, Material mat)
        {
            var t = root.transform;
            // 11-pointed star pedestal (approximate with octagon)
            Prim(PrimitiveType.Cylinder, t, new Vector3(0, 1.5f, 0), new Vector3(7, 3, 7), default, CreateDefaultMaterial(new Color(0.50f, 0.42f, 0.35f)));
            // Plinth
            Prim(PrimitiveType.Cube, t, new Vector3(0, 4, 0), new Vector3(4, 2, 4));

            // Robe
            Prim(PrimitiveType.Cube,     t, new Vector3(0, 8, 0), new Vector3(2.4f, 6, 1.8f));
            Prim(PrimitiveType.Cube,     t, new Vector3(0, 5.5f, 0), new Vector3(3.0f, 1.5f, 2.2f));

            // Right arm raised holding torch
            Prim(PrimitiveType.Cube, t, new Vector3(1.0f, 12, 0), new Vector3(0.6f, 4, 0.6f), new Vector3(0, 0, 25));
            Prim(PrimitiveType.Cube, t, new Vector3(2.6f, 14.5f, 0), new Vector3(0.4f, 1, 0.4f));
            // Torch base
            Prim(PrimitiveType.Cylinder, t, new Vector3(2.6f, 15.3f, 0), new Vector3(0.7f, 0.4f, 0.7f));
            // Flame
            Prim(PrimitiveType.Sphere, t, new Vector3(2.6f, 16.2f, 0), new Vector3(0.8f, 1.2f, 0.8f), default, CreateDefaultMaterial(new Color(1f, 0.85f, 0.3f)));

            // Left arm down holding tablet
            Prim(PrimitiveType.Cube, t, new Vector3(-1.5f, 9, 0), new Vector3(0.5f, 3, 0.5f), new Vector3(0, 0, -10));
            Prim(PrimitiveType.Cube, t, new Vector3(-2.0f, 7, 0.4f), new Vector3(1.3f, 1.8f, 0.2f), new Vector3(0, 0, -8));

            // Head
            Prim(PrimitiveType.Sphere, t, new Vector3(0, 12, 0), new Vector3(1.2f, 1.4f, 1.2f));

            // 7-spike crown
            for (int i = 0; i < 7; i++)
            {
                float a = -90f + i * 30f;
                Vector3 dir = Quaternion.Euler(0, 0, a) * Vector3.up;
                Prim(PrimitiveType.Cube, t, new Vector3(0, 12.6f, 0) + dir * 0.9f, new Vector3(0.18f, 1.2f, 0.18f), new Vector3(0, 0, a));
            }
        }

        private static void BuildMoai(GameObject root, Material mat)
        {
            var t = root.transform;
            // Base
            Prim(PrimitiveType.Cube, t, new Vector3(0, 0.5f, 0), new Vector3(4, 1, 6), default, CreateDefaultMaterial(new Color(0.25f, 0.20f, 0.15f)));

            // Body / torso
            Prim(PrimitiveType.Cube, t, new Vector3(0, 4, 0), new Vector3(3, 6, 2));

            // Head — elongated
            Prim(PrimitiveType.Cube, t, new Vector3(0, 8.5f, 0), new Vector3(2.6f, 4, 2.4f));
            // Brow ridge
            Prim(PrimitiveType.Cube, t, new Vector3(0, 9.5f, 1.3f), new Vector3(2.6f, 0.5f, 0.3f));
            // Nose (long)
            Prim(PrimitiveType.Cube, t, new Vector3(0, 8.6f, 1.3f), new Vector3(0.6f, 1.6f, 0.5f));
            // Eye sockets
            Prim(PrimitiveType.Cube, t, new Vector3( 0.7f, 9.2f, 1.25f), new Vector3(0.6f, 0.4f, 0.1f), default, CreateDefaultMaterial(new Color(0.05f,0.05f,0.05f)));
            Prim(PrimitiveType.Cube, t, new Vector3(-0.7f, 9.2f, 1.25f), new Vector3(0.6f, 0.4f, 0.1f), default, CreateDefaultMaterial(new Color(0.05f,0.05f,0.05f)));
            // Mouth
            Prim(PrimitiveType.Cube, t, new Vector3(0, 7.8f, 1.25f), new Vector3(1.0f, 0.25f, 0.1f), default, CreateDefaultMaterial(new Color(0.05f,0.05f,0.05f)));

            // Pukao (red headstone hat)
            Prim(PrimitiveType.Cylinder, t, new Vector3(0, 11, 0), new Vector3(2.2f, 0.7f, 2.2f), default, CreateDefaultMaterial(new Color(0.55f, 0.20f, 0.15f)));
        }

        private static void BuildAngkorWat(GameObject root, Material mat)
        {
            var t = root.transform;
            // Base platform with moat
            Prim(PrimitiveType.Cube, t, new Vector3(0, 0.5f, 0), new Vector3(40, 1, 40), default, CreateDefaultMaterial(new Color(0.3f, 0.5f, 0.7f))); // moat = bluish
            Prim(PrimitiveType.Cube, t, new Vector3(0, 1.2f, 0), new Vector3(34, 1, 34));

            // Outer galleries (rectangular wall)
            float gw = 30f;
            for (int side = 0; side < 4; side++)
            {
                float a = side * 90f;
                Vector3 dir = Quaternion.Euler(0, a, 0) * Vector3.forward;
                Prim(PrimitiveType.Cube, t, dir * (gw / 2) + new Vector3(0, 3, 0), new Vector3(gw, 4, 0.6f), new Vector3(0, a, 0));
            }

            // 5 main towers in quincunx pattern
            float towerHeight = 12f;
            BuildAngkorTower(t, new Vector3(0, 2, 0), towerHeight);            // central
            BuildAngkorTower(t, new Vector3( 8, 2,  8), towerHeight * 0.7f);
            BuildAngkorTower(t, new Vector3(-8, 2,  8), towerHeight * 0.7f);
            BuildAngkorTower(t, new Vector3( 8, 2, -8), towerHeight * 0.7f);
            BuildAngkorTower(t, new Vector3(-8, 2, -8), towerHeight * 0.7f);

            // Causeway
            Prim(PrimitiveType.Cube, t, new Vector3(0, 1.5f, 22), new Vector3(3, 0.6f, 14));
        }

        private static void BuildAngkorTower(Transform t, Vector3 offset, float height)
        {
            // Stepped beehive shaped tower
            int steps = 8;
            float baseSize = 5f;
            for (int i = 0; i < steps; i++)
            {
                float frac = (float)i / steps;
                float s = baseSize * (1f - frac * 0.7f);
                Prim(PrimitiveType.Cube, t, offset + new Vector3(0, height * frac, 0), new Vector3(s, height / steps, s));
            }
            // Lotus cap
            Prim(PrimitiveType.Sphere, t, offset + new Vector3(0, height + 0.3f, 0), new Vector3(2, 1.6f, 2));
            Prim(PrimitiveType.Cylinder, t, offset + new Vector3(0, height + 1.2f, 0), new Vector3(0.4f, 0.5f, 0.4f));
        }

        private static void BuildGreatWall(GameObject root, Material mat)
        {
            var t = root.transform;
            // Snaking wall (5 segments at increasing y to suggest hills)
            int segs = 5;
            float length = 12f;
            float[] heightOffsets = { 0, 1.5f, 3f, 1.5f, 0f };
            for (int i = 0; i < segs; i++)
            {
                Vector3 c = new Vector3((i - segs / 2f) * length + length / 2, heightOffsets[i] + 2, Mathf.Sin(i * 0.8f) * 2);
                // Wall body
                Prim(PrimitiveType.Cube, t, c, new Vector3(length, 4, 2.4f), new Vector3(0, Mathf.Sin(i) * 5, 0));
                // Crenellations
                for (int k = 0; k < 8; k++)
                {
                    Prim(PrimitiveType.Cube, t, c + new Vector3((k - 3.5f) * 1.2f, 2.2f, 1.0f), new Vector3(0.6f, 0.6f, 0.4f));
                    Prim(PrimitiveType.Cube, t, c + new Vector3((k - 3.5f) * 1.2f, 2.2f, -1.0f), new Vector3(0.6f, 0.6f, 0.4f));
                }
            }
            // Watchtower at center
            Prim(PrimitiveType.Cube, t, new Vector3(0, 6, 0), new Vector3(4, 5, 4));
            Prim(PrimitiveType.Cube, t, new Vector3(0, 8.7f, 0), new Vector3(4.4f, 0.4f, 4.4f));
            Prim(PrimitiveType.Cube, t, new Vector3(0, 9.5f, 0), new Vector3(3.5f, 1.4f, 3.5f));

            // Mountains as backdrop
            for (int i = 0; i < 6; i++)
            {
                float x = -25 + i * 10;
                Prim(PrimitiveType.Cube, t, new Vector3(x, -2, -8), new Vector3(8, 4 + Random.value * 3, 6), new Vector3(0, 0, 45), CreateDefaultMaterial(new Color(0.35f, 0.42f, 0.45f)));
            }
        }

        private static void BuildPetraTreasury(GameObject root, Material mat)
        {
            var t = root.transform;
            // Rock face
            Prim(PrimitiveType.Cube, t, new Vector3(0, 12, -2), new Vector3(28, 24, 4), default, CreateDefaultMaterial(new Color(0.78f, 0.42f, 0.30f)));

            // Lower facade — 6 columns
            for (int i = -2; i <= 3; i++)
            {
                Prim(PrimitiveType.Cylinder, t, new Vector3(i * 2.5f - 1.25f, 4, 0.5f), new Vector3(0.7f, 4, 0.7f));
                Prim(PrimitiveType.Cube, t, new Vector3(i * 2.5f - 1.25f, 8.1f, 0.5f), new Vector3(1.0f, 0.3f, 1.0f));
                Prim(PrimitiveType.Cylinder, t, new Vector3(i * 2.5f - 1.25f, -0.05f, 0.5f), new Vector3(0.85f, 0.1f, 0.85f));
            }

            // Pediment (triangular)
            Prim(PrimitiveType.Cube, t, new Vector3(0, 9, 0.5f), new Vector3(15, 1, 1));
            Prim(PrimitiveType.Cube, t, new Vector3(-3, 10, 0.5f), new Vector3(7, 0.5f, 0.8f), new Vector3(0, 0, 20));
            Prim(PrimitiveType.Cube, t, new Vector3( 3, 10, 0.5f), new Vector3(7, 0.5f, 0.8f), new Vector3(0, 0, -20));

            // Upper level — central tholos with broken pediments
            Prim(PrimitiveType.Cylinder, t, new Vector3(0, 16, 0.5f), new Vector3(4, 4, 4));
            Prim(PrimitiveType.Sphere,   t, new Vector3(0, 18.5f, 0.5f), new Vector3(2.5f, 1.8f, 2.5f));
            // Urn on top
            Prim(PrimitiveType.Sphere, t, new Vector3(0, 20, 0.5f), new Vector3(0.8f, 1.2f, 0.8f));
            Prim(PrimitiveType.Cylinder, t, new Vector3(0, 20.8f, 0.5f), new Vector3(0.3f, 0.3f, 0.3f));

            // Side wings
            Prim(PrimitiveType.Cube, t, new Vector3(-7, 14, 0.5f), new Vector3(4, 6, 1));
            Prim(PrimitiveType.Cube, t, new Vector3( 7, 14, 0.5f), new Vector3(4, 6, 1));

            // Doorway
            Prim(PrimitiveType.Cube, t, new Vector3(0, 3.5f, 0.7f), new Vector3(2, 5, 0.4f), default, CreateDefaultMaterial(Color.black));
        }

        private static void BuildChichenItza(GameObject root, Material mat)
        {
            var t = root.transform;
            // 9 tiered stepped pyramid
            int tiers = 9;
            float baseSize = 18f;
            float tierH = 1.4f;
            for (int i = 0; i < tiers; i++)
            {
                float s = baseSize - i * 1.7f;
                Prim(PrimitiveType.Cube, t, new Vector3(0, i * tierH + tierH * 0.5f, 0), new Vector3(s, tierH, s));
            }

            // Stairway on each face
            for (int s = 0; s < 4; s++)
            {
                float a = s * 90f;
                Vector3 dir = Quaternion.Euler(0, a, 0) * Vector3.forward;
                Vector3 mid = new Vector3(0, tiers * tierH * 0.5f, 0);
                Prim(PrimitiveType.Cube, t, dir * (baseSize / 2 + 0.1f) + mid, new Vector3(3, tiers * tierH, baseSize), new Vector3(0, a, 0));
            }

            // Top temple
            Prim(PrimitiveType.Cube, t, new Vector3(0, tiers * tierH + 1.5f, 0), new Vector3(5, 3, 5));
            Prim(PrimitiveType.Cube, t, new Vector3(0, tiers * tierH + 3.5f, 0), new Vector3(5.4f, 0.6f, 5.4f));
        }

        private static void BuildLeaningTower(GameObject root, Material mat)
        {
            // Build vertical, then tilt the entire tower
            var t = root.transform;
            var pivot = new GameObject("Pivot");
            pivot.transform.SetParent(t, false);
            pivot.transform.localRotation = Quaternion.Euler(0, 0, 5.5f); // famous tilt

            for (int i = 0; i < 8; i++)
            {
                Prim(PrimitiveType.Cylinder, pivot.transform, new Vector3(0, i * 2.4f + 1.2f, 0), new Vector3(4.2f, 1.2f, 4.2f));
                // Arched colonnade
                int cols = 24;
                for (int c = 0; c < cols; c++)
                {
                    float a = c * (360f / cols);
                    Vector3 dir = Quaternion.Euler(0, a, 0) * Vector3.forward;
                    Prim(PrimitiveType.Cube, pivot.transform, dir * 2.05f + new Vector3(0, i * 2.4f + 1.2f, 0), new Vector3(0.25f, 2.0f, 0.25f), new Vector3(0, a, 0));
                }
            }

            // Bell chamber at top
            Prim(PrimitiveType.Cylinder, pivot.transform, new Vector3(0, 8 * 2.4f + 1.4f, 0), new Vector3(3.5f, 1.4f, 3.5f));
            Prim(PrimitiveType.Sphere,   pivot.transform, new Vector3(0, 8 * 2.4f + 2.6f, 0), new Vector3(1.5f, 1.0f, 1.5f));
        }

        private static void BuildSydneyOperaHouse(GameObject root, Material mat)
        {
            var t = root.transform;
            // Podium / harbor base
            Prim(PrimitiveType.Cube, t, new Vector3(0, 0.5f, 0), new Vector3(24, 1, 12), default, CreateDefaultMaterial(new Color(0.65f, 0.55f, 0.40f)));

            // Three groups of "sails" (each made of overlapping half-spheres tilted)
            BuildOperaSailGroup(t, new Vector3(-7, 1, 0), 1.0f);
            BuildOperaSailGroup(t, new Vector3( 0, 1, 0), 1.2f);
            BuildOperaSailGroup(t, new Vector3( 7, 1, 0), 0.9f);

            // Water around
            Prim(PrimitiveType.Cube, t, new Vector3(0, 0.05f, 0), new Vector3(40, 0.1f, 30), default, CreateDefaultMaterial(new Color(0.20f, 0.45f, 0.65f)));
        }

        private static void BuildOperaSailGroup(Transform t, Vector3 offset, float scale)
        {
            for (int i = 0; i < 4; i++)
            {
                float tiltZ = -10f + i * 8f;
                float yOffset = i * 0.6f;
                Prim(PrimitiveType.Sphere, t, offset + new Vector3(i * 0.6f, 3f * scale + yOffset, 0),
                     new Vector3(4f * scale, 5f * scale, 3f * scale), new Vector3(0, 0, tiltZ));
            }
        }
    }
}
