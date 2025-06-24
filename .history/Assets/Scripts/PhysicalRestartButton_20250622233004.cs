using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

namespace VRProject
{
    public class PhysicalRestartButton : XRBaseInteractable
    {
        [Header("Visual Components")]
        public Transform buttonTransform;
        public MeshRenderer buttonRenderer;
        public Material defaultMaterial;
        public Material pressedMaterial;
        public Material glowMaterial;

        [Header("Animation Settings")]
        public float pressDepth = 0.02f;
        public float animationSpeed = 10f;
        public AnimationCurve pressureCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Restart Settings")]
        public float restartDelay = 1f;
        public bool showCountdown = true;
        public bool returnToMenuInsteadOfRestart = true;

        [Header("Audio")]
        public AudioSource audioSource;
        public AudioClip pressSound;
        public AudioClip confirmSound;

        [Header("Effects")]
        public ParticleSystem pressEffect;
        public Light buttonLight;

        private Vector3 originalPosition;
        private Vector3 pressedPosition;
        private bool isPressed = false;
        private bool isRestarting = false;
        private Coroutine restartCoroutine;
        private Coroutine glowCoroutine;

        protected override void Awake()
        {
            base.Awake();

            if (buttonTransform == null)
                buttonTransform = transform;

            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();

            originalPosition = buttonTransform.localPosition;
            pressedPosition = originalPosition - Vector3.up * pressDepth;

            if (buttonRenderer == null)
                buttonRenderer = GetComponent<MeshRenderer>();

            if (buttonLight != null)
                buttonLight.enabled = false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            selectEntered.AddListener(OnButtonPressed);
            selectExited.AddListener(OnButtonReleased);
            hoverEntered.AddListener(OnHoverEnter);
            hoverExited.AddListener(OnHoverExit);
        }

        protected override void OnDisable()
        {
            selectEntered.RemoveListener(OnButtonPressed);
            selectExited.RemoveListener(OnButtonReleased);
            hoverEntered.RemoveListener(OnHoverEnter);
            hoverExited.RemoveListener(OnHoverExit);
            base.OnDisable();
        }

        private void OnButtonPressed(SelectEnterEventArgs args)
        {
            if (isRestarting) return;

            isPressed = true;
            StartCoroutine(AnimateButtonPress(true));

            if (audioSource != null && pressSound != null)
                audioSource.PlayOneShot(pressSound);

            if (pressEffect != null)
                pressEffect.Play();

            if (buttonRenderer != null && pressedMaterial != null)
                buttonRenderer.material = pressedMaterial;

            if (restartCoroutine != null)
                StopCoroutine(restartCoroutine);

            restartCoroutine = StartCoroutine(RestartWithDelay());
        }

        private void OnButtonReleased(SelectExitEventArgs args)
        {
            if (isRestarting) return;

            isPressed = false;
            StartCoroutine(AnimateButtonPress(false));

            if (buttonRenderer != null && defaultMaterial != null)
                buttonRenderer.material = defaultMaterial;

            if (restartCoroutine != null)
            {
                StopCoroutine(restartCoroutine);
                restartCoroutine = null;
            }
        }

        private void OnHoverEnter(HoverEnterEventArgs args)
        {
            if (isRestarting) return;

            if (glowCoroutine != null)
                StopCoroutine(glowCoroutine);

            glowCoroutine = StartCoroutine(AnimateGlow(true));
        }

        private void OnHoverExit(HoverExitEventArgs args)
        {
            if (isRestarting) return;

            if (glowCoroutine != null)
                StopCoroutine(glowCoroutine);

            glowCoroutine = StartCoroutine(AnimateGlow(false));
        }

        private IEnumerator AnimateButtonPress(bool pressing)
        {
            Vector3 startPos = buttonTransform.localPosition;
            Vector3 targetPos = pressing ? pressedPosition : originalPosition;
            float elapsedTime = 0f;
            float duration = 1f / animationSpeed;

            while (elapsedTime < duration)
            {
                float progress = elapsedTime / duration;
                float curveValue = pressureCurve.Evaluate(progress);

                buttonTransform.localPosition = Vector3.Lerp(startPos, targetPos, curveValue);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            buttonTransform.localPosition = targetPos;
        }

        private IEnumerator AnimateGlow(bool glowing)
        {
            if (buttonLight != null)
            {
                buttonLight.enabled = glowing;

                if (glowing)
                {
                    float startIntensity = 0f;
                    float targetIntensity = 1f;
                    float elapsedTime = 0f;
                    float duration = 0.3f;

                    while (elapsedTime < duration)
                    {
                        float progress = elapsedTime / duration;
                        buttonLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, progress);
                        elapsedTime += Time.deltaTime;
                        yield return null;
                    }
                    buttonLight.intensity = targetIntensity;
                }
                else
                {
                    buttonLight.enabled = false;
                }
            }

            if (buttonRenderer != null && glowMaterial != null && !isPressed)
            {
                buttonRenderer.material = glowing ? glowMaterial : defaultMaterial;
            }
        }

                private IEnumerator RestartWithDelay()
        {
            isRestarting = true;

            if (audioSource != null && confirmSound != null)
                audioSource.PlayOneShot(confirmSound);

            if (showCountdown)
            {
                string message = returnToMenuInsteadOfRestart ?
                    $"Volviendo al menú en {restartDelay} segundos..." :
                    $"Reiniciando en {restartDelay} segundos...";
                Debug.Log(message);

                // Optional: Display countdown UI here
                float remainingTime = restartDelay;
                while (remainingTime > 0)
                {
                    remainingTime -= Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                yield return new WaitForSeconds(restartDelay);
            }

            if (returnToMenuInsteadOfRestart)
            {
                // Return to menu instead of restarting scene
                ReturnToMenu();
            }
            else
            {
                // Restart the scene
                Scene currentScene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(currentScene.buildIndex);
            }
        }

        private void ReturnToMenu()
        {
            Debug.Log("🔙 PhysicalRestartButton: Returning to menu...");

            // Find the simple furniture selector and return to main menu
            SimpleFurnitureSelector simpleSelector = FindObjectOfType<SimpleFurnitureSelector>();
            if (simpleSelector != null)
            {
                Debug.Log("✅ Found SimpleFurnitureSelector, calling ReturnToMainMenu");
                simpleSelector.ReturnToMainMenu();
            }
            else
            {
                // Fallback: Try to find old FurnitureSelectionManager for compatibility
                FurnitureSelectionManager selectionManager = FindObjectOfType<FurnitureSelectionManager>();
                if (selectionManager != null)
                {
                    Debug.Log("✅ Found FurnitureSelectionManager, calling ReturnToMainMenu");
                    selectionManager.ReturnToMainMenu();
                }
                else
                {
                    Debug.LogWarning("⚠️ No menu manager found, restarting scene as fallback");
                    // Fallback to scene restart if no menu manager found
                    Scene currentScene = SceneManager.GetActiveScene();
                    SceneManager.LoadScene(currentScene.buildIndex);
                }
            }
        }

        public void RestartImmediately()
        {
            if (isRestarting) return;

            if (restartCoroutine != null)
                StopCoroutine(restartCoroutine);

            if (returnToMenuInsteadOfRestart)
            {
                ReturnToMenu();
            }
            else
            {
                Scene currentScene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(currentScene.buildIndex);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (buttonTransform == null) return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(buttonTransform.position, Vector3.one * 0.1f);

            Gizmos.color = Color.yellow;
            Vector3 pressedPos = buttonTransform.position - Vector3.up * pressDepth;
            Gizmos.DrawWireCube(pressedPos, Vector3.one * 0.08f);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(buttonTransform.position, pressedPos);
        }
    }
}