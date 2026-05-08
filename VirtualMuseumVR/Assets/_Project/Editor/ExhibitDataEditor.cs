// =============================================================================
// ExhibitDataEditor.cs — Custom Inspector for ExhibitData ScriptableObjects
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VirtualMuseumVR.Core;

namespace VirtualMuseumVR.Editor
{
    [CustomEditor(typeof(ExhibitData))]
    public class ExhibitDataEditor : UnityEditor.Editor
    {
        private bool _showTechnical = true;
        private bool _showPresentation = true;
        private bool _showLighting = true;

        public override void OnInspectorGUI()
        {
            var data = (ExhibitData)target;

            // Header
            EditorGUILayout.LabelField("═══ Exhibit Data ═══", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            // Preview box
            DrawPreviewBox(data);
            EditorGUILayout.Space(10);

            // Draw default inspector with foldouts
            serializedObject.Update();

            // Identification
            EditorGUILayout.LabelField("Identification", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("exhibitId"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("title"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("room"));

            EditorGUILayout.Space(5);

            // Description
            EditorGUILayout.LabelField("Description & History", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("description"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("era"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("location"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("country"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("photographer"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("captureYear"));

            // Quick presets
            EditorGUILayout.Space(3);
            EditorGUILayout.LabelField("Quick Presets:", EditorStyles.miniLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Iraq", EditorStyles.miniButton))
            {
                data.country = "Iraq";
                data.room = MuseumRoom.IraqSection;
            }
            if (GUILayout.Button("Czech Republic", EditorStyles.miniButton))
            {
                data.country = "Czech Republic";
                data.room = MuseumRoom.PragueMonuments;
            }
            if (GUILayout.Button("India", EditorStyles.miniButton))
            {
                data.country = "India";
                data.room = MuseumRoom.IndianArchitecture;
            }
            if (GUILayout.Button("UK", EditorStyles.miniButton))
            {
                data.country = "United Kingdom";
                data.room = MuseumRoom.AerialPhotogrammetry;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // Technical specifications
            _showTechnical = EditorGUILayout.Foldout(_showTechnical,
                "Technical Specifications", true, EditorStyles.foldoutHeader);
            if (_showTechnical)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("acquisitionMethod"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("sourcePhotoCount"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("originalPolygonCount"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("optimizedPolygonCount"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("texturePartCount"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxTextureResolution"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("reconstructionSoftware"));

                // Show decimation summary
                EditorGUILayout.Space(3);
                EditorGUILayout.HelpBox(
                    $"Polygon Optimization: {data.PolygonSummary}\n" +
                    $"Acquisition: {data.AcquisitionMethodDisplay}",
                    MessageType.None);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // Presentation
            _showPresentation = EditorGUILayout.Foldout(_showPresentation,
                "Presentation", true, EditorStyles.foldoutHeader);
            if (_showPresentation)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("thumbnail"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("narrationClip"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("pedestalDisplayScale"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("inspectionScale"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("rotationSpeed"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("autoRotateOnPedestal"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("autoRotationSpeed"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("isGrabbable"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("isWalkThrough"));
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(5);

            // Lighting
            _showLighting = EditorGUILayout.Foldout(_showLighting,
                "Exhibit Lighting", true, EditorStyles.foldoutHeader);
            if (_showLighting)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("spotlightColor"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("spotlightIntensity"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("pedestalGlowColor"));
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawPreviewBox(ExhibitData data)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();

            // Thumbnail preview
            if (data.thumbnail != null)
            {
                var rect = GUILayoutUtility.GetRect(64, 64, GUILayout.Width(64));
                EditorGUI.DrawPreviewTexture(rect, data.thumbnail.texture);
                GUILayout.Space(10);
            }

            // Info summary
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(
                string.IsNullOrEmpty(data.title) ? "(Untitled)" : data.title,
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"{data.era} | {data.location}, {data.country}",
                EditorStyles.miniLabel);
            EditorGUILayout.LabelField($"{data.AcquisitionMethodDisplay}",
                EditorStyles.miniLabel);
            EditorGUILayout.LabelField($"Polygons: {data.PolygonSummary}",
                EditorStyles.miniLabel);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
    }
}
#endif
