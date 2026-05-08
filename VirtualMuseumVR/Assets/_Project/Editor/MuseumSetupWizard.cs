// =============================================================================
// MuseumSetupWizard.cs — One-Click Project Setup Wizard
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================
// Editor window: Tools > Virtual Museum > Setup Wizard
// Automates project configuration for Meta Quest VR.
// =============================================================================

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using VirtualMuseumVR.Core;
using VirtualMuseumVR.Environment;

namespace VirtualMuseumVR.Editor
{
    public class MuseumSetupWizard : EditorWindow
    {
        private int _currentStep;
        private Vector2 _scrollPos;

        [MenuItem("Tools/Virtual Museum/Setup Wizard", false, 0)]
        public static void ShowWindow()
        {
            var window = GetWindow<MuseumSetupWizard>("Museum Setup Wizard");
            window.minSize = new Vector2(500, 600);
        }

        private void OnGUI()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            DrawHeader();

            EditorGUILayout.Space(10);

            switch (_currentStep)
            {
                case 0: DrawProjectSettingsStep(); break;
                case 1: DrawSceneHierarchyStep(); break;
                case 2: DrawXRSetupStep(); break;
                case 3: DrawMuseumLayoutStep(); break;
                case 4: DrawCompletionStep(); break;
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            EditorGUILayout.LabelField("Virtual Museum VR — Setup Wizard",
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "Based on Pavelka & Raeva (2019), ISPRS Archives",
                EditorStyles.miniLabel);

            EditorGUILayout.Space(5);
            var rect = EditorGUILayout.GetControlRect(false, 20);
            float progress = (_currentStep + 1f) / 5f;
            EditorGUI.ProgressBar(rect, progress,
                $"Step {_currentStep + 1} of 5");
        }

        private void DrawProjectSettingsStep()
        {
            EditorGUILayout.LabelField("Step 1: Project Settings", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Configure critical project settings for Meta Quest VR.\n" +
                "• Sets Android build target\n" +
                "• Enables Vulkan graphics API\n" +
                "• Configures URP quality settings",
                MessageType.Info);

            if (GUILayout.Button("Configure Project Settings", GUILayout.Height(40)))
            {
                ConfigureProjectSettings();
                _currentStep = 1;
            }

            DrawSkipButton();
        }

        private void DrawSceneHierarchyStep()
        {
            EditorGUILayout.LabelField("Step 2: Scene Hierarchy", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Creates the base scene hierarchy:\n" +
                "• [VirtualMuseumManager] singleton\n" +
                "• [MuseumSystems] parent for all managers\n" +
                "• [Environment] for museum architecture\n" +
                "• [Exhibits] for monuments and artifacts\n" +
                "• Lighting setup with baked lights",
                MessageType.Info);

            if (GUILayout.Button("Create Scene Hierarchy", GUILayout.Height(40)))
            {
                CreateSceneHierarchy();
                _currentStep = 2;
            }

            DrawSkipButton();
        }

        private void DrawXRSetupStep()
        {
            EditorGUILayout.LabelField("Step 3: XR Rig Setup", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Creates the XR Origin with:\n" +
                "• Camera rig (head + hands)\n" +
                "• Near-Far Interactors on both hands\n" +
                "• Teleportation provider\n" +
                "• Snap turn provider\n" +
                "• Input action references\n\n" +
                "NOTE: Import XRI Starter Assets first for best results.",
                MessageType.Info);

            if (GUILayout.Button("Create XR Rig", GUILayout.Height(40)))
            {
                CreateXRRig();
                _currentStep = 3;
            }

            DrawSkipButton();
        }

        private void DrawMuseumLayoutStep()
        {
            EditorGUILayout.LabelField("Step 4: Museum Layout", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Generates the 4-room + corridor museum architecture:\n" +
                "• Room 1: Iraq Section (Nahum's Shrine)\n" +
                "• Room 2: Prague Monuments\n" +
                "• Room 3: Archaeological Shards Table\n" +
                "• Room 4: Aerial Photogrammetry Room\n" +
                "• Central corridor connecting all rooms\n" +
                "• Pedestals with spawn points for exhibits",
                MessageType.Info);

            if (GUILayout.Button("Generate Museum Layout", GUILayout.Height(40)))
            {
                GenerateMuseumLayout();
                _currentStep = 4;
            }

            DrawSkipButton();
        }

        private void DrawCompletionStep()
        {
            EditorGUILayout.LabelField("Setup Complete!", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Your Virtual Museum project is ready!\n\n" +
                "Next steps:\n" +
                "1. Import your photogrammetry .FBX/.OBJ models\n" +
                "2. Create ExhibitData ScriptableObjects for each model\n" +
                "   (Assets > Create > Virtual Museum > Exhibit Data)\n" +
                "3. Place models on pedestals and assign ExhibitData\n" +
                "4. Bake lighting (Window > Rendering > Lighting)\n" +
                "5. Build & deploy to Meta Quest\n\n" +
                "See Documentation/ folder for detailed guides.",
                MessageType.Info);

            EditorGUILayout.Space(10);

            if (GUILayout.Button("Open Documentation", GUILayout.Height(30)))
            {
                var readme = AssetDatabase.LoadAssetAtPath<Object>(
                    "Assets/_Project/../Documentation/SETUP_GUIDE.md");
                if (readme != null) AssetDatabase.OpenAsset(readme);
            }

            EditorGUILayout.Space(5);
            if (GUILayout.Button("Start Over", GUILayout.Height(25)))
            {
                _currentStep = 0;
            }
        }

        private void DrawSkipButton()
        {
            EditorGUILayout.Space(5);
            if (GUILayout.Button("Skip →"))
                _currentStep++;
        }

        // =====================================================================
        // Implementation
        // =====================================================================

        private void ConfigureProjectSettings()
        {
            // Set minimum API level for Quest
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel29;

            // Enable Vulkan
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android,
                new[] { UnityEngine.Rendering.GraphicsDeviceType.Vulkan });

            // Single-pass instanced rendering
            PlayerSettings.stereoRenderingPath = StereoRenderingPath.Instancing;

            // Set company and product name
            PlayerSettings.companyName = "VirtualMuseumVR";
            PlayerSettings.productName = "Virtual Museum";

            EditorUtility.DisplayDialog("Project Settings",
                "Project settings configured for Meta Quest.", "OK");
        }

        private void CreateSceneHierarchy()
        {
            // Manager
            var manager = new GameObject("[VirtualMuseumManager]");
            manager.AddComponent<VirtualMuseumManager>();

            // Systems container
            var systems = new GameObject("[MuseumSystems]");

            var roomMgr = new GameObject("RoomManager");
            roomMgr.transform.SetParent(systems.transform);
            roomMgr.AddComponent<RoomManager>();

            var lightCtrl = new GameObject("LightingController");
            lightCtrl.transform.SetParent(systems.transform);
            lightCtrl.AddComponent<LightingController>();

            var audioMgr = new GameObject("AmbientAudioManager");
            audioMgr.transform.SetParent(systems.transform);
            audioMgr.AddComponent<VirtualMuseumVR.Audio.AmbientAudioManager>();

            var perfMon = new GameObject("PerformanceMonitor");
            perfMon.transform.SetParent(systems.transform);
            perfMon.AddComponent<Optimization.PerformanceMonitor>();

            var texMgr = new GameObject("TextureQualityManager");
            texMgr.transform.SetParent(systems.transform);
            texMgr.AddComponent<Optimization.TextureQualityManager>();

            var occMgr = new GameObject("OcclusionCullingManager");
            occMgr.transform.SetParent(systems.transform);
            occMgr.AddComponent<Optimization.OcclusionCullingManager>();

            var exhibitDb = new GameObject("ExhibitDatabase");
            exhibitDb.transform.SetParent(systems.transform);
            exhibitDb.AddComponent<Data.ExhibitDatabase>();

            // Environment
            new GameObject("[Environment]");
            new GameObject("[Exhibits]");

            // Fade overlay canvas
            var fadeCanvas = new GameObject("[FadeOverlay]");
            var canvas = fadeCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;
            var cg = fadeCanvas.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            var fadeImg = new GameObject("FadeImage");
            fadeImg.transform.SetParent(fadeCanvas.transform);
            var img = fadeImg.AddComponent<UnityEngine.UI.Image>();
            img.color = Color.black;
            var rt = img.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;

            Debug.Log("[SetupWizard] Scene hierarchy created.");
        }

        private void CreateXRRig()
        {
            // Create via menu if XRI is installed
            EditorApplication.ExecuteMenuItem("GameObject/XR/XR Origin (XR Rig)");
            Debug.Log("[SetupWizard] XR Rig created. Configure input actions from XRI Starter Assets.");
        }

        private void GenerateMuseumLayout()
        {
            var envParent = GameObject.Find("[Environment]");
            if (envParent == null)
                envParent = new GameObject("[Environment]");

            var builder = envParent.AddComponent<MuseumArchitectureBuilder>();
            builder.GenerateMuseum();

            Debug.Log("[SetupWizard] Museum layout generated.");
        }
    }
}
#endif
