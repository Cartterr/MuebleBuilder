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
                .Where(obj => obj.name.StartsWith("furniture_"))
                .ToArray();

            Debug.Log($"🔍 Found {allFurniture.Length} furniture objects to track");

            foreach (GameObject furniture in allFurniture)
            {
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
                    Debug.Log($"🎯 Now tracking: {furniture.name}");
                }
            }

            Debug.Log($"✅ Successfully tracking {trackedInteractables.Count} furniture pieces");
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
            InstructionData instruction = instructionDatabase.FirstOrDefault(i =>
                objectName.Contains(i.objectName) || i.objectName.Contains(objectName));

            if (instruction != null)
            {
                ShowInstruction(instruction.instructionText, instruction.displayDuration);
                Debug.Log($"📝 Showing instruction for {objectName}: {instruction.instructionText}");
            }
            else
            {
                string fallbackText = GetFallbackInstruction(objectName);
                ShowInstruction(fallbackText, 3f);
                Debug.Log($"📝 No specific instruction found for {objectName}, using fallback");
            }
        }

        string GetFallbackInstruction(string objectName)
        {
            if (objectName.Contains("furniture_"))
            {
                if (objectName.Contains("_1"))
                    return "Place this piece carefully to start building";
                else if (objectName.Contains("_2"))
                    return "Connect this piece to the existing structure";
                else if (objectName.Contains("_3"))
                    return "This is a finishing piece - position it precisely";
                else
                    return "Use this furniture piece to continue building";
            }
            return "Follow the visual guides to position this piece correctly";
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

            UpdatePanelPosition();

            float elapsedTime = 0f;
            Vector3 startScale = isPanelVisible ? transform.localScale : Vector3.zero;
            Vector3 targetScale = Vector3.one;
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

            yield return new WaitForSeconds(duration);

            yield return StartCoroutine(HideInstructionCoroutine());
        }

        System.Collections.IEnumerator HideInstructionCoroutine()
        {
            if (!isPanelVisible) yield break;

            float elapsedTime = 0f;
            float startAlpha = canvasGroup.alpha;
            Vector3 startScale = transform.localScale;
            Vector3 targetScale = Vector3.one * 0.8f;

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
            if (isPanelVisible)
            {
                UpdatePanelPosition();
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                TestInstructionPanel();
            }
        }

        [ContextMenu("🧪 Test Instruction Panel")]
        public void TestInstructionPanel()
        {
            ShowInstruction("This is a test instruction to verify the beautiful panel is working correctly!", 4f);
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