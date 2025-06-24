using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VRProject
{
    public class AutoMenuSetup : MonoBehaviour
    {
        [Header("Auto Setup")]
        public Transform playerSpawnPoint;
        public GameObject cubePrefab;

        [ContextMenu("🚀 CREATE COMPLETE MENU SYSTEM")]
        public void CreateCompleteMenuSystem()
        {
            CreateCanvas();
            CreateMenuManager();
            CreateRestartButton();
            SetupFurniture();
            Debug.Log("✅ Complete menu system created!");
        }

        [ContextMenu("📱 Create Canvas & UI")]
        public void CreateCanvas()
        {
            GameObject canvasObj = new GameObject("MenuCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

            canvasObj.AddComponent<GraphicRaycaster>();

            if (playerSpawnPoint != null)
            {
                canvasObj.transform.position = playerSpawnPoint.position + Vector3.forward * 2f + Vector3.up * 1.5f;
            }
            canvasObj.transform.localScale = Vector3.one * 0.01f;

            CreateUIPanels(canvasObj.transform);
        }

        private void CreateUIPanels(Transform canvasParent)
        {
            // Main Menu Panel
            GameObject mainPanel = CreatePanel("MainMenuPanel", canvasParent);
            AddUIAnimator(mainPanel);
            CreateMainMenuContent(mainPanel.transform);

            // Model Selection Panel
            GameObject selectionPanel = CreatePanel("ModelSelectionPanel", canvasParent);
            AddUIAnimator(selectionPanel);
            selectionPanel.SetActive(false);
            CreateModelSelectionContent(selectionPanel.transform);

            // Gameplay Panel
            GameObject gameplayPanel = CreatePanel("GameplayPanel", canvasParent);
            AddUIAnimator(gameplayPanel);
            gameplayPanel.SetActive(false);
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
            // Title
            CreateText("MuebleBuilder", parent, new Vector2(0, 100), 60);

            // Iniciar Button
            CreateButton("IniciarButton", "INICIAR", parent, new Vector2(0, 0), new Vector2(200, 60));

            // Salir Button
            CreateButton("SalirButton", "SALIR", parent, new Vector2(0, -80), new Vector2(200, 60));
        }

        private void CreateModelSelectionContent(Transform parent)
        {
            // Title
            CreateText("Selecciona tu Mueble", parent, new Vector2(0, 200), 40);

            // Back Button
            CreateButton("BackButton", "◀ VOLVER", parent, new Vector2(-300, 200), new Vector2(150, 50));

            // Scroll View
            CreateScrollView(parent);
        }

        private GameObject CreateButton(string name, string text, Transform parent, Vector2 position, Vector2 size)
        {
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent);

            RectTransform rect = buttonObj.AddComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            Image image = buttonObj.AddComponent<Image>();
            image.color = new Color(0.2f, 0.3f, 0.8f, 0.8f);

            Button button = buttonObj.AddComponent<Button>();
            buttonObj.AddComponent<ModernButtonEffect>();

            // Button Text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform);

            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.text = text;
            textComponent.fontSize = 24;
            textComponent.color = Color.white;
            textComponent.alignment = TextAlignmentOptions.Center;

            return buttonObj;
        }

        private void CreateText(string text, Transform parent, Vector2 position, float fontSize)
        {
            GameObject textObj = new GameObject("Text_" + text.Replace(" ", ""));
            textObj.transform.SetParent(parent);

            RectTransform rect = textObj.AddComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(400, 80);

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
            scrollRect.sizeDelta = new Vector2(600, 300);

            ScrollRect scroll = scrollObj.AddComponent<ScrollRect>();
            scrollObj.AddComponent<Image>().color = new Color(0, 0, 0, 0.3f);

            // Content
            GameObject content = new GameObject("Content");
            content.transform.SetParent(scrollObj.transform);

            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.sizeDelta = new Vector2(0, 600);

            GridLayoutGroup grid = content.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(120, 120);
            grid.spacing = new Vector2(10, 10);
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;

            scroll.content = contentRect;
            scroll.horizontal = false;
            scroll.vertical = true;
        }

        [ContextMenu("🎮 Create Menu Manager")]
        public void CreateMenuManager()
        {
            GameObject managerObj = new GameObject("MenuManager");

            AnimatedMenuManager menuManager = managerObj.AddComponent<AnimatedMenuManager>();
            MenuSetupHelper setupHelper = managerObj.AddComponent<MenuSetupHelper>();
            FurniturePrefabGenerator generator = managerObj.AddComponent<FurniturePrefabGenerator>();

            // Auto-assign references
            menuManager.mainMenuAnimator = GameObject.Find("MainMenuPanel")?.GetComponent<UIAnimator>();
            menuManager.modelSelectionAnimator = GameObject.Find("ModelSelectionPanel")?.GetComponent<UIAnimator>();
            menuManager.gameplayAnimator = GameObject.Find("GameplayPanel")?.GetComponent<UIAnimator>();

            menuManager.iniciarButton = GameObject.Find("IniciarButton")?.GetComponent<Button>();
            menuManager.salirButton = GameObject.Find("SalirButton")?.GetComponent<Button>();
            menuManager.backToMenuButton = GameObject.Find("BackButton")?.GetComponent<Button>();

            Transform scrollContent = GameObject.Find("Content")?.transform;
            if (scrollContent != null)
                menuManager.modelGridParent = scrollContent;

            setupHelper.animatedMenuManager = menuManager;
            setupHelper.furnitureGenerator = generator;

            if (cubePrefab != null)
                generator.baseCubePrefab = cubePrefab;
        }

        [ContextMenu("🔴 Create Restart Button")]
        public void CreateRestartButton()
        {
            GameObject restartParent = new GameObject("RestartButtonParent");

            if (playerSpawnPoint != null)
            {
                restartParent.transform.position = playerSpawnPoint.position + Vector3.right * 1.5f + Vector3.up * 1.2f;
            }

            RestartButtonSetup setup = restartParent.AddComponent<RestartButtonSetup>();
            restartParent.SetActive(false);

            // Link to menu manager
            AnimatedMenuManager menuManager = FindObjectOfType<AnimatedMenuManager>();
            if (menuManager != null)
                menuManager.restartButtonSetup = setup;
        }

        [ContextMenu("🪑 Setup Furniture")]
        public void SetupFurniture()
        {
            MenuSetupHelper helper = FindObjectOfType<MenuSetupHelper>();
            if (helper != null)
            {
                helper.CreateSampleMaterials();
                helper.SetupMenuWithGeneratedFurniture();
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
    }
}