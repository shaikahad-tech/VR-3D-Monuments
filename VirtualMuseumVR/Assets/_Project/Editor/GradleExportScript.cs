// =============================================================================
// GradleExportScript.cs — Export Unity project as Gradle project
// Virtual Museum VR Project
// =============================================================================
// Phase 1 of the two-phase build:
//   1. Unity CLI exports a Gradle project to Builds/GradleExport/
//   2. Gradle then builds the APK from that project
//
// Run via terminal:
//   Unity -batchmode -executeMethod GradleExportScript.Export -quit
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
    public static class GradleExportScript
    {
        private const string EXPORT_DIR = "Builds/GradleExport";

        /// <summary>
        /// Export a Gradle project that Gradle can then build independently.
        /// Called via: -executeMethod GradleExportScript.Export
        /// </summary>
        public static void Export()
        {
            Console.WriteLine("══════════════════════════════════════════════");
            Console.WriteLine("  Virtual Museum VR — Gradle Export (Phase 1)");
            Console.WriteLine("══════════════════════════════════════════════");

            // Get export path from args or use default
            string exportPath = GetArgValue("-exportPath") ?? EXPORT_DIR;
            Directory.CreateDirectory(exportPath);

            // Configure Android settings
            PlayerSettings.applicationIdentifier = "com.virtualmuseum.vr";
            PlayerSettings.bundleVersion = GetArgValue("-version") ?? "1.0.0";
            int versionCode = int.TryParse(GetArgValue("-versionCode"), out int vc) ? vc : 1;
            PlayerSettings.Android.bundleVersionCode = versionCode;

            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel29;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel32;
            PlayerSettings.SetScriptingBackend(
                BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.stereoRenderingPath = StereoRenderingPath.Instancing;
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android,
                new[] { UnityEngine.Rendering.GraphicsDeviceType.Vulkan });

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

            Console.WriteLine($"[Export] Scenes: {scenes.Length}");
            Console.WriteLine($"[Export] Output: {exportPath}");

            var options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = exportPath,
                target = BuildTarget.Android,
                // KEY: Export as Gradle project instead of building APK directly
                options = BuildOptions.AcceptExternalModificationsToPlayer
            };

            var report = BuildPipeline.BuildPlayer(options);
            var summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Console.WriteLine("[Export] ✅ Gradle project exported successfully.");
                Console.WriteLine($"[Export] Location: {Path.GetFullPath(exportPath)}");
                Console.WriteLine("[Export] Next step: run 'gradle build' or 'make apk'");
                EditorApplication.Exit(0);
            }
            else
            {
                Console.WriteLine($"[Export] ❌ Export FAILED: {summary.result}");
                EditorApplication.Exit(1);
            }
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
