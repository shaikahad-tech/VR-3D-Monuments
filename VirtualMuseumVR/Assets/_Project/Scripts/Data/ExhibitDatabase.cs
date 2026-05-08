// =============================================================================
// ExhibitDatabase.cs — Runtime Registry of All Exhibits
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================

using System.Collections.Generic;
using UnityEngine;
using VirtualMuseumVR.Core;

namespace VirtualMuseumVR.Data
{
    public class ExhibitDatabase : MonoBehaviour
    {
        [Header("Exhibit Data Assets")]
        [Tooltip("Drag all ExhibitData ScriptableObjects here")]
        [SerializeField] private List<ExhibitData> allExhibits = new List<ExhibitData>();

        private Dictionary<string, ExhibitData> _exhibitLookup;
        private Dictionary<MuseumRoom, List<ExhibitData>> _roomExhibits;

        private static ExhibitDatabase _instance;
        public static ExhibitDatabase Instance => _instance;

        private void Awake()
        {
            _instance = this;
            BuildLookups();
        }

        private void BuildLookups()
        {
            _exhibitLookup = new Dictionary<string, ExhibitData>();
            _roomExhibits = new Dictionary<MuseumRoom, List<ExhibitData>>();

            foreach (var exhibit in allExhibits)
            {
                if (exhibit == null) continue;

                _exhibitLookup[exhibit.exhibitId] = exhibit;

                if (!_roomExhibits.ContainsKey(exhibit.room))
                    _roomExhibits[exhibit.room] = new List<ExhibitData>();
                _roomExhibits[exhibit.room].Add(exhibit);
            }

            Debug.Log($"[ExhibitDB] Loaded {allExhibits.Count} exhibits " +
                $"across {_roomExhibits.Count} rooms.");
        }

        /// <summary>Get exhibit data by ID.</summary>
        public ExhibitData GetExhibit(string exhibitId)
        {
            _exhibitLookup.TryGetValue(exhibitId, out var data);
            return data;
        }

        /// <summary>Get all exhibits in a specific room.</summary>
        public List<ExhibitData> GetExhibitsInRoom(MuseumRoom room)
        {
            if (_roomExhibits.TryGetValue(room, out var exhibits))
                return new List<ExhibitData>(exhibits);
            return new List<ExhibitData>();
        }

        /// <summary>Get all exhibits.</summary>
        public List<ExhibitData> GetAllExhibits() => new List<ExhibitData>(allExhibits);

        /// <summary>Get total exhibit count.</summary>
        public int TotalExhibitCount => allExhibits.Count;

        /// <summary>Get count of exhibits by acquisition method.</summary>
        public int GetCountByMethod(AcquisitionMethod method)
        {
            int count = 0;
            foreach (var exhibit in allExhibits)
            {
                if (exhibit.acquisitionMethod == method) count++;
            }
            return count;
        }

        /// <summary>Get total polygon count across all exhibits.</summary>
        public long GetTotalOriginalPolygons()
        {
            long total = 0;
            foreach (var exhibit in allExhibits)
                total += exhibit.originalPolygonCount;
            return total;
        }

        /// <summary>Get total optimized polygon count.</summary>
        public long GetTotalOptimizedPolygons()
        {
            long total = 0;
            foreach (var exhibit in allExhibits)
                total += exhibit.optimizedPolygonCount;
            return total;
        }
    }
}
