using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace VRProject
{
    [System.Serializable]
    public class InstructionData
    {
        public string objectName;
        public string instructionText;
        public float displayDuration = 3f;
    }

    public class AutoMenuSetup : MonoBehaviour
    {
        [Header("Auto Setup")]
        public Transform playerSpawnPoint;

        [Header("Instruction System")]
        public List<InstructionData> instructionDatabase = new List<InstructionData>
        {
            new InstructionData { objectName = "furniture_1_1_spawned", instructionText = "Coloca esta base de mesa en el área de construcción", displayDuration = 4f },
            new InstructionData { objectName = "furniture_1_2_spawned", instructionText = "Conecta esta pata a una esquina de la base de la mesa", displayDuration = 4f },
            new InstructionData { objectName = "furniture_1_3_spawned", instructionText = "Conecta esta pata a otra esquina de la base", displayDuration = 4f },
            new InstructionData { objectName = "furniture_1_4_spawned", instructionText = "Coloca esta pata en la tercera esquina de la mesa", displayDuration = 4f },
            new InstructionData { objectName = "furniture_1_5_spawned", instructionText = "Conecta la última pata para completar la mesa", displayDuration = 4f },
            new InstructionData { objectName = "furniture_1_6_spawned", instructionText = "Esta es tu mesa de construcción - úsala para construir", displayDuration = 3f }
        };

        [ContextMenu("🚀 CREATE COMPLETE MENU SYSTEM")]
        public void CreateCompleteMenuSystem()
        {
            Debug.Log("🚀🚀🚀 STARTING COMPLETE MENU SYSTEM CREATION 🚀🚀🚀");

            try
            {
                Debug.Log("🎯 Step 1: Finding player spawn point...");
                if (playerSpawnPoint == null)
                {
                    FindPlayerSpawnPoint();
                }
                if (playerSpawnPoint != null)
                    Debug.Log($"✅ Player spawn point found: {playerSpawnPoint.name}");
                else
                    Debug.LogError("❌ RED X RED - Player spawn point NOT found!");

                Debug.Log("🎯 Step 2: Creating origin point...");
                CreateOriginPointIfNotExists();
                Debug.Log("✅ Origin point creation completed");

                Debug.Log("🎯 Step 3: Skipping old furniture visibility setup (using new system)...");
                Debug.Log("✅ Furniture visibility setup skipped - new FurnitureSpawner handles this");

                Debug.Log("🎯 Step 4: Creating canvas and UI panels...");
                CreateCanvasIfNotExists();
                Debug.Log("✅ Canvas and UI panels created");

                Debug.Log("🎯 Step 5: Creating menu manager...");
                CreateMenuManagerIfNotExists();
                Debug.Log("✅ Menu manager created");

                Debug.Log("🎯 Step 6: Creating restart button...");
                CreateRestartButtonIfNotExists();
                Debug.Log("✅ Restart button created");

                Debug.Log("🎯 Step 7: Creating beautiful instruction panel...");
                CreateInstructionPanelIfNotExists();
                Debug.Log("✅ Beautiful instruction panel created");

                Debug.Log("🎯 Step 8: Adding grab detectors to all furniture objects...");
                AddFurnitureGrabDetectors();
                Debug.Log("✅ Furniture grab detectors added");

                Debug.Log("🎯 Step 9: SimpleFurnitureSelector will handle furniture automatically");
                Debug.Log("✅ Furniture selection will be handled by SimpleFurnitureSelector");

                Debug.Log("🎯 Step 10: SimpleFurnitureSelector will handle button events automatically");
                Debug.Log("✅ Button events will be connected by SimpleFurnitureSelector");

                Debug.Log("🎉🎉🎉 COMPLETE MENU SYSTEM CREATED SUCCESSFULLY! 🎉🎉🎉");
                ValidateCompleteSetup();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ RED X RED - CRITICAL ERROR during menu system creation: {e.Message}");
                Debug.LogError($"❌ RED X RED - Stack trace: {e.StackTrace}");
            }
        }

                                private void FindPlayerSpawnPoint()
        {
            GameObject xrOrigin = GameObject.Find("XR Origin (XR Rig)");
            if (xrOrigin == null)
                xrOrigin = GameObject.Find("XROrigin");
            if (xrOrigin == null)
                xrOrigin = FindObjectOfType<Camera>()?.transform.parent?.gameObject;

            if (xrOrigin != null)
            {
                playerSpawnPoint = xrOrigin.transform;
                Debug.Log($"🎯 Auto-found player spawn point: {xrOrigin.name}");
            }
            else
            {
                Debug.LogWarning("⚠️ Could not find player spawn point. Please assign manually.");
            }
        }

                private Material GetCableMaterial()
        {
            Material cableMaterial = Resources.Load<Material>("Assets/Material/Cable");
            if (cableMaterial == null)
            {
#if UNITY_EDITOR
                cableMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Material/Cable.mat");
#endif
            }

            if (cableMaterial != null)
            {
                Debug.Log("✅ Using Cable material for buttons");
                return cableMaterial;
            }
            else
            {
                Debug.LogWarning("⚠️ Cable material not found, creating fallback material");
                Material fallbackMaterial = new Material(Shader.Find("Standard"));
                fallbackMaterial.color = new Color(0.2f, 0.3f, 0.8f, 1f);
                fallbackMaterial.SetFloat("_Metallic", 0.2f);
                fallbackMaterial.SetFloat("_Glossiness", 0.8f);
                return fallbackMaterial;
            }
        }

        [ContextMenu("🎯 Create Origin Point")]
        public void CreateOriginPoint()
        {
            CreateOriginPointIfNotExists();
        }

        private void CreateOriginPointIfNotExists()
        {
            GameObject existingOrigin = GameObject.Find("RecenterOriginPoint");
            if (existingOrigin != null)
            {
                Debug.Log("✅ RecenterOriginPoint already exists, skipping creation");
                return;
            }

            GameObject originPoint = new GameObject("RecenterOriginPoint");

            Vector3 originPosition = Vector3.zero;
            Quaternion originRotation = Quaternion.identity;

            if (playerSpawnPoint != null)
            {
                originPosition = playerSpawnPoint.position;
                originRotation = playerSpawnPoint.rotation;
            }
            else
            {
                originPosition = new Vector3(0, 0, 0);
                originRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
            }

            originPoint.transform.position = originPosition;
            originPoint.transform.rotation = originRotation * Quaternion.Euler(0, 180, 0);

            RecenterOriginPoint recenterScript = originPoint.AddComponent<RecenterOriginPoint>();

            CreateOriginVisualIndicator(originPoint.transform);

            Debug.Log($"🎯 Recenter origin point created at position: {originPosition}, rotation: {originRotation.eulerAngles}");
        }

        private void CreateOriginVisualIndicator(Transform parent)
        {
            GameObject indicator = new GameObject("OriginIndicator");
            indicator.transform.SetParent(parent);
            indicator.transform.localPosition = Vector3.zero;
            indicator.transform.localRotation = Quaternion.identity;

            MeshRenderer meshRenderer = indicator.AddComponent<MeshRenderer>();
            MeshFilter meshFilter = indicator.AddComponent<MeshFilter>();

            meshFilter.mesh = CreateOriginIndicatorMesh();

                        Material indicatorMaterial = GetCableMaterial();
            if (indicatorMaterial != null)
            {
                Material transparentCable = new Material(indicatorMaterial);
                transparentCable.color = new Color(indicatorMaterial.color.r, indicatorMaterial.color.g, indicatorMaterial.color.b, 0.7f);
                transparentCable.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                transparentCable.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                transparentCable.SetInt("_ZWrite", 0);
                transparentCable.DisableKeyword("_ALPHATEST_ON");
                transparentCable.EnableKeyword("_ALPHABLEND_ON");
                transparentCable.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                transparentCable.renderQueue = 3000;
                meshRenderer.material = transparentCable;
            }
            else
            {
                Material fallbackMaterial = new Material(Shader.Find("Standard"));
                fallbackMaterial.color = new Color(0f, 1f, 0f, 0.7f);
                fallbackMaterial.SetFloat("_Metallic", 0.1f);
                fallbackMaterial.SetFloat("_Glossiness", 0.9f);
                fallbackMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                fallbackMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                fallbackMaterial.SetInt("_ZWrite", 0);
                fallbackMaterial.DisableKeyword("_ALPHATEST_ON");
                fallbackMaterial.EnableKeyword("_ALPHABLEND_ON");
                fallbackMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                fallbackMaterial.renderQueue = 3000;
                meshRenderer.material = fallbackMaterial;
            }

            GameObject arrowIndicator = new GameObject("ArrowIndicator");
            arrowIndicator.transform.SetParent(indicator.transform);
            arrowIndicator.transform.localPosition = new Vector3(0, 0.05f, 0.3f);
            arrowIndicator.transform.localRotation = Quaternion.identity;

            MeshRenderer arrowRenderer = arrowIndicator.AddComponent<MeshRenderer>();
            MeshFilter arrowFilter = arrowIndicator.AddComponent<MeshFilter>();
            arrowFilter.mesh = CreateArrowMesh();

            Material arrowBaseMaterial = GetCableMaterial();
            if (arrowBaseMaterial != null)
            {
                Material arrowMaterial = new Material(arrowBaseMaterial);
                arrowMaterial.color = new Color(1f, 0f, 0f, 0.8f);
                arrowRenderer.material = arrowMaterial;
            }
            else
            {
                Material arrowMaterial = new Material(Shader.Find("Standard"));
                arrowMaterial.color = new Color(1f, 0f, 0f, 0.8f);
                arrowMaterial.SetFloat("_Metallic", 0.1f);
                arrowMaterial.SetFloat("_Glossiness", 0.9f);
                arrowRenderer.material = arrowMaterial;
            }
        }

                private Mesh CreateOriginIndicatorMesh()
        {
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[8];
            float radius = 0.5f;
            float height = 0.02f;

            vertices[0] = new Vector3(-radius, 0, -radius);
            vertices[1] = new Vector3(radius, 0, -radius);
            vertices[2] = new Vector3(radius, 0, radius);
            vertices[3] = new Vector3(-radius, 0, radius);
            vertices[4] = new Vector3(-radius, height, -radius);
            vertices[5] = new Vector3(radius, height, -radius);
            vertices[6] = new Vector3(radius, height, radius);
            vertices[7] = new Vector3(-radius, height, radius);

            mesh.vertices = vertices;

            int[] triangles = new int[36];

            triangles[0] = 0; triangles[1] = 2; triangles[2] = 1;
            triangles[3] = 0; triangles[4] = 3; triangles[5] = 2;

            triangles[6] = 4; triangles[7] = 5; triangles[8] = 6;
            triangles[9] = 4; triangles[10] = 6; triangles[11] = 7;

            triangles[12] = 0; triangles[13] = 1; triangles[14] = 5;
            triangles[15] = 0; triangles[16] = 5; triangles[17] = 4;

            triangles[18] = 1; triangles[19] = 2; triangles[20] = 6;
            triangles[21] = 1; triangles[22] = 6; triangles[23] = 5;

            triangles[24] = 2; triangles[25] = 3; triangles[26] = 7;
            triangles[27] = 2; triangles[28] = 7; triangles[29] = 6;

            triangles[30] = 3; triangles[31] = 0; triangles[32] = 4;
            triangles[33] = 3; triangles[34] = 4; triangles[35] = 7;

            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }

        private Mesh CreateArrowMesh()
        {
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[7];
            vertices[0] = new Vector3(0, 0, 0.2f);
            vertices[1] = new Vector3(-0.1f, 0, 0);
            vertices[2] = new Vector3(-0.05f, 0, 0);
            vertices[3] = new Vector3(-0.05f, 0, -0.15f);
            vertices[4] = new Vector3(0.05f, 0, -0.15f);
            vertices[5] = new Vector3(0.05f, 0, 0);
            vertices[6] = new Vector3(0.1f, 0, 0);

            mesh.vertices = vertices;

            int[] triangles = new int[15];
            triangles[0] = 0; triangles[1] = 1; triangles[2] = 2;
            triangles[3] = 0; triangles[4] = 2; triangles[5] = 3;
            triangles[6] = 0; triangles[7] = 3; triangles[8] = 4;
            triangles[9] = 0; triangles[10] = 4; triangles[11] = 5;
            triangles[12] = 0; triangles[13] = 5; triangles[14] = 6;

            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }

        [ContextMenu("👁️ Setup Furniture Visibility (DEPRECATED)")]
        public void SetupFurnitureVisibility()
        {
            Debug.LogWarning("⚠️ SetupFurnitureVisibility is DEPRECATED!");
            Debug.LogWarning("⚠️ The new FurnitureSpawner system doesn't need FurnitureVisibilityController components.");
            Debug.LogWarning("⚠️ Use the cleanup tool: VRProject → 🧹 Remove All FurnitureVisibilityController Components");
        }

        [ContextMenu("🐛 Debug Menu System")]
        public void DebugMenuSystem()
        {
            Debug.Log("🐛 === MENU SYSTEM DEBUG ===");

            GameObject canvas = GameObject.Find("MenuCanvas");
            Debug.Log($"Canvas found: {canvas != null}");

            GameObject mainPanel = GameObject.Find("MainMenuPanel");
            GameObject modelPanel = GameObject.Find("ModelSelectionPanel");
            GameObject gameplayPanel = GameObject.Find("GameplayPanel");

            Debug.Log($"MainMenuPanel: {mainPanel != null} (Active: {mainPanel?.activeSelf})");
            Debug.Log($"ModelSelectionPanel: {modelPanel != null} (Active: {modelPanel?.activeSelf})");
            Debug.Log($"GameplayPanel: {gameplayPanel != null} (Active: {gameplayPanel?.activeSelf})");

            FurnitureSelectionManager manager = FindObjectOfType<FurnitureSelectionManager>();
            if (manager != null)
            {
                Debug.Log($"FurnitureSelectionManager found");
                Debug.Log($"Current state: {manager.GetType().GetField("currentState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(manager)}");

                GameObject[] allFurniture = GameObject.FindObjectsOfType<GameObject>().Where(obj => obj.name.StartsWith("furniture_")).ToArray();
                Debug.Log($"Total furniture objects found: {allFurniture.Length}");

                FurnitureVisibilityController[] controllers = FindObjectsOfType<FurnitureVisibilityController>();
                Debug.Log($"FurnitureVisibilityControllers found: {controllers.Length}");
            }
            else
            {
                Debug.LogError("❌ FurnitureSelectionManager not found!");
            }
        }

        [ContextMenu("📱 Create Canvas & UI")]
        public void CreateCanvas()
        {
            CreateCanvasIfNotExists();
        }

        private void CreateCanvasIfNotExists()
        {
            GameObject existingCanvas = GameObject.Find("MenuCanvas");
            if (existingCanvas != null)
            {
                Debug.Log("✅ MenuCanvas already exists, skipping creation");
                return;
            }

            GameObject canvasObj = new GameObject("MenuCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            canvasObj.AddComponent<GraphicRaycaster>();

            if (playerSpawnPoint != null)
            {
                canvasObj.transform.position = playerSpawnPoint.position + Vector3.forward * 1f + Vector3.up * 1.5f;
            }
            canvasObj.transform.localScale = Vector3.one * 0.000125f;

            RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(2.5f, 1.875f);

            CreateUIPanels(canvasObj.transform);
        }

        private void CreateUIPanels(Transform canvasParent)
        {
            GameObject mainPanel = CreatePanel("MainMenuPanel", canvasParent);
            CreateMainMenuContent(mainPanel.transform);

            Debug.Log("✅ Simplified UI - Only main menu with 3 direct furniture buttons");
        }

        private GameObject CreatePanel(string name, Transform parent)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent);

            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            panel.AddComponent<CanvasGroup>();
            return panel;
        }

        private void AddUIAnimator(GameObject obj)
        {
            UIAnimator animator = obj.AddComponent<UIAnimator>();
            animator.animationDuration = 0.3f;
            animator.startScale = Vector3.zero;
            animator.targetScale = Vector3.one;
        }

        private void CreateMainMenuContent(Transform parent)
        {
            CreateText("MuebleBuilder", parent, new Vector2(0, 0.6f), 0.15f);
            CreatePhysicalButton("Mueble1Button", "MUEBLE 1", parent, new Vector2(0, 0.25f), new Vector2(0.6f, 0.15f));
            CreatePhysicalButton("Mueble2Button", "MUEBLE 2", parent, new Vector2(0, 0.05f), new Vector2(0.6f, 0.15f));
            CreatePhysicalButton("Mueble3Button", "MUEBLE 3", parent, new Vector2(0, -0.15f), new Vector2(0.6f, 0.15f));
            CreatePhysicalButton("SalirButton", "SALIR", parent, new Vector2(0, -0.35f), new Vector2(0.6f, 0.15f));
        }



                private GameObject CreatePhysicalButton(string name, string text, Transform parent, Vector2 position, Vector2 size)
        {
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent);

            RectTransform rect = buttonObj.AddComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            MeshRenderer meshRenderer = buttonObj.AddComponent<MeshRenderer>();
            MeshFilter meshFilter = buttonObj.AddComponent<MeshFilter>();
            meshFilter.mesh = CreateRectangleMesh(size.x, size.y, 0.05f);

            meshRenderer.material = GetCableMaterial();

                        Button button = buttonObj.AddComponent<Button>();

            BoxCollider collider = buttonObj.AddComponent<BoxCollider>();
            collider.size = new Vector3(size.x, size.y, 0.1f);

            PhysicalButton physButton = buttonObj.AddComponent<PhysicalButton>();

            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform);

            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            textRect.anchoredPosition3D = new Vector3(0, 0, -0.03f);

            TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.text = text;
            textComponent.fontSize = 0.0875f;
            textComponent.color = Color.white;
            textComponent.alignment = TextAlignmentOptions.Center;

            return buttonObj;
        }

        private Mesh CreateRectangleMesh(float width, float height, float depth)
        {
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[24];

            float halfWidth = width * 0.5f;
            float halfHeight = height * 0.5f;
            float halfDepth = depth * 0.5f;

            vertices[0] = new Vector3(-halfWidth, -halfHeight, -halfDepth);
            vertices[1] = new Vector3(halfWidth, -halfHeight, -halfDepth);
            vertices[2] = new Vector3(halfWidth, halfHeight, -halfDepth);
            vertices[3] = new Vector3(-halfWidth, halfHeight, -halfDepth);

            vertices[4] = new Vector3(-halfWidth, halfHeight, halfDepth);
            vertices[5] = new Vector3(halfWidth, halfHeight, halfDepth);
            vertices[6] = new Vector3(halfWidth, -halfHeight, halfDepth);
            vertices[7] = new Vector3(-halfWidth, -halfHeight, halfDepth);

            vertices[8] = new Vector3(-halfWidth, halfHeight, -halfDepth);
            vertices[9] = new Vector3(-halfWidth, halfHeight, halfDepth);
            vertices[10] = new Vector3(-halfWidth, -halfHeight, halfDepth);
            vertices[11] = new Vector3(-halfWidth, -halfHeight, -halfDepth);

            vertices[12] = new Vector3(halfWidth, -halfHeight, -halfDepth);
            vertices[13] = new Vector3(halfWidth, -halfHeight, halfDepth);
            vertices[14] = new Vector3(halfWidth, halfHeight, halfDepth);
            vertices[15] = new Vector3(halfWidth, halfHeight, -halfDepth);

            vertices[16] = new Vector3(-halfWidth, -halfHeight, -halfDepth);
            vertices[17] = new Vector3(-halfWidth, -halfHeight, halfDepth);
            vertices[18] = new Vector3(halfWidth, -halfHeight, halfDepth);
            vertices[19] = new Vector3(halfWidth, -halfHeight, -halfDepth);

            vertices[20] = new Vector3(halfWidth, halfHeight, -halfDepth);
            vertices[21] = new Vector3(halfWidth, halfHeight, halfDepth);
            vertices[22] = new Vector3(-halfWidth, halfHeight, halfDepth);
            vertices[23] = new Vector3(-halfWidth, halfHeight, -halfDepth);

            mesh.vertices = vertices;

            int[] triangles = new int[36];

            triangles[0] = 0; triangles[1] = 2; triangles[2] = 1;
            triangles[3] = 0; triangles[4] = 3; triangles[5] = 2;

            triangles[6] = 4; triangles[7] = 5; triangles[8] = 6;
            triangles[9] = 4; triangles[10] = 6; triangles[11] = 7;

            triangles[12] = 8; triangles[13] = 9; triangles[14] = 10;
            triangles[15] = 8; triangles[16] = 10; triangles[17] = 11;

            triangles[18] = 12; triangles[19] = 14; triangles[20] = 13;
            triangles[21] = 12; triangles[22] = 15; triangles[23] = 14;

            triangles[24] = 16; triangles[25] = 17; triangles[26] = 18;
            triangles[27] = 16; triangles[28] = 18; triangles[29] = 19;

            triangles[30] = 20; triangles[31] = 22; triangles[32] = 21;
            triangles[33] = 20; triangles[34] = 23; triangles[35] = 22;

            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }

        private void CreateText(string text, Transform parent, Vector2 position, float fontSize)
        {
            GameObject textObj = new GameObject("Text_" + text.Replace(" ", ""));
            textObj.transform.SetParent(parent);

            RectTransform rect = textObj.AddComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(1.25f, 0.25f);

            TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.text = text;
            textComponent.fontSize = fontSize;
            textComponent.color = Color.white;
            textComponent.alignment = TextAlignmentOptions.Center;
        }



                [ContextMenu("🎮 Create Menu Manager")]
        public void CreateMenuManager()
        {
            CreateMenuManagerIfNotExists();
        }

        private void CreateMenuManagerIfNotExists()
        {
            GameObject existingManager = GameObject.Find("MenuManager");
            if (existingManager != null)
            {
                Debug.Log("✅ MenuManager already exists, checking components...");

                FurnitureSpawner existingSpawner = existingManager.GetComponent<FurnitureSpawner>();
                SimpleFurnitureSelector existingSelector = existingManager.GetComponent<SimpleFurnitureSelector>();

                if (existingSpawner == null)
                {
                    Debug.Log("Adding missing FurnitureSpawner component");
                    existingSpawner = existingManager.AddComponent<FurnitureSpawner>();
                }

                if (existingSelector == null)
                {
                    Debug.Log("Adding missing SimpleFurnitureSelector component");
                    existingSelector = existingManager.AddComponent<SimpleFurnitureSelector>();
                    existingSelector.furnitureSpawner = existingSpawner;
                    StartCoroutine(InitializeSimpleSelector(existingSelector));
                }
                else if (existingSelector.furnitureSpawner == null)
                {
                    existingSelector.furnitureSpawner = existingSpawner;
                }

                return;
            }

            GameObject managerObj = new GameObject("MenuManager");

            FurnitureSpawner spawner = managerObj.AddComponent<FurnitureSpawner>();
            SimpleFurnitureSelector simpleSelector = managerObj.AddComponent<SimpleFurnitureSelector>();

            simpleSelector.furnitureSpawner = spawner;

            StartCoroutine(InitializeSimpleSelector(simpleSelector));

            Debug.Log("✅ Created FurnitureSpawner and SimpleFurnitureSelector - Direct 3-button furniture selection");
        }

        private System.Collections.IEnumerator InitializeSimpleSelector(SimpleFurnitureSelector selector)
        {
            yield return new WaitForSeconds(0.3f);

            Debug.Log("🔄 Force initializing SimpleFurnitureSelector after delay...");
            selector.ForceInitialize();
            Debug.Log("✅ SimpleFurnitureSelector force initialization completed");
        }





        [ContextMenu("🔴 Create Restart Button")]
        public void CreateRestartButton()
        {
            CreateRestartButtonIfNotExists();
        }

                private void CreateRestartButtonIfNotExists()
        {
            Debug.Log("🔍 Checking for existing RestartButtonParent...");

            GameObject existingRestart = GameObject.Find("RestartButtonParent");
            if (existingRestart == null)
            {
                GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
                foreach (GameObject obj in allObjects)
                {
                    if (obj.name == "RestartButtonParent")
                    {
                        existingRestart = obj;
                        break;
                    }
                }
            }

            if (existingRestart != null)
            {
                Debug.Log($"✅ RestartButtonParent already exists: {existingRestart.name} (Active: {existingRestart.activeInHierarchy})");

                RestartButtonSetup existingSetup = existingRestart.GetComponent<RestartButtonSetup>();
                if (existingSetup == null)
                {
                    Debug.Log("🔧 Adding missing RestartButtonSetup component to existing RestartButtonParent");
                    existingSetup = existingRestart.AddComponent<RestartButtonSetup>();
                }
                else
                {
                    Debug.Log("✅ RestartButtonSetup component already exists on RestartButtonParent");
                }

                SimpleFurnitureSelector existingSelector = FindObjectOfType<SimpleFurnitureSelector>();
                if (existingSelector != null)
                {
                    if (existingSelector.restartButtonSetup == null)
                    {
                        existingSelector.restartButtonSetup = existingSetup;
                        Debug.Log("🔗 Reconnected restart button to SimpleFurnitureSelector");
                    }
                    else
                    {
                        Debug.Log("✅ SimpleFurnitureSelector already has restart button reference");
                    }
                }

                Debug.Log("✅ RestartButtonParent validation completed - SKIPPING CREATION");
                return;
            }

            Debug.Log("🔄 No existing RestartButtonParent found, creating new one...");

            GameObject restartParent = new GameObject("RestartButtonParent");

            if (playerSpawnPoint != null)
            {
                restartParent.transform.position = playerSpawnPoint.position + Vector3.right * 1.5f + Vector3.up * 1.2f;
            }

            RestartButtonSetup setup = restartParent.AddComponent<RestartButtonSetup>();
            restartParent.SetActive(false);

            SimpleFurnitureSelector foundSelector = FindObjectOfType<SimpleFurnitureSelector>();
            if (foundSelector != null)
            {
                foundSelector.restartButtonSetup = setup;
                Debug.Log("🔴 NEW RestartButtonParent created and connected to SimpleFurnitureSelector!");
            }
            else
            {
                Debug.LogWarning("⚠️ Could not find SimpleFurnitureSelector to connect restart button");
            }
        }



        [ContextMenu("🗑️ Clear All")]
        public void ClearAll()
        {
            GameObject[] toDestroy = {
                GameObject.Find("MenuCanvas"),
                GameObject.Find("MenuManager"),
                GameObject.Find("RestartButtonParent")
            };

            foreach (GameObject obj in toDestroy)
            {
                if (obj != null)
                {
                    if (Application.isPlaying)
                        Destroy(obj);
                    else
                        DestroyImmediate(obj);
                }
            }

            Debug.Log("🗑️ All menu objects cleared");
        }

        private void ValidateCompleteSetup()
        {
            Debug.Log("🔍🔍🔍 VALIDATING COMPLETE SETUP 🔍🔍🔍");

            bool isValid = true;
            int errorCount = 0;

            GameObject canvas = GameObject.Find("MenuCanvas");
            if (canvas == null) {
                Debug.LogError("❌ RED X RED - MenuCanvas not found!");
                isValid = false;
                errorCount++;
            }
            else {
                Debug.Log("✅ MenuCanvas found");

                GameObject mainPanel = GameObject.Find("MainMenuPanel");

                if (mainPanel == null) { Debug.LogError("❌ RED X RED - MainMenuPanel not found!"); errorCount++; }
                else { Debug.Log("✅ MainMenuPanel found"); }
            }

            GameObject manager = GameObject.Find("MenuManager");
            if (manager == null) {
                Debug.LogError("❌ RED X RED - MenuManager not found!");
                isValid = false;
                errorCount++;
            }
            else {
                Debug.Log("✅ MenuManager found");
            }

            SimpleFurnitureSelector simpleSelector = FindObjectOfType<SimpleFurnitureSelector>();
            if (simpleSelector == null) {
                Debug.LogError("❌ RED X RED - SimpleFurnitureSelector component not found!");
                isValid = false;
                errorCount++;
            }
            else {
                Debug.Log("✅ SimpleFurnitureSelector component found");

                Debug.Log("🔍 Checking button references:");
                Debug.Log($"   - mueble1Button: {(simpleSelector.mueble1Button != null ? "✅" : "❌ RED X RED")}");
                Debug.Log($"   - mueble2Button: {(simpleSelector.mueble2Button != null ? "✅" : "❌ RED X RED")}");
                Debug.Log($"   - mueble3Button: {(simpleSelector.mueble3Button != null ? "✅" : "❌ RED X RED")}");
                Debug.Log($"   - salirButton: {(simpleSelector.salirButton != null ? "✅" : "❌ RED X RED")}");
                Debug.Log($"   - restartButtonSetup: {(simpleSelector.restartButtonSetup != null ? "✅" : "❌ RED X RED")}");

                if (simpleSelector.mueble1Button == null || simpleSelector.mueble2Button == null ||
                    simpleSelector.mueble3Button == null || simpleSelector.salirButton == null)
                {
                    Debug.LogWarning("⚠️ Some buttons missing, trying to force initialize...");
                    simpleSelector.ForceInitialize();

                    Debug.Log("🔍 Re-checking after force initialization:");
                    Debug.Log($"   - mueble1Button: {(simpleSelector.mueble1Button != null ? "✅" : "❌ RED X RED")}");
                    Debug.Log($"   - mueble2Button: {(simpleSelector.mueble2Button != null ? "✅" : "❌ RED X RED")}");
                    Debug.Log($"   - mueble3Button: {(simpleSelector.mueble3Button != null ? "✅" : "❌ RED X RED")}");
                    Debug.Log($"   - salirButton: {(simpleSelector.salirButton != null ? "✅" : "❌ RED X RED")}");
                }

                if (simpleSelector.mueble1Button == null) errorCount++;
                if (simpleSelector.mueble2Button == null) errorCount++;
                if (simpleSelector.mueble3Button == null) errorCount++;
                if (simpleSelector.salirButton == null) errorCount++;
            }

            GameObject originPoint = GameObject.Find("RecenterOriginPoint");
            if (originPoint == null) {
                Debug.LogError("❌ RED X RED - RecenterOriginPoint not found!");
                errorCount++;
            }
            else {
                Debug.Log("✅ RecenterOriginPoint found");
            }

            FurnitureVisibilityController[] controllers = FindObjectsOfType<FurnitureVisibilityController>();
            Debug.Log($"🪑 Found {controllers.Length} FurnitureVisibilityControllers");

            if (errorCount == 0) {
                Debug.Log("🎉🎉🎉 VALIDATION PASSED - ALL SYSTEMS READY! 🎉🎉🎉");
            }
            else {
                Debug.LogError($"❌ RED X RED - VALIDATION FAILED! {errorCount} errors found!");
            }
        }

        [ContextMenu("🔍 Validate Setup")]
        public void ValidateSetup()
        {
            ValidateCompleteSetup();
        }

        [ContextMenu("🔧 Fix Validation Issues")]
        public void FixValidationIssues()
        {
            Debug.Log("🔧 ATTEMPTING TO FIX VALIDATION ISSUES...");

            SimpleFurnitureSelector selector = FindObjectOfType<SimpleFurnitureSelector>();
            if (selector != null)
            {
                Debug.Log("🔄 Force initializing SimpleFurnitureSelector...");
                selector.ForceInitialize();
            }

            RestartButtonSetup restartSetup = FindObjectOfType<RestartButtonSetup>();
            if (restartSetup != null && selector != null)
            {
                Debug.Log("🔗 Reconnecting restart button to selector...");
                selector.restartButtonSetup = restartSetup;
            }

            Debug.Log("🔍 Re-validating after fixes...");
            ValidateCompleteSetup();
        }

        [ContextMenu("✨ Create Beautiful Instruction Panel")]
        public void CreateInstructionPanel()
        {
            CreateInstructionPanelIfNotExists();
        }

                private void CreateInstructionPanelIfNotExists()
        {
            Debug.Log("🔍 Checking for existing InstructionPanel...");

            GameObject existingPanel = GameObject.Find("InstructionPanel");
            if (existingPanel == null)
            {
                GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
                foreach (GameObject obj in allObjects)
                {
                    if (obj.name == "InstructionPanel")
                    {
                        existingPanel = obj;
                        break;
                    }
                }
            }

            if (existingPanel != null)
            {
                Debug.Log($"✅ InstructionPanel already exists: {existingPanel.name} (Active: {existingPanel.activeInHierarchy})");

                InstructionPanelController existingController = existingPanel.GetComponent<InstructionPanelController>();
                if (existingController == null)
                {
                    Debug.Log("🔧 Adding missing InstructionPanelController component to existing InstructionPanel");
                    existingController = existingPanel.AddComponent<InstructionPanelController>();
                    existingController.instructionDatabase = this.instructionDatabase;
                }
                else
                {
                    Debug.Log("✅ InstructionPanelController already exists, updating database");
                    existingController.instructionDatabase = this.instructionDatabase;
                }

                Debug.Log("✅ InstructionPanel validation completed - SKIPPING CREATION");
                return;
            }

            Debug.Log("🔄 No existing InstructionPanel found, creating new one...");

            GameObject panelRoot = new GameObject("InstructionPanel");

            if (playerSpawnPoint != null)
            {
                panelRoot.transform.position = playerSpawnPoint.position + Vector3.forward * 0.8f + Vector3.up * 1.8f;
            }
            else
            {
                panelRoot.transform.position = new Vector3(0, 1.8f, 0.8f);
            }

            Canvas canvas = panelRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 100;

            CanvasScaler scaler = panelRoot.AddComponent<CanvasScaler>();
            scaler.scaleFactor = 0.001f;
            scaler.dynamicPixelsPerUnit = 10f;

            GraphicRaycaster raycaster = panelRoot.AddComponent<GraphicRaycaster>();

            RectTransform canvasRect = panelRoot.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(400, 100);

            GameObject backgroundPanel = CreateRoundedPanel("BackgroundPanel", panelRoot.transform);
            backgroundPanel.transform.localPosition = Vector3.zero;

            RectTransform bgRect = backgroundPanel.GetComponent<RectTransform>();
            bgRect.sizeDelta = new Vector2(400, 100);

            Image bgImage = backgroundPanel.GetComponent<Image>();
            bgImage.color = new Color(0.05f, 0.05f, 0.1f, 0.85f);

            GameObject textContainer = new GameObject("TextContainer");
            textContainer.transform.SetParent(backgroundPanel.transform);

            RectTransform textRect = textContainer.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(40, 20);
            textRect.offsetMax = new Vector2(-40, -20);

            TextMeshProUGUI instructionText = textContainer.AddComponent<TextMeshProUGUI>();
            instructionText.text = "";
            instructionText.fontSize = 28;
            instructionText.color = new Color(1f, 1f, 1f, 0.9f);
            instructionText.alignment = TextAlignmentOptions.Center;
            instructionText.fontStyle = FontStyles.Bold;
            instructionText.enableWordWrapping = true;

            CanvasGroup canvasGroup = backgroundPanel.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;

            InstructionPanelController controller = panelRoot.AddComponent<InstructionPanelController>();
            controller.instructionText = instructionText;
            controller.canvasGroup = canvasGroup;
            controller.instructionDatabase = this.instructionDatabase;

            panelRoot.SetActive(false);

            Debug.Log("✨ Beautiful instruction panel created with fade animations!");
        }

        private GameObject CreateRoundedPanel(string name, Transform parent)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent);

            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image image = panel.AddComponent<Image>();

            Texture2D roundedTexture = CreateRoundedRectTexture(400, 100, 25);
            Sprite roundedSprite = Sprite.Create(roundedTexture, new Rect(0, 0, roundedTexture.width, roundedTexture.height), new Vector2(0.5f, 0.5f));

            image.sprite = roundedSprite;
            image.type = Image.Type.Sliced;

            return panel;
        }

        private Texture2D CreateRoundedRectTexture(int width, int height, int radius)
        {
            Texture2D texture = new Texture2D(width, height);
            Color[] pixels = new Color[width * height];

            Vector2 center = new Vector2(width * 0.5f, height * 0.5f);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float alpha = 1f;

                    if (x < radius && y < radius)
                    {
                        float distance = Vector2.Distance(new Vector2(x, y), new Vector2(radius, radius));
                        alpha = distance <= radius ? 1f : 0f;
                    }
                    else if (x >= width - radius && y < radius)
                    {
                        float distance = Vector2.Distance(new Vector2(x, y), new Vector2(width - radius, radius));
                        alpha = distance <= radius ? 1f : 0f;
                    }
                    else if (x < radius && y >= height - radius)
                    {
                        float distance = Vector2.Distance(new Vector2(x, y), new Vector2(radius, height - radius));
                        alpha = distance <= radius ? 1f : 0f;
                    }
                    else if (x >= width - radius && y >= height - radius)
                    {
                        float distance = Vector2.Distance(new Vector2(x, y), new Vector2(width - radius, height - radius));
                        alpha = distance <= radius ? 1f : 0f;
                    }

                    pixels[y * width + x] = new Color(1f, 1f, 1f, alpha);
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }

        [ContextMenu("🎯 Add Furniture Grab Detectors")]
        public void AddFurnitureGrabDetectors()
        {
            GameObject[] allFurniture = GameObject.FindObjectsOfType<GameObject>()
                .Where(obj => obj.name.StartsWith("furniture_"))
                .ToArray();

            Debug.Log($"🔍 Found {allFurniture.Length} furniture objects to add grab detectors to");

            InstructionPanelController instructionController = FindObjectOfType<InstructionPanelController>();

            int addedCount = 0;
            int skippedCount = 0;

            foreach (GameObject furniture in allFurniture)
            {
                FurnitureGrabDetector existingDetector = furniture.GetComponent<FurnitureGrabDetector>();
                if (existingDetector != null)
                {
                    Debug.Log($"✅ FurnitureGrabDetector already exists on {furniture.name}, skipping");

                    if (existingDetector.instructionController == null && instructionController != null)
                    {
                        existingDetector.instructionController = instructionController;
                        Debug.Log($"🔗 Connected instruction controller to existing detector on {furniture.name}");
                    }
                    skippedCount++;
                    continue;
                }

                FurnitureGrabDetector detector = furniture.AddComponent<FurnitureGrabDetector>();
                detector.instructionController = instructionController;

                BoxCollider existingCollider = furniture.GetComponent<BoxCollider>();
                if (existingCollider == null)
                {
                    BoxCollider collider = furniture.AddComponent<BoxCollider>();

                    Renderer renderer = furniture.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        collider.size = renderer.bounds.size;
                        collider.center = renderer.bounds.center - furniture.transform.position;
                    }
                    else
                    {
                        collider.size = Vector3.one * 0.1f;
                    }
                    Debug.Log($"🔧 Added BoxCollider to {furniture.name}");
                }

                Button buttonComponent = furniture.GetComponent<Button>();
                if (buttonComponent == null)
                {
                    buttonComponent = furniture.AddComponent<Button>();
                    buttonComponent.onClick.AddListener(detector.OnFurnitureGrabbed);
                    Debug.Log($"🔧 Added Button component to {furniture.name}");
                }

                Debug.Log($"✅ Added FurnitureGrabDetector to {furniture.name}");
                addedCount++;
            }

            Debug.Log($"🎯 Furniture grab detector setup completed!");
            Debug.Log($"   - Added detectors: {addedCount}");
            Debug.Log($"   - Skipped (already had): {skippedCount}");
            Debug.Log($"   - Total furniture objects: {allFurniture.Length}");

            if (instructionController == null)
            {
                Debug.LogWarning("⚠️ No InstructionPanelController found! Create the instruction panel first.");
            }
        }
    }
}