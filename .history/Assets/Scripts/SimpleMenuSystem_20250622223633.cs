using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VRProject
{
    public class SimpleMenuSystem : MonoBehaviour
    {
        [Header("Menu Panels")]
        public GameObject mainMenuPanel;
        public GameObject modelSelectionPanel;
        public GameObject gameplayPanel;

        [Header("Current State")]
        public MenuState currentState = MenuState.MainMenu;

        public enum MenuState
        {
            MainMenu,
            ModelSelection,
            Gameplay
        }

        private void Start()
        {
            Debug.Log("🚀🚀🚀 SIMPLE MENU SYSTEM STARTING 🚀🚀🚀");

            FindMenuPanels();
            SetupInitialState();
            ConnectPhysicalButtons();

            Debug.Log("✅ Simple menu system initialized!");
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

        private void SetupInitialState()
        {
            Debug.Log("🎯 Setting up initial menu state...");

            ShowMainMenu();
        }

        private void ConnectPhysicalButtons()
        {
            Debug.Log("🔗 Connecting physical buttons...");

            GameObject iniciarButton = GameObject.Find("IniciarButton");
            GameObject salirButton = GameObject.Find("SalirButton");
            GameObject backButton = GameObject.Find("BackButton");

            Debug.Log($"   - IniciarButton: {(iniciarButton != null ? "✅ Found" : "❌ RED X RED - NOT FOUND")}");
            Debug.Log($"   - SalirButton: {(salirButton != null ? "✅ Found" : "❌ RED X RED - NOT FOUND")}");
            Debug.Log($"   - BackButton: {(backButton != null ? "✅ Found" : "❌ RED X RED - NOT FOUND")}");

            if (iniciarButton != null)
            {
                PhysicalButton physButton = iniciarButton.GetComponent<PhysicalButton>();
                if (physButton != null)
                {
                    physButton.OnButtonPressed.RemoveAllListeners();
                    physButton.OnButtonPressed.AddListener(() => {
                        Debug.Log("🚀🚀🚀 INICIAR PHYSICAL BUTTON PRESSED! 🚀🚀🚀");
                        TransitionToModelSelection();
                    });
                    Debug.Log("✅ INICIAR PhysicalButton connected!");
                }
                else
                {
                    Debug.LogError("❌ RED X RED - IniciarButton has no PhysicalButton component!");
                }
            }

            if (salirButton != null)
            {
                PhysicalButton physButton = salirButton.GetComponent<PhysicalButton>();
                if (physButton != null)
                {
                    physButton.OnButtonPressed.RemoveAllListeners();
                    physButton.OnButtonPressed.AddListener(() => {
                        Debug.Log("👋👋👋 SALIR PHYSICAL BUTTON PRESSED! 👋👋👋");
                        ExitApplication();
                    });
                    Debug.Log("✅ SALIR PhysicalButton connected!");
                }
                else
                {
                    Debug.LogError("❌ RED X RED - SalirButton has no PhysicalButton component!");
                }
            }

            if (backButton != null)
            {
                PhysicalButton physButton = backButton.GetComponent<PhysicalButton>();
                if (physButton != null)
                {
                    physButton.OnButtonPressed.RemoveAllListeners();
                    physButton.OnButtonPressed.AddListener(() => {
                        Debug.Log("🔙🔙🔙 BACK PHYSICAL BUTTON PRESSED! 🔙🔙🔙");
                        ShowMainMenu();
                    });
                    Debug.Log("✅ BACK PhysicalButton connected!");
                }
                else
                {
                    Debug.LogError("❌ RED X RED - BackButton has no PhysicalButton component!");
                }
            }
        }

        public void ShowMainMenu()
        {
            Debug.Log("🏠🏠🏠 SHOWING MAIN MENU 🏠🏠🏠");

            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(true);
                Debug.Log("✅ MainMenuPanel activated");
            }
            else
            {
                Debug.LogError("❌ RED X RED - MainMenuPanel is NULL!");
            }

            if (modelSelectionPanel != null)
            {
                modelSelectionPanel.SetActive(false);
                Debug.Log("🙈 ModelSelectionPanel deactivated");
            }

            if (gameplayPanel != null)
            {
                gameplayPanel.SetActive(false);
                Debug.Log("🙈 GameplayPanel deactivated");
            }

            currentState = MenuState.MainMenu;
            Debug.Log($"🎯 Current state: {currentState}");
        }

        public void TransitionToModelSelection()
        {
            Debug.Log("🚀🚀🚀 TRANSITIONING TO MODEL SELECTION 🚀🚀🚀");

            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(false);
                Debug.Log("🙈 MainMenuPanel deactivated");
            }
            else
            {
                Debug.LogError("❌ RED X RED - MainMenuPanel is NULL!");
            }

            if (modelSelectionPanel != null)
            {
                EditorVisibilityHelper editorHelper = modelSelectionPanel.GetComponent<EditorVisibilityHelper>();
                if (editorHelper != null)
                {
                    Debug.Log("✅ Found EditorVisibilityHelper, removing it temporarily");
                    editorHelper.enabled = false;
                }

                modelSelectionPanel.SetActive(true);
                Debug.Log("✅ ModelSelectionPanel activated");

                CanvasGroup canvasGroup = modelSelectionPanel.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f;
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                    Debug.Log("✅ CanvasGroup set to fully visible and interactive");
                }

                Transform rectTransform = modelSelectionPanel.GetComponent<Transform>();
                if (rectTransform != null)
                {
                    rectTransform.localScale = Vector3.one;
                    Debug.Log("✅ Scale set to normal");
                }
            }
            else
            {
                Debug.LogError("❌ RED X RED - ModelSelectionPanel is NULL!");
            }

            if (gameplayPanel != null)
            {
                gameplayPanel.SetActive(false);
                Debug.Log("🙈 GameplayPanel deactivated");
            }

            currentState = MenuState.ModelSelection;
            Debug.Log($"🎯 Current state: {currentState}");
            Debug.Log("🎉🎉🎉 MODEL SELECTION TRANSITION COMPLETED! 🎉🎉🎉");
        }

        public void StartGameplay()
        {
            Debug.Log("🎮🎮🎮 STARTING GAMEPLAY 🎮🎮🎮");

            if (mainMenuPanel != null)
                mainMenuPanel.SetActive(false);

            if (modelSelectionPanel != null)
                modelSelectionPanel.SetActive(false);

            if (gameplayPanel != null)
                gameplayPanel.SetActive(true);

            currentState = MenuState.Gameplay;
            Debug.Log($"🎯 Current state: {currentState}");
        }

        public void ExitApplication()
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
            Debug.Log("🔍🔍🔍 CURRENT MENU STATE DEBUG 🔍🔍🔍");
            Debug.Log($"Current State: {currentState}");

            if (mainMenuPanel != null)
                Debug.Log($"MainMenuPanel: Active={mainMenuPanel.activeSelf}");
            else
                Debug.LogError("❌ MainMenuPanel is NULL!");

            if (modelSelectionPanel != null)
            {
                Debug.Log($"ModelSelectionPanel: Active={modelSelectionPanel.activeSelf}");

                CanvasGroup cg = modelSelectionPanel.GetComponent<CanvasGroup>();
                if (cg != null)
                    Debug.Log($"   - CanvasGroup: Alpha={cg.alpha}, Interactable={cg.interactable}");

                EditorVisibilityHelper evh = modelSelectionPanel.GetComponent<EditorVisibilityHelper>();
                if (evh != null)
                    Debug.Log($"   - EditorVisibilityHelper: Enabled={evh.enabled}, HideAtRuntime={evh.hideAtRuntime}");
            }
            else
                Debug.LogError("❌ ModelSelectionPanel is NULL!");

            if (gameplayPanel != null)
                Debug.Log($"GameplayPanel: Active={gameplayPanel.activeSelf}");
            else
                Debug.LogError("❌ GameplayPanel is NULL!");
        }

        [ContextMenu("🚀 Force Show Model Selection")]
        public void ForceShowModelSelection()
        {
            Debug.Log("🚀 FORCING MODEL SELECTION TO SHOW");
            TransitionToModelSelection();
        }
    }
}