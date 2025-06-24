using UnityEngine;

namespace VRProject
{
    public class FurnitureVisibilityController : MonoBehaviour
    {
        [Header("Auto-Hide Settings")]
        [Tooltip("If true, this furniture will be hidden when the scene starts")]
        public bool hideOnStart = true;

        [Tooltip("The furniture set this piece belongs to (e.g., '1' for furniture_1_x pieces)")]
        public string furnitureSetNumber = "";

        private void Start()
        {
            if (hideOnStart)
            {
                HideFurniture();

                if (string.IsNullOrEmpty(furnitureSetNumber))
                {
                    AutoDetectFurnitureSet();
                }
            }
        }

        private void AutoDetectFurnitureSet()
        {
            if (gameObject.name.StartsWith("furniture_"))
            {
                string[] parts = gameObject.name.Split('_');
                if (parts.Length >= 2)
                {
                    furnitureSetNumber = parts[1];
                    Debug.Log($"Auto-detected furniture set: {furnitureSetNumber} for {gameObject.name}");
                }
            }
        }

        public void HideFurniture()
        {
            gameObject.SetActive(false);
        }

        public void ShowFurniture()
        {
            gameObject.SetActive(true);
        }

        public bool BelongsToSet(string setNumber)
        {
            return furnitureSetNumber == setNumber;
        }
    }
}