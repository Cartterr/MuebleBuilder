using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace VRProject
{
    public class FurnitureInstructionManager : MonoBehaviour
    {
        [Header("Smart Instruction System")]
        public InstructionPanelController instructionPanelController;
        public List<InstructionData> instructionDatabase = new List<InstructionData>();

        [Header("Dynamic Tracking")]
        public float scanInterval = 1f;

        private List<XRGrabInteractable> trackedFurniture = new List<XRGrabInteractable>();
        private Dictionary<string, bool> lastGrabStates = new Dictionary<string, bool>();

        void Start()
        {
            Initialize();
        }

        void Initialize()
        {
            if (instructionPanelController == null)
            {
                instructionPanelController = FindObjectOfType<InstructionPanelController>();
            }

            StartCoroutine(ContinuousFurnitureScan());
            Debug.Log("🧠 FurnitureInstructionManager initialized - Starting smart furniture tracking!");
        }

        System.Collections.IEnumerator ContinuousFurnitureScan()
        {
            while (true)
            {
                yield return new WaitForSeconds(scanInterval);
                ScanForNewFurniture();
            }
        }

        void ScanForNewFurniture()
        {
            GameObject[] allSpawnedFurniture = GameObject.FindObjectsOfType<GameObject>()
                .Where(obj => obj.name.Contains("furniture_") && obj.name.Contains("_spawned"))
                .ToArray();

            foreach (GameObject furniture in allSpawnedFurniture)
            {
                XRGrabInteractable grabInteractable = furniture.GetComponent<XRGrabInteractable>();
                if (grabInteractable == null)
                {
                    grabInteractable = furniture.GetComponentInChildren<XRGrabInteractable>();
                }

                if (grabInteractable != null && !trackedFurniture.Contains(grabInteractable))
                {
                    trackedFurniture.Add(grabInteractable);
                    lastGrabStates[furniture.name] = false;

                    grabInteractable.selectEntered.AddListener((args) => OnFurnitureGrabbed(args, furniture.name));
                    grabInteractable.selectExited.AddListener((args) => OnFurnitureReleased(args, furniture.name));

                    Debug.Log($"🎯 Now tracking furniture: {furniture.name}");
                }
            }

            CheckForMissingComponents();
        }

        void CheckForMissingComponents()
        {
            GameObject[] allSpawnedFurniture = GameObject.FindObjectsOfType<GameObject>()
                .Where(obj => obj.name.Contains("furniture_") && obj.name.Contains("_spawned"))
                .ToArray();

            foreach (GameObject furniture in allSpawnedFurniture)
            {
                XRGrabInteractable grabInteractable = furniture.GetComponent<XRGrabInteractable>();
                if (grabInteractable == null)
                {
                    Debug.Log($"🔧 Adding missing XRGrabInteractable to {furniture.name}");
                    grabInteractable = furniture.AddComponent<XRGrabInteractable>();

                    grabInteractable.selectEntered.AddListener((args) => OnFurnitureGrabbed(args, furniture.name));
                    grabInteractable.selectExited.AddListener((args) => OnFurnitureReleased(args, furniture.name));

                    if (!trackedFurniture.Contains(grabInteractable))
                    {
                        trackedFurniture.Add(grabInteractable);
                        lastGrabStates[furniture.name] = false;
                    }
                }

                Rigidbody rb = furniture.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    Debug.Log($"🔧 Adding missing Rigidbody to {furniture.name}");
                    rb = furniture.AddComponent<Rigidbody>();
                }

                Collider collider = furniture.GetComponent<Collider>();
                if (collider == null)
                {
                    BoxCollider boxCollider = furniture.AddComponent<BoxCollider>();

                    Renderer renderer = furniture.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        boxCollider.size = renderer.bounds.size;
                    }
                    Debug.Log($"🔧 Added missing Collider to {furniture.name}");
                }
            }
        }

        void OnFurnitureGrabbed(SelectEnterEventArgs args, string furnitureName)
        {
            Debug.Log($"🎯 FURNITURE GRABBED VIA VR CONTROLLER: {furnitureName}");

            if (instructionPanelController != null)
            {
                instructionPanelController.ShowInstructionForObject(furnitureName);
                Debug.Log($"📝 Showing VR instruction for: {furnitureName}");
            }
            else
            {
                Debug.LogWarning("⚠️ InstructionPanelController not found!");
                instructionPanelController = FindObjectOfType<InstructionPanelController>();
                if (instructionPanelController != null)
                {
                    instructionPanelController.ShowInstructionForObject(furnitureName);
                }
            }

            lastGrabStates[furnitureName] = true;
        }

        void OnFurnitureReleased(SelectExitEventArgs args, string furnitureName)
        {
            Debug.Log($"🎯 FURNITURE RELEASED: {furnitureName}");
            lastGrabStates[furnitureName] = false;

            if (instructionPanelController != null)
            {
                instructionPanelController.HideInstruction();
                Debug.Log($"📝 Hiding instruction for released furniture: {furnitureName}");
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("🔍 Manual furniture scan triggered");
                ScanForNewFurniture();
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                TestInstructionSystem();
            }
        }

        [ContextMenu("🔍 Force Scan Furniture")]
        public void ForceScanFurniture()
        {
            Debug.Log("🔍 Force scanning for furniture...");
            ScanForNewFurniture();

            Debug.Log($"📊 Currently tracking {trackedFurniture.Count} furniture objects:");
            foreach (var furniture in trackedFurniture)
            {
                if (furniture != null)
                    Debug.Log($"   - {furniture.gameObject.name}");
            }
        }

        [ContextMenu("🧪 Test Instruction System")]
        public void TestInstructionSystem()
        {
            if (instructionPanelController != null)
            {
                instructionPanelController.ShowInstruction("¡Sistema de instrucciones funcionando correctamente!", 3f);
                Debug.Log("🧪 Test instruction displayed");
            }
            else
            {
                Debug.LogWarning("⚠️ No InstructionPanelController found for testing!");
            }
        }

        [ContextMenu("🐛 Debug Tracking Status")]
        public void DebugTrackingStatus()
        {
            Debug.Log("🐛 === FURNITURE TRACKING DEBUG ===");
            Debug.Log($"Tracked furniture count: {trackedFurniture.Count}");
            Debug.Log($"Instruction database entries: {instructionDatabase.Count}");
            Debug.Log($"InstructionPanelController: {(instructionPanelController != null ? "Found" : "Missing")}");

            GameObject[] allSpawned = GameObject.FindObjectsOfType<GameObject>()
                .Where(obj => obj.name.Contains("furniture_") && obj.name.Contains("_spawned"))
                .ToArray();
            Debug.Log($"Total spawned furniture in scene: {allSpawned.Length}");

            foreach (GameObject furniture in allSpawned)
            {
                XRGrabInteractable grab = furniture.GetComponent<XRGrabInteractable>();
                Rigidbody rb = furniture.GetComponent<Rigidbody>();
                Collider col = furniture.GetComponent<Collider>();

                Debug.Log($"📋 {furniture.name}:");
                Debug.Log($"   XRGrabInteractable: {(grab != null ? "✅" : "❌")}");
                Debug.Log($"   Rigidbody: {(rb != null ? "✅" : "❌")}");
                Debug.Log($"   Collider: {(col != null ? "✅" : "❌")}");
            }
        }

        void OnDestroy()
        {
            foreach (var furniture in trackedFurniture)
            {
                if (furniture != null)
                {
                    furniture.selectEntered.RemoveAllListeners();
                    furniture.selectExited.RemoveAllListeners();
                }
            }
        }
    }
}