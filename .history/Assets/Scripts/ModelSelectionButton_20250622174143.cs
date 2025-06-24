using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VRProject
{
    public class ModelSelectionButton : MonoBehaviour
    {
        [Header("UI Components")]
        public Image previewImage;
        public TextMeshProUGUI modelNameText;
        public TextMeshProUGUI descriptionText;
        public Button selectButton;

        private ModelData modelData;
        private MenuManager menuManager;

        public void SetupButton(ModelData data, MenuManager manager)
        {
            modelData = data;
            menuManager = manager;

            if (previewImage != null && data.previewImage != null)
                previewImage.sprite = data.previewImage;

            if (modelNameText != null)
                modelNameText.text = data.modelName;

            if (descriptionText != null)
                descriptionText.text = data.description;

            if (selectButton != null)
                selectButton.onClick.AddListener(OnSelectModel);
        }

        private void OnSelectModel()
        {
            if (menuManager != null && modelData != null)
            {
                menuManager.SelectModel(modelData);
            }
            else
            {
                var animatedManager = FindObjectOfType<AnimatedMenuManager>();
                if (animatedManager != null && modelData != null)
                {
                    animatedManager.SelectModel(modelData);
                }
            }
        }
    }
}