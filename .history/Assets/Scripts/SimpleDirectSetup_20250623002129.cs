using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

namespace VRProject
{
    public class SimpleDirectSetup : MonoBehaviour
    {
        [ContextMenu("🚀 CREATE EVERYTHING NOW")]
        public void CreateEverythingNow()
        {
            Debug.Log("🚀 Creating everything directly...");

            CreateMenuManager();
            CreateMenuCanvas();
            CreateRestartButton();
            CreateRecenterPoint();
            AddFurnitureComponents();
            WireEverything();

            Debug.Log("✅ Everything created successfully!");
        }

        private void CreateMenuManager()
        {
            Debug.Log("🎮 Creating MenuManager...");

            GameObject existing = GameObject.Find("MenuManager");
            if (existing != null)
            {
                DestroyImmediate(existing);
            }

            GameObject manager = new GameObject("MenuManager");

            FurnitureSpawner spawner = manager.AddComponent<FurnitureSpawner>();
            SimpleFurnitureSelector selector = manager.AddComponent<SimpleFurnitureSelector>();

            selector.furnitureSpawner = spawner;

            Debug.Log("✅ MenuManager created");
        }

        private void CreateMenuCanvas()
        {
            Debug.Log("📱 Creating MenuCanvas...");

            GameObject existing = GameObject.Find("MenuCanvas");
            if (existing != null)
            {
                DestroyImmediate(existing);
            }

            GameObject canvasObj = new GameObject("MenuCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            GraphicRaycaster raycaster = canvasObj.AddComponent<GraphicRaycaster>();

            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;

            canvasObj.transform.position = new Vector3(0, 1.5f, 2f);
            canvasObj.transform.localScale = Vector3.one * 0.00025f;

            RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(1920, 1080);

            CreateMenuButtons(canvasObj.transform);

            Debug.Log("✅ MenuCanvas created");
        }

        private void CreateMenuButtons(Transform parent)
        {
            Debug.Log("🔘 Creating menu buttons...");

            GameObject mainPanel = new GameObject("MainMenuPanel");
            mainPanel.transform.SetParent(parent);

            RectTransform panelRect = mainPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            CreatePhysicalButton("Mueble1Button", "MUEBLE 1", mainPanel.transform, new Vector2(0, 0.25f));
            CreatePhysicalButton("Mueble2Button", "MUEBLE 2", mainPanel.transform, new Vector2(0, 0.05f));
            CreatePhysicalButton("Mueble3Button", "MUEBLE 3", mainPanel.transform, new Vector2(0, -0.15f));
            CreatePhysicalButton("SalirButton", "SALIR", mainPanel.transform, new Vector2(0, -0.35f));

            Debug.Log("✅ Menu buttons created");
        }

        private void CreatePhysicalButton(string name, string text, Transform parent, Vector2 position)
        {
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent);

            RectTransform rect = buttonObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position * 1000f;
            rect.sizeDelta = new Vector2(600f, 150f);

            MeshFilter meshFilter = buttonObj.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = buttonObj.AddComponent<MeshRenderer>();
            BoxCollider boxCollider = buttonObj.AddComponent<BoxCollider>();

            meshFilter.mesh = CreateButtonMesh();

            Material buttonMaterial = GetCableMaterial();
            if (buttonMaterial != null)
            {
                meshRenderer.material = buttonMaterial;
            }
            else
            {
                Material defaultMat = new Material(Shader.Find("Standard"));
                defaultMat.color = new Color(0.2f, 0.2f, 0.2f, 1f);
                meshRenderer.material = defaultMat;
            }

            Button unityButton = buttonObj.AddComponent<Button>();
            XRSimpleInteractable interactable = buttonObj.AddComponent<XRSimpleInteractable>();
            PhysicalButton physicalButton = buttonObj.AddComponent<PhysicalButton>();

            CreateButtonText(text, buttonObj.transform);
        }

        private Mesh CreateButtonMesh()
        {
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[8];
            vertices[0] = new Vector3(-0.3f, -0.075f, -0.01f);
            vertices[1] = new Vector3(0.3f, -0.075f, -0.01f);
            vertices[2] = new Vector3(0.3f, 0.075f, -0.01f);
            vertices[3] = new Vector3(-0.3f, 0.075f, -0.01f);
            vertices[4] = new Vector3(-0.3f, -0.075f, 0.01f);
            vertices[5] = new Vector3(0.3f, -0.075f, 0.01f);
            vertices[6] = new Vector3(0.3f, 0.075f, 0.01f);
            vertices[7] = new Vector3(-0.3f, 0.075f, 0.01f);

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

        private void CreateButtonText(string text, Transform parent)
        {
            GameObject textObj = new GameObject("ButtonText");
            textObj.transform.SetParent(parent);

            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            Text textComponent = textObj.AddComponent<Text>();
            textComponent.text = text;
            textComponent.fontSize = 100;
            textComponent.color = Color.white;
            textComponent.alignment = TextAnchor.MiddleCenter;
            textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        private void CreateRestartButton()
        {
            Debug.Log("🔴 Creating restart button...");

            GameObject existing = GameObject.Find("RestartButtonParent");
            if (existing != null)
            {
                DestroyImmediate(existing);
            }

            GameObject restartParent = new GameObject("RestartButtonParent");
            restartParent.transform.position = new Vector3(1f, 1f, 0f);

            RestartButtonSetup restartSetup = restartParent.AddComponent<RestartButtonSetup>();

            Debug.Log("✅ Restart button created");
        }

        private void CreateRecenterPoint()
        {
            Debug.Log("🎯 Creating recenter point...");

            GameObject existing = GameObject.Find("RecenterOriginPoint");
            if (existing != null)
            {
                DestroyImmediate(existing);
            }

            GameObject originPoint = new GameObject("RecenterOriginPoint");
            originPoint.transform.position = Vector3.zero;
            originPoint.transform.rotation = Quaternion.Euler(0, 180, 0);

            RecenterOriginPoint recenterScript = originPoint.AddComponent<RecenterOriginPoint>();

            CreateOriginVisualIndicator(originPoint.transform);

            Debug.Log("✅ Recenter point created");
        }

        private void CreateOriginVisualIndicator(Transform parent)
        {
            GameObject indicator = new GameObject("OriginIndicator");
            indicator.transform.SetParent(parent);
            indicator.transform.localPosition = Vector3.zero;

            MeshRenderer meshRenderer = indicator.AddComponent<MeshRenderer>();
            MeshFilter meshFilter = indicator.AddComponent<MeshFilter>();

            meshFilter.mesh = CreatePlatformMesh();

            Material platformMaterial = GetCableMaterial();
            if (platformMaterial != null)
            {
                Material transparentMaterial = new Material(platformMaterial);
                transparentMaterial.color = new Color(0f, 1f, 0f, 0.7f);
                meshRenderer.material = transparentMaterial;
            }
            else
            {
                Material defaultMat = new Material(Shader.Find("Standard"));
                defaultMat.color = new Color(0f, 1f, 0f, 0.7f);
                meshRenderer.material = defaultMat;
            }

            GameObject arrow = new GameObject("Arrow");
            arrow.transform.SetParent(indicator.transform);
            arrow.transform.localPosition = new Vector3(0, 0.05f, 0.3f);

            MeshRenderer arrowRenderer = arrow.AddComponent<MeshRenderer>();
            MeshFilter arrowFilter = arrow.AddComponent<MeshFilter>();
            arrowFilter.mesh = CreateArrowMesh();

            Material arrowMaterial = new Material(Shader.Find("Standard"));
            arrowMaterial.color = new Color(1f, 0f, 0f, 0.8f);
            arrowRenderer.material = arrowMaterial;
        }

        private Mesh CreatePlatformMesh()
        {
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[8];
            vertices[0] = new Vector3(-0.5f, 0, -0.5f);
            vertices[1] = new Vector3(0.5f, 0, -0.5f);
            vertices[2] = new Vector3(0.5f, 0, 0.5f);
            vertices[3] = new Vector3(-0.5f, 0, 0.5f);
            vertices[4] = new Vector3(-0.5f, 0.02f, -0.5f);
            vertices[5] = new Vector3(0.5f, 0.02f, -0.5f);
            vertices[6] = new Vector3(0.5f, 0.02f, 0.5f);
            vertices[7] = new Vector3(-0.5f, 0.02f, 0.5f);

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

        private void AddFurnitureComponents()
        {
            Debug.Log("🪑 Adding components to existing furniture...");

            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            int furnitureCount = 0;

            foreach (GameObject obj in allObjects)
            {
                if (obj.name.StartsWith("furniture_"))
                {
                    FurnitureVisibilityController controller = obj.GetComponent<FurnitureVisibilityController>();
                    if (controller == null)
                    {
                        controller = obj.AddComponent<FurnitureVisibilityController>();
                        furnitureCount++;
                        Debug.Log($"   ✅ Added controller to: {obj.name}");
                    }
                }
            }

            Debug.Log($"✅ Added components to {furnitureCount} furniture pieces");
        }

        private void WireEverything()
        {
            Debug.Log("🔌 Wiring everything together...");

            SimpleFurnitureSelector selector = FindObjectOfType<SimpleFurnitureSelector>();
            if (selector == null)
            {
                Debug.LogError("❌ SimpleFurnitureSelector not found!");
                return;
            }

            selector.menuCanvas = GameObject.Find("MenuCanvas");
            selector.restartButtonSetup = FindObjectOfType<RestartButtonSetup>();

            Button[] buttons = FindObjectsOfType<Button>();
            foreach (Button button in buttons)
            {
                string buttonName = button.name.ToLower();

                if (buttonName.Contains("mueble1"))
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => selector.SelectMueble1());
                    Debug.Log($"   🔗 Wired: {button.name} → SelectMueble1()");
                }
                else if (buttonName.Contains("mueble2"))
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => selector.SelectMueble2());
                    Debug.Log($"   🔗 Wired: {button.name} → SelectMueble2()");
                }
                else if (buttonName.Contains("mueble3"))
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => selector.SelectMueble3());
                    Debug.Log($"   🔗 Wired: {button.name} → SelectMueble3()");
                }
                else if (buttonName.Contains("salir"))
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => selector.ExitApplicationPublic());
                    Debug.Log($"   🔗 Wired: {button.name} → ExitApplicationPublic()");
                }
            }

            selector.ForceInitialize();

            Debug.Log("✅ Everything wired successfully!");
        }

        private Material GetCableMaterial()
        {
            #if UNITY_EDITOR
            string[] guids = UnityEditor.AssetDatabase.FindAssets("Cable t:Material");
            if (guids.Length > 0)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                return UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(path);
            }
            #endif
            return null;
        }
    }
}