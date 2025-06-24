using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace VRProject
{
    public class StandardMenuSystem : MonoBehaviour
    {
        private enum MenuState
        {
            MainMenu,
            ModelSelection,
            Gameplay
        }

        private MenuState currentState = MenuState.MainMenu;
        private List<string> furnitureSets = new List<string>();

        private void Start()
        {
            Debug.Log("🎮🎮🎮 STANDARD MENU SYSTEM STARTING 🎮🎮🎮");

            SetupCanvasGroups();
            ConnectStandardButtons();
            FindFurnitureSets();
            ShowMainMenu();

            Debug.Log("✅✅✅ STANDARD MENU SYSTEM READY ✅✅✅");
        }

        private void SetupCanvasGroups()
        {
            Debug.Log("🔧 Setting up Canvas Groups...");

            GameObject mainPanel = GameObject.Find("MainMenuPanel");
            if (mainPanel != null)
            {
                GetOrAddCanvasGroup(mainPanel);
                Debug.Log("✅ MainMenuPanel CanvasGroup ready");
            }

            GameObject modelPanel = GameObject.Find("ModelSelectionPanel");
            if (modelPanel != null)
            {
                GetOrAddCanvasGroup(modelPanel);
                Debug.Log("✅ ModelSelectionPanel CanvasGroup ready");
            }

            GameObject gameplayPanel = GameObject.Find("GameplayPanel");
            if (gameplayPanel != null)
            {
                GetOrAddCanvasGroup(gameplayPanel);
                Debug.Log("✅ GameplayPanel CanvasGroup ready");
            }
        }

        private CanvasGroup GetOrAddCanvasGroup(GameObject obj)
        {
            CanvasGroup group = obj.GetComponent<CanvasGroup>();
            if (group == null)
            {
                group = obj.AddComponent<CanvasGroup>();
                Debug.Log($"➕ Added CanvasGroup to {obj.name}");
            }
            return group;
        }

        private void ConnectStandardButtons()
        {
            Debug.Log("🔗 Connecting Standard UI Buttons...");

            GameObject iniciarObj = GameObject.Find("IniciarButton");
            if (iniciarObj != null)
            {
                Button iniciarButton = iniciarObj.GetComponent<Button>();
                if (iniciarButton != null)
                {
                    iniciarButton.onClick.RemoveAllListeners();
                    iniciarButton.onClick.AddListener(() => {
                        Debug.Log("🚀🚀🚀 INICIAR CLICKED - STANDARD WAY! 🚀🚀🚀");
                        ShowModelSelection();
                    });
                    Debug.Log("✅ INICIAR connected via standard Button.onClick");
                }
            }

            GameObject salirObj = GameObject.Find("SalirButton");
            if (salirObj != null)
            {
                Button salirButton = salirObj.GetComponent<Button>();
                if (salirButton != null)
                {
                    salirButton.onClick.RemoveAllListeners();
                    salirButton.onClick.AddListener(() => {
                        Debug.Log("👋 SALIR CLICKED - EXITING!");
                        ExitApplication();
                    });
                    Debug.Log("✅ SALIR connected via standard Button.onClick");
                }
            }

            GameObject backObj = GameObject.Find("BackButton");
            if (backObj != null)
            {
                Button backButton = backObj.GetComponent<Button>();
                if (backButton != null)
                {
                    backButton.onClick.RemoveAllListeners();
                    backButton.onClick.AddListener(() => {
                        Debug.Log("🔙 BACK CLICKED - RETURNING TO MAIN!");
                        ShowMainMenu();
                    });
                    Debug.Log("✅ BACK connected via standard Button.onClick");
                }
            }
        }

        private void FindFurnitureSets()
        {
            Debug.Log("🪑 Finding furniture sets...");
            furnitureSets.Clear();

            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            HashSet<string> setNumbers = new HashSet<string>();

            foreach (GameObject obj in allObjects)
            {
                if (obj.name.StartsWith("furniture_"))
                {
                    string[] parts = obj.name.Split('_');
                    if (parts.Length >= 2)
                    {
                        setNumbers.Add(parts[1]);
                    }
                }
            }

            foreach (string setNumber in setNumbers)
            {
                furnitureSets.Add($"furniture_{setNumber}");
            }

            Debug.Log($"✅ Found {furnitureSets.Count} furniture sets: {string.Join(", ", furnitureSets)}");
        }

        public void ShowMainMenu()
        {
            Debug.Log("🏠🏠🏠 SHOWING MAIN MENU (STANDARD WAY) 🏠🏠🏠");

            SetCanvasGroupVisible("MainMenuPanel", true);
            SetCanvasGroupVisible("ModelSelectionPanel", false);
            SetCanvasGroupVisible("GameplayPanel", false);

            currentState = MenuState.MainMenu;
            Debug.Log("✅ Main menu now visible via CanvasGroup");
        }

        public void ShowModelSelection()
        {
            Debug.Log("📋📋📋 SHOWING MODEL SELECTION (STANDARD WAY) 📋📋📋");

            SetCanvasGroupVisible("MainMenuPanel", false);
            SetCanvasGroupVisible("ModelSelectionPanel", true);
            SetCanvasGroupVisible("GameplayPanel", false);

            CreateModelSelectionButtons();

            currentState = MenuState.ModelSelection;
            Debug.Log("✅ Model selection now visible via CanvasGroup");
        }

        public void ShowGameplay()
        {
            Debug.Log("🎮🎮🎮 SHOWING GAMEPLAY (STANDARD WAY) 🎮🎮🎮");

            SetCanvasGroupVisible("MainMenuPanel", false);
            SetCanvasGroupVisible("ModelSelectionPanel", false);
            SetCanvasGroupVisible("GameplayPanel", true);

            currentState = MenuState.Gameplay;
            Debug.Log("✅ Gameplay now visible via CanvasGroup");
        }

        private void SetCanvasGroupVisible(string panelName, bool visible)
        {
            GameObject panel = GameObject.Find(panelName);
            if (panel != null)
            {
                CanvasGroup group = panel.GetComponent<CanvasGroup>();
                if (group != null)
                {
                    group.alpha = visible ? 1f : 0f;
                    group.interactable = visible;
                    group.blocksRaycasts = visible;

                    Debug.Log($"{(visible ? "👁️" : "🙈")} {panelName} alpha={group.alpha}, interactable={group.interactable}");
                }
                else
                {
                    Debug.LogError($"❌ RED X RED - {panelName} has no CanvasGroup!");
                }
            }
            else
            {
                Debug.LogError($"❌ RED X RED - {panelName} not found!");
            }
        }

        private void CreateModelSelectionButtons()
        {
            Debug.Log("🔨 Creating model selection buttons from scratch...");

            GameObject content = GameObject.Find("Content");
            if (content == null)
            {
                Debug.LogError("❌ RED X RED - Content container not found!");
                return;
            }

            ClearExistingButtons(content.transform);

            for (int i = 0; i < furnitureSets.Count; i++)
            {
                string setName = furnitureSets[i];
                CreateStandardFurnitureButton(setName, content.transform);
            }

            Debug.Log($"✅ Created {furnitureSets.Count} furniture selection buttons");
        }

        private void ClearExistingButtons(Transform container)
        {
            Debug.Log("🗑️ Clearing existing buttons...");

            foreach (Transform child in container)
            {
                if (Application.isPlaying)
                    Destroy(child.gameObject);
                else
                    DestroyImmediate(child.gameObject);
            }
        }

        private void CreateStandardFurnitureButton(string setName, Transform container)
        {
            Debug.Log($"🔨 Creating button for {setName}...");

            GameObject buttonObj = new GameObject($"Button_{setName}");
            buttonObj.transform.SetParent(container);
            buttonObj.transform.localScale = Vector3.one;

            RectTransform rect = buttonObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(150, 150);

            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.4f, 0.8f, 1f);

            Button button = buttonObj.AddComponent<Button>();
            button.targetGraphic = buttonImage;

            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform);

            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            Text text = textObj.AddComponent<Text>();
            text.text = setName.Replace("furniture_", "MUEBLE ");
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 14;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;

            string capturedSetName = setName;
            button.onClick.AddListener(() => {
                Debug.Log($"🪑🪑🪑 FURNITURE SELECTED: {capturedSetName} 🪑🪑🪑");
                SelectFurnitureSet(capturedSetName);
            });

            Debug.Log($"✅ Created button for {setName}");
        }

        private void SelectFurnitureSet(string setName)
        {
            Debug.Log($"🪑 Selecting furniture set: {setName}");

            HideAllFurniture();
            ShowFurnitureSet(setName);
            ShowGameplay();
        }

        private void HideAllFurniture()
        {
            Debug.Log("🙈 Hiding all furniture...");

            FurnitureVisibilityController[] controllers = FindObjectsOfType<FurnitureVisibilityController>();
            foreach (FurnitureVisibilityController controller in controllers)
            {
                controller.HideFurniture();
            }

            Debug.Log($"🙈 Hid {controllers.Length} furniture pieces");
        }

        private void ShowFurnitureSet(string setName)
        {
            Debug.Log($"👁️ Showing furniture set: {setName}");

            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            int shownCount = 0;

            foreach (GameObject obj in allObjects)
            {
                if (obj.name.StartsWith(setName))
                {
                    FurnitureVisibilityController controller = obj.GetComponent<FurnitureVisibilityController>();
                    if (controller != null)
                    {
                        controller.ShowFurniture();
                        shownCount++;
                    }
                    else
                    {
                        obj.SetActive(true);
                        shownCount++;
                    }
                }
            }

            Debug.Log($"👁️ Showed {shownCount} pieces from {setName}");
        }

        private void ExitApplication()
        {
            Debug.Log("👋 Exiting application...");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        [ContextMenu("🔍 Debug Menu State")]
        public void DebugMenuState()
        {
            Debug.Log($"🔍 Current State: {currentState}");
        }

        [ContextMenu("🚀 Force Show Model Selection")]
        public void ForceShowModelSelection()
        {
            Debug.Log("🚀 FORCE SHOWING MODEL SELECTION!");
            ShowModelSelection();
        }
    }
}