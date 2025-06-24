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
            selectionPanel.SetActive(false);
            CreateModelSelectionContent(selectionPanel.transform);

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

            Image image = buttonObj.AddComponent<Image>();
            image.color = new Color(0.2f, 0.3f, 0.8f, 0.8f);

            Button button = buttonObj.AddComponent<Button>();

            BoxCollider collider = buttonObj.AddComponent<BoxCollider>();
            collider.size = new Vector3(size.x, size.y, 0.1f);
            collider.isTrigger = true;

            Rigidbody rb = buttonObj.AddComponent<Rigidbody>();
            rb.isKinematic = true;

            PhysicalButton physButton = buttonObj.AddComponent<PhysicalButton>();

            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform);

            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.text = text;
            textComponent.fontSize = 0.0875f;
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
            GameObject managerObj = new GameObject("MenuManager");

            FurnitureSelectionManager selectionManager = managerObj.AddComponent<FurnitureSelectionManager>();

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