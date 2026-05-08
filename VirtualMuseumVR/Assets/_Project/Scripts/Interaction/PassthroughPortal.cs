// =============================================================================
// PassthroughPortal.cs — Walk-Through Monument Sections
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================
// For large monuments (e.g., Nahum's Shrine), allows the player to walk
// through cross-sections using stencil buffer masking to reveal interior.
// =============================================================================

using UnityEngine;
using VirtualMuseumVR.Core;

namespace VirtualMuseumVR.Interaction
{
    public class PassthroughPortal : MonoBehaviour
    {
        [Header("Portal Configuration")]
        [SerializeField] private string exhibitId;
        [SerializeField] private ExhibitData exhibitData;

        [Header("Cross-Section Planes")]
        [Tooltip("The plane meshes that act as portals into the monument")]
        [SerializeField] private MeshRenderer[] sectionPlanes;

        [Header("Monument Parts")]
        [Tooltip("External shell of the monument (visible from outside)")]
        [SerializeField] private GameObject exteriorShell;
        [Tooltip("Internal structure revealed when passing through")]
        [SerializeField] private GameObject interiorStructure;

        [Header("Settings")]
        [SerializeField] private float transitionDistance = 0.5f;
        [SerializeField] private float blendSpeed = 3f;
        [SerializeField] private Material portalMaterial;
        [SerializeField] private Material interiorMaterial;

        private Transform _playerHead;
        private bool _isInsideMonument;
        private float _currentBlend;

        private void Start()
        {
            var cam = Camera.main;
            if (cam != null) _playerHead = cam.transform;

            // Initialize: show exterior, hide interior
            if (exteriorShell != null) SetRendererAlpha(exteriorShell, 1f);
            if (interiorStructure != null) interiorStructure.SetActive(false);

            if (string.IsNullOrEmpty(exhibitId) && exhibitData != null)
                exhibitId = exhibitData.exhibitId;
        }

        private void Update()
        {
            if (_playerHead == null) return;

            // Check if player head is inside the monument bounds
            float distance = GetDistanceToCenter();
            bool shouldBeInside = distance < transitionDistance;

            if (shouldBeInside != _isInsideMonument)
            {
                _isInsideMonument = shouldBeInside;
                OnPassthroughStateChanged();
            }

            // Smooth blend between exterior and interior
            float targetBlend = _isInsideMonument ? 1f : 0f;
            _currentBlend = Mathf.Lerp(_currentBlend, targetBlend, Time.deltaTime * blendSpeed);

            UpdateVisuals();
        }

        private float GetDistanceToCenter()
        {
            // Use collider bounds or simple distance
            var col = GetComponent<Collider>();
            if (col != null)
            {
                return Vector3.Distance(_playerHead.position, col.ClosestPoint(_playerHead.position));
            }
            return Vector3.Distance(_playerHead.position, transform.position);
        }

        private void OnPassthroughStateChanged()
        {
            if (_isInsideMonument)
            {
                if (interiorStructure != null)
                    interiorStructure.SetActive(true);

                Debug.Log($"[Portal] Player entered monument interior: {exhibitId}");
            }
            else
            {
                Debug.Log($"[Portal] Player exited monument interior: {exhibitId}");
            }
        }

        private void UpdateVisuals()
        {
            // Blend exterior opacity (fade out as player enters)
            if (exteriorShell != null)
                SetRendererAlpha(exteriorShell, 1f - _currentBlend);

            // Blend interior opacity (fade in as player enters)
            if (interiorStructure != null)
                SetRendererAlpha(interiorStructure, _currentBlend);

            // Deactivate interior when fully blended out
            if (interiorStructure != null && _currentBlend < 0.01f && !_isInsideMonument)
                interiorStructure.SetActive(false);
        }

        private void SetRendererAlpha(GameObject obj, float alpha)
        {
            var renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                foreach (var mat in renderer.materials)
                {
                    if (mat.HasProperty("_Color"))
                    {
                        var color = mat.color;
                        color.a = alpha;
                        mat.color = color;
                    }
                    if (mat.HasProperty("_BaseColor"))
                    {
                        var color = mat.GetColor("_BaseColor");
                        color.a = alpha;
                        mat.SetColor("_BaseColor", color);
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
            Gizmos.DrawSphere(transform.position, transitionDistance);
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.6f);
            Gizmos.DrawWireSphere(transform.position, transitionDistance);
        }
    }
}
