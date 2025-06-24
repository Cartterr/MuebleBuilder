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
                StartCoroutine(InitializeWithDelay());
            }
        }

                private System.Collections.IEnumerator InitializeWithDelay()
        {
            yield return new WaitForSeconds(0.1f);

            Debug.Log("🔍 Searching for furniture in scene...");

            // Store data BEFORE hiding furniture to preserve active state
            StoreFurnitureData();

            if (originalFurnitureData.Count == 0)
            {
                Debug.LogWarning("⚠️ No furniture found on first attempt, retrying...");
                yield return new WaitForSeconds(0.5f);
                StoreFurnitureData();
            }

            if (originalFurnitureData.Count > 0)
            {
                Debug.Log($"✅ Found {originalFurnitureData.Count} furniture pieces, now hiding them");
                HideAllOriginalFurniture();
            }
            else
            {
                Debug.LogError("❌ No furniture found after retries!");
            }

            Debug.Log("✅ FurnitureSpawner ready");
        }

                [ContextMenu("📦 Store Furniture Data")]
        public void StoreFurnitureData()
        {
            Debug.Log("📦 Storing all furniture data...");

            originalFurnitureData.Clear();

            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            int storedCount = 0;

            foreach (GameObject obj in allObjects)
            {
                if (obj.scene.IsValid() && obj.name.StartsWith("furniture_") && !obj.name.Contains("_spawned"))
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

            var sets = originalFurnitureData.GroupBy(f => f.setNumber).ToList();
            foreach (var set in sets)
            {
                Debug.Log($"   📦 Set {set.Key}: {set.Count()} pieces");
            }
        }

                private void HideAllOriginalFurniture()
        {
            Debug.Log("🙈 Hiding all original furniture...");

            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            int hiddenCount = 0;

            foreach (GameObject obj in allObjects)
            {
                if (obj.name.StartsWith("furniture_"))
                {
                    obj.SetActive(false);
                    hiddenCount++;
                }
            }

            Debug.Log($"🙈 Hidden {hiddenCount} original furniture pieces");
        }

        [ContextMenu("💥 Clear Spawned Furniture")]
        public void ClearSpawnedFurniture()
        {
            Debug.Log("💥 Clearing spawned furniture...");

            foreach (GameObject obj in currentSpawnedFurniture)
            {
                if (obj != null)
                {
                    if (Application.isPlaying)
                        Destroy(obj);
                }
            }
            currentSpawnedFurniture.Clear();

            Debug.Log($"💥 Cleared {currentSpawnedFurniture.Count} spawned furniture pieces");
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

            ClearSpawnedFurniture();
            HideAllOriginalFurniture();

            var furnitureToSpawn = originalFurnitureData.Where(f => f.setNumber == setNumber).ToList();
            Debug.Log($"🔍 Found {furnitureToSpawn.Count} pieces to spawn for set {setNumber}");

            int spawnedCount = 0;

            foreach (FurnitureData data in furnitureToSpawn)
            {
                if (data.prefab != null)
                {
                    GameObject spawnedObj = Instantiate(data.prefab, data.position, data.rotation);
                    spawnedObj.name = data.name + "_spawned";
                    spawnedObj.transform.localScale = data.scale;
                    spawnedObj.SetActive(true);

                    currentSpawnedFurniture.Add(spawnedObj);
                    spawnedCount++;

                    Debug.Log($"   🏗️ Spawned: {data.name} at {data.position}");
                }
                else
                {
                    Debug.LogWarning($"⚠️ Prefab is null for {data.name}");
                }
            }

            Debug.Log($"🏗️ Successfully spawned {spawnedCount} pieces from set {setNumber}");
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

        [ContextMenu("🔍 Debug Current Scene Furniture")]
        public void DebugCurrentSceneFurniture()
        {
            Debug.Log("🔍 Debugging current scene furniture...");

            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            int furnitureCount = 0;

            foreach (GameObject obj in allObjects)
            {
                if (obj.scene.IsValid() && obj.name.StartsWith("furniture_") && !obj.name.Contains("_spawned"))
                {
                    furnitureCount++;
                    string[] parts = obj.name.Split('_');
                    string setNumber = parts.Length >= 2 ? parts[1] : "unknown";
                    Debug.Log($"   🔍 Found: {obj.name} (Set: {setNumber}) - Active: {obj.activeSelf} - Scene: {obj.scene.name}");
                }
            }

            Debug.Log($"🔍 Total furniture found in scene: {furnitureCount}");
        }

        public void ClearAllFurniture()
        {
            if (Application.isPlaying)
            {
                Debug.Log("🧹 Clearing all furniture for clean slate...");
                ClearSpawnedFurniture();
                HideAllOriginalFurniture();
            }
            else
            {
                Debug.LogWarning("⚠️ Cannot clear furniture in editor mode for safety!");
            }
        }
    }
}