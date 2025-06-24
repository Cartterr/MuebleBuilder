using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

namespace VRProject
{
    public class AnimatedMenuManager : MonoBehaviour
    {
        [Header("UI Panels with Animators")]
        public UIAnimator mainMenuAnimator;
        public UIAnimator modelSelectionAnimator;
        public UIAnimator gameplayAnimator;

        [Header("Buttons")]
        public Button iniciarButton;
        public Button salirButton;
        public Button backToMenuButton;

        [Header("Model Selection")]
        public Transform modelGridParent;
        public GameObject modelButtonPrefab;
        public List<ModelData> availableModels;
        public ScrollRect modelScrollRect;

        [Header("Gameplay")]
        public Transform spawnPoint;
        public RestartButtonSetup restartButtonSetup;

        [Header("Audio")]
        public AudioSource buttonAudioSource;
        public AudioClip buttonClickSound;
        public AudioClip menuTransitionSound;

        private MenuState currentState = MenuState.MainMenu;

        private enum MenuState
        {
            MainMenu,
            ModelSelection,
            Gameplay
        }

        private void Start()
        {
            SetupUI();
            ShowMainMenu();
        }

        private void SetupUI()
        {
            if (iniciarButton != null)
            {
                iniciarButton.onClick.AddListener(() => {
                    PlayButtonSound();
                    TransitionToModelSelection();
                });
            }

            if (salirButton != null)
            {
                salirButton.onClick.AddListener(() => {
                    PlayButtonSound();
                    ExitApplication();
                });
            }

            if (backToMenuButton != null)
            {
                backToMenuButton.onClick.AddListener(() => {
                    PlayButtonSound();
                    TransitionToMainMenu();
                });
            }

            SetupButtonHoverEffects();
        }

        private void SetupButtonHoverEffects()
        {
            AddHoverEffect(iniciarButton);
            AddHoverEffect(salirButton);
            AddHoverEffect(backToMenuButton);
        }

        private void AddHoverEffect(Button button)
        {
            if (button == null) return;

            var animator = button.GetComponent<UIAnimator>();
            if (animator == null)
            {
                animator = button.gameObject.AddComponent<UIAnimator>();
                animator.animationDuration = 0.15f;
                animator.startScale = Vector3.one;
                animator.targetScale = Vector3.one * 1.1f;
                animator.animateFade = false;
            }
        }

        public void ShowMainMenu()
        {
            if (currentState == MenuState.MainMenu) return;

            HideCurrentPanel(() => {
                mainMenuAnimator?.AnimateIn();
                currentState = MenuState.MainMenu;
                PlayTransitionSound();
            });
        }

        public void TransitionToModelSelection()
        {
            if (currentState == MenuState.ModelSelection) return;

            HideCurrentPanel(() => {
                PopulateModelGrid();
                modelSelectionAnimator?.AnimateIn();
                currentState = MenuState.ModelSelection;
                PlayTransitionSound();
            });
        }

        public void TransitionToMainMenu()
        {
            if (currentState == MenuState.MainMenu) return;

            HideCurrentPanel(() => {
                mainMenuAnimator?.AnimateIn();
                currentState = MenuState.MainMenu;
                PlayTransitionSound();
            });
        }

        public void StartGameplay()
        {
            if (currentState == MenuState.Gameplay) return;

            HideCurrentPanel(() => {
                gameplayAnimator?.AnimateIn();
                currentState = MenuState.Gameplay;
                PlayTransitionSound();

                // Show restart button when entering gameplay
                ShowRestartButton();
            });
        }

        private void HideCurrentPanel(System.Action onComplete)
        {
            switch (currentState)
            {
                case MenuState.MainMenu:
                    if (mainMenuAnimator != null)
                        mainMenuAnimator.AnimateOut(onComplete);
                    else
                        onComplete?.Invoke();
                    break;

                case MenuState.ModelSelection:
                    if (modelSelectionAnimator != null)
                        modelSelectionAnimator.AnimateOut(onComplete);
                    else
                        onComplete?.Invoke();
                    break;

                case MenuState.Gameplay:
                    if (gameplayAnimator != null)
                        gameplayAnimator.AnimateOut(onComplete);
                    else
                        onComplete?.Invoke();
                    break;

                default:
                    onComplete?.Invoke();
                    break;
            }
        }

        private void PopulateModelGrid()
        {
            foreach (Transform child in modelGridParent)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < availableModels.Count; i++)
            {
                ModelData model = availableModels[i];
                GameObject buttonObj = Instantiate(modelButtonPrefab, modelGridParent);
                ModelSelectionButton buttonScript = buttonObj.GetComponent<ModelSelectionButton>();

                if (buttonScript != null)
                {
                    buttonScript.SetupButton(model, this);
                }

                StartCoroutine(AnimateModelButtonIn(buttonObj, i * 0.1f));
            }

            if (modelScrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                modelScrollRect.verticalNormalizedPosition = 1f;
            }
        }

        private IEnumerator AnimateModelButtonIn(GameObject buttonObj, float delay)
        {
            var animator = buttonObj.GetComponent<UIAnimator>();
            if (animator == null)
            {
                animator = buttonObj.AddComponent<UIAnimator>();
                animator.animationDuration = 0.4f;
                animator.startScale = Vector3.zero;
                animator.targetScale = Vector3.one;
            }

            buttonObj.transform.localScale = Vector3.zero;
            yield return new WaitForSeconds(delay);
            animator.AnimateIn();
        }

        public void SelectModel(ModelData model)
        {
            if (model.prefab != null && spawnPoint != null)
            {
                Instantiate(model.prefab, spawnPoint.position, spawnPoint.rotation);
            }
            StartGameplay();
        }

        private void PlayButtonSound()
        {
            if (buttonAudioSource != null && buttonClickSound != null)
            {
                buttonAudioSource.PlayOneShot(buttonClickSound);
            }
        }

        private void PlayTransitionSound()
        {
            if (buttonAudioSource != null && menuTransitionSound != null)
            {
                buttonAudioSource.PlayOneShot(menuTransitionSound);
            }
        }

        private void ShowRestartButton()
        {
            if (restartButtonSetup != null)
            {
                // Clear any existing restart button first
                restartButtonSetup.ClearRestartButton();

                // Create new restart button
                restartButtonSetup.CreateRestartButton();

                // Make sure it's positioned properly
                restartButtonSetup.gameObject.SetActive(true);
            }
        }

        private void HideRestartButton()
        {
            if (restartButtonSetup != null)
            {
                restartButtonSetup.ClearRestartButton();
            }
        }

        public void ReturnToMainMenu()
        {
            // Hide restart button
            HideRestartButton();

            // Clear any spawned models
            ClearSpawnedModels();

            // Return to main menu
            ShowMainMenu();
        }

        private void ClearSpawnedModels()
        {
            if (spawnPoint != null)
            {
                // Find and destroy all spawned furniture
                FurnitureAttachable[] spawnedFurniture = FindObjectsOfType<FurnitureAttachable>();
                foreach (FurnitureAttachable furniture in spawnedFurniture)
                {
                    // Only destroy furniture that's not part of our prefab system
                    if (!availableModels.Exists(model => model.prefab == furniture.gameObject))
                    {
                        Destroy(furniture.gameObject);
                    }
                }
            }
        }

        public void ExitApplication()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}