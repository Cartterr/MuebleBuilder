using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VRProject
{
    public class InstructionPanelController : MonoBehaviour
    {
        [Header("UI References")]
        public TextMeshProUGUI instructionText;
        public CanvasGroup canvasGroup;

        [Header("Animation Settings")]
        [Range(0.1f, 2f)]
        public float fadeInDuration = 0.5f;
        [Range(0.1f, 2f)]
        public float fadeOutDuration = 0.3f;
        [Range(0.1f, 1f)]
        public float scaleAnimationDuration = 0.4f;

        [Header("Instruction Database")]
        public List<InstructionData> instructionDatabase = new List<InstructionData>();

        [Header("Detection Settings")]
        public float detectionRadius = 2f;
        public LayerMask furnitureLayerMask = -1;

        private Coroutine currentAnimation;
        private string currentlyDisplayedObject = "";
        private bool isPanelVisible = false;
        private Transform playerTransform;
        private List<XRGrabInteractable> trackedInteractables = new List<XRGrabInteractable>();

        void Start()
        {
            Initialize();
        }

        void Initialize()
        {
            if (canvasGroup == null)
                canvasGroup = GetComponentInChildren<CanvasGroup>();

            if (instructionText == null)
                instructionText = GetComponentInChildren<TextMeshProUGUI>();

            FindPlayerTransform();
            SetupFurnitureTracking();

            gameObject.SetActive(true);
            canvasGroup.alpha = 0f;
            transform.localScale = Vector3.zero;

            Debug.Log("✨ InstructionPanelController initialized!");
        }

        void FindPlayerTransform()
        {
            GameObject xrOrigin = GameObject.Find("XR Origin (XR Rig)");
            if (xrOrigin == null)
                xrOrigin = GameObject.Find("XROrigin");

            if (xrOrigin != null)
            {
                Camera playerCamera = xrOrigin.GetComponentInChildren<Camera>();
                if (playerCamera != null)
                    playerTransform = playerCamera.transform;
                else
                    playerTransform = xrOrigin.transform;
            }
            else
            {
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                    playerTransform = mainCamera.transform;
            }

            if (playerTransform != null)
                Debug.Log($"✅ Player transform found: {playerTransform.name}");
            else
                Debug.LogWarning("⚠️ Could not find player transform");
        }

        void SetupFurnitureTracking()
        {
            StartCoroutine(FindAndTrackFurniture());
        }

        System.Collections.IEnumerator FindAndTrackFurniture()
        {
            yield return new WaitForSeconds(1f);

            GameObject[] allFurniture = GameObject.FindObjectsOfType<GameObject>()
                .Where(obj => obj.name.Contains("furniture_") && obj.name.Contains("_spawned"))
                .ToArray();

            Debug.Log($"🔍 Found {allFurniture.Length} spawned furniture objects to track");

            foreach (GameObject furniture in allFurniture)
            {
                Debug.Log($"🔍 Checking components on {furniture.name}:");

                Component[] allComponents = furniture.GetComponents<Component>();
                foreach (Component comp in allComponents)
                {
                    Debug.Log($"   - {comp.GetType().Name}");
                }

                XRGrabInteractable grabInteractable = furniture.GetComponent<XRGrabInteractable>();
                if (grabInteractable == null)
                {
                    grabInteractable = furniture.GetComponentInChildren<XRGrabInteractable>();
                }

                if (grabInteractable != null && !trackedInteractables.Contains(grabInteractable))
                {
                    trackedInteractables.Add(grabInteractable);
                    grabInteractable.selectEntered.AddListener(OnObjectGrabbed);
                    grabInteractable.selectExited.AddListener(OnObjectReleased);
                    Debug.Log($"✅ Now tracking XRGrabInteractable on: {furniture.name}");
                }
                else
                {
                    Debug.LogWarning($"⚠️ No XRGrabInteractable found on {furniture.name}, trying alternative tracking...");
                    SetupAlternativeTracking(furniture);
                }
            }

            Debug.Log($"✅ Successfully tracking {trackedInteractables.Count} furniture pieces with XRGrabInteractable");
        }

        void SetupAlternativeTracking(GameObject furniture)
        {
            FurnitureTracker tracker = furniture.GetComponent<FurnitureTracker>();
            if (tracker == null)
            {
                tracker = furniture.AddComponent<FurnitureTracker>();
                tracker.instructionController = this;
                Debug.Log($"🔧 Added FurnitureTracker to {furniture.name}");
            }
        }

        void OnObjectGrabbed(SelectEnterEventArgs args)
        {
            GameObject grabbedObject = args.interactableObject.transform.gameObject;
            ShowInstructionForObject(grabbedObject.name);
        }

        void OnObjectReleased(SelectExitEventArgs args)
        {
            StartCoroutine(DelayedHideInstruction());
        }

        System.Collections.IEnumerator DelayedHideInstruction()
        {
            yield return new WaitForSeconds(0.5f);
            HideInstruction();
        }

                public void ShowInstructionForObject(string objectName)
        {
            Debug.Log($"🔍 Looking for instruction for object: {objectName}");

            InstructionData instruction = instructionDatabase.FirstOrDefault(i =>
                objectName.Equals(i.objectName, System.StringComparison.OrdinalIgnoreCase) ||
                objectName.Contains(i.objectName) ||
                i.objectName.Contains(objectName));

            if (instruction != null)
            {
                ShowInstruction(instruction.instructionText, instruction.displayDuration);
                Debug.Log($"📝 Showing instruction for {objectName}: {instruction.instructionText}");
            }
            else
            {
                string fallbackText = GetFallbackInstruction(objectName);
                ShowInstruction(fallbackText, 3f);
                Debug.Log($"📝 No specific instruction found for {objectName}, using fallback: {fallbackText}");
            }
        }

        string GetFallbackInstruction(string objectName)
        {
            Debug.Log($"🔄 Generating fallback instruction for: {objectName}");

            if (objectName.Contains("furniture_"))
            {
                if (objectName.Contains("_1_1"))
                    return "Coloca esta base de mesa en el área de construcción";
                else if (objectName.Contains("_1_2") || objectName.Contains("_1_3") || objectName.Contains("_1_4") || objectName.Contains("_1_5"))
                    return "Conecta esta pata a una esquina de la base de la mesa";
                else if (objectName.Contains("_1_6"))
                    return "Esta es tu mesa de construcción - úsala para construir";
                else if (objectName.Contains("_1"))
                    return "Coloca esta pieza cuidadosamente para construir";
                else if (objectName.Contains("_2"))
                    return "Conecta esta pieza a la estructura existente";
                else if (objectName.Contains("_3"))
                    return "Esta es una pieza de acabado - colócala con precisión";
                else
                    return "Usa esta pieza de mobiliario para continuar construyendo";
            }
            return "Sigue las guías visuales para posicionar esta pieza correctamente";
        }

        public void ShowInstruction(string text, float duration = 3f)
        {
            if (currentAnimation != null)
                StopCoroutine(currentAnimation);

            currentAnimation = StartCoroutine(ShowInstructionCoroutine(text, duration));
        }

        public void HideInstruction()
        {
            if (currentAnimation != null)
                StopCoroutine(currentAnimation);

            currentAnimation = StartCoroutine(HideInstructionCoroutine());
        }

                System.Collections.IEnumerator ShowInstructionCoroutine(string text, float duration)
        {
            instructionText.text = text;
            currentlyDisplayedObject = text;

                        float elapsedTime = 0f;
            Vector3 startScale = isPanelVisible ? transform.localScale : Vector3.zero;
            Vector3 targetScale = Vector3.one * 0.02f;
            float startAlpha = canvasGroup.alpha;

            while (elapsedTime < fadeInDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / fadeInDuration;

                float smoothT = Mathf.SmoothStep(0f, 1f, t);

                canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, smoothT);
                transform.localScale = Vector3.Lerp(startScale, targetScale, smoothT);

                yield return null;
            }

            canvasGroup.alpha = 1f;
            transform.localScale = targetScale;
            isPanelVisible = true;
        }

        System.Collections.IEnumerator HideInstructionCoroutine()
        {
            if (!isPanelVisible) yield break;

            float elapsedTime = 0f;
            float startAlpha = canvasGroup.alpha;
            Vector3 startScale = transform.localScale;
            Vector3 targetScale = Vector3.one * 0.008f;

            while (elapsedTime < fadeOutDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / fadeOutDuration;

                float smoothT = Mathf.SmoothStep(0f, 1f, t);

                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, smoothT);
                transform.localScale = Vector3.Lerp(startScale, targetScale, smoothT);

                yield return null;
            }

            canvasGroup.alpha = 0f;
            transform.localScale = Vector3.zero;
            isPanelVisible = false;
            currentlyDisplayedObject = "";
        }

        void UpdatePanelPosition()
        {
            if (playerTransform == null) return;

            Vector3 targetPosition = playerTransform.position +
                                   playerTransform.forward * 0.8f +
                                   Vector3.up * 0.3f;

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 2f);

            Vector3 lookDirection = (playerTransform.position - transform.position).normalized;
            lookDirection.y = 0f;

            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(-lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3f);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                TestInstructionPanel();
            }
        }

        [ContextMenu("🧪 Test Instruction Panel")]
        public void TestInstructionPanel()
        {
            ShowInstruction("¡Esta es una instrucción de prueba para verificar que el panel funciona correctamente!", 4f);
        }

        [ContextMenu("🔍 Refresh Furniture Tracking")]
        public void RefreshFurnitureTracking()
        {
            foreach (var interactable in trackedInteractables)
            {
                if (interactable != null)
                {
                    interactable.selectEntered.RemoveListener(OnObjectGrabbed);
                    interactable.selectExited.RemoveListener(OnObjectReleased);
                }
            }

            trackedInteractables.Clear();
            SetupFurnitureTracking();
        }

        [ContextMenu("🐛 Debug Panel Status")]
        public void DebugPanelStatus()
        {
            Debug.Log("🐛 === INSTRUCTION PANEL DEBUG ===");
            Debug.Log($"Panel GameObject: {gameObject.name} (Active: {gameObject.activeInHierarchy})");
            Debug.Log($"Canvas Group: {(canvasGroup != null ? $"Found (Alpha: {canvasGroup.alpha})" : "Missing")}");
            Debug.Log($"Instruction Text: {(instructionText != null ? $"Found (Text: '{instructionText.text}')" : "Missing")}");
            Debug.Log($"Player Transform: {(playerTransform != null ? playerTransform.name : "Missing")}");
            Debug.Log($"Panel Position: {transform.position}");
            Debug.Log($"Panel Scale: {transform.localScale}");
            Debug.Log($"Is Panel Visible: {isPanelVisible}");
            Debug.Log($"Tracked Interactables: {trackedInteractables.Count}");
            Debug.Log($"Instruction Database Entries: {instructionDatabase.Count}");

            FurnitureTracker[] trackers = FindObjectsOfType<FurnitureTracker>();
            Debug.Log($"FurnitureTracker Components Found: {trackers.Length}");

            foreach (var tracker in trackers)
            {
                Debug.Log($"  - {tracker.gameObject.name}");
            }
        }

        [ContextMenu("🎯 Force Show Test Instruction")]
        public void ForceShowTestInstruction()
        {
            Debug.Log("🎯 Force showing test instruction...");
            gameObject.SetActive(true);
            ShowInstruction("¡PRUEBA FORZADA! Si ves esto, el panel funciona correctamente.", 5f);
        }

        [ContextMenu("🔧 Fix Furniture Components")]
        public void FixFurnitureComponents()
        {
            GameObject[] allFurniture = GameObject.FindObjectsOfType<GameObject>()
                .Where(obj => obj.name.Contains("furniture_") && obj.name.Contains("_spawned"))
                .ToArray();

            Debug.Log($"🔧 Checking {allFurniture.Length} spawned furniture objects for missing components...");

            foreach (GameObject furniture in allFurniture)
            {
                XRGrabInteractable grabInteractable = furniture.GetComponent<XRGrabInteractable>();
                if (grabInteractable == null)
                {
                    Debug.Log($"🔧 Adding XRGrabInteractable to {furniture.name}");
                    grabInteractable = furniture.AddComponent<XRGrabInteractable>();

                    grabInteractable.selectEntered.AddListener(OnObjectGrabbed);
                    grabInteractable.selectExited.AddListener(OnObjectReleased);

                    if (!trackedInteractables.Contains(grabInteractable))
                    {
                        trackedInteractables.Add(grabInteractable);
                    }
                }
            }

            Debug.Log("✅ Furniture component fixing completed!");
            RefreshFurnitureTracking();
        }

        void OnDestroy()
        {
            foreach (var interactable in trackedInteractables)
            {
                if (interactable != null)
                {
                    interactable.selectEntered.RemoveListener(OnObjectGrabbed);
                    interactable.selectExited.RemoveListener(OnObjectReleased);
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            if (playerTransform != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(playerTransform.position, detectionRadius);

                Gizmos.color = Color.yellow;
                Vector3 panelPos = playerTransform.position + playerTransform.forward * 0.8f + Vector3.up * 0.3f;
                Gizmos.DrawWireCube(panelPos, new Vector3(0.8f, 0.2f, 0.1f));
            }
        }
    }
}