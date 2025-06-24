using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

namespace VRProject
{
    public class FurnitureSelectionManager : MonoBehaviour
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

        [Header("Restart Button")]
        public RestartButtonSetup restartButtonSetup;

        [Header("Audio")]
        public AudioSource buttonAudioSource;
        public AudioClip buttonClickSound;
        public AudioClip menuTransitionSound;

        private MenuState currentState = MenuState.MainMenu;
        private List<FurnitureSet> furnitureSets = new List<FurnitureSet>();
        private FurnitureSet currentSelectedSet;

        private enum MenuState
        {
            MainMenu,
            ModelSelection,
            Gameplay
        }

        [System.Serializable]
        public class FurnitureSet
        {
            public string name;
            public List<GameObject> pieces;
            public Sprite previewImage;
        }

        private void Start()
        {
            SetupUI();
            HideAllFurniture();
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
                    ShowMainMenu();
                });
            }
        }

        public void FindAllFurnitureSets()
        {
            furnitureSets.Clear();

            // Find all furniture sets by looking for parent objects named furniture_1, furniture_2, etc.
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            Dictionary<string, FurnitureSet> setsDict = new Dictionary<string, FurnitureSet>();

            foreach (GameObject obj in allObjects)
            {
                if (obj.name.StartsWith("furniture_"))
                {
                    string[] parts = obj.name.Split('_');
                    if (parts.Length >= 2)
                    {
                        string setNumber = parts[1];
                        string setName = $"furniture_{setNumber}";

                        if (!setsDict.ContainsKey(setName))
                        {
                            setsDict[setName] = new FurnitureSet
                            {
                                name = setName,
                                pieces = new List<GameObject>()
                            };
                        }

                        setsDict[setName].pieces.Add(obj);
                    }
                }
            }

            furnitureSets.AddRange(setsDict.Values);
            Debug.Log($"Found {furnitureSets.Count} furniture sets with {GetTotalPieceCount()} total pieces");
        }

        private int GetTotalPieceCount()
        {
            int total = 0;
            foreach (FurnitureSet set in furnitureSets)
            {
                total += set.pieces.Count;
            }
            return total;
        }

        public void SetupFurnitureButtons()
        {
            if (modelGridParent == null) return;

            // Clear existing buttons
            foreach (Transform child in modelGridParent)
            {
                if (Application.isPlaying)
                    Destroy(child.gameObject);
                else
                    DestroyImmediate(child.gameObject);
            }

            // Create button for each furniture set
            for (int i = 0; i < furnitureSets.Count; i++)
            {
                FurnitureSet set = furnitureSets[i];
                CreateFurnitureButton(set, i);
            }
        }

        private void CreateFurnitureButton(FurnitureSet furnitureSet, int index)
        {
            GameObject buttonObj = new GameObject($"FurnitureButton_{furnitureSet.name}");
            buttonObj.transform.SetParent(modelGridParent);

            RectTransform rect = buttonObj.AddComponent<RectTransform>();
            rect.localScale = Vector3.one;

            Image image = buttonObj.AddComponent<Image>();
            image.color = new Color(0.3f, 0.5f, 0.8f, 0.9f);

            Button button = buttonObj.AddComponent<Button>();
            buttonObj.AddComponent<ModernButtonEffect>();

            // Button text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform);

            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            Text textComponent = textObj.AddComponent<Text>();
            textComponent.text = furnitureSet.name.Replace("furniture_", "MUEBLE ");
            textComponent.fontSize = 16;
            textComponent.color = Color.white;
            textComponent.alignment = TextAnchor.MiddleCenter;
            textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            // Add click listener
            button.onClick.AddListener(() => SelectFurnitureSet(furnitureSet));

            // Animate in with delay
            StartCoroutine(AnimateButtonIn(buttonObj, index * 0.1f));
        }

        private IEnumerator AnimateButtonIn(GameObject buttonObj, float delay)
        {
            buttonObj.transform.localScale = Vector3.zero;
            yield return new WaitForSeconds(delay);

            float elapsedTime = 0f;
            float duration = 0.4f;

            while (elapsedTime < duration)
            {
                float progress = elapsedTime / duration;
                buttonObj.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, progress);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            buttonObj.transform.localScale = Vector3.one;
        }

        public void SelectFurnitureSet(FurnitureSet furnitureSet)
        {
            currentSelectedSet = furnitureSet;
            HideAllFurniture();
            ShowFurnitureSet(furnitureSet);
            StartGameplay();
        }

        private void HideAllFurniture()
        {
            foreach (FurnitureSet set in furnitureSets)
            {
                foreach (GameObject piece in set.pieces)
                {
                    if (piece != null)
                        piece.SetActive(false);
                }
            }
        }

        private void ShowFurnitureSet(FurnitureSet furnitureSet)
        {
            foreach (GameObject piece in furnitureSet.pieces)
            {
                if (piece != null)
                    piece.SetActive(true);
            }

            Debug.Log($"Showing furniture set: {furnitureSet.name} with {furnitureSet.pieces.Count} pieces");
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
                modelSelectionAnimator?.AnimateIn();
                currentState = MenuState.ModelSelection;
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

        private void ShowRestartButton()
        {
            if (restartButtonSetup != null)
            {
                restartButtonSetup.ClearRestartButton();
                restartButtonSetup.CreateRestartButton();
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
            HideRestartButton();
            HideAllFurniture();
            currentSelectedSet = null;
            ShowMainMenu();
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