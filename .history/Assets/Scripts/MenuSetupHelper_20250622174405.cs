using UnityEngine;
using System.Collections.Generic;

namespace VRProject
{
    public class MenuSetupHelper : MonoBehaviour
    {
        [Header("Menu Manager Reference")]
        public AnimatedMenuManager animatedMenuManager;
        public MenuManager basicMenuManager;

        [Header("Furniture Generator")]
        public FurniturePrefabGenerator furnitureGenerator;

        [Header("Auto Setup")]
        public bool autoSetupOnStart = true;

        private void Start()
        {
            if (autoSetupOnStart)
            {
                SetupMenuWithGeneratedFurniture();
            }
        }

        [ContextMenu("Setup Menu with Generated Furniture")]
        public void SetupMenuWithGeneratedFurniture()
        {
            if (furnitureGenerator == null)
            {
                Debug.LogError("Furniture generator is not assigned!");
                return;
            }

            furnitureGenerator.GenerateAllVariants();
            List<ModelData> modelList = furnitureGenerator.GetModelDataList();

            if (animatedMenuManager != null)
            {
                animatedMenuManager.availableModels = modelList;
                Debug.Log($"Setup AnimatedMenuManager with {modelList.Count} models");
            }

            if (basicMenuManager != null)
            {
                basicMenuManager.availableModels = modelList;
                Debug.Log($"Setup MenuManager with {modelList.Count} models");
            }

            if (animatedMenuManager == null && basicMenuManager == null)
            {
                Debug.LogWarning("No menu managers assigned! Please assign either AnimatedMenuManager or MenuManager.");
            }
        }

        [ContextMenu("Create Sample Materials")]
        public void CreateSampleMaterials()
        {
            if (furnitureGenerator == null)
            {
                Debug.LogError("Furniture generator is not assigned!");
                return;
            }

            furnitureGenerator.materialSets.Clear();

            Color[] colors = {
                new Color(0.8f, 0.4f, 0.2f), // Wood Brown
                new Color(0.6f, 0.6f, 0.6f), // Metal Gray
                new Color(0.2f, 0.2f, 0.8f), // Blue Plastic
                new Color(0.8f, 0.2f, 0.2f), // Red
                new Color(0.2f, 0.8f, 0.2f), // Green
                new Color(0.8f, 0.8f, 0.2f), // Yellow
                new Color(0.4f, 0.2f, 0.8f)  // Purple
            };

            string[] names = { "Madera", "Metal", "Plastico", "Rojo", "Verde", "Amarillo", "Morado" };

            for (int i = 0; i < colors.Length; i++)
            {
                Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                mat.color = colors[i];
                mat.name = $"Material_{names[i]}";

                var matSet = new FurniturePrefabGenerator.MaterialSet
                {
                    name = names[i],
                    material = mat,
                    accentColor = colors[i] * 1.2f
                };

                furnitureGenerator.materialSets.Add(matSet);
            }

            Debug.Log($"Created {colors.Length} sample materials");
        }
    }
}