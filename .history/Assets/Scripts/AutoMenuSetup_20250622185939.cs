using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VRProject
{
    public class AutoMenuSetup : MonoBehaviour
    {
        [Header("Auto Setup")]
        public Transform playerSpawnPoint;

        [ContextMenu("🚀 CREATE COMPLETE MENU SYSTEM")]
        public void CreateCompleteMenuSystem()
        {
            CreateCanvas();
            CreateMenuManager();
            CreateRestartButton();
            SetupFurnitureSelection();
            Debug.Log("✅ Complete menu system created!");
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
                canvasObj.transform.position = playerSpawnPoint.position + Vector3.forward * 2f + Vector3.up * 1.5f;
            }
            canvasObj.transform.localScale = Vector3.one * 0.001f;

            // Set the canvas rect transform size
            RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(400, 300);

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
            CreateText("MuebleBuilder", parent, new Vector2(0, 5), 3);

            // Iniciar Button
            CreateButton("IniciarButton", "INICIAR", parent, new Vector2(0, 0), new Vector2(12, 4));

            // Salir Button
            CreateButton("SalirButton", "SALIR", parent, new Vector2(0, -5), new Vector2(12, 4));
        }

                        private void CreateModelSelectionContent(Transform parent)
        {
            // Title
            CreateText("Selecciona tu Mueble", parent, new Vector2(0, 12), 2);

            // Back Button
            CreateButton("BackButton", "◀ VOLVER", parent, new Vector2(-15, 12), new Vector2(10, 3));

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
            textComponent.fontSize = 1.4f;
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
            rect.sizeDelta = new Vector2(200, 40);

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
            scrollRect.sizeDelta = new Vector2(300, 150);

            ScrollRect scroll = scrollObj.AddComponent<ScrollRect>();
            scrollObj.AddComponent<Image>().color = new Color(0, 0, 0, 0.3f);

            // Content
            GameObject content = new GameObject("Content");
            content.transform.SetParent(scrollObj.transform);

            RectTransform contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
                        contentRect.sizeDelta = new Vector2(0, 300);

            GridLayoutGroup grid = content.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(60, 60);
            grid.spacing = new Vector2(5, 5);
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;

            scroll.content = contentRect;
            scroll.horizontal = false;
            scroll.vertical = true;
        }

                [ContextMenu("🎮 Create Menu Manager")]
        public void CreateMenuManager()
        {
            GameObject managerObj = new GameObject("MenuManager");

            FurnitureSelectionManager selectionManager = managerObj.AddComponent<FurnitureSelectionManager>();

            // Auto-assign references
            selectionManager.mainMenuAnimator = GameObject.Find("MainMenuPanel")?.GetComponent<UIAnimator>();
            selectionManager.modelSelectionAnimator = GameObject.Find("ModelSelectionPanel")?.GetComponent<UIAnimator>();
            selectionManager.gameplayAnimator = GameObject.Find("GameplayPanel")?.GetComponent<UIAnimator>();

            selectionManager.iniciarButton = GameObject.Find("IniciarButton")?.GetComponent<Button>();
            selectionManager.salirButton = GameObject.Find("SalirButton")?.GetComponent<Button>();
            selectionManager.backToMenuButton = GameObject.Find("BackButton")?.GetComponent<Button>();

            Transform scrollContent = GameObject.Find("Content")?.transform;
            if (scrollContent != null)
                selectionManager.modelGridParent = scrollContent;
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
            FurnitureSelectionManager selectionManager = FindObjectOfType<FurnitureSelectionManager>();
            if (selectionManager != null)
                selectionManager.restartButtonSetup = setup;
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
    }
}