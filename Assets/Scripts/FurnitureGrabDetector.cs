using UnityEngine;
using UnityEngine.UI;

namespace VRProject
{
    public class FurnitureGrabDetector : MonoBehaviour
    {
        [Header("Grab Detection")]
        public InstructionPanelController instructionController;

        private bool isGrabbed = false;
        private Button buttonComponent;
        private BoxCollider boxCollider;

        void Start()
        {
            Initialize();
        }

        void Initialize()
        {
            if (instructionController == null)
            {
                instructionController = FindObjectOfType<InstructionPanelController>();
            }

            buttonComponent = GetComponent<Button>();
            if (buttonComponent == null)
            {
                buttonComponent = gameObject.AddComponent<Button>();
            }

            boxCollider = GetComponent<BoxCollider>();
            if (boxCollider == null)
            {
                boxCollider = gameObject.AddComponent<BoxCollider>();

                Renderer renderer = GetComponent<Renderer>();
                if (renderer != null)
                {
                    boxCollider.size = renderer.bounds.size;
                }
                else
                {
                    boxCollider.size = Vector3.one * 0.1f;
                }
            }

            if (buttonComponent != null)
            {
                buttonComponent.onClick.AddListener(OnFurnitureGrabbed);
            }

            Debug.Log($"📍 FurnitureGrabDetector initialized on {gameObject.name}");
        }

        public void OnFurnitureGrabbed()
        {
            Debug.Log($"🎯 FURNITURE GRABBED: {gameObject.name}");

            if (instructionController != null)
            {
                instructionController.ShowInstructionForObject(gameObject.name);
                Debug.Log($"📝 Showing instruction for grabbed furniture: {gameObject.name}");
            }
            else
            {
                Debug.LogWarning($"⚠️ No InstructionPanelController found for {gameObject.name}!");
                instructionController = FindObjectOfType<InstructionPanelController>();
                if (instructionController != null)
                {
                    instructionController.ShowInstructionForObject(gameObject.name);
                    Debug.Log($"📝 Found controller and showing instruction for: {gameObject.name}");
                }
            }
        }

        void OnMouseDown()
        {
            OnFurnitureGrabbed();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.name.Contains("Hand") || other.name.Contains("Controller"))
            {
                OnFurnitureGrabbed();
            }
        }

        [ContextMenu("🧪 Test Furniture Grab")]
        public void TestFurnitureGrab()
        {
            OnFurnitureGrabbed();
        }
    }
}