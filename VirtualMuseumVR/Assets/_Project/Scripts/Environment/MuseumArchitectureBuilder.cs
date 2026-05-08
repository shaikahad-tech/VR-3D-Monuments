// =============================================================================
// MuseumArchitectureBuilder.cs — Procedural Room/Corridor Generator
// Virtual Museum VR Project (Pavelka et al., 2019)
// =============================================================================
// Editor-time script that generates the 4-room + corridor museum layout
// described in the paper from configurable ScriptableObject definitions.
// =============================================================================

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VirtualMuseumVR.Environment
{
    [System.Serializable]
    public class RoomDimensions
    {
        public float width = 8f;
        public float depth = 8f;
        public float height = 4f;
        public float wallThickness = 0.3f;
        public float doorWidth = 2f;
        public float doorHeight = 3f;
    }

    [System.Serializable]
    public class CorridorDimensions
    {
        public float width = 3f;
        public float length = 6f;
        public float height = 3.5f;
    }

    public class MuseumArchitectureBuilder : MonoBehaviour
    {
        [Header("Room Layout")]
        [SerializeField] private RoomDimensions iraqRoomDims = new RoomDimensions();
        [SerializeField] private RoomDimensions pragueRoomDims = new RoomDimensions();
        [SerializeField] private RoomDimensions shardsRoomDims = new RoomDimensions();
        [SerializeField] private RoomDimensions aerialRoomDims = new RoomDimensions();
        [SerializeField] private RoomDimensions indianRoomDims = new RoomDimensions
        {
            width = 12f, depth = 12f, height = 5f // Larger room for monumental exhibits
        };

        [Header("Corridor")]
        [SerializeField] private CorridorDimensions corridorDims = new CorridorDimensions();

        [Header("Materials")]
        [SerializeField] private Material floorMaterial;
        [SerializeField] private Material wallMaterial;
        [SerializeField] private Material ceilingMaterial;
        [SerializeField] private Material corridorMaterial;

        [Header("Pedestal Settings")]
        [SerializeField] private int pedestalsPerRoom = 4;
        [SerializeField] private float pedestalHeight = 1.2f;
        [SerializeField] private float pedestalRadius = 0.3f;
        [SerializeField] private Material pedestalMaterial;

        [Header("Generation")]
        [SerializeField] private bool markAsStatic = true;

        /// <summary>
        /// Generate the complete museum layout. Called from Editor menu or setup wizard.
        /// </summary>
        public void GenerateMuseum()
        {
            Debug.Log("[ArchitectureBuilder] Generating museum layout...");

            // Clear existing children
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }

            // Generate corridor (center hub)
            var corridor = GenerateRoom("Corridor", corridorDims.width,
                corridorDims.length, corridorDims.height, Vector3.zero, corridorMaterial);

            // Generate 5 rooms around the corridor
            float corridorHalfLength = corridorDims.length * 0.5f;
            float roomOffset = corridorHalfLength + iraqRoomDims.depth * 0.5f;

            // Room 1: Iraq Section (North)
            var iraqRoom = GenerateRoom("Room_Iraq", iraqRoomDims,
                new Vector3(0, 0, roomOffset));

            // Room 2: Prague Monuments (South)
            var pragueRoom = GenerateRoom("Room_Prague", pragueRoomDims,
                new Vector3(0, 0, -roomOffset));

            // Room 3: Archaeological Shards (East)
            float eastOffset = corridorDims.width * 0.5f + shardsRoomDims.depth * 0.5f;
            var shardsRoom = GenerateRoom("Room_Shards", shardsRoomDims,
                new Vector3(eastOffset, 0, 0));

            // Room 4: Aerial Photogrammetry (West)
            float westOffset = -(corridorDims.width * 0.5f + aerialRoomDims.depth * 0.5f);
            var aerialRoom = GenerateRoom("Room_Aerial", aerialRoomDims,
                new Vector3(westOffset, 0, 0));

            // Room 5: Indian Architecture (Southeast — larger room)
            float indiaOffsetX = corridorDims.width * 0.5f + indianRoomDims.depth * 0.5f;
            float indiaOffsetZ = -(corridorHalfLength + indianRoomDims.depth * 0.5f);
            var indianRoom = GenerateRoom("Room_India", indianRoomDims,
                new Vector3(indiaOffsetX, 0, indiaOffsetZ));

            // Generate pedestals in each room
            GeneratePedestals(iraqRoom.transform, iraqRoomDims);
            GeneratePedestals(pragueRoom.transform, pragueRoomDims);
            GeneratePedestals(shardsRoom.transform, shardsRoomDims);
            GeneratePedestals(aerialRoom.transform, aerialRoomDims);
            GeneratePedestals(indianRoom.transform, indianRoomDims, 5); // 5 large pedestals

            Debug.Log("[ArchitectureBuilder] Museum layout generated with 5 rooms.");
        }

        private GameObject GenerateRoom(string name, RoomDimensions dims, Vector3 position)
        {
            return GenerateRoom(name, dims.width, dims.depth, dims.height, position, wallMaterial);
        }

        private GameObject GenerateRoom(string name, float width, float depth,
            float height, Vector3 position, Material material)
        {
            var roomObj = new GameObject(name);
            roomObj.transform.SetParent(transform);
            roomObj.transform.localPosition = position;

            // Floor
            var floor = CreateQuad($"{name}_Floor", roomObj.transform,
                new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0),
                new Vector3(width, depth, 1), floorMaterial ?? material);

            // Ceiling
            CreateQuad($"{name}_Ceiling", roomObj.transform,
                new Vector3(0, height, 0), Quaternion.Euler(180, 0, 0),
                new Vector3(width, depth, 1), ceilingMaterial ?? material);

            // Walls
            CreateQuad($"{name}_Wall_North", roomObj.transform,
                new Vector3(0, height * 0.5f, depth * 0.5f), Quaternion.Euler(90, 0, 0),
                new Vector3(width, height, 1), material);

            CreateQuad($"{name}_Wall_South", roomObj.transform,
                new Vector3(0, height * 0.5f, -depth * 0.5f), Quaternion.Euler(-90, 0, 0),
                new Vector3(width, height, 1), material);

            CreateQuad($"{name}_Wall_East", roomObj.transform,
                new Vector3(width * 0.5f, height * 0.5f, 0), Quaternion.Euler(0, 0, 90),
                new Vector3(height, depth, 1), material);

            CreateQuad($"{name}_Wall_West", roomObj.transform,
                new Vector3(-width * 0.5f, height * 0.5f, 0), Quaternion.Euler(0, 0, -90),
                new Vector3(height, depth, 1), material);

            if (markAsStatic)
                SetStaticRecursive(roomObj);

            return roomObj;
        }

        private GameObject CreateQuad(string name, Transform parent,
            Vector3 localPos, Quaternion localRot, Vector3 scale, Material mat)
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
            obj.name = name;
            obj.transform.SetParent(parent);
            obj.transform.localPosition = localPos;
            obj.transform.localRotation = localRot;
            obj.transform.localScale = scale;

            if (mat != null)
                obj.GetComponent<Renderer>().sharedMaterial = mat;

            // Add mesh collider for teleportation (floors only)
            if (name.Contains("Floor"))
            {
                var col = obj.GetComponent<Collider>();
                if (col != null) col.enabled = true;
            }

            return obj;
        }

        private void GeneratePedestals(Transform roomTransform, RoomDimensions dims,
            int overrideCount = -1)
        {
            int count = overrideCount > 0 ? overrideCount : pedestalsPerRoom;
            float spacing = dims.width / (count + 1);

            for (int i = 0; i < count; i++)
            {
                float x = -dims.width * 0.5f + spacing * (i + 1);
                float z = dims.depth * 0.25f * (i % 2 == 0 ? 1 : -1);

                var pedestal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                pedestal.name = $"Pedestal_{i}";
                pedestal.transform.SetParent(roomTransform);
                pedestal.transform.localPosition = new Vector3(x, pedestalHeight * 0.5f, z);
                pedestal.transform.localScale = new Vector3(
                    pedestalRadius * 2, pedestalHeight * 0.5f, pedestalRadius * 2);

                if (pedestalMaterial != null)
                    pedestal.GetComponent<Renderer>().sharedMaterial = pedestalMaterial;

                // Exhibit spawn point on top
                var spawnPoint = new GameObject("ExhibitSpawnPoint");
                spawnPoint.transform.SetParent(pedestal.transform);
                spawnPoint.transform.localPosition = new Vector3(0, 1.1f, 0);

                if (markAsStatic)
                    pedestal.isStatic = true;
            }
        }

        private void SetStaticRecursive(GameObject obj)
        {
            obj.isStatic = true;
            foreach (Transform child in obj.transform)
                SetStaticRecursive(child.gameObject);
        }
    }
}
