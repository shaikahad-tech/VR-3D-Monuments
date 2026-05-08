// =============================================================================
// MonumentSpawner.cs — Runtime Pedestal Population
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================
// Drop this on a Pedestal (or anywhere) and it will instantiate a procedurally
// generated monument from WorldMonumentLibrary at runtime, parent it to the
// pedestal, and wire up the standard MonumentGrabInteractable + auto-rotation.
// This makes every exhibit functional out-of-the-box without requiring imported
// .FBX/.OBJ photogrammetry assets.
// =============================================================================

using UnityEngine;
using VirtualMuseumVR.Core;
using VirtualMuseumVR.Interaction;

namespace VirtualMuseumVR.Environment
{
    [DisallowMultipleComponent]
    public class MonumentSpawner : MonoBehaviour
    {
        [Header("Monument Source")]
        [Tooltip("Which monument from the world library to instantiate")]
        public WorldMonument monument = WorldMonument.TajMahal;

        [Tooltip("Optional shared material override (otherwise picks the catalog stoneColor)")]
        public Material materialOverride;

        [Header("Placement")]
        [Tooltip("Local Y offset above the pedestal center")]
        public float verticalOffset = 0.6f;

        [Tooltip("Spawn rotation (Y degrees)")]
        public float yawDegrees = 0f;

        [Header("Behaviour")]
        [Tooltip("Slow auto-rotate on the pedestal when not grabbed")]
        public bool autoRotate = true;
        [Range(1f, 30f)] public float autoRotateSpeed = 6f;

        [Tooltip("Wire the spawned monument up for VR grab + rotate inspection")]
        public bool addGrabInteractable = true;

        [Tooltip("Optional ExhibitData to drive the info panel and HUD")]
        public ExhibitData exhibitData;

        private GameObject _spawned;
        private float _yaw;

        private void Start()
        {
            if (_spawned == null) Spawn();
        }

        public void Spawn()
        {
            if (_spawned != null) DestroyImmediate(_spawned);

            _spawned = WorldMonumentLibrary.Build(monument, materialOverride, transform);
            _spawned.transform.localPosition = new Vector3(0f, verticalOffset, 0f);
            _spawned.transform.localRotation = Quaternion.Euler(0, yawDegrees, 0);

            if (addGrabInteractable)
            {
                var rb = _spawned.GetComponent<Rigidbody>();
                if (rb == null) rb = _spawned.AddComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.useGravity = false;

                // Add a single bounding-box collider that wraps the entire model
                var bounds = ComputeRendererBounds(_spawned);
                var col = _spawned.GetComponent<BoxCollider>();
                if (col == null) col = _spawned.AddComponent<BoxCollider>();
                col.center = _spawned.transform.InverseTransformPoint(bounds.center);
                col.size = bounds.size;

                if (_spawned.GetComponent<MonumentGrabInteractable>() == null)
                    _spawned.AddComponent<MonumentGrabInteractable>();
            }
        }

        private Bounds ComputeRendererBounds(GameObject root)
        {
            var rs = root.GetComponentsInChildren<Renderer>();
            if (rs.Length == 0) return new Bounds(root.transform.position, Vector3.one);
            var b = rs[0].bounds;
            for (int i = 1; i < rs.Length; i++) b.Encapsulate(rs[i].bounds);
            return b;
        }

        private void Update()
        {
            if (autoRotate && _spawned != null)
            {
                _yaw += autoRotateSpeed * Time.deltaTime;
                _spawned.transform.localRotation = Quaternion.Euler(0, yawDegrees + _yaw, 0);
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Respawn Monument")]
        private void EditorRespawn() => Spawn();
#endif
    }
}
