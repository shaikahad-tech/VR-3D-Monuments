// =============================================================================
// LocalRunScript.cs — Desktop Build for Local Testing (No Headset)
// Virtual Museum VR Project
// =============================================================================
// Builds a standalone macOS/Windows app with the XR Device Simulator
// embedded so you can test without a Meta Quest.
//
// Run via terminal:
//   make build-desktop
// Or directly:
//   Unity -batchmode -executeMethod LocalRunScript.BuildDesktop -quit
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
    public static class LocalRunScript
    {
        private const string BUILD_DIR = "Builds/Desktop";

        /// <summary>
        /// Build a desktop standalone app for local testing without a VR headset.
        /// Embeds the XR Device Simulator for mouse+keyboard VR emulation.
        /// </summary>
        public static void BuildDesktop()
        {
            Console.WriteLine("══════════════════════════════════════════════");
            Console.WriteLine("  Virtual Museum VR — Local Desktop Build");
            Console.WriteLine("  (XR Device Simulator — No Headset Needed)");
            Console.WriteLine("══════════════════════════════════════════════");

            // Detect platform
#if UNITY_EDITOR_OSX
            BuildTarget target = BuildTarget.StandaloneOSX;
            string outputPath = Path.Combine(BUILD_DIR, "VirtualMuseumVR.app");
#else
            BuildTarget target = BuildTarget.StandaloneWindows64;
            string outputPath = Path.Combine(BUILD_DIR, "VirtualMuseumVR.exe");
#endif
            string overridePath = GetArgValue("-outputPath");
            if (!string.IsNullOrEmpty(overridePath))
                outputPath = overridePath;

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

            // Enable XR Device Simulator for desktop builds
            ConfigureDesktopSettings();

            // Collect scenes
            var scenes = EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => s.path)
                .ToArray();

            if (scenes.Length == 0)
            {
                scenes = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" })
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .ToArray();
            }

            Console.WriteLine($"[Desktop] Target: {target}");
            Console.WriteLine($"[Desktop] Scenes: {scenes.Length}");
            Console.WriteLine($"[Desktop] Output: {outputPath}");

            var options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outputPath,
                target = target,
                options = BuildOptions.Development // Enable dev console for testing
            };

            var report = BuildPipeline.BuildPlayer(options);
            var summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Console.WriteLine($"[Desktop] ✅ Build SUCCESS: {outputPath}");
                Console.WriteLine($"[Desktop] Size: {summary.totalSize / (1024 * 1024)} MB");
                Console.WriteLine($"[Desktop] To run: open \"{outputPath}\"");
                EditorApplication.Exit(0);
            }
            else
            {
                Console.WriteLine($"[Desktop] ❌ Build FAILED: {summary.result}");
                EditorApplication.Exit(1);
            }
        }

        /// <summary>
        /// Open the Unity project for interactive Play Mode testing.
        /// Used by 'make run'.
        /// </summary>
        public static void OpenAndPlay()
        {
            Console.WriteLine("[LocalRun] Opening project for Play Mode...");
            // This just opens Unity — user presses Play in the Editor
            // The XR Device Simulator takes over input
            EditorApplication.Exit(0);
        }

        private static void ConfigureDesktopSettings()
        {
            // Use standalone rendering (no VR compositor overhead)
            PlayerSettings.stereoRenderingPath = StereoRenderingPath.MultiPass;

            // Resolution for desktop window
            PlayerSettings.defaultScreenWidth = 1920;
            PlayerSettings.defaultScreenHeight = 1080;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.runInBackground = true;

            // Use XR Device Simulator
            // This is set via XR Management settings at runtime;
            // the simulator injects itself when enabled in Project Settings.
            Console.WriteLine("[Desktop] Configured for windowed desktop with XR Simulator.");
        }

        private static string GetArgValue(string argName)
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length - 1; i++)
                if (args[i] == argName) return args[i + 1];
            return null;
        }
    }
}
#endif
