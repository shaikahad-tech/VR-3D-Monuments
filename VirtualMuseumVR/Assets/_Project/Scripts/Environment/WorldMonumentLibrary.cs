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
        // Indian heritage builders (Highly detailed)
        // =================================================================
        private static GameObject AddBox(Transform parent, Vector3 pos, Vector3 size, Vector3 rot = default, Material mat = null) {
            return Prim(PrimitiveType.Cube, parent, pos, size, rot, mat);
        }
        private static GameObject AddCylinder(Transform parent, float radius, float height, Vector3 pos, Vector3 rot = default, Material mat = null) {
            return Prim(PrimitiveType.Cylinder, parent, pos, new Vector3(radius * 2f, height / 2f, radius * 2f), rot, mat);
        }
        private static GameObject AddSphere(Transform parent, float radius, Vector3 pos, Vector3 scale = default, Material mat = null) {
            if (scale == default) scale = Vector3.one;
            return Prim(PrimitiveType.Sphere, parent, pos, new Vector3(radius * 2f * scale.x, radius * 2f * scale.y, radius * 2f * scale.z), default, mat);
        }

        private static void BuildTajMahal(GameObject root, Material mat)
        {
            var t = root.transform;
            var plinthMat = CreateDefaultMaterial(new Color(0.55f, 0.27f, 0.20f));
            var dark = CreateDefaultMaterial(new Color(0.15f, 0.15f, 0.15f));

            AddBox(t, new Vector3(0, 0.5f, 0), new Vector3(34, 1f, 34), default, plinthMat);
            AddBox(t, new Vector3(0, 1.5f, 0), new Vector3(28, 1f, 28));
            AddBox(t, new Vector3(0, 5.5f, 0), new Vector3(20, 7f, 20));

            for (int s = 0; s < 4; s++)
            {
                float a = s * 90f;
                Vector3 dir = Quaternion.Euler(0, a, 0) * Vector3.forward;
                AddBox(t, dir * 10.05f + new Vector3(0, 5.5f, 0), new Vector3(8, 6, 0.5f), new Vector3(0, a, 0), dark);
                AddCylinder(t, 4f, 1f, dir * 10.05f + new Vector3(0, 8.5f, 0), new Vector3(90, a, 0), dark);
            }

            AddCylinder(t, 5.5f, 1f, new Vector3(0, 9.5f, 0));
            AddSphere(t, 5.5f, new Vector3(0, 11.5f, 0), new Vector3(1, 1.1f, 1));
            AddCylinder(t, 0.25f, 2f, new Vector3(0, 18.5f, 0));
            AddSphere(t, 0.7f, new Vector3(0, 19.5f, 0));

            Vector3[] chhatris = { new Vector3(7,10,7), new Vector3(-7,10,7), new Vector3(7,10,-7), new Vector3(-7,10,-7) };
            foreach (var p in chhatris)
            {
                for (int i=0; i<8; i++) {
                    float ang = i * 45f;
                    Vector3 d = Quaternion.Euler(0, ang, 0) * Vector3.forward;
                    AddCylinder(t, 0.15f, 1.5f, p + d * 1.5f + new Vector3(0, -0.75f, 0));
                }
                AddCylinder(t, 2f, 0.2f, p + new Vector3(0, -1.5f, 0));
                AddSphere(t, 1.8f, p + new Vector3(0, 0.5f, 0), new Vector3(1, 0.8f, 1));
            }

            Vector3[] minarets = { new Vector3(13, 8, 13), new Vector3(-13, 8, 13), new Vector3(13, 8, -13), new Vector3(-13, 8, -13) };
            foreach (var p in minarets)
            {
                AddCylinder(t, 0.9f, 14f, p);
                for (int g = 0; g < 3; g++) AddCylinder(t, 1.2f, 0.2f, p + new Vector3(0, -6 + g*5, 0));
                for (int i=0; i<8; i++) {
                    float ang = i * 45f;
                    Vector3 d = Quaternion.Euler(0, ang, 0) * Vector3.forward;
                    AddCylinder(t, 0.1f, 1.2f, p + d * 0.7f + new Vector3(0, 7.6f, 0));
                }
                AddSphere(t, 0.9f, p + new Vector3(0, 8.5f, 0));
            }

            var poolMat = CreateDefaultMaterial(new Color(0.25f, 0.45f, 0.65f));
            AddBox(t, new Vector3(0, 1.55f, 22), new Vector3(3, 0.05f, 30), default, poolMat);
        }

        private static void BuildQutubMinar(GameObject root, Material mat)
        {
            var t = root.transform;
            var white = CreateDefaultMaterial(new Color(0.85f, 0.82f, 0.78f));
            float h = 0f, r = 4.5f;

            for (int i = 0; i < 5; i++)
            {
                var curMat = (i == 3 || i == 4) ? white : mat;
                float tierH = 8f - i;
                float nextR = r * 0.75f;
                
                // Base cylinder
                var cyl = AddCylinder(t, r, tierH, new Vector3(0, h + tierH/2f, 0), default, curMat);
                // Fake taper by scaling top smaller than bottom isn't possible with basic Cylinder
                // so we approximate with a cylinder

                // Detailed fluting
                int flutes = 24;
                for (int f = 0; f < flutes; f++)
                {
                    float a = f * 360f / flutes;
                    Vector3 dir = Quaternion.Euler(0, a, 0) * Vector3.forward;
                    bool isAngular = (i == 0 && f % 2 == 0) || (i == 2);
                    
                    if (isAngular) {
                        AddBox(t, dir * (r * 0.9f) + new Vector3(0, h + tierH/2f, 0), new Vector3(r*0.3f, tierH, r*0.3f), new Vector3(0, a + 45f, 0), curMat);
                    } else {
                        AddCylinder(t, r*0.15f, tierH, dir * (r * 0.9f) + new Vector3(0, h + tierH/2f, 0), default, curMat);
                    }
                }

                if (i < 4) {
                    AddCylinder(t, r*1.3f, 0.4f, new Vector3(0, h + tierH, 0), default, mat);
                    for (int b = 0; b < 32; b++) {
                        float a = b * 360f / 32;
                        Vector3 dir = Quaternion.Euler(0, a, 0) * Vector3.forward;
                        AddBox(t, dir * (r * 1.1f) + new Vector3(0, h + tierH - 0.6f, 0), new Vector3(0.3f, 1.2f, 1.8f), new Vector3(0, a, 0), mat);
                    }
                }
                h += tierH;
                r = nextR;
            }
            AddCylinder(t, r * 0.9f, 1.5f, new Vector3(0, h + 0.75f, 0), default, mat);
        }

        private static void BuildHampiChariot(GameObject root, Material mat)
        {
            var t = root.transform;
            var dark = CreateDefaultMaterial(new Color(0.35f, 0.29f, 0.21f));

            AddBox(t, new Vector3(0, 1.8f, 0), new Vector3(6.5f, 1.8f, 8.5f));
            AddBox(t, new Vector3(0, 3.1f, 0), new Vector3(5.8f, 0.8f, 7.8f));

            Vector3[] wheels = { new Vector3(3.4f, 1.4f, 2.5f), new Vector3(-3.4f, 1.4f, 2.5f), new Vector3(3.4f, 1.4f, -2.5f), new Vector3(-3.4f, 1.4f, -2.5f) };
            foreach (var w in wheels)
            {
                AddCylinder(t, 1.4f, 0.4f, w, new Vector3(0, 0, 90));
                AddCylinder(t, 1.2f, 0.45f, w, new Vector3(0, 0, 90), dark);
                AddSphere(t, 0.35f, w + new Vector3(Mathf.Sign(w.x) * 0.25f, 0, 0));
                for (int s = 0; s < 8; s++) {
                    float a = s * 45f;
                    AddBox(t, w, new Vector3(0.3f, 2.4f, 0.1f), new Vector3(90, a, 90));
                }
            }

            AddBox(t, new Vector3(0, 5.75f, 0), new Vector3(4.2f, 4.5f, 4.2f));
            AddBox(t, new Vector3(0, 5.5f, 0), new Vector3(2f, 3f, 4.3f), default, dark);

            float[] pilX = { -1.9f, 1.9f };
            foreach (var x in pilX) {
                foreach (var z in pilX) {
                    AddCylinder(t, 0.3f, 4.5f, new Vector3(x, 5.75f, z));
                    AddBox(t, new Vector3(x, 3.7f, z), new Vector3(0.8f, 0.4f, 0.8f));
                    AddBox(t, new Vector3(x, 7.8f, z), new Vector3(0.9f, 0.4f, 0.9f));
                }
            }

            AddBox(t, new Vector3(0, 8.3f, 0), new Vector3(4.8f, 0.6f, 4.8f));
            AddBox(t, new Vector3(0, 9.35f, 0), new Vector3(3.8f, 1.5f, 3.8f));
            AddSphere(t, 2.2f, new Vector3(0, 10.5f, 0), new Vector3(1, 0.8f, 1));
            AddCylinder(t, 0.3f, 1.5f, new Vector3(0, 12.0f, 0));
        }

        private static void BuildKonarkTemple(GameObject root, Material mat)
        {
            var t = root.transform;
            var dark = CreateDefaultMaterial(new Color(0.36f, 0.26f, 0.17f));

            AddBox(t, new Vector3(0, 2, 0), new Vector3(26, 4, 36));
            AddBox(t, new Vector3(0, 5, 0), new Vector3(24, 2, 34));

            for (int i = 0; i < 12; i++)
            {
                float z = -15f + i * 2.7f;
                foreach (float side in new float[] { -1, 1 }) {
                    float x = side * 13.2f;
                    AddCylinder(t, 1.8f, 0.5f, new Vector3(x, 1.8f, z), new Vector3(0, 0, 90));
                    AddCylinder(t, 1.5f, 0.55f, new Vector3(x, 1.8f, z), new Vector3(0, 0, 90), dark);
                    AddSphere(t, 0.4f, new Vector3(x - side * 0.1f, 1.8f, z));
                    for (int s = 0; s < 8; s++) {
                        float a = s * 45f;
                        AddBox(t, new Vector3(x, 1.8f, z), new Vector3(0.5f, 3f, 0.2f), new Vector3(90, a, 90));
                        AddBox(t, new Vector3(x, 1.8f, z), new Vector3(0.15f, 3f, 0.1f), new Vector3(90, a + 22.5f, 90));
                    }
                }
            }

            AddBox(t, new Vector3(0, 11, 6), new Vector3(18, 10, 18));
            float h = 16f, s = 19f;
            for (int tier = 0; tier < 3; tier++) {
                int steps = tier == 2 ? 5 : 6;
                for (int step = 0; step < steps; step++) {
                    AddBox(t, new Vector3(0, h, 6), new Vector3(s, 0.6f, s), default, dark);
                    s -= 0.6f; h += 0.8f;
                }
                AddBox(t, new Vector3(0, h + 0.75f, 6), new Vector3(s - 1, 1.5f, s - 1));
                s -= 1.5f; h += 1.5f;
            }
            AddCylinder(t, 3.5f, 1.5f, new Vector3(0, h + 0.75f, 6));
            AddSphere(t, 1.5f, new Vector3(0, h + 2.5f, 6));

            AddBox(t, new Vector3(0, 8, -12), new Vector3(20, 12, 20));
            AddBox(t, new Vector3(0, 16, -12), new Vector3(18, 4, 18), default, dark);
        }

        private static void BuildAjantaCaves(GameObject root, Material mat)
        {
            var t = root.transform;
            var dark = CreateDefaultMaterial(new Color(0.1f, 0.08f, 0.06f));

            AddBox(t, new Vector3(0, 10, 15), new Vector3(40, 20, 20));
            
            var facade = new GameObject("Facade").transform;
            facade.SetParent(t, false);
            facade.localPosition = new Vector3(0, 0, -10);

            AddBox(facade, new Vector3(0, 10, 0), new Vector3(18, 20, 5));
            AddBox(facade, new Vector3(0, 8, 0), new Vector3(14, 16, 6), default, dark);

            AddCylinder(facade, 5.5f, 2f, new Vector3(0, 12, 1), new Vector3(90, 0, 0));
            AddCylinder(facade, 4.5f, 2.5f, new Vector3(0, 12, 1), new Vector3(90, 0, 0), dark);

            float[] colX = { -4.5f, -1.5f, 1.5f, 4.5f };
            foreach (float x in colX) {
                AddCylinder(facade, 0.6f, 6f, new Vector3(x, 3, 1));
                AddBox(facade, new Vector3(x, 6.4f, 1), new Vector3(1.5f, 0.8f, 1.5f));
            }

            AddCylinder(facade, 2.5f, 3f, new Vector3(0, 1.5f, -4));
            AddSphere(facade, 2.5f, new Vector3(0, 3, -4), new Vector3(1, 0.8f, 1));
            AddBox(facade, new Vector3(0, 1.5f, -2), new Vector3(2, 3, 1));
            AddSphere(facade, 0.7f, new Vector3(0, 3.5f, -1.8f));
        }

        private static void BuildGreatPyramid(GameObject root, Material mat)
        {
            var t = root.transform;
            var darkSand = CreateDefaultMaterial(new Color(0.72f, 0.62f, 0.42f));

            int tiers = 40;
            float size = 30f;
            float h = 20f;
            for (int i = 0; i < tiers; i++) {
                float tierSize = size * (1f - (float)i / tiers);
                float tierH = h / tiers;
                AddBox(t, new Vector3(0, i * tierH + tierH / 2f, 0), new Vector3(tierSize, tierH, tierSize));
            }

            Vector3 sb = new Vector3(0, 0, -25);
            AddBox(t, sb + new Vector3(-2, 0.75f, 6), new Vector3(3.5f, 1.5f, 4), default, darkSand);
            AddBox(t, sb + new Vector3(2, 0.75f, 6), new Vector3(3.5f, 1.5f, 4), default, darkSand);
            AddBox(t, sb + new Vector3(0, 2f, -1), new Vector3(7, 4, 12), default, darkSand);
            AddBox(t, sb + new Vector3(0, 3f, 3.5f), new Vector3(6, 6, 4), default, darkSand);
            AddSphere(t, 2.2f, sb + new Vector3(0, 7.5f, 4));
            AddBox(t, sb + new Vector3(0, 6.5f, 2.5f), new Vector3(6, 5, 3), default, darkSand);
            AddBox(t, sb + new Vector3(0, 8.5f, 3), new Vector3(7, 2, 3), default, darkSand);
        }

        private static void BuildColosseum(GameObject root, Material mat)
        {
            var t = root.transform;
            float cx = 22f, cz = 18f;
            int arcCount = 48;

            for (int tier = 0; tier < 4; tier++) {
                float y = tier * 3.5f + 1.75f;
                for (int a = 0; a < arcCount; a++) {
                    float ang = a * 360f / arcCount;
                    bool isRuined = (ang > 36f && ang < 324f);
                    if (tier >= 2 && isRuined) continue;

                    Vector3 dir = Quaternion.Euler(0, ang, 0) * Vector3.forward;
                    Vector3 p = new Vector3(dir.x * cx, y, dir.z * cz);

                    AddBox(t, p, new Vector3(1.2f, 3.5f, 1.5f), new Vector3(0, ang, 0));

                    if (tier < 3) {
                        float nAng = (a + 0.5f) * 360f / arcCount;
                        Vector3 nDir = Quaternion.Euler(0, nAng, 0) * Vector3.forward;
                        Vector3 nP = new Vector3(nDir.x * cx, y + 1.25f, nDir.z * cz);
                        AddBox(t, nP, new Vector3(2.2f, 1.0f, 1.5f), new Vector3(0, nAng, 0));
                        AddCylinder(t, 0.4f, 3.5f, p + dir * 0.8f);
                    } else {
                        float nAng = (a + 0.5f) * 360f / arcCount;
                        Vector3 nDir = Quaternion.Euler(0, nAng, 0) * Vector3.forward;
                        Vector3 nP = new Vector3(nDir.x * cx, y, nDir.z * cz);
                        AddBox(t, nP, new Vector3(2.5f, 3.5f, 1.2f), new Vector3(0, nAng, 0));
                        AddBox(t, p + dir * 0.6f, new Vector3(0.8f, 3.5f, 0.4f), new Vector3(0, ang, 0));
                    }
                }
            }

            var arenaMat = CreateDefaultMaterial(new Color(0.36f, 0.30f, 0.24f));
            AddCylinder(t, 11f, 2f, new Vector3(0, 1, 0), default, arenaMat);
            for (int x = -6; x <= 6; x += 2) AddBox(t, new Vector3(x, 2, 0), new Vector3(0.5f, 2, 14));
        }

        private static void BuildStonehenge(GameObject root, Material mat)
        {
            var t = root.transform;
            var grass = CreateDefaultMaterial(new Color(0.25f, 0.37f, 0.18f));

            int sars = 30;
            float r = 8f;
            for (int i = 0; i < sars; i++) {
                float a = i * 360f / sars;
                if (i % 7 == 0 || i % 11 == 0) {
                    if (i % 7 == 0) {
                        Vector3 dir = Quaternion.Euler(0, a, 0) * Vector3.forward;
                        AddBox(t, dir * r + new Vector3(0, 0.5f, 0), new Vector3(1.5f, 4f, 1f), new Vector3(90, a, 0));
                    }
                    continue;
                }
                Vector3 d = Quaternion.Euler(0, a, 0) * Vector3.forward;
                float tiltX = (Random.value - 0.5f) * 10f;
                float tiltZ = (Random.value - 0.5f) * 10f;
                AddBox(t, d * r + new Vector3(0, 2.25f, 0), new Vector3(1.6f, 4.5f, 1.1f), new Vector3(tiltX, a, tiltZ));

                if (i % 2 == 0 && Random.value > 0.1f) {
                    float ma = a + 180f / sars;
                    Vector3 md = Quaternion.Euler(0, ma, 0) * Vector3.forward;
                    AddBox(t, md * r + new Vector3(0, 4.9f, 0), new Vector3(2.2f, 0.8f, 1.2f), new Vector3(0, ma + 90f, 0));
                }
            }

            Vector2[] trilis = { new Vector2(-3, -2), new Vector2(-4.5f, 2), new Vector2(0, 4.5f), new Vector2(4.5f, 2), new Vector2(3, -2) };
            for (int i = 0; i < trilis.Length; i++) {
                float a = Mathf.Atan2(trilis[i].x, trilis[i].y) * Mathf.Rad2Deg;
                float h = 6f + (i == 2 ? 1f : 0f);
                Vector3 dir = Quaternion.Euler(0, a, 0) * Vector3.forward;
                Vector3 right = Quaternion.Euler(0, a + 90f, 0) * Vector3.forward;
                
                Vector3 baseP = new Vector3(trilis[i].x, h/2f, trilis[i].y);
                AddBox(t, baseP - right * 0.9f, new Vector3(1.8f, h, 1.2f), new Vector3(0, a, 0));
                AddBox(t, baseP + right * 0.9f, new Vector3(1.8f, h, 1.2f), new Vector3(0, a, 0));
                AddBox(t, new Vector3(trilis[i].x, h + 0.5f, trilis[i].y), new Vector3(3.8f, 1.0f, 1.4f), new Vector3(0, a + 90f, 0));
            }

            for (int i = 0; i < 40; i++) {
                if (Random.value > 0.4f) continue;
                float a = i * 360f / 40;
                Vector3 dir = Quaternion.Euler(0, a, 0) * Vector3.forward;
                AddBox(t, dir * 5.5f + new Vector3(0, 1, 0), new Vector3(0.8f, 2f, 0.8f), new Vector3(0, a, 0));
            }

            AddBox(t, new Vector3(0, 0.25f, 2), new Vector3(3, 0.5f, 1.5f));
            AddBox(t, new Vector3(0, 2.4f, 22), new Vector3(1.5f, 4.8f, 1.5f), new Vector3(6, 30, 6));
            AddCylinder(t, 28f, 0.2f, new Vector3(0, -0.1f, 0), default, grass);
        }

        private static void BuildParthenon(GameObject root, Material mat)
        {
            var t = root.transform;
            var darkMarble = CreateDefaultMaterial(new Color(0.82f, 0.78f, 0.68f));

            for (int i = 0; i < 3; i++) AddBox(t, new Vector3(0, 0.25f + i * 0.5f, 0), new Vector3(22 - i * 0.6f, 0.5f, 12 - i * 0.6f));

            int cx = 17, cz = 8;
            float sx = 1.2f, sz = 1.35f;
            for (int x = 0; x < cx; x++) {
                for (int z = 0; z < cz; z++) {
                    if (!(x == 0 || x == cx - 1 || z == 0 || z == cz - 1)) continue;
                    float px = (x - (cx - 1) / 2f) * sx;
                    float pz = (z - (cz - 1) / 2f) * sz;
                    AddCylinder(t, 0.42f, 5f, new Vector3(px, 4.0f, pz));
                    AddBox(t, new Vector3(px, 6.6f, pz), new Vector3(0.9f, 0.2f, 0.9f));
                }
            }

            AddBox(t, new Vector3(0, 7.1f, 0), new Vector3(20.4f, 0.8f, 10.4f));
            AddBox(t, new Vector3(0, 7.9f, 0), new Vector3(20.4f, 0.8f, 10.4f), default, darkMarble);

            AddCylinder(t, 5.2f, 20.4f, new Vector3(0, 9.6f, 0), new Vector3(0, 0, 90));
            // Flatten cylinder into triangle
            var ped = t.GetChild(t.childCount - 1);
            ped.localScale = new Vector3(ped.localScale.x, ped.localScale.y, ped.localScale.z * 0.35f);

            AddBox(t, new Vector3(0, 4.0f, 0), new Vector3(14, 5, 6), default, darkMarble);
            
            var roofMat = CreateDefaultMaterial(new Color(0.54f, 0.23f, 0.16f));
            AddBox(t, new Vector3(-5, 10.5f, 0), new Vector3(5, 0.2f, 5), new Vector3(0, 0, 15), roofMat);
        }

        private static void BuildEiffelTower(GameObject root, Material mat)
        {
            var t = root.transform;
            var iron = mat; // Treat as wireframe ideally, but solid here
            var solidIron = CreateDefaultMaterial(new Color(0.23f, 0.20f, 0.18f));

            for (int s = 0; s < 4; s++) {
                float a = s * 90f + 45f;
                Vector3 dir = Quaternion.Euler(0, a, 0) * Vector3.forward;
                Vector3 b1 = dir * 8.5f + new Vector3(0, 4, 0);
                AddBox(t, b1, new Vector3(2.5f, 8, 2.5f), new Vector3(15, a, 0), iron);
                Vector3 b2 = dir * 5.5f + new Vector3(0, 11, 0);
                AddBox(t, b2, new Vector3(1.8f, 8, 1.8f), new Vector3(10, a, 0), iron);
                AddCylinder(t, 4f, 0.5f, dir * 4.2f + new Vector3(0, 2, 0), new Vector3(90, 0, 0), solidIron);
            }

            AddBox(t, new Vector3(0, 8, 0), new Vector3(13, 0.6f, 13), default, solidIron);
            AddBox(t, new Vector3(0, 15, 0), new Vector3(8, 0.6f, 8), default, solidIron);
            AddBox(t, new Vector3(0, 27, 0), new Vector3(3, 0.6f, 3), default, solidIron);

            AddCylinder(t, 4f, 12f, new Vector3(0, 21, 0), new Vector3(0, 45, 0), iron);
            
            AddCylinder(t, 0.8f, 2f, new Vector3(0, 28, 0), default, solidIron);
            AddCylinder(t, 0.1f, 4f, new Vector3(0, 31, 0), default, solidIron);
        }

        private static void BuildChristTheRedeemer(GameObject root, Material mat)
        {
            var t = root.transform;
            var baseMat = CreateDefaultMaterial(new Color(0.33f, 0.33f, 0.33f));

            AddCylinder(t, 3f, 3f, new Vector3(0, 1.5f, 0), default, baseMat);
            AddCylinder(t, 1.5f, 2f, new Vector3(0, 4, 0));

            AddCylinder(t, 1.8f, 8f, new Vector3(0, 9, 0));
            AddCylinder(t, 1.4f, 4f, new Vector3(0, 9, 0.5f));

            AddBox(t, new Vector3(0, 13, 0), new Vector3(15, 1.5f, 1.5f));
            AddBox(t, new Vector3(-8, 13, 0), new Vector3(1, 0.8f, 1));
            AddBox(t, new Vector3(8, 13, 0), new Vector3(1, 0.8f, 1));

            AddSphere(t, 1.2f, new Vector3(0, 14.8f, 0));
            AddBox(t, new Vector3(0, 15.2f, 0), new Vector3(1.4f, 1.0f, 1.4f));
        }

        private static void BuildStatueOfLiberty(GameObject root, Material mat)
        {
            var t = root.transform;
            var pedMat = CreateDefaultMaterial(new Color(0.80f, 0.74f, 0.62f));
            var gold = CreateDefaultMaterial(new Color(1f, 0.85f, 0f));

            for (int i = 0; i < 11; i++) AddBox(t, new Vector3(0, 1, 0), new Vector3(9, 2, 3), new Vector3(0, i * 360f / 11, 0), pedMat);
            AddBox(t, new Vector3(0, 3, 0), new Vector3(6, 2, 6), default, pedMat);
            AddBox(t, new Vector3(0, 8, 0), new Vector3(4.5f, 8, 4.5f), default, pedMat);
            AddBox(t, new Vector3(0, 12.5f, 0), new Vector3(5, 1, 5), default, pedMat);

            AddCylinder(t, 2.5f, 9f, new Vector3(0, 17.5f, 0));
            for (int i = 0; i < 6; i++) AddCylinder(t, 0.6f, 9f, new Vector3(Mathf.Sin(i)*1.5f, 17.5f, Mathf.Cos(i)*1.5f));

            AddSphere(t, 1.2f, new Vector3(0, 23, 0));
            AddCylinder(t, 1.4f, 0.4f, new Vector3(0, 23.5f, 0));
            for (int i = 0; i < 7; i++) {
                float a = -60f + i * 20f;
                Vector3 dir = Quaternion.Euler(0, 0, a) * Vector3.up;
                AddCylinder(t, 0.1f, 1.5f, new Vector3(0, 23.5f, 0) + dir * 0.75f, new Vector3(0, 0, a));
            }

            AddCylinder(t, 0.5f, 5f, new Vector3(1.8f, 21, 0), new Vector3(0, 0, -22.5f));
            AddCylinder(t, 0.8f, 1f, new Vector3(2.8f, 23.5f, 0));
            AddSphere(t, 0.7f, new Vector3(2.8f, 24.2f, 0), default, gold);

            AddBox(t, new Vector3(-1.2f, 19, 1.5f), new Vector3(1.5f, 2, 0.2f), new Vector3(30, 45, 0));
        }

        private static void BuildMoai(GameObject root, Material mat)
        {
            var t = root.transform;
            var pukaoMat = CreateDefaultMaterial(new Color(0.65f, 0.33f, 0.25f));
            var ahuMat = CreateDefaultMaterial(new Color(0.31f, 0.27f, 0.24f));

            AddBox(t, new Vector3(0, 0.75f, 0), new Vector3(8, 1.5f, 6), default, ahuMat);
            AddCylinder(t, 2.5f, 6f, new Vector3(0, 4.5f, 0));
            AddBox(t, new Vector3(0, 3.5f, 1.5f), new Vector3(3, 0.5f, 2));

            AddBox(t, new Vector3(0, 10, 0.5f), new Vector3(3.2f, 5.5f, 3.2f));
            AddBox(t, new Vector3(0, 12, 2.2f), new Vector3(3.6f, 1.0f, 1.2f));
            AddBox(t, new Vector3(0, 10, 2.5f), new Vector3(0.8f, 3.5f, 1.5f));
            AddBox(t, new Vector3(0, 7.5f, 2f), new Vector3(3.6f, 1.8f, 1.5f));
            AddBox(t, new Vector3(-1.8f, 10, 0.5f), new Vector3(0.5f, 3, 0.5f));
            AddBox(t, new Vector3(1.8f, 10, 0.5f), new Vector3(0.5f, 3, 0.5f));

            AddCylinder(t, 2.2f, 1.8f, new Vector3(0, 13.6f, 0.5f), default, pukaoMat);
            AddCylinder(t, 1.0f, 0.5f, new Vector3(0, 14.7f, 0.5f), default, pukaoMat);
        }

        private static void BuildAngkorWat(GameObject root, Material mat)
        {
            var t = root.transform;
            var dark = CreateDefaultMaterial(new Color(0.42f, 0.38f, 0.27f));

            AddBox(t, new Vector3(0, 1, 0), new Vector3(32, 2, 24));
            AddBox(t, new Vector3(0, 3, 0), new Vector3(22, 2, 16));
            AddBox(t, new Vector3(0, 5.5f, 0), new Vector3(12, 3, 10));

            AddBox(t, new Vector3(0, 2.2f, 0), new Vector3(30, 0.5f, 1), default, dark);
            AddBox(t, new Vector3(0, 2.2f, 0), new Vector3(1, 0.5f, 22), default, dark);

            Vector3[] towers = { new Vector3(0,7,0), new Vector3(-5,7,-4), new Vector3(5,7,-4), new Vector3(-5,7,4), new Vector3(5,7,4) };
            float[] scales = { 1.4f, 0.7f, 0.7f, 0.7f, 0.7f };
            for (int j = 0; j < 5; j++) {
                float s = scales[j];
                Vector3 p = towers[j];
                AddBox(t, p + new Vector3(0, 1.5f*s, 0), new Vector3(2.5f*s, 3*s, 2.5f*s));
                for (int i = 0; i < 6; i++) {
                    float r = (1.8f - i * 0.25f) * s;
                    AddCylinder(t, r, 0.8f * s, p + new Vector3(0, (3.4f + i * 0.8f) * s, 0));
                }
                AddSphere(t, 0.5f * s, p + new Vector3(0, 8.2f * s, 0));
            }
        }

        private static void BuildGreatWall(GameObject root, Material mat)
        {
            var t = root.transform;
            var brick = CreateDefaultMaterial(new Color(0.50f, 0.45f, 0.38f));

            for (int i = -2; i <= 2; i++) {
                float x = Mathf.Sin(i) * 3f;
                float z = i * 6f;
                float rotY = Mathf.Cos(i) * 11.4f;
                AddBox(t, new Vector3(x, 3, z), new Vector3(8, 6, 6.5f), new Vector3(0, rotY, 0));

                for (float dz = -2.5f; dz <= 2.5f; dz += 1.5f) {
                    Vector3 d = Quaternion.Euler(0, rotY, 0) * Vector3.forward;
                    Vector3 r = Quaternion.Euler(0, rotY + 90f, 0) * Vector3.forward;
                    AddBox(t, new Vector3(x, 6.75f, z) + r * 3.6f + d * dz, new Vector3(0.8f, 1.5f, 1), new Vector3(0, rotY, 0), brick);
                    AddBox(t, new Vector3(x, 6.75f, z) - r * 3.6f + d * dz, new Vector3(0.8f, 1.5f, 1), new Vector3(0, rotY, 0), brick);
                }
            }

            AddBox(t, new Vector3(0, 5, 0), new Vector3(11, 10, 11), default, brick);
            AddBox(t, new Vector3(0, 10.5f, 0), new Vector3(12, 1, 12));
            AddBox(t, new Vector3(0, 7, 0), new Vector3(3, 4, 11.5f), default, CreateDefaultMaterial(Color.black));

            for (float x = -5f; x <= 5f; x += 2.5f) {
                AddBox(t, new Vector3(x, 11.75f, -5.75f), new Vector3(1, 1.5f, 0.5f));
                AddBox(t, new Vector3(x, 11.75f, 5.75f), new Vector3(1, 1.5f, 0.5f));
            }
            for (float z = -3.5f; z <= 3.5f; z += 2.5f) {
                AddBox(t, new Vector3(-5.75f, 11.75f, z), new Vector3(0.5f, 1.5f, 1));
                AddBox(t, new Vector3(5.75f, 11.75f, z), new Vector3(0.5f, 1.5f, 1));
            }
        }

        private static void BuildPetraTreasury(GameObject root, Material mat)
        {
            var t = root.transform;
            var darkRock = CreateDefaultMaterial(new Color(0.54f, 0.29f, 0.19f));
            var voidMat = CreateDefaultMaterial(new Color(0.1f, 0.06f, 0.04f));

            AddBox(t, new Vector3(0, 15, -4), new Vector3(26, 30, 8));
            AddBox(t, new Vector3(0, 1, 2), new Vector3(20, 2, 4));

            float[] cols = { -8, -4.5f, -1.5f, 1.5f, 4.5f, 8 };
            foreach (var x in cols) {
                AddCylinder(t, 0.8f, 10f, new Vector3(x, 7, 3));
                AddBox(t, new Vector3(x, 12.5f, 3), new Vector3(1.6f, 1.0f, 1.6f));
            }
            AddBox(t, new Vector3(0, 6, 1), new Vector3(4, 8, 3), default, voidMat);

            AddCylinder(t, 10f, 4f, new Vector3(0, 15, 3), new Vector3(90, 0, 90));
            // flatten into triangle
            var ped = t.GetChild(t.childCount - 1);
            ped.localScale = new Vector3(ped.localScale.x, ped.localScale.y, ped.localScale.z * 0.4f);

            AddBox(t, new Vector3(0, 17.5f, 2), new Vector3(20, 1, 3), default, darkRock);
            AddBox(t, new Vector3(-6.5f, 22, 2), new Vector3(5, 8, 2));
            AddBox(t, new Vector3(6.5f, 22, 2), new Vector3(5, 8, 2));

            float[] upperCols = { -8, -5, 5, 8 };
            foreach (var x in upperCols) AddCylinder(t, 0.6f, 8f, new Vector3(x, 22, 3));

            AddCylinder(t, 3.5f, 8f, new Vector3(0, 22, 2));
            for (int i = 0; i < 6; i++) {
                float a = i * 60f;
                AddCylinder(t, 0.5f, 8f, new Vector3(Mathf.Cos(a * Mathf.Deg2Rad) * 3.8f, 22, 2 + Mathf.Sin(a * Mathf.Deg2Rad) * 3.8f));
            }
            AddCylinder(t, 4f, 3f, new Vector3(0, 27.5f, 2)); // Cone approximation
            var roof = t.GetChild(t.childCount - 1);
            roof.localScale = new Vector3(4f, 1.5f, 4f); // Not a true cone, but close enough with small top radius if we used custom mesh. Using cylinder.
            AddSphere(t, 0.8f, new Vector3(0, 29.5f, 2));
        }

        private static void BuildChichenItza(GameObject root, Material mat)
        {
            var t = root.transform;
            var darkStone = CreateDefaultMaterial(new Color(0.48f, 0.44f, 0.38f));

            int tiers = 9;
            float baseSize = 24f;
            float tierH = 1.4f;

            for (int i = 0; i < tiers; i++) {
                float s = baseSize - i * 2.2f;
                AddBox(t, new Vector3(0, i * tierH + tierH / 2f, 0), new Vector3(s, tierH, s));
                AddBox(t, new Vector3(0, i * tierH + tierH, 0), new Vector3(s + 0.2f, 0.2f, s + 0.2f), default, darkStone);
            }

            var stairMat = CreateDefaultMaterial(new Color(0.55f, 0.52f, 0.45f));
            for (int s = 0; s < 4; s++) {
                float a = s * 90f;
                Vector3 dir = Quaternion.Euler(0, a, 0) * Vector3.forward;
                Vector3 pos = dir * (baseSize / 2f) + new Vector3(0, tiers * tierH * 0.5f, 0);
                AddBox(t, pos, new Vector3(3, tiers * tierH * 1.4f, 6), new Vector3(45, a, 0), stairMat);

                if (s == 0) {
                    AddBox(t, pos + dir * 6.5f + new Vector3(1.5f, -tiers * tierH * 0.5f + 0.75f, 0), new Vector3(1.5f, 1.5f, 2));
                    AddBox(t, pos + dir * 6.5f + new Vector3(-1.5f, -tiers * tierH * 0.5f + 0.75f, 0), new Vector3(1.5f, 1.5f, 2));
                }
            }

            float h = tiers * tierH;
            AddBox(t, new Vector3(0, h + 2, 0), new Vector3(6, 4, 6));
            AddBox(t, new Vector3(0, h + 4.4f, 0), new Vector3(6.5f, 0.8f, 6.5f), default, darkStone);
            AddBox(t, new Vector3(0, h + 1.5f, 0), new Vector3(2, 3, 6.2f), default, CreateDefaultMaterial(Color.black));
        }

        private static void BuildLeaningTower(GameObject root, Material mat)
        {
            var t = root.transform;
            var shadow = CreateDefaultMaterial(new Color(0.1f, 0.1f, 0.1f));

            var pivot = new GameObject("Pivot").transform;
            pivot.SetParent(t, false);
            pivot.localRotation = Quaternion.Euler(0, 0, 3.99f);

            AddCylinder(pivot, 4.5f, 2.5f, new Vector3(0, 1.25f, 0));
            for (int c = 0; c < 15; c++) {
                float a = c * 360f / 15;
                Vector3 dir = Quaternion.Euler(0, a, 0) * Vector3.forward;
                AddCylinder(pivot, 0.2f, 2.5f, dir * 4.6f + new Vector3(0, 1.25f, 0));
            }

            for (int i = 0; i < 6; i++) {
                float y = i * 2.2f + 3.6f;
                AddCylinder(pivot, 3.2f, 2.2f, new Vector3(0, y, 0), default, shadow);
                AddCylinder(pivot, 4.2f, 0.3f, new Vector3(0, y - 0.95f, 0));
                for (int c = 0; c < 30; c++) {
                    float a = c * 360f / 30;
                    Vector3 dir = Quaternion.Euler(0, a, 0) * Vector3.forward;
                    AddCylinder(pivot, 0.12f, 1.9f, dir * 3.9f + new Vector3(0, y, 0));
                }
            }

            float belfryY = 17.5f;
            AddCylinder(pivot, 2.8f, 2.5f, new Vector3(0, belfryY, 0), default, shadow);
            AddCylinder(pivot, 3.4f, 0.3f, new Vector3(0, belfryY - 1.1f, 0));
            for (int c = 0; c < 16; c++) {
                float a = c * 360f / 16;
                Vector3 dir = Quaternion.Euler(0, a, 0) * Vector3.forward;
                AddCylinder(pivot, 0.15f, 2.2f, dir * 3.1f + new Vector3(0, belfryY, 0));
            }
        }

        private static void BuildSydneyOperaHouse(GameObject root, Material mat)
        {
            var t = root.transform;
            var sailMat = mat;
            var glassMat = CreateDefaultMaterial(new Color(0.1f, 0.15f, 0.19f));
            var baseMat = CreateDefaultMaterial(new Color(0.65f, 0.51f, 0.35f));
            var waterMat = CreateDefaultMaterial(new Color(0.18f, 0.38f, 0.52f));

            AddBox(t, new Vector3(0, 0.75f, 0), new Vector3(20, 1.5f, 12), default, baseMat);
            AddBox(t, new Vector3(0, 0.25f, 0), new Vector3(24, 0.5f, 14), default, baseMat);

            System.Action<Vector3, float> AddShell = (offset, scale) => {
                // Approximate shell with flattened spheres
                var s1 = AddSphere(t, 5f * scale, offset + new Vector3(0, 1.5f, 0), new Vector3(0.2f, 1f, 1f), sailMat);
                s1.transform.rotation = Quaternion.Euler(45, 0, 15);
                var s2 = AddSphere(t, 5f * scale, offset + new Vector3(0, 1.5f, 0), new Vector3(0.2f, 1f, 1f), sailMat);
                s2.transform.rotation = Quaternion.Euler(45, 0, -15);
                AddBox(t, offset + new Vector3(0, 1.5f + 2f * scale, 2f * scale), new Vector3(4f * scale, 3f * scale, 0.2f), new Vector3(45, 0, 0), glassMat);
            };

            AddShell(new Vector3(-4, 0, 0), 1.2f);
            AddShell(new Vector3(-2, 0, 1), 1.0f);
            AddShell(new Vector3(0, 0, 2), 0.8f);

            AddShell(new Vector3(4, 0, -2), 1.0f);
            AddShell(new Vector3(5.5f, 0, -1), 0.8f);
            AddShell(new Vector3(7, 0, 0), 0.6f);

            AddBox(t, new Vector3(0, 0.05f, 0), new Vector3(36, 0.1f, 24), default, waterMat);
        }
    }
}
