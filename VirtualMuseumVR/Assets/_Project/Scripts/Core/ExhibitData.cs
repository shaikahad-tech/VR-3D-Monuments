// =============================================================================
// ExhibitData.cs — ScriptableObject for Exhibit Metadata
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================
// Each exhibit in the museum has one of these ScriptableObjects containing
// all metadata: title, description, era, acquisition method, polygon counts,
// narration audio, thumbnail, etc.
//
// Create via: Assets > Create > Virtual Museum > Exhibit Data
// =============================================================================

using UnityEngine;

namespace VirtualMuseumVR.Core
{
    /// <summary>
    /// Defines how the 3D model was originally captured,
    /// matching the acquisition methods described in the paper.
    /// </summary>
    public enum AcquisitionMethod
    {
        /// <summary>Drone/UAV photogrammetry (e.g., DJI Mavic, eBee)</summary>
        Aerial,

        /// <summary>Ground-based DSLR photogrammetry</summary>
        Ground,

        /// <summary>Combined aerial and ground photogrammetry</summary>
        Combined,

        /// <summary>Terrestrial laser scanning (TLS)</summary>
        LaserScan,

        /// <summary>Combination of photogrammetry and laser scanning</summary>
        PhotogrammetryAndLaserScan,

        /// <summary>Structured light scanning (for small artifacts)</summary>
        StructuredLight
    }

    /// <summary>
    /// Which room this exhibit belongs to in the 5-room museum layout.
    /// </summary>
    public enum MuseumRoom
    {
        Corridor,
        IraqSection,
        PragueMonuments,
        ArchaeologicalShards,
        AerialPhotogrammetry,
        IndianArchitecture
    }

    /// <summary>
    /// ScriptableObject containing all metadata for a single museum exhibit.
    /// This data drives the info panel, narration, and exhibit behavior.
    /// </summary>
    [CreateAssetMenu(
        fileName = "NewExhibitData",
        menuName = "Virtual Museum/Exhibit Data",
        order = 0)]
    public class ExhibitData : ScriptableObject
    {
        [Header("Puzzle Configuration")]
        [Tooltip("If true, this exhibit starts broken and requires restoration.")]
        public bool isRestorationPuzzle = false;
        [Tooltip("Number of pieces the monument is broken into.")]
        public int puzzlePieceCount = 0;
        
        // =================================================================
        // Identification
        // =================================================================

        [Header("Identification")]
        [Tooltip("Unique identifier for this exhibit (auto-generated if empty)")]
        public string exhibitId;

        [Tooltip("Display name shown on the info panel and pedestal")]
        public string title = "Unnamed Exhibit";

        [Tooltip("Which room this exhibit is displayed in")]
        public MuseumRoom room = MuseumRoom.Corridor;

        // =================================================================
        // Description & History
        // =================================================================

        [Header("Description & History")]
        [TextArea(3, 10)]
        [Tooltip("Detailed description shown on the info panel")]
        public string description = "";

        [Tooltip("Historical era or period (e.g., '7th century BCE', 'Medieval')")]
        public string era = "";

        [Tooltip("Geographic location of the original monument")]
        public string location = "";

        [Tooltip("Country of origin")]
        public string country = "";

        [Tooltip("Name of the photographer/scanner operator")]
        public string photographer = "";

        [Tooltip("Year the model was captured")]
        public int captureYear = 2019;

        // =================================================================
        // Technical Specifications (from the paper's methodology)
        // =================================================================

        [Header("Technical Specifications")]
        [Tooltip("How the 3D model was originally captured")]
        public AcquisitionMethod acquisitionMethod = AcquisitionMethod.Ground;

        [Tooltip("Number of source photographs used for reconstruction")]
        public int sourcePhotoCount = 0;

        [Tooltip("Original polygon count before decimation")]
        public int originalPolygonCount = 100000;

        [Tooltip("Optimized polygon count after decimation for VR")]
        public int optimizedPolygonCount = 10000;

        [Tooltip("Number of texture parts this model is split into (paper recommends splitting large models)")]
        public int texturePartCount = 1;

        [Tooltip("Maximum texture resolution per part (e.g., 4096 for 4K, 8192 for 8K)")]
        public int maxTextureResolution = 4096;

        [Tooltip("Software used for 3D reconstruction")]
        public string reconstructionSoftware = "Agisoft Metashape";

        // =================================================================
        // Presentation
        // =================================================================

        [Header("Presentation")]
        [Tooltip("Thumbnail image shown in the info panel and HUD")]
        public Sprite thumbnail;

        [Tooltip("Audio narration clip for this exhibit")]
        public AudioClip narrationClip;

        [Tooltip("Scale multiplier when displayed on the pedestal (1.0 = real-world scale)")]
        [Range(0.01f, 10.0f)]
        public float pedestalDisplayScale = 0.1f;

        [Tooltip("Scale multiplier when grabbed for inspection")]
        [Range(0.01f, 5.0f)]
        public float inspectionScale = 0.3f;

        [Tooltip("Rotation speed multiplier when grabbed")]
        [Range(10f, 200f)]
        public float rotationSpeed = 50f;

        [Tooltip("Should the model slowly rotate on its pedestal when not grabbed?")]
        public bool autoRotateOnPedestal = true;

        [Tooltip("Auto-rotation speed in degrees per second")]
        [Range(1f, 30f)]
        public float autoRotationSpeed = 5f;

        [Tooltip("Can this exhibit be grabbed and inspected?")]
        public bool isGrabbable = true;

        [Tooltip("Can the player walk through this exhibit (for large monuments)?")]
        public bool isWalkThrough = false;

        // =================================================================
        // Lighting
        // =================================================================

        [Header("Exhibit Lighting")]
        [Tooltip("Color of the accent spotlight for this exhibit")]
        public Color spotlightColor = new Color(1f, 0.95f, 0.8f, 1f); // warm white

        [Tooltip("Intensity of the accent spotlight")]
        [Range(0.5f, 5.0f)]
        public float spotlightIntensity = 2.0f;

        [Tooltip("Color of the pedestal emission ring")]
        public Color pedestalGlowColor = new Color(0.3f, 0.6f, 1.0f, 1f); // blue glow

        // =================================================================
        // Runtime State (not serialized)
        // =================================================================

        /// <summary>Returns the decimation ratio as a percentage.</summary>
        public float DecimationRatio =>
            originalPolygonCount > 0
                ? (1f - (float)optimizedPolygonCount / originalPolygonCount) * 100f
                : 0f;

        /// <summary>Returns a formatted string describing the polygon optimization.</summary>
        public string PolygonSummary =>
            $"{originalPolygonCount:N0} → {optimizedPolygonCount:N0} ({DecimationRatio:F1}% reduction)";

        /// <summary>Returns the acquisition method as a human-readable string.</summary>
        public string AcquisitionMethodDisplay
        {
            get
            {
                return acquisitionMethod switch
                {
                    AcquisitionMethod.Aerial => "Aerial (Drone/UAV)",
                    AcquisitionMethod.Ground => "Ground-based DSLR",
                    AcquisitionMethod.Combined => "Combined Aerial + Ground",
                    AcquisitionMethod.LaserScan => "Terrestrial Laser Scanning",
                    AcquisitionMethod.PhotogrammetryAndLaserScan => "Photogrammetry + Laser Scan",
                    AcquisitionMethod.StructuredLight => "Structured Light Scanning",
                    _ => "Unknown"
                };
            }
        }

        // =================================================================
        // Validation
        // =================================================================

        private void OnValidate()
        {
            // Auto-generate exhibitId if empty
            if (string.IsNullOrEmpty(exhibitId))
            {
                exhibitId = name.Replace(" ", "_").ToLower();
            }

            // Ensure optimized count doesn't exceed original
            if (optimizedPolygonCount > originalPolygonCount && originalPolygonCount > 0)
            {
                optimizedPolygonCount = originalPolygonCount;
            }
        }
    }
}
