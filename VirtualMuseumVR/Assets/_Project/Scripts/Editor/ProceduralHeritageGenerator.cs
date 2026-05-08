#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace VirtualMuseumVR.Editor
{
    /// <summary>
    /// Editor tool to procedurally generate massive "heritage site" placeholder models
    /// to simulate the performance load and scale of real high-poly photogrammetry scans.
    /// </summary>
    public class ProceduralHeritageGenerator : EditorWindow
    {
        private string siteName = "New_Indian_Heritage_Site";
        private int pillarCount = 50;
        private int complexityLevel = 3;
        private float spreadRadius = 20f;
        private bool generateCollision = true;

        [MenuItem("Tools/Virtual Museum/Generate Heritage Site")]
        public static void ShowWindow()
        {
            GetWindow<ProceduralHeritageGenerator>("Heritage Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Procedural Indian Architecture Generator", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUILayout.HelpBox("This tool generates massive procedurally constructed game objects using primitives to simulate the scale and polygon count of raw photogrammetry data for performance testing.", MessageType.Info);

            siteName = EditorGUILayout.TextField("Site Name", siteName);
            pillarCount = EditorGUILayout.IntSlider("Structure Count", pillarCount, 10, 500);
            complexityLevel = EditorGUILayout.IntSlider("Detail Level (Meshes per structure)", complexityLevel, 1, 10);
            spreadRadius = EditorGUILayout.Slider("Spread Radius", spreadRadius, 5f, 100f);
            generateCollision = EditorGUILayout.Toggle("Generate Mesh Colliders", generateCollision);

            EditorGUILayout.Space();

            if (GUILayout.Button("Generate Site", GUILayout.Height(40)))
            {
                GenerateSite();
            }
        }

        private void GenerateSite()
        {
            GameObject root = new GameObject(siteName);
            
            // Base platform
            GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            platform.transform.parent = root.transform;
            platform.transform.localScale = new Vector3(spreadRadius * 2, 0.5f, spreadRadius * 2);
            platform.transform.localPosition = Vector3.zero;

            int totalPrimitives = 0;

            for (int i = 0; i < pillarCount; i++)
            {
                // Random position within radius
                Vector2 randomCircle = Random.insideUnitCircle * spreadRadius;
                Vector3 position = new Vector3(randomCircle.x, 0, randomCircle.y);

                GameObject pillarRoot = new GameObject($"Structure_{i}");
                pillarRoot.transform.parent = root.transform;
                pillarRoot.transform.localPosition = position;

                // Create a complex pillar out of stacked primitives to bloat the draw calls / poly count slightly
                float currentHeight = 0.5f;
                for (int j = 0; j < complexityLevel; j++)
                {
                    PrimitiveType type = (PrimitiveType)Random.Range(0, 4); // Sphere, Capsule, Cylinder, Cube
                    GameObject piece = GameObject.CreatePrimitive(type);
                    
                    float sizeX = Random.Range(0.5f, 2f);
                    float sizeY = Random.Range(1f, 4f);
                    
                    piece.transform.parent = pillarRoot.transform;
                    piece.transform.localScale = new Vector3(sizeX, sizeY, sizeX);
                    piece.transform.localPosition = new Vector3(0, currentHeight + (sizeY / 2), 0);
                    
                    currentHeight += sizeY;
                    totalPrimitives++;

                    if (!generateCollision)
                    {
                        DestroyImmediate(piece.GetComponent<Collider>());
                    }
                }
            }

            // Apply a default shader if we have one, otherwise just standard
            Material defaultMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            defaultMat.color = new Color(0.8f, 0.7f, 0.6f); // Sandstone color
            
            MeshRenderer[] renderers = root.GetComponentsInChildren<MeshRenderer>();
            foreach (var renderer in renderers)
            {
                renderer.sharedMaterial = defaultMat;
            }

            Debug.Log($"[Generator] Created '{siteName}' with {pillarCount} structures and {totalPrimitives} primitive meshes.");
            
            // Focus scene view on the new object
            Selection.activeGameObject = root;
            SceneView.FrameLastActiveSceneView();
        }
    }
}
#endif
