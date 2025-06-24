using UnityEngine;
using System.Collections.Generic;

namespace VRProject
{
    public class FurniturePrefabGenerator : MonoBehaviour
    {
        [Header("Base Prefab")]
        public GameObject baseCubePrefab;

        [Header("Material Sets")]
        public List<MaterialSet> materialSets = new List<MaterialSet>();

        [Header("Size Variations")]
        public List<Vector3> furnitureSizes = new List<Vector3>
        {
            new Vector3(1f, 1f, 1f),    // Cube
            new Vector3(2f, 0.1f, 1f),  // Table Top
            new Vector3(0.1f, 1f, 0.1f),// Table Leg
            new Vector3(1f, 2f, 0.1f),  // Wall Panel
            new Vector3(0.5f, 0.5f, 2f),// Beam
            new Vector3(1.5f, 0.8f, 0.8f), // Chair
            new Vector3(0.3f, 1.8f, 0.3f)  // Pillar
        };

        [Header("Generated Prefabs")]
        public List<GameObject> generatedFurniture = new List<GameObject>();

        [System.Serializable]
        public class MaterialSet
        {
            public string name;
            public Material material;
            public Color accentColor = Color.white;
        }

        [ContextMenu("Generate Furniture Variants")]
        public void GenerateAllVariants()
        {
            ClearExistingFurniture();

            if (baseCubePrefab == null)
            {
                Debug.LogError("Base cube prefab is not assigned!");
                return;
            }

            int furnitureIndex = 0;
            string[] furnitureNames = { "Mesa", "Silla", "Estante", "Panel", "Viga", "Bloque", "Columna" };

            foreach (Vector3 size in furnitureSizes)
            {
                foreach (MaterialSet matSet in materialSets)
                {
                    string furnitureName = furnitureNames[Mathf.Min(furnitureIndex, furnitureNames.Length - 1)];
                    GameObject newFurniture = CreateFurnitureVariant(size, matSet, $"{furnitureName}_{matSet.name}");
                    if (newFurniture != null)
                    {
                        generatedFurniture.Add(newFurniture);
                    }
                }
                furnitureIndex++;
            }

            Debug.Log($"Generated {generatedFurniture.Count} furniture variants!");
        }

        private GameObject CreateFurnitureVariant(Vector3 size, MaterialSet materialSet, string name)
        {
            GameObject furniture = Instantiate(baseCubePrefab, transform);
            furniture.name = name;

            Transform meshTransform = furniture.transform.GetChild(0);
            if (meshTransform != null)
            {
                meshTransform.localScale = size;

                MeshRenderer renderer = meshTransform.GetComponent<MeshRenderer>();
                if (renderer != null && materialSet.material != null)
                {
                    renderer.material = materialSet.material;
                }
            }

            FurnitureAttachable attachable = furniture.GetComponent<FurnitureAttachable>();
            if (attachable != null)
            {
                SetupAttachmentPoints(attachable, size);

                if (attachable.highlightMaterial != null)
                {
                    attachable.highlightMaterial.color = materialSet.accentColor;
                }
            }

            return furniture;
        }

        private void SetupAttachmentPoints(FurnitureAttachable attachable, Vector3 size)
        {
            List<FurnitureAttachmentPoint> points = new List<FurnitureAttachmentPoint>();

            Vector3 halfSize = size * 0.5f;

            Vector3[] positions = {
                new Vector3(0, halfSize.y, 0),      // Top
                new Vector3(0, -halfSize.y, 0),     // Bottom
                new Vector3(halfSize.x, 0, 0),      // Right
                new Vector3(-halfSize.x, 0, 0),     // Left
                new Vector3(0, 0, halfSize.z),      // Front
                new Vector3(0, 0, -halfSize.z)      // Back
            };

            Vector3[] normals = {
                Vector3.up,      // Top
                Vector3.down,    // Bottom
                Vector3.right,   // Right
                Vector3.left,    // Left
                Vector3.forward, // Front
                Vector3.back     // Back
            };

            for (int i = 0; i < positions.Length; i++)
            {
                GameObject pointObj = new GameObject($"AttachmentPoint_{i}");
                pointObj.transform.SetParent(attachable.transform);
                pointObj.transform.localPosition = positions[i];

                FurnitureAttachmentPoint point = pointObj.AddComponent<FurnitureAttachmentPoint>();
                point.parentFurniture = attachable;

                // Set connection type based on position
                if (i == 0) point.connectionType = FurnitureAttachmentPoint.ConnectionType.Top;
                else if (i == 1) point.connectionType = FurnitureAttachmentPoint.ConnectionType.Bottom;
                else point.connectionType = FurnitureAttachmentPoint.ConnectionType.Side;

                points.Add(point);
            }

            attachable.attachmentPoints = points;
        }

        [ContextMenu("Clear Generated Furniture")]
        public void ClearExistingFurniture()
        {
            foreach (GameObject furniture in generatedFurniture)
            {
                if (furniture != null)
                {
                    if (Application.isPlaying)
                        Destroy(furniture);
                    else
                        DestroyImmediate(furniture);
                }
            }
            generatedFurniture.Clear();
        }

        public List<ModelData> GetModelDataList()
        {
            List<ModelData> modelList = new List<ModelData>();

            foreach (GameObject furniture in generatedFurniture)
            {
                if (furniture != null)
                {
                    ModelData modelData = new ModelData
                    {
                        modelName = furniture.name,
                        prefab = furniture,
                        description = $"Mueble de construcción: {furniture.name}"
                    };
                    modelList.Add(modelData);
                }
            }

            return modelList;
        }
    }
}