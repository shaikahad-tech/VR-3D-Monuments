// =============================================================================
// ModelImportProcessor.cs — Auto-Configure Imported FBX/OBJ Models
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VirtualMuseumVR.Editor
{
    /// <summary>
    /// AssetPostprocessor that auto-configures imported photogrammetry models
    /// with optimal settings for Meta Quest VR.
    /// </summary>
    public class ModelImportProcessor : AssetPostprocessor
    {
        // Only process models in the _Project folder
        private bool ShouldProcess =>
            assetPath.Contains("_Project") || assetPath.Contains("Models");

        // =====================================================================
        // Model Import
        // =====================================================================

        private void OnPreprocessModel()
        {
            if (!ShouldProcess) return;

            var importer = assetImporter as ModelImporter;
            if (importer == null) return;

            // Enable read/write for runtime mesh access
            importer.isReadable = true;

            // Generate lightmap UVs (essential for baked lighting)
            importer.generateSecondaryUV = true;

            // Optimize mesh for rendering
            importer.optimizeMeshPolygons = true;
            importer.optimizeMeshVertices = true;

            // Import normals (photogrammetry models need their calculated normals)
            importer.importNormals = ModelImporterNormals.Import;

            // Import tangents for normal mapping
            importer.importTangents = ModelImporterTangents.CalculateMikk;

            // Don't import cameras or lights from the FBX
            importer.importCameras = false;
            importer.importLights = false;

            // Set mesh compression for Quest storage
            importer.meshCompression = ModelImporterMeshCompression.Medium;

            // Enable mesh optimization
            importer.weldVertices = true;

            Debug.Log($"[ModelImport] Pre-processing model: {assetPath}");
        }

        // =====================================================================
        // Texture Import
        // =====================================================================

        private void OnPreprocessTexture()
        {
            if (!ShouldProcess) return;

            var importer = assetImporter as TextureImporter;
            if (importer == null) return;

            // Enable mipmaps for LOD
            importer.mipmapEnabled = true;

            // Set streaming mipmaps for memory management
            importer.streamingMipmaps = true;

            // Set compression for Android/Quest (ASTC)
            var androidSettings = importer.GetPlatformTextureSettings("Android");
            androidSettings.overridden = true;
            androidSettings.format = TextureImporterFormat.ASTC_6x6;
            androidSettings.maxTextureSize = 4096;
            androidSettings.compressionQuality = (int)TextureCompressionQuality.Best;
            importer.SetPlatformTextureSettings(androidSettings);

            // Set max size based on texture name hints
            if (assetPath.Contains("_8k") || assetPath.Contains("8192"))
            {
                importer.maxTextureSize = 8192;
                androidSettings.maxTextureSize = 4096; // Downscale for Quest
            }
            else if (assetPath.Contains("_4k") || assetPath.Contains("4096"))
            {
                importer.maxTextureSize = 4096;
                androidSettings.maxTextureSize = 2048;
            }
            else
            {
                importer.maxTextureSize = 2048;
                androidSettings.maxTextureSize = 2048;
            }

            importer.SetPlatformTextureSettings(androidSettings);

            Debug.Log($"[ModelImport] Pre-processing texture: {assetPath} " +
                $"(ASTC, max {androidSettings.maxTextureSize}px)");
        }

        // =====================================================================
        // Post-processing
        // =====================================================================

        private void OnPostprocessModel(GameObject model)
        {
            if (!ShouldProcess) return;

            // Log polygon count for the imported model
            int totalPolygons = 0;
            var meshFilters = model.GetComponentsInChildren<MeshFilter>();
            foreach (var mf in meshFilters)
            {
                if (mf.sharedMesh != null)
                    totalPolygons += mf.sharedMesh.triangles.Length / 3;
            }

            Debug.Log($"[ModelImport] Imported '{model.name}': " +
                $"{totalPolygons:N0} polygons, {meshFilters.Length} mesh(es)");

            // Warn if polygon count is too high for Quest
            if (totalPolygons > 50000)
            {
                Debug.LogWarning($"[ModelImport] '{model.name}' has {totalPolygons:N0} polygons! " +
                    "Consider decimating to ~10,000 for Meta Quest performance.");
            }
        }
    }
}
#endif
