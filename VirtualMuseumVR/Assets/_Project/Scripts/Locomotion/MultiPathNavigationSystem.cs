// =============================================================================
// MultiPathNavigationSystem.cs — Branching Path Graph for the Museum
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================
// Replaces the linear "every room connects to a single corridor hub" topology
// with a fully connected graph. Each node is an exhibit room, a corridor
// segment, or a "secret" passage. Each edge is a one-way or two-way path with
// metadata: lighting mood, narration cue, allowed locomotion type, and
// optional unlock condition.
//
// This drives:
//   - The path-selection prompt the player sees at each room exit (UI)
//   - The auto-routing used by GuidedTourController
//   - The teleportation waypoint list shown in MuseumTeleportManager
// =============================================================================

using System;
using System.Collections.Generic;
using UnityEngine;
using VirtualMuseumVR.Core;

namespace VirtualMuseumVR.Locomotion
{
    public enum PathTheme
    {
        Default,
        Ancient,    // dim, torch-lit, distant chants
        Modern,     // bright, glass, ambient electronic
        Mystical,   // particles, cool lighting, Sanskrit chant
        Industrial, // metallic, footsteps echo
        Reflective  // water sounds, gentle reverb
    }

    public enum LocomotionMode
    {
        Walking,    // continuous controller-stick movement
        Teleport,   // arc teleport
        Glide,      // long-distance auto-glide for cinematic transitions
        Floating    // free-fly (used for aerial photogrammetry room)
    }

    [Serializable]
    public class PathNode
    {
        public string nodeId;
        public string displayName;
        public MuseumRoom room;
        public Transform anchor;            // where the player lands when entering
        public Color ambientColor = new Color(0.20f, 0.22f, 0.27f);
        [Range(0f, 2f)] public float ambientIntensity = 1f;
        public AudioClip ambientLoop;
        [TextArea(2, 4)] public string description;

        // visited state used by HUD to grey-out nodes already explored
        [NonSerialized] public bool visited;
    }

    [Serializable]
    public class PathEdge
    {
        public string edgeId;
        public string fromNodeId;
        public string toNodeId;
        public bool bidirectional = true;
        public PathTheme theme = PathTheme.Default;
        public LocomotionMode locomotion = LocomotionMode.Teleport;
        [TextArea(1, 3)] public string narratorCue;
        public bool requiresUnlock;
        public string unlockEventId;        // raised via GameEvents.RaisePathUnlocked(...)
        public float lengthMeters = 4f;     // for routing cost
    }

    /// <summary>
    /// Holds the graph and exposes path-finding + UI-friendly queries. A
    /// MonoBehaviour rather than a ScriptableObject so it can serialize
    /// scene-bound Transform anchors.
    /// </summary>
    public class MultiPathNavigationSystem : MonoBehaviour
    {
        [Header("Graph")]
        [SerializeField] private List<PathNode> nodes = new List<PathNode>();
        [SerializeField] private List<PathEdge> edges = new List<PathEdge>();

        [Header("Defaults")]
        [SerializeField] private string startNodeId = "corridor_main";

        private Dictionary<string, PathNode> _nodeMap;
        private Dictionary<string, List<PathEdge>> _outgoing;
        private HashSet<string> _unlockedEdges;
        private string _currentNodeId;

        public event Action<PathNode, PathNode, PathEdge> OnPathTraversed;
        public event Action<PathNode> OnNodeEntered;

        public IReadOnlyList<PathNode> Nodes => nodes;
        public IReadOnlyList<PathEdge> Edges => edges;
        public string CurrentNodeId => _currentNodeId;
        public PathNode CurrentNode => GetNode(_currentNodeId);

        // -----------------------------------------------------------------
        // Lifecycle
        // -----------------------------------------------------------------
        private void Awake()
        {
            RebuildIndex();
            _unlockedEdges = new HashSet<string>();
            // Auto-unlock any edges that don't require a key
            foreach (var e in edges)
                if (!e.requiresUnlock) _unlockedEdges.Add(e.edgeId);
        }

        private void Start()
        {
            if (!string.IsNullOrEmpty(startNodeId)) EnterNode(startNodeId);
        }

        private void RebuildIndex()
        {
            _nodeMap = new Dictionary<string, PathNode>();
            foreach (var n in nodes)
                if (!string.IsNullOrEmpty(n.nodeId)) _nodeMap[n.nodeId] = n;

            _outgoing = new Dictionary<string, List<PathEdge>>();
            foreach (var e in edges)
            {
                if (!_outgoing.ContainsKey(e.fromNodeId)) _outgoing[e.fromNodeId] = new List<PathEdge>();
                _outgoing[e.fromNodeId].Add(e);

                if (e.bidirectional)
                {
                    if (!_outgoing.ContainsKey(e.toNodeId)) _outgoing[e.toNodeId] = new List<PathEdge>();
                    _outgoing[e.toNodeId].Add(e);
                }
            }
        }

        // -----------------------------------------------------------------
        // Public API
        // -----------------------------------------------------------------
        public PathNode GetNode(string id)
        {
            if (string.IsNullOrEmpty(id) || _nodeMap == null) return null;
            _nodeMap.TryGetValue(id, out var n);
            return n;
        }

        /// <summary>
        /// Returns every edge leaving the given node, regardless of direction.
        /// UI uses this to render a fan of "go this way" choices.
        /// </summary>
        public List<PathEdge> GetOutgoing(string nodeId)
        {
            var result = new List<PathEdge>();
            if (_outgoing != null && _outgoing.TryGetValue(nodeId, out var list))
            {
                foreach (var e in list)
                {
                    if (e.requiresUnlock && !_unlockedEdges.Contains(e.edgeId)) continue;
                    result.Add(e);
                }
            }
            return result;
        }

        public bool TryTraverse(string edgeId)
        {
            var edge = FindEdge(edgeId);
            if (edge == null) return false;
            if (edge.requiresUnlock && !_unlockedEdges.Contains(edge.edgeId)) return false;

            string targetId;
            if (edge.fromNodeId == _currentNodeId) targetId = edge.toNodeId;
            else if (edge.bidirectional && edge.toNodeId == _currentNodeId) targetId = edge.fromNodeId;
            else return false;

            var from = CurrentNode;
            EnterNode(targetId);
            OnPathTraversed?.Invoke(from, CurrentNode, edge);
            return true;
        }

        public void UnlockEdge(string edgeId)
        {
            if (!string.IsNullOrEmpty(edgeId)) _unlockedEdges.Add(edgeId);
        }

        public void EnterNode(string nodeId)
        {
            var node = GetNode(nodeId);
            if (node == null) return;
            _currentNodeId = nodeId;
            node.visited = true;
            ApplyAmbient(node);
            OnNodeEntered?.Invoke(node);
            GameEvents.RaiseRoomEntered(nodeId);
        }

        private PathEdge FindEdge(string id)
        {
            foreach (var e in edges) if (e.edgeId == id) return e;
            return null;
        }

        private void ApplyAmbient(PathNode node)
        {
            RenderSettings.ambientLight = node.ambientColor;
            RenderSettings.ambientIntensity = node.ambientIntensity;
        }

        // -----------------------------------------------------------------
        // Routing — Dijkstra over `lengthMeters`
        // -----------------------------------------------------------------
        /// <summary>
        /// Computes the shortest path from <paramref name="fromId"/> to
        /// <paramref name="toId"/>. Returns the ordered list of node ids
        /// (inclusive of both endpoints) or null if unreachable.
        /// </summary>
        public List<string> ShortestPath(string fromId, string toId)
        {
            if (GetNode(fromId) == null || GetNode(toId) == null) return null;
            var dist = new Dictionary<string, float>();
            var prev = new Dictionary<string, string>();
            var queue = new List<string>();
            foreach (var n in nodes) { dist[n.nodeId] = float.PositiveInfinity; queue.Add(n.nodeId); }
            dist[fromId] = 0f;

            while (queue.Count > 0)
            {
                // pick minimum
                queue.Sort((a, b) => dist[a].CompareTo(dist[b]));
                string u = queue[0];
                queue.RemoveAt(0);
                if (u == toId) break;
                if (float.IsInfinity(dist[u])) break;

                foreach (var e in GetOutgoing(u))
                {
                    string v = (e.fromNodeId == u) ? e.toNodeId : e.fromNodeId;
                    float alt = dist[u] + Mathf.Max(0.5f, e.lengthMeters);
                    if (alt < dist[v])
                    {
                        dist[v] = alt;
                        prev[v] = u;
                    }
                }
            }

            if (!prev.ContainsKey(toId) && fromId != toId) return null;
            var path = new List<string>();
            string cur = toId;
            while (cur != fromId) { path.Add(cur); cur = prev[cur]; }
            path.Add(fromId);
            path.Reverse();
            return path;
        }

        // -----------------------------------------------------------------
        // Programmatic graph construction (used by RuntimeMuseumBootstrapper)
        // -----------------------------------------------------------------
        public void Clear()
        {
            nodes.Clear();
            edges.Clear();
            RebuildIndex();
        }

        public PathNode AddNode(PathNode node) { nodes.Add(node); RebuildIndex(); return node; }
        public PathEdge AddEdge(PathEdge edge) { edges.Add(edge); RebuildIndex(); return edge; }

        // -----------------------------------------------------------------
        // Default museum layout factory
        // -----------------------------------------------------------------
        /// <summary>
        /// Wipes the graph and installs the canonical Virtual-Museum topology:
        ///
        ///     entrance ──► main_corridor ─────────┬─► east_corridor ─► colosseum_room ─► roman_aqueduct
        ///                       │                 │
        ///                       ├─► iraq_room     └─► greece_room ───► parthenon_inner
        ///                       ├─► prague_room
        ///                       ├─► shards_room
        ///                       ├─► aerial_room ─► sky_observatory (locomotion: Floating)
        ///                       └─► indian_hall ─┬─► taj_mahal_garden
        ///                                        ├─► konark_courtyard
        ///                                        └─► hidden_treasury (requiresUnlock = "found_key")
        ///
        /// Plus a "shortcut" path between iraq_room ↔ shards_room.
        /// </summary>
        public void InstallDefaultMuseumGraph()
        {
            Clear();

            void AddN(string id, string name, MuseumRoom r, PathTheme _)
            {
                AddNode(new PathNode { nodeId = id, displayName = name, room = r });
            }

            AddN("entrance",         "Grand Entrance",       MuseumRoom.Corridor,                PathTheme.Reflective);
            AddN("main_corridor",    "Main Corridor",        MuseumRoom.Corridor,                PathTheme.Modern);
            AddN("east_corridor",    "East Wing Corridor",   MuseumRoom.Corridor,                PathTheme.Industrial);

            AddN("iraq_room",        "Iraq — Nahum Shrine",  MuseumRoom.IraqSection,             PathTheme.Ancient);
            AddN("prague_room",      "Prague Monuments",     MuseumRoom.PragueMonuments,         PathTheme.Reflective);
            AddN("shards_room",      "Archaeological Shards",MuseumRoom.ArchaeologicalShards,    PathTheme.Industrial);
            AddN("aerial_room",      "Aerial Photogrammetry",MuseumRoom.AerialPhotogrammetry,    PathTheme.Modern);
            AddN("sky_observatory",  "Sky Observatory",      MuseumRoom.AerialPhotogrammetry,    PathTheme.Modern);

            AddN("indian_hall",      "Indian Architecture Hall", MuseumRoom.IndianArchitecture,  PathTheme.Mystical);
            AddN("taj_mahal_garden", "Taj Mahal Garden",        MuseumRoom.IndianArchitecture,   PathTheme.Reflective);
            AddN("konark_courtyard", "Konark Courtyard",        MuseumRoom.IndianArchitecture,   PathTheme.Mystical);
            AddN("hidden_treasury",  "Hidden Treasury",         MuseumRoom.IndianArchitecture,   PathTheme.Mystical);

            AddN("colosseum_room",   "Colosseum Atrium",     MuseumRoom.PragueMonuments,         PathTheme.Ancient);
            AddN("roman_aqueduct",   "Roman Aqueduct",       MuseumRoom.PragueMonuments,         PathTheme.Ancient);
            AddN("greece_room",      "Greece — Parthenon",   MuseumRoom.PragueMonuments,         PathTheme.Ancient);
            AddN("parthenon_inner",  "Parthenon Cella",      MuseumRoom.PragueMonuments,         PathTheme.Reflective);

            void AddE(string id, string a, string b, PathTheme th, LocomotionMode loco, float len, bool unlock = false, string evt = null)
            {
                AddEdge(new PathEdge
                {
                    edgeId = id,
                    fromNodeId = a, toNodeId = b,
                    theme = th, locomotion = loco,
                    lengthMeters = len, bidirectional = true,
                    requiresUnlock = unlock, unlockEventId = evt
                });
            }

            AddE("e_entrance_main",      "entrance",        "main_corridor",     PathTheme.Reflective, LocomotionMode.Walking, 6f);
            AddE("e_main_iraq",          "main_corridor",   "iraq_room",         PathTheme.Ancient,    LocomotionMode.Teleport, 4f);
            AddE("e_main_prague",        "main_corridor",   "prague_room",       PathTheme.Reflective, LocomotionMode.Teleport, 4f);
            AddE("e_main_shards",        "main_corridor",   "shards_room",       PathTheme.Industrial, LocomotionMode.Teleport, 4f);
            AddE("e_main_aerial",        "main_corridor",   "aerial_room",       PathTheme.Modern,     LocomotionMode.Teleport, 4f);
            AddE("e_main_indian",        "main_corridor",   "indian_hall",       PathTheme.Mystical,   LocomotionMode.Teleport, 5f);
            AddE("e_main_east",          "main_corridor",   "east_corridor",     PathTheme.Industrial, LocomotionMode.Walking, 5f);

            AddE("e_east_colosseum",     "east_corridor",   "colosseum_room",    PathTheme.Ancient,    LocomotionMode.Teleport, 6f);
            AddE("e_east_greece",        "east_corridor",   "greece_room",       PathTheme.Ancient,    LocomotionMode.Teleport, 6f);
            AddE("e_colosseum_aqueduct", "colosseum_room",  "roman_aqueduct",    PathTheme.Ancient,    LocomotionMode.Walking, 8f);
            AddE("e_greece_inner",       "greece_room",     "parthenon_inner",   PathTheme.Reflective, LocomotionMode.Walking, 4f);

            AddE("e_aerial_sky",         "aerial_room",     "sky_observatory",   PathTheme.Modern,     LocomotionMode.Floating, 12f);
            AddE("e_indian_taj",         "indian_hall",     "taj_mahal_garden",  PathTheme.Reflective, LocomotionMode.Walking, 6f);
            AddE("e_indian_konark",      "indian_hall",     "konark_courtyard",  PathTheme.Mystical,   LocomotionMode.Walking, 6f);
            AddE("e_indian_treasury",    "indian_hall",     "hidden_treasury",   PathTheme.Mystical,   LocomotionMode.Glide,    4f, true, "found_key");

            // Cross-room shortcut (bypass the corridor)
            AddE("e_shortcut_iraq_shards", "iraq_room",     "shards_room",       PathTheme.Ancient,    LocomotionMode.Walking, 9f);

            // Direct loop-back (multi-path: many ways to reach indian_hall)
            AddE("e_taj_konark_loop",    "taj_mahal_garden","konark_courtyard",  PathTheme.Mystical,   LocomotionMode.Walking, 7f);
            AddE("e_konark_treasury",    "konark_courtyard","hidden_treasury",   PathTheme.Mystical,   LocomotionMode.Walking, 5f, true, "found_key");

            startNodeId = "entrance";
        }
    }
}
