using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.EventSystems;

namespace VRProject
{
    public class SimpleUIRaySetup : MonoBehaviour
    {
        [ContextMenu("🎯 SETUP UI RAY INTERACTION")]
        public void SetupUIRayInteraction()
        {
            SetupEventSystem();
            SetupXRUIInputModule();
            EnableRayInteractorUI();
            Debug.Log("✅ UI Ray interaction setup complete!");
        }

        private void SetupEventSystem()
        {
            EventSystem eventSystem = FindObjectOfType<EventSystem>();
            if (eventSystem == null)
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystem = eventSystemObj.AddComponent<EventSystem>();
                Debug.Log("✅ Created EventSystem");
            }

            // Remove standard input module if present
            StandaloneInputModule standaloneInput = eventSystem.GetComponent<StandaloneInputModule>();
            if (standaloneInput != null)
            {
                if (Application.isPlaying)
                    Destroy(standaloneInput);
                else
                    DestroyImmediate(standaloneInput);
            }
        }

        private void SetupXRUIInputModule()
        {
            EventSystem eventSystem = FindObjectOfType<EventSystem>();
            XRUIInputModule xrInputModule = eventSystem.GetComponent<XRUIInputModule>();

            if (xrInputModule == null)
            {
                xrInputModule = eventSystem.gameObject.AddComponent<XRUIInputModule>();
                Debug.Log("✅ Added XRUIInputModule");
            }
        }

        private void EnableRayInteractorUI()
        {
            XRRayInteractor[] rayInteractors = FindObjectsOfType<XRRayInteractor>();

            foreach (XRRayInteractor rayInteractor in rayInteractors)
            {
                // Make sure ray interactor can interact with UI
                rayInteractor.enableUIInteraction = true;

                // Ensure it has a line renderer for visual feedback
                LineRenderer lineRenderer = rayInteractor.GetComponent<LineRenderer>();
                if (lineRenderer == null)
                {
                    lineRenderer = rayInteractor.gameObject.AddComponent<LineRenderer>();
                    SetupLineRenderer(lineRenderer);
                }

                // Make sure it has an XR Interactor Line Visual
                XRInteractorLineVisual lineVisual = rayInteractor.GetComponent<XRInteractorLineVisual>();
                if (lineVisual == null)
                {
                    lineVisual = rayInteractor.gameObject.AddComponent<XRInteractorLineVisual>();
                    lineVisual.lineWidth = 0.005f;
                }

                Debug.Log($"✅ Configured ray interactor: {rayInteractor.name}");
            }

            if (rayInteractors.Length == 0)
            {
                Debug.LogWarning("⚠️ No XRRayInteractor found! Make sure you have XR controllers setup.");
            }
        }

        private void SetupLineRenderer(LineRenderer lineRenderer)
        {
            // Create a simple material for the laser
            Material laserMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            laserMaterial.color = Color.cyan;
            laserMaterial.name = "LaserPointer";

            lineRenderer.material = laserMaterial;
            lineRenderer.startWidth = 0.005f;
            lineRenderer.endWidth = 0.005f;
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;
        }

        [ContextMenu("🔍 Check Current Setup")]
        public void CheckCurrentSetup()
        {
            Debug.Log("=== UI RAY SETUP CHECK ===");

            // Check EventSystem
            EventSystem eventSystem = FindObjectOfType<EventSystem>();
            if (eventSystem != null)
            {
                Debug.Log("✅ EventSystem found");

                XRUIInputModule xrInput = eventSystem.GetComponent<XRUIInputModule>();
                if (xrInput != null)
                    Debug.Log("✅ XRUIInputModule found");
                else
                    Debug.LogWarning("⚠️ XRUIInputModule missing");
            }
            else
            {
                Debug.LogError("❌ EventSystem missing");
            }

            // Check Ray Interactors
            XRRayInteractor[] rayInteractors = FindObjectsOfType<XRRayInteractor>();
            Debug.Log($"Found {rayInteractors.Length} XRRayInteractor(s)");

            foreach (XRRayInteractor ray in rayInteractors)
            {
                Debug.Log($"Ray '{ray.name}' - UI Interaction: {ray.enableUIInteraction}");
            }

            // Check Canvas
            Canvas[] canvases = FindObjectsOfType<Canvas>();
            foreach (Canvas canvas in canvases)
            {
                if (canvas.renderMode == RenderMode.WorldSpace)
                {
                    GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
                    Debug.Log($"Canvas '{canvas.name}' - WorldSpace: ✅, GraphicRaycaster: {(raycaster != null ? "✅" : "❌")}");
                }
            }

            Debug.Log("=== CHECK COMPLETE ===");
        }

        private void Start()
        {
            // Auto-setup on start
            Invoke(nameof(SetupUIRayInteraction), 0.5f);
        }
    }
}