using UnityEngine;
using UnityEngine.UI;

namespace VRProject
{
    public class SimpleMenuController : MonoBehaviour
    {
        [Header("Menu Panels")]
        public GameObject mainMenuPanel;
        public GameObject modelSelectionPanel;
        public GameObject gameplayPanel;

        [Header("Physical Buttons")]
        public PhysicalButton iniciarButton;
        public PhysicalButton salirButton;
        public PhysicalButton backButton;

        private enum MenuState
        {
            MainMenu,
            ModelSelection,
            Gameplay
        }

        private MenuState currentState = MenuState.MainMenu;

        private void Start()
        {
            Debug.Log("🎮 SimpleMenuController Starting...");

            FindMenuPanels();
            FindPhysicalButtons();
            ConnectButtonEvents();
            ShowMainMenu();

            Debug.Log("🎮 SimpleMenuController Setup Complete!");
        }

        private void FindMenuPanels()
        {
            Debug.Log("🔍 Finding menu panels...");

            if (mainMenuPanel == null)
                mainMenuPanel = GameObject.Find("MainMenuPanel");
            if (modelSelectionPanel == null)
                modelSelectionPanel = GameObject.Find("ModelSelectionPanel");
            if (gameplayPanel == null)
                gameplayPanel = GameObject.Find("GameplayPanel");

            Debug.Log($"   - MainMenuPanel: {(mainMenuPanel != null ? "✅ Found" : "❌ RED X RED - NOT FOUND")}");
            Debug.Log($"   - ModelSelectionPanel: {(modelSelectionPanel != null ? "✅ Found" : "❌ RED X RED - NOT FOUND")}");
            Debug.Log($"   - GameplayPanel: {(gameplayPanel != null ? "✅ Found" : "❌ RED X RED - NOT FOUND")}");
        }

        private void FindPhysicalButtons()
        {
            Debug.Log("🔍 Finding physical buttons...");

            if (iniciarButton == null)
            {
                GameObject iniciarObj = GameObject.Find("IniciarButton");
                if (iniciarObj != null)
                    iniciarButton = iniciarObj.GetComponent<PhysicalButton>();
            }

            if (salirButton == null)
            {
                GameObject salirObj = GameObject.Find("SalirButton");
                if (salirObj != null)
                    salirButton = salirObj.GetComponent<PhysicalButton>();
            }

            if (backButton == null)
            {
                GameObject backObj = GameObject.Find("BackButton");
                if (backObj != null)
                    backButton = backObj.GetComponent<PhysicalButton>();
            }

            Debug.Log($"   - IniciarButton: {(iniciarButton != null ? "✅ Found" : "❌ RED X RED - NOT FOUND")}");
            Debug.Log($"   - SalirButton: {(salirButton != null ? "✅ Found" : "❌ RED X RED - NOT FOUND")}");
            Debug.Log($"   - BackButton: {(backButton != null ? "✅ Found" : "❌ RED X RED - NOT FOUND")}");
        }

        private void ConnectButtonEvents()
        {
            Debug.Log("🔗 Connecting button events...");

            if (iniciarButton != null)
            {
                iniciarButton.OnButtonPressed.RemoveAllListeners();
                iniciarButton.OnButtonPressed.AddListener(() => {
                    Debug.Log("🚀🚀🚀 INICIAR PRESSED - SHOWING MODEL SELECTION! 🚀🚀🚀");
                    ShowModelSelection();
                });
                Debug.Log("✅ INICIAR button connected");
            }
            else
            {
                Debug.LogError("❌ RED X RED - Cannot connect INICIAR button - not found!");
            }

            if (salirButton != null)
            {
                salirButton.OnButtonPressed.RemoveAllListeners();
                salirButton.OnButtonPressed.AddListener(() => {
                    Debug.Log("👋 SALIR PRESSED - EXITING!");
                    ExitApplication();
                });
                Debug.Log("✅ SALIR button connected");
            }

            if (backButton != null)
            {
                backButton.OnButtonPressed.RemoveAllListeners();
                backButton.OnButtonPressed.AddListener(() => {
                    Debug.Log("🔙 BACK PRESSED - SHOWING MAIN MENU!");
                    ShowMainMenu();
                });
                Debug.Log("✅ BACK button connected");
            }
        }

        public void ShowMainMenu()
        {
            Debug.Log("🏠🏠🏠 SHOWING MAIN MENU 🏠🏠🏠");

            SetPanelActive(mainMenuPanel, true);
            SetPanelActive(modelSelectionPanel, false);
            SetPanelActive(gameplayPanel, false);

            currentState = MenuState.MainMenu;
            Debug.Log("✅ Main menu is now active");
        }

        public void ShowModelSelection()
        {
            Debug.Log("📋📋📋 SHOWING MODEL SELECTION 📋📋📋");

            SetPanelActive(mainMenuPanel, false);
            SetPanelActive(modelSelectionPanel, true);
            SetPanelActive(gameplayPanel, false);

            SetupModelSelectionButtons();

            currentState = MenuState.ModelSelection;
            Debug.Log("✅ Model selection is now active");
        }

        public void ShowGameplay()
        {
            Debug.Log("🎮🎮🎮 SHOWING GAMEPLAY 🎮🎮🎮");

            SetPanelActive(mainMenuPanel, false);
            SetPanelActive(modelSelectionPanel, false);
            SetPanelActive(gameplayPanel, true);

            currentState = MenuState.Gameplay;
            Debug.Log("✅ Gameplay is now active");
        }

        private void SetPanelActive(GameObject panel, bool active)
        {
            if (panel != null)
            {
                EditorVisibilityHelper helper = panel.GetComponent<EditorVisibilityHelper>();
                if (helper != null)
                {
                    if (active)
                    {
                        panel.SetActive(true);
                        helper.SetVisibility(true);
                        Debug.Log($"✅ Panel {panel.name} activated via EditorVisibilityHelper");
                    }
                    else
                    {
                        helper.SetVisibility(false);
                        Debug.Log($"🙈 Panel {panel.name} hidden via EditorVisibilityHelper");
                    }
                }
                else
                {
                    panel.SetActive(active);
                    Debug.Log($"{(active ? "✅" : "🙈")} Panel {panel.name} {(active ? "activated" : "deactivated")} directly");
                }
            }
            else
            {
                Debug.LogError($"❌ RED X RED - Cannot set panel active - panel is null!");
            }
        }

        private void SetupModelSelectionButtons()
        {
            Debug.Log("🪑 Setting up model selection buttons...");

            FurnitureSelectionManager furnitureManager = FindObjectOfType<FurnitureSelectionManager>();
            if (furnitureManager != null)
            {
                furnitureManager.FindAllFurnitureSets();
                furnitureManager.SetupFurnitureButtons();
                Debug.Log("✅ Furniture buttons setup completed");
            }
            else
            {
                Debug.LogError("❌ RED X RED - FurnitureSelectionManager not found!");
            }
        }

        public void SelectFurniture(string furnitureSetName)
        {
            Debug.Log($"🪑 Furniture selected: {furnitureSetName}");

            FurnitureSelectionManager furnitureManager = FindObjectOfType<FurnitureSelectionManager>();
            if (furnitureManager != null)
            {
                ShowGameplay();
            }
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

        [ContextMenu("🔍 Debug Current State")]
        public void DebugCurrentState()
        {
            Debug.Log($"🔍 Current State: {currentState}");
            Debug.Log($"🔍 Panel States:");
            Debug.Log($"   - MainMenuPanel: {(mainMenuPanel != null ? mainMenuPanel.activeSelf.ToString() : "NULL")}");
            Debug.Log($"   - ModelSelectionPanel: {(modelSelectionPanel != null ? modelSelectionPanel.activeSelf.ToString() : "NULL")}");
            Debug.Log($"   - GameplayPanel: {(gameplayPanel != null ? gameplayPanel.activeSelf.ToString() : "NULL")}");
        }
    }
}