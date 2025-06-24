using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace VRProject
{
    public class MenuManager : MonoBehaviour
    {
        [Header("UI Panels")]
        public GameObject mainMenuPanel;
        public GameObject modelSelectionPanel;
        public GameObject gameplayPanel;

        [Header("Buttons")]
        public Button iniciarButton;
        public Button salirButton;
        public Button backToMenuButton;

        [Header("Model Selection")]
        public Transform modelGridParent;
        public GameObject modelButtonPrefab;
        public List<ModelData> availableModels;

        [Header("Gameplay")]
        public Transform spawnPoint;

        private void Start()
        {
            SetupUI();
            ShowMainMenu();
        }

        private void SetupUI()
        {
            if (iniciarButton != null)
                iniciarButton.onClick.AddListener(ShowModelSelection);

            if (salirButton != null)
                salirButton.onClick.AddListener(ExitApplication);

            if (backToMenuButton != null)
                backToMenuButton.onClick.AddListener(ShowMainMenu);
        }

        public void ShowMainMenu()
        {
            mainMenuPanel?.SetActive(true);
            modelSelectionPanel?.SetActive(false);
            gameplayPanel?.SetActive(false);
        }

        public void ShowModelSelection()
        {
            mainMenuPanel?.SetActive(false);
            modelSelectionPanel?.SetActive(true);
            gameplayPanel?.SetActive(false);

            PopulateModelGrid();
        }

        public void StartGameplay()
        {
            mainMenuPanel?.SetActive(false);
            modelSelectionPanel?.SetActive(false);
            gameplayPanel?.SetActive(true);
        }

        private void PopulateModelGrid()
        {
            foreach (Transform child in modelGridParent)
            {
                Destroy(child.gameObject);
            }

            foreach (ModelData model in availableModels)
            {
                GameObject buttonObj = Instantiate(modelButtonPrefab, modelGridParent);
                ModelSelectionButton buttonScript = buttonObj.GetComponent<ModelSelectionButton>();

                if (buttonScript != null)
                {
                    buttonScript.SetupButton(model, this);
                }
            }
        }

        public void SelectModel(ModelData model)
        {
            if (model.prefab != null && spawnPoint != null)
            {
                Instantiate(model.prefab, spawnPoint.position, spawnPoint.rotation);
            }
            StartGameplay();
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

    [System.Serializable]
    public class ModelData
    {
        public string modelName;
        public GameObject prefab;
        public Sprite previewImage;
        public string description;
    }
}