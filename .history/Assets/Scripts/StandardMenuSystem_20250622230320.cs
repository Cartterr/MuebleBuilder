using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace VRProject
{
    public class StandardMenuSystem : MonoBehaviour
    {
        [Header("Canvas Groups (Standard Unity Way)")]
        public CanvasGroup mainMenuGroup;
        public CanvasGroup modelSelectionGroup;
        public CanvasGroup gameplayGroup;

        [Header("UI Buttons (Standard Unity)")]
        public Button iniciarButton;
        public Button salirButton;
        public Button backButton;

        [Header("Model Selection Content")]
        public Transform modelButtonContainer;
        public GameObject modelButtonPrefab;

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
            FindUIButtons();
            ConnectStandardButtons();
            FindFurnitureSets();
            ShowMainMenu();

            Debug.Log("✅✅✅ STANDARD MENU SYSTEM READY ✅✅✅");
        }

        private void SetupCanvasGroups()
        {
            Debug.Log("🔧 Setting up Canvas Groups...");

            if (mainMenuGroup == null)
            {
                GameObject mainPanel = GameObject.Find("MainMenuPanel");
                if (mainPanel != null)
                {
                    mainMenuGroup = GetOrAddCanvasGroup(mainPanel);
                    Debug.Log("✅ MainMenuPanel CanvasGroup ready");
                }
                else
                {
                    Debug.LogError("❌ RED X RED - MainMenuPanel not found!");
                }
            }

            if (modelSelectionGroup == null)
            {
                GameObject modelPanel = GameObject.Find("ModelSelectionPanel");
                if (modelPanel != null)
                {
                    modelSelectionGroup = GetOrAddCanvasGroup(modelPanel);
                    Debug.Log("✅ ModelSelectionPanel CanvasGroup ready");
                }
                else
                {
                    Debug.LogError("❌ RED X RED - ModelSelectionPanel not found!");
                }
            }

            if (gameplayGroup == null)
            {
                GameObject gameplayPanel = GameObject.Find("GameplayPanel");
                if (gameplayPanel != null)
                {
                    gameplayGroup = GetOrAddCanvasGroup(gameplayPanel);
                    Debug.Log("✅ GameplayPanel CanvasGroup ready");
                }
                else
                {
                    Debug.LogError("❌ RED X RED - GameplayPanel not found!");
                }
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

        private void FindUIButtons()
        {
            Debug.Log("🔍 Finding UI Buttons...");

            if (iniciarButton == null)
            {
                GameObject iniciarObj = GameObject.Find("IniciarButton");
                if (iniciarObj != null)
                {
                    iniciarButton = iniciarObj.GetComponent<Button>();
                    Debug.Log($"✅ Found IniciarButton: {iniciarButton != null}");
                }
                else
                {
                    Debug.LogError("❌ RED X RED - IniciarButton GameObject not found!");
                }
            }

            if (salirButton == null)
            {
                GameObject salirObj = GameObject.Find("SalirButton");
                if (salirObj != null)
                {
                    salirButton = salirObj.GetComponent<Button>();
                    Debug.Log($"✅ Found SalirButton: {salirButton != null}");
                }
                else
                {
                    Debug.LogError("❌ RED X RED - SalirButton GameObject not found!");
                }
            }

            if (backButton == null)
            {
                GameObject backObj = GameObject.Find("BackButton");
                if (backObj != null)
                {
                    backButton = backObj.GetComponent<Button>();
                    Debug.Log($"✅ Found BackButton: {backButton != null}");
                }
                else
                {
                    Debug.LogError("❌ RED X RED - BackButton GameObject not found!");
                }
            }
        }

        private void ConnectStandardButtons()
        {
            Debug.Log("🔗 Connecting Standard UI Buttons...");

            if (iniciarButton != null)
            {
                iniciarButton.onClick.RemoveAllListeners();
                iniciarButton.onClick.AddListener(() => {
                    Debug.Log("🚀🚀🚀 INICIAR CLICKED - STANDARD WAY! 🚀🚀🚀");
                    ShowModelSelection();
                });
                Debug.Log("✅ INICIAR connected via standard Button.onClick");
            }
            else
            {
                Debug.LogError("❌ RED X RED - Cannot connect INICIAR - Button component missing!");
            }

            if (salirButton != null)
            {
                salirButton.onClick.RemoveAllListeners();
                salirButton.onClick.AddListener(() => {
                    Debug.Log("👋 SALIR CLICKED - EXITING!");
                    ExitApplication();
                });
                Debug.Log("✅ SALIR connected via standard Button.onClick");
            }

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

            SetCanvasGroupVisible(mainMenuGroup, true);
            SetCanvasGroupVisible(modelSelectionGroup, false);
            SetCanvasGroupVisible(gameplayGroup, false);

            currentState = MenuState.MainMenu;
            Debug.Log("✅ Main menu now visible via CanvasGroup");
        }

        public void ShowModelSelection()
        {
            Debug.Log("📋📋📋 SHOWING MODEL SELECTION (STANDARD WAY) 📋📋📋");

            SetCanvasGroupVisible(mainMenuGroup, false);
            SetCanvasGroupVisible(modelSelectionGroup, true);
            SetCanvasGroupVisible(gameplayGroup, false);

            CreateModelSelectionButtons();

            currentState = MenuState.ModelSelection;
            Debug.Log("✅ Model selection now visible via CanvasGroup");
        }

        public void ShowGameplay()
        {
            Debug.Log("🎮🎮🎮 SHOWING GAMEPLAY (STANDARD WAY) 🎮🎮🎮");

            SetCanvasGroupVisible(mainMenuGroup, false);
            SetCanvasGroupVisible(modelSelectionGroup, false);
            SetCanvasGroupVisible(gameplayGroup, true);

            currentState = MenuState.Gameplay;
            Debug.Log("✅ Gameplay now visible via CanvasGroup");
        }

        private void SetCanvasGroupVisible(CanvasGroup group, bool visible)
        {
            if (group != null)
            {
                group.alpha = visible ? 1f : 0f;
                group.interactable = visible;
                group.blocksRaycasts = visible;

                Debug.Log($"{(visible ? "👁️" : "🙈")} CanvasGroup {group.name} alpha={group.alpha}, interactable={group.interactable}");
            }
            else
            {
                Debug.LogError("❌ RED X RED - CanvasGroup is null!");
            }
        }

        private void CreateModelSelectionButtons()
        {
            Debug.Log("🔨 Creating model selection buttons from scratch...");

            if (modelButtonContainer == null)
            {
                GameObject content = GameObject.Find("Content");
                if (content != null)
                {
                    modelButtonContainer = content.transform;
                    Debug.Log("✅ Found Content container for buttons");
                }
                else
                {
                    Debug.LogError("❌ RED X RED - Content container not found!");
                    return;
                }
            }

            ClearExistingButtons();

            for (int i = 0; i < furnitureSets.Count; i++)
            {
                string setName = furnitureSets[i];
                CreateStandardFurnitureButton(setName, i);
            }

            Debug.Log($"✅ Created {furnitureSets.Count} furniture selection buttons");
        }

        private void ClearExistingButtons()
        {
            Debug.Log("🗑️ Clearing existing buttons...");

            foreach (Transform child in modelButtonContainer)
            {
                if (Application.isPlaying)
                    Destroy(child.gameObject);
                else
                    DestroyImmediate(child.gameObject);
            }
        }

        private void CreateStandardFurnitureButton(string setName, int index)
        {
            Debug.Log($"🔨 Creating button for {setName}...");

            GameObject buttonObj = new GameObject($"Button_{setName}");
            buttonObj.transform.SetParent(modelButtonContainer);
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
            Debug.Log($"🔍 Canvas Group States:");
            if (mainMenuGroup != null)
                Debug.Log($"   - MainMenu: alpha={mainMenuGroup.alpha}, interactable={mainMenuGroup.interactable}");
            if (modelSelectionGroup != null)
                Debug.Log($"   - ModelSelection: alpha={modelSelectionGroup.alpha}, interactable={modelSelectionGroup.interactable}");
            if (gameplayGroup != null)
                Debug.Log($"   - Gameplay: alpha={gameplayGroup.alpha}, interactable={gameplayGroup.interactable}");
        }

        [ContextMenu("🚀 Force Show Model Selection")]
        public void ForceShowModelSelection()
        {
            Debug.Log("🚀 FORCE SHOWING MODEL SELECTION!");
            ShowModelSelection();
        }
    }
}