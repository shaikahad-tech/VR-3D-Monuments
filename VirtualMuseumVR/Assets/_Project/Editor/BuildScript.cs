// =============================================================================
// BuildScript.cs — Unity CLI Build Automation
// Virtual Museum VR Project
// =============================================================================
// Triggered via terminal with:
//   Unity -batchmode -executeMethod BuildScript.BuildAndroid -quit
// =============================================================================

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

namespace VirtualMuseumVR.Editor
{
    public static class BuildScript
    {
        // Output paths
        private const string BUILD_DIR = "Builds";
        private const string APK_NAME = "VirtualMuseumVR.apk";

        // Scenes to include in the build
        private static string[] GetScenes()
        {
            // Auto-collect all scenes from EditorBuildSettings
            var scenes = EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => s.path)
                .ToArray();

            if (scenes.Length == 0)
            {
                // Fallback: find any scene in the project
                scenes = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" })
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .ToArray();
            }

            return scenes;
        }

        // =====================================================================
        // Android / Meta Quest Build
        // =====================================================================

        /// <summary>
        /// Build APK for Meta Quest. Call via:
        ///   -executeMethod BuildScript.BuildAndroid
        /// </summary>
        public static void BuildAndroid()
        {
            Console.WriteLine("══════════════════════════════════════");
            Console.WriteLine("  Virtual Museum VR — Android Build");
            Console.WriteLine("══════════════════════════════════════");

            // Parse optional output path from command line args
            string outputPath = GetArgValue("-outputPath") ??
                Path.Combine(BUILD_DIR, "Android", APK_NAME);

            // Ensure output directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

            // Apply Quest-specific player settings
            ConfigureAndroidSettings();

            var buildOptions = new BuildPlayerOptions
            {
                scenes = GetScenes(),
                locationPathName = outputPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            Console.WriteLine($"[Build] Scenes: {buildOptions.scenes.Length}");
            Console.WriteLine($"[Build] Output: {outputPath}");
            Console.WriteLine($"[Build] Starting build...");

            var report = BuildPipeline.BuildPlayer(buildOptions);
            var summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Console.WriteLine($"[Build] ✅ SUCCESS");
                Console.WriteLine($"[Build] Size: {summary.totalSize / (1024 * 1024)} MB");
                Console.WriteLine($"[Build] Time: {summary.totalTime.TotalSeconds:F1}s");
                Console.WriteLine($"[Build] Output: {outputPath}");
                EditorApplication.Exit(0);
            }
            else
            {
                Console.WriteLine($"[Build] ❌ FAILED — {summary.result}");
                Console.WriteLine($"[Build] Errors: {summary.totalErrors}");
                EditorApplication.Exit(1);
            }
        }

        // =====================================================================
        // Windows / Mac Standalone Build (for Editor testing)
        // =====================================================================

        /// <summary>
        /// Build standalone desktop executable for testing without a headset.
        /// Call via: -executeMethod BuildScript.BuildStandalone
        /// </summary>
        public static void BuildStandalone()
        {
#if UNITY_EDITOR_OSX
            BuildTarget target = BuildTarget.StandaloneOSX;
            string ext = ".app";
#else
            BuildTarget target = BuildTarget.StandaloneWindows64;
            string ext = ".exe";
#endif
            string outputPath = GetArgValue("-outputPath") ??
                Path.Combine(BUILD_DIR, "Standalone", $"VirtualMuseumVR{ext}");

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

            var buildOptions = new BuildPlayerOptions
            {
                scenes = GetScenes(),
                locationPathName = outputPath,
                target = target,
                options = BuildOptions.None
            };

            Console.WriteLine($"[Build] Building Standalone → {outputPath}");
            var report = BuildPipeline.BuildPlayer(buildOptions);

            EditorApplication.Exit(
                report.summary.result == BuildResult.Succeeded ? 0 : 1);
        }

        // =====================================================================
        // Development Build (with profiler, debug symbols)
        // =====================================================================

        /// <summary>
        /// Development/debug build with profiler attached.
        /// Call via: -executeMethod BuildScript.BuildDevelopment
        /// </summary>
        public static void BuildDevelopment()
        {
            string outputPath = GetArgValue("-outputPath") ??
                Path.Combine(BUILD_DIR, "Development", "VirtualMuseumVR_Dev.apk");

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

            ConfigureAndroidSettings();

            var buildOptions = new BuildPlayerOptions
            {
                scenes = GetScenes(),
                locationPathName = outputPath,
                target = BuildTarget.Android,
                options = BuildOptions.Development |
                          BuildOptions.ConnectWithProfiler |
                          BuildOptions.AllowDebugging
            };

            Console.WriteLine("[Build] Building DEVELOPMENT APK...");
            var report = BuildPipeline.BuildPlayer(buildOptions);

            EditorApplication.Exit(
                report.summary.result == BuildResult.Succeeded ? 0 : 1);
        }

        // =====================================================================
        // Player Settings Configuration
        // =====================================================================

        private static void ConfigureAndroidSettings()
        {
            // Package name (must match what's registered in Meta Developer Dashboard)
            PlayerSettings.applicationIdentifier = "com.virtualmuseum.vr";
            PlayerSettings.bundleVersion = "1.0.0";
            PlayerSettings.Android.bundleVersionCode = 1;

            // Quest minimum API level
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel29;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel32;

            // ARM64 required for Quest
            PlayerSettings.SetScriptingBackend(
                BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

            // Single-pass instanced stereo rendering
            PlayerSettings.stereoRenderingPath = StereoRenderingPath.Instancing;

            // Vulkan graphics API
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android,
                new[] { UnityEngine.Rendering.GraphicsDeviceType.Vulkan });

            Console.WriteLine("[Build] Android settings configured for Meta Quest.");
        }

        // =====================================================================
        // Utility
        // =====================================================================

        /// <summary>Read a named argument from the Unity command line.</summary>
        private static string GetArgValue(string argName)
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == argName)
                    return args[i + 1];
            }
            return null;
        }
    }
}
#endif
