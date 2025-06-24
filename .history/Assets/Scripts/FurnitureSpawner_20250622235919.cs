using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace VRProject
{
    public class FurnitureSpawner : MonoBehaviour
    {
        [System.Serializable]
        public class FurnitureData
        {
            public string name;
            public Vector3 position;
            public Quaternion rotation;
            public Vector3 scale;
            public GameObject prefab;
            public string setNumber;
        }

        [Header("Furniture Storage")]
        public List<FurnitureData> originalFurnitureData = new List<FurnitureData>();

        [Header("Current Spawned Furniture")]
        public List<GameObject> currentSpawnedFurniture = new List<GameObject>();

        private void Start()
        {
            if (Application.isPlaying)
            {
                Debug.Log("🏭 FurnitureSpawner starting...");
                StoreFurnitureData();
                DestroyAllFurniture();
                Debug.Log("✅ FurnitureSpawner ready - all furniture stored and cleared");
            }
        }

                [ContextMenu("📦 Store Furniture Data")]
        public void StoreFurnitureData()
        {
            Debug.Log("📦 Storing all furniture data...");

            originalFurnitureData.Clear();

            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            int storedCount = 0;

            foreach (GameObject obj in allObjects)
            {
                if (obj.name.StartsWith("furniture_"))
                {
                    FurnitureData data = new FurnitureData();
                    data.name = obj.name;
                    data.position = obj.transform.position;
                    data.rotation = obj.transform.rotation;
                    data.scale = obj.transform.localScale;
                    data.prefab = obj;

                    string[] parts = obj.name.Split('_');
                    if (parts.Length >= 2)
                    {
                        data.setNumber = parts[1];
                    }

                    originalFurnitureData.Add(data);
                    storedCount++;

                    Debug.Log($"   📦 Stored: {data.name} (Set: {data.setNumber}) at {data.position}");
                }
            }

            Debug.Log($"📦 Stored {storedCount} furniture pieces");
        }

                [ContextMenu("💥 Destroy All Furniture")]
        public void DestroyAllFurniture()
        {
            Debug.Log("💥 Destroying all furniture...");

            foreach (GameObject obj in currentSpawnedFurniture)
            {
                if (obj != null)
                {
                    if (Application.isPlaying)
                        Destroy(obj);
                }
            }
            currentSpawnedFurniture.Clear();

            if (Application.isPlaying)
            {
                GameObject[] allObjects = FindObjectsOfType<GameObject>();
                int destroyedCount = 0;

                foreach (GameObject obj in allObjects)
                {
                    if (obj.name.StartsWith("furniture_"))
                    {
                        Destroy(obj);
                        destroyedCount++;
                    }
                }

                Debug.Log($"💥 Destroyed {destroyedCount} furniture pieces");
            }
            else
            {
                Debug.Log("⚠️ Not destroying furniture in editor mode for safety!");
            }
        }

        [ContextMenu("🏗️ Spawn Furniture Set 1")]
        public void SpawnFurnitureSet1()
        {
            SpawnFurnitureSet("1");
        }

        [ContextMenu("🏗️ Spawn Furniture Set 2")]
        public void SpawnFurnitureSet2()
        {
            SpawnFurnitureSet("2");
        }

        [ContextMenu("🏗️ Spawn Furniture Set 3")]
        public void SpawnFurnitureSet3()
        {
            SpawnFurnitureSet("3");
        }

        public void SpawnFurnitureSet(string setNumber)
        {
            Debug.Log($"🏗️ Spawning furniture set: {setNumber}");

            DestroyAllFurniture();

            var furnitureToSpawn = originalFurnitureData.Where(f => f.setNumber == setNumber).ToList();
            int spawnedCount = 0;

            foreach (FurnitureData data in furnitureToSpawn)
            {
                if (data.prefab != null)
                {
                    GameObject spawnedObj = Instantiate(data.prefab, data.position, data.rotation);
                    spawnedObj.name = data.name;
                    spawnedObj.transform.localScale = data.scale;

                    FurnitureVisibilityController controller = spawnedObj.GetComponent<FurnitureVisibilityController>();
                    if (controller != null)
                    {
                        controller.hideOnStart = false;
                        controller.furnitureSetNumber = data.setNumber;
                    }

                    currentSpawnedFurniture.Add(spawnedObj);
                    spawnedCount++;

                    Debug.Log($"   🏗️ Spawned: {data.name} at {data.position}");
                }
            }

            Debug.Log($"🏗️ Spawned {spawnedCount} pieces from set {setNumber}");
        }

        [ContextMenu("🔍 Debug Stored Furniture")]
        public void DebugStoredFurniture()
        {
            Debug.Log("🔍 Debugging stored furniture...");

            var set1 = originalFurnitureData.Where(f => f.setNumber == "1").Count();
            var set2 = originalFurnitureData.Where(f => f.setNumber == "2").Count();
            var set3 = originalFurnitureData.Where(f => f.setNumber == "3").Count();

            Debug.Log($"🔍 Stored furniture: Set 1: {set1}, Set 2: {set2}, Set 3: {set3}");

            foreach (FurnitureData data in originalFurnitureData)
            {
                Debug.Log($"   📦 {data.name} (Set: {data.setNumber}) - Prefab: {(data.prefab != null ? "✅" : "❌")}");
            }
        }

        public void ClearAllFurniture()
        {
            Debug.Log("🧹 Clearing all furniture for clean slate...");
            DestroyAllFurniture();
        }
    }
}