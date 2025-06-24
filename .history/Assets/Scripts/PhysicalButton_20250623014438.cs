using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

namespace VRProject
{
    public class PhysicalButton : XRSimpleInteractable
    {
        [Header("Visual Effects")]
        public float pressScale = 0.9f;
        public float animationSpeed = 10f;

        [Header("Audio")]
        public AudioSource audioSource;
        public AudioClip pressSound;

        private Vector3 originalScale;
        private Button button;
        private MeshRenderer meshRenderer;
        private Material buttonMaterial;
        private Color originalColor;
        private Color pressedColor;
        private Color hoverColor;

        protected override void Awake()
        {
            base.Awake();

            originalScale = transform.localScale;
            button = GetComponent<Button>();
            meshRenderer = GetComponent<MeshRenderer>();

            if (meshRenderer != null)
            {
                buttonMaterial = meshRenderer.material;
                originalColor = buttonMaterial.color;
                pressedColor = new Color(originalColor.r * 0.7f, originalColor.g * 0.7f, originalColor.b * 0.7f, originalColor.a);
                hoverColor = new Color(originalColor.r * 1.2f, originalColor.g * 1.2f, originalColor.b * 1.2f, originalColor.a);
            }

            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
        }

        protected override void OnHoverEntered(HoverEnterEventArgs args)
        {
            base.OnHoverEntered(args);
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(AnimateToColor(hoverColor));
                StartCoroutine(AnimateToScale(originalScale * 1.05f));
            }
        }

        protected override void OnHoverExited(HoverExitEventArgs args)
        {
            base.OnHoverExited(args);
            if (!isSelected && gameObject.activeInHierarchy)
            {
                StartCoroutine(AnimateToColor(originalColor));
                StartCoroutine(AnimateToScale(originalScale));
            }
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(AnimateToColor(pressedColor));
                StartCoroutine(AnimateToScale(originalScale * pressScale));
            }
            PlayPressSound();
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);

            if (button != null && button.interactable)
            {
                button.onClick.Invoke();
                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(ButtonPressEffect());
                }
            }

            if (gameObject.activeInHierarchy)
            {
                if (isHovered)
                {
                    StartCoroutine(AnimateToColor(hoverColor));
                    StartCoroutine(AnimateToScale(originalScale * 1.05f));
                }
                else
                {
                    StartCoroutine(AnimateToColor(originalColor));
                    StartCoroutine(AnimateToScale(originalScale));
                }
            }
        }

        private IEnumerator AnimateToScale(Vector3 targetScale)
        {
            while (Vector3.Distance(transform.localScale, targetScale) > 0.01f)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
                yield return null;
            }
            transform.localScale = targetScale;
        }

        private IEnumerator AnimateToColor(Color targetColor)
        {
            if (buttonMaterial == null) yield break;

            while (Mathf.Abs(buttonMaterial.color.r - targetColor.r) > 0.01f ||
                   Mathf.Abs(buttonMaterial.color.g - targetColor.g) > 0.01f ||
                   Mathf.Abs(buttonMaterial.color.b - targetColor.b) > 0.01f)
            {
                buttonMaterial.color = Color.Lerp(buttonMaterial.color, targetColor, Time.deltaTime * animationSpeed);
                yield return null;
            }
            buttonMaterial.color = targetColor;
        }

        private IEnumerator ButtonPressEffect()
        {
            Vector3 flashScale = originalScale * 1.1f;
            Color flashColor = Color.white;

            transform.localScale = flashScale;
            if (buttonMaterial != null)
                buttonMaterial.color = flashColor;

            yield return new WaitForSeconds(0.1f);

            float elapsed = 0f;
            float duration = 0.2f;

            while (elapsed < duration)
            {
                float progress = elapsed / duration;
                transform.localScale = Vector3.Lerp(flashScale, originalScale, progress);

                if (buttonMaterial != null)
                {
                    buttonMaterial.color = Color.Lerp(flashColor, originalColor, progress);
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localScale = originalScale;
            if (buttonMaterial != null)
                buttonMaterial.color = originalColor;
        }

        private void PlayPressSound()
        {
            if (audioSource != null && pressSound != null)
            {
                audioSource.PlayOneShot(pressSound);
            }
        }
    }
}