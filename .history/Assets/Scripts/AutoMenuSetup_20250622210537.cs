using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace VRProject
{
    public class AutoMenuSetup : MonoBehaviour
    {
        [Header("Auto Setup")]
        public Transform playerSpawnPoint;

                        [ContextMenu("🚀 CREATE COMPLETE MENU SYSTEM")]
        public void CreateCompleteMenuSystem()
        {
            if (playerSpawnPoint == null)
            {
                FindPlayerSpawnPoint();
            }

            CreateOriginPoint();
            SetupFurnitureVisibility();
            CreateCanvas();
            CreateMenuManager();
            CreateRestartButton();
            SetupFurnitureSelection();
            ConnectButtonEvents();
            Debug.Log("✅ Complete menu system created and fully connected!");
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
                cableMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Material/Cable.mat");
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
            GameObject existingOrigin = GameObject.Find("RecenterOriginPoint");
            if (existingOrigin != null)
            {
                if (Application.isPlaying)
                    Destroy(existingOrigin);
                else
                    DestroyImmediate(existingOrigin);
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
            originPoint.transform.rotation = originRotation;

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

            Material indicatorMaterial = new Material(Shader.Find("Standard"));
            indicatorMaterial.color = new Color(0f, 1f, 0f, 0.7f);
            indicatorMaterial.SetFloat("_Metallic", 0.1f);
            indicatorMaterial.SetFloat("_Glossiness", 0.9f);
            indicatorMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            indicatorMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            indicatorMaterial.SetInt("_ZWrite", 0);
            indicatorMaterial.DisableKeyword("_ALPHATEST_ON");
            indicatorMaterial.EnableKeyword("_ALPHABLEND_ON");
            indicatorMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            indicatorMaterial.renderQueue = 3000;

            meshRenderer.material = indicatorMaterial;

            GameObject arrowIndicator = new GameObject("ArrowIndicator");
            arrowIndicator.transform.SetParent(indicator.transform);
            arrowIndicator.transform.localPosition = new Vector3(0, 0.05f, 0.3f);
            arrowIndicator.transform.localRotation = Quaternion.identity;

            MeshRenderer arrowRenderer = arrowIndicator.AddComponent<MeshRenderer>();
            MeshFilter arrowFilter = arrowIndicator.AddComponent<MeshFilter>();
            arrowFilter.mesh = CreateArrowMesh();

            Material arrowMaterial = new Material(Shader.Find("Standard"));
            arrowMaterial.color = new Color(1f, 0f, 0f, 0.8f);
            arrowMaterial.SetFloat("_Metallic", 0.1f);
            arrowMaterial.SetFloat("_Glossiness", 0.9f);
            arrowRenderer.material = arrowMaterial;
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

        [ContextMenu("👁️ Setup Furniture Visibility")]
        public void SetupFurnitureVisibility()
        {
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            int furnitureCount = 0;
            int alreadyHasScript = 0;

            foreach (GameObject obj in allObjects)
            {
                if (obj.name.StartsWith("furniture_"))
                {
                    furnitureCount++;

                    FurnitureVisibilityController controller = obj.GetComponent<FurnitureVisibilityController>();
                    if (controller == null)
                    {
                        controller = obj.AddComponent<FurnitureVisibilityController>();
                        Debug.Log($"Added FurnitureVisibilityController to {obj.name}");
                    }
                    else
                    {
                        alreadyHasScript++;
                    }
                }
            }

            Debug.Log($"👁️ Furniture visibility setup complete! Found {furnitureCount} furniture pieces. " +
                     $"Added script to {furnitureCount - alreadyHasScript} objects, {alreadyHasScript} already had the script.");
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
            GameObject canvasObj = new GameObject("MenuCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            canvasObj.AddComponent<GraphicRaycaster>();

            if (playerSpawnPoint != null)
            {
                canvasObj.transform.position = playerSpawnPoint.position + Vector3.forward * 1f + Vector3.up * 1.5f;
            }
            canvasObj.transform.localScale = Vector3.one * 0.0005f;

            RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(2.5f, 1.875f);

            CreateUIPanels(canvasObj.transform);
        }

        private void CreateUIPanels(Transform canvasParent)
        {
            GameObject mainPanel = CreatePanel("MainMenuPanel", canvasParent);
            AddUIAnimator(mainPanel);
            CreateMainMenuContent(mainPanel.transform);

            GameObject selectionPanel = CreatePanel("ModelSelectionPanel", canvasParent);
            AddUIAnimator(selectionPanel);
            CreateModelSelectionContent(selectionPanel.transform);

            EditorVisibilityHelper editorHelper = selectionPanel.AddComponent<EditorVisibilityHelper>();
            editorHelper.hideAtRuntime = true;

            GameObject gameplayPanel = CreatePanel("GameplayPanel", canvasParent);
            AddUIAnimator(gameplayPanel);

            EditorVisibilityHelper gameplayHelper = gameplayPanel.AddComponent<EditorVisibilityHelper>();
            gameplayHelper.hideAtRuntime = true;
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
            CreateText("MuebleBuilder", parent, new Vector2(0, 0.3125f), 0.1875f);
            CreatePhysicalButton("IniciarButton", "INICIAR", parent, new Vector2(0, 0), new Vector2(0.75f, 0.25f));
            CreatePhysicalButton("SalirButton", "SALIR", parent, new Vector2(0, -0.3125f), new Vector2(0.75f, 0.25f));
        }

        private void CreateModelSelectionContent(Transform parent)
        {
            CreateText("Selecciona tu Mueble", parent, new Vector2(0, 0.75f), 0.125f);
            CreatePhysicalButton("BackButton", "◀ VOLVER", parent, new Vector2(-0.9375f, 0.75f), new Vector2(0.625f, 0.1875f));
            CreateScrollView(parent);
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

        private void CreateScrollView(Transform parent)
        {
            GameObject scrollObj = new GameObject("ModelScrollView");
            scrollObj.transform.SetParent(parent);

            RectTransform scrollRect = scrollObj.AddComponent<RectTransform>();
            scrollRect.anchoredPosition = Vector2.zero;
            scrollRect.sizeDelta = new Vector2(1.875f, 0.9375f);

            ScrollRect scroll = scrollObj.AddComponent<ScrollRect>();
            scrollObj.AddComponent<Image>().color = new Color(0, 0, 0, 0.3f);

            GameObject content = new GameObject("Content");
            content.transform.SetParent(scrollObj.transform);

            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.sizeDelta = new Vector2(0, 1.875f);

            GridLayoutGroup grid = content.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(0.375f, 0.375f);
            grid.spacing = new Vector2(0.03125f, 0.03125f);
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;

            scroll.content = contentRect;
            scroll.horizontal = false;
            scroll.vertical = true;
        }

        [ContextMenu("🎮 Create Menu Manager")]
        public void CreateMenuManager()
        {
            GameObject existingManager = GameObject.Find("MenuManager");
            if (existingManager != null)
            {
                if (Application.isPlaying)
                    Destroy(existingManager);
                else
                    DestroyImmediate(existingManager);
            }

            GameObject managerObj = new GameObject("MenuManager");
            FurnitureSelectionManager selectionManager = managerObj.AddComponent<FurnitureSelectionManager>();

            AssignUIReferences(selectionManager);
        }

        private void AssignUIReferences(FurnitureSelectionManager selectionManager)
        {
            selectionManager.mainMenuAnimator = GameObject.Find("MainMenuPanel")?.GetComponent<UIAnimator>();
            selectionManager.modelSelectionAnimator = GameObject.Find("ModelSelectionPanel")?.GetComponent<UIAnimator>();
            selectionManager.gameplayAnimator = GameObject.Find("GameplayPanel")?.GetComponent<UIAnimator>();

            selectionManager.iniciarButton = GameObject.Find("IniciarButton")?.GetComponent<Button>();
            selectionManager.salirButton = GameObject.Find("SalirButton")?.GetComponent<Button>();
            selectionManager.backToMenuButton = GameObject.Find("BackButton")?.GetComponent<Button>();

            Transform scrollContent = GameObject.Find("Content")?.transform;
            if (scrollContent != null)
                selectionManager.modelGridParent = scrollContent;

            Debug.Log($"🔗 UI References assigned: " +
                     $"MainMenu={selectionManager.mainMenuAnimator != null}, " +
                     $"ModelSelection={selectionManager.modelSelectionAnimator != null}, " +
                     $"Gameplay={selectionManager.gameplayAnimator != null}, " +
                     $"Buttons={selectionManager.iniciarButton != null && selectionManager.salirButton != null && selectionManager.backToMenuButton != null}, " +
                     $"Grid={selectionManager.modelGridParent != null}");
        }

        private void ConnectButtonEvents()
        {
            FurnitureSelectionManager selectionManager = FindObjectOfType<FurnitureSelectionManager>();
            if (selectionManager == null)
            {
                Debug.LogError("❌ No FurnitureSelectionManager found!");
                return;
            }

            if (selectionManager.iniciarButton != null)
            {
                selectionManager.iniciarButton.onClick.RemoveAllListeners();
                selectionManager.iniciarButton.onClick.AddListener(() => selectionManager.TransitionToModelSelection());
            }

            if (selectionManager.salirButton != null)
            {
                selectionManager.salirButton.onClick.RemoveAllListeners();
                selectionManager.salirButton.onClick.AddListener(() => selectionManager.ExitApplication());
            }

            if (selectionManager.backToMenuButton != null)
            {
                selectionManager.backToMenuButton.onClick.RemoveAllListeners();
                selectionManager.backToMenuButton.onClick.AddListener(() => selectionManager.ShowMainMenu());
            }

            Debug.Log("🔗 Button events connected successfully!");
        }

        [ContextMenu("🔴 Create Restart Button")]
        public void CreateRestartButton()
        {
            GameObject existingRestart = GameObject.Find("RestartButtonParent");
            if (existingRestart != null)
            {
                if (Application.isPlaying)
                    Destroy(existingRestart);
                else
                    DestroyImmediate(existingRestart);
            }

            GameObject restartParent = new GameObject("RestartButtonParent");

            if (playerSpawnPoint != null)
            {
                restartParent.transform.position = playerSpawnPoint.position + Vector3.right * 1.5f + Vector3.up * 1.2f;
            }

            RestartButtonSetup setup = restartParent.AddComponent<RestartButtonSetup>();
            restartParent.SetActive(false);

            FurnitureSelectionManager selectionManager = FindObjectOfType<FurnitureSelectionManager>();
            if (selectionManager != null)
            {
                selectionManager.restartButtonSetup = setup;
                Debug.Log("🔴 Restart button created and connected to menu manager!");
            }
        }

        [ContextMenu("🪑 Setup Furniture Selection")]
        public void SetupFurnitureSelection()
        {
            FurnitureSelectionManager selectionManager = FindObjectOfType<FurnitureSelectionManager>();
            if (selectionManager != null)
            {
                selectionManager.FindAllFurnitureSets();
                selectionManager.SetupFurnitureButtons();
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

        [ContextMenu("🔍 Validate Setup")]
        public void ValidateSetup()
        {
            Debug.Log("🔍 Validating menu system setup...");

            bool isValid = true;

            GameObject canvas = GameObject.Find("MenuCanvas");
            if (canvas == null) { Debug.LogError("❌ MenuCanvas not found!"); isValid = false; }
            else { Debug.Log("✅ MenuCanvas found"); }

            GameObject manager = GameObject.Find("MenuManager");
            if (manager == null) { Debug.LogError("❌ MenuManager not found!"); isValid = false; }
            else { Debug.Log("✅ MenuManager found"); }

            FurnitureSelectionManager selectionManager = FindObjectOfType<FurnitureSelectionManager>();
            if (selectionManager == null) { Debug.LogError("❌ FurnitureSelectionManager not found!"); isValid = false; }
            else
            {
                Debug.Log("✅ FurnitureSelectionManager found");

                if (selectionManager.iniciarButton == null) { Debug.LogError("❌ Iniciar button not assigned!"); isValid = false; }
                if (selectionManager.salirButton == null) { Debug.LogError("❌ Salir button not assigned!"); isValid = false; }
                if (selectionManager.backToMenuButton == null) { Debug.LogError("❌ Back button not assigned!"); isValid = false; }
                if (selectionManager.modelGridParent == null) { Debug.LogError("❌ Model grid parent not assigned!"); isValid = false; }

                if (selectionManager.iniciarButton != null && selectionManager.salirButton != null &&
                    selectionManager.backToMenuButton != null && selectionManager.modelGridParent != null)
                {
                    Debug.Log("✅ All button references assigned");
                }
            }

            if (playerSpawnPoint == null) { Debug.LogWarning("⚠️ Player spawn point not assigned!"); }
            else { Debug.Log("✅ Player spawn point assigned"); }

            if (isValid)
            {
                Debug.Log("🎉 Menu system validation PASSED! Everything looks good.");
            }
            else
            {
                Debug.LogError("💥 Menu system validation FAILED! Check errors above.");
            }
        }
    }
}