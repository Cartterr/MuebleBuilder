using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace VRProject
{
    public class ModernButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Scale Effects")]
        public float hoverScale = 1.1f;
        public float clickScale = 0.95f;
        public float animationSpeed = 8f;

        [Header("Color Effects")]
        public bool useColorTransition = true;
        public Color normalColor = Color.white;
        public Color hoverColor = new Color(0.9f, 0.9f, 1f, 1f);
        public Color clickColor = new Color(0.8f, 0.8f, 0.9f, 1f);

        [Header("Glow Effects")]
        public bool useGlowEffect = true;
        public Image glowImage;
        public float glowIntensity = 0.5f;

        [Header("Audio")]
        public AudioSource audioSource;
        public AudioClip hoverSound;
        public AudioClip clickSound;

        private Vector3 originalScale;
        private Vector3 targetScale;
        private Color targetColor;
        private Image buttonImage;
        private Button button;
        private bool isPressed = false;
        private bool isHovered = false;
        private Coroutine animationCoroutine;

        private void Awake()
        {
            originalScale = transform.localScale;
            targetScale = originalScale;

            buttonImage = GetComponent<Image>();
            button = GetComponent<Button>();

            if (buttonImage != null)
            {
                normalColor = buttonImage.color;
                targetColor = normalColor;
            }

            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();

            if (glowImage != null && useGlowEffect)
            {
                var glowCanvasGroup = glowImage.GetComponent<CanvasGroup>();
                if (glowCanvasGroup == null)
                    glowCanvasGroup = glowImage.gameObject.AddComponent<CanvasGroup>();
                glowCanvasGroup.alpha = 0f;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (button != null && !button.interactable) return;

            isHovered = true;
            if (!isPressed)
            {
                targetScale = originalScale * hoverScale;
                if (useColorTransition)
                    targetColor = hoverColor;
            }

            if (useGlowEffect && glowImage != null)
            {
                StartCoroutine(AnimateGlow(glowIntensity));
            }

            PlaySound(hoverSound);
            StartAnimation();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;
            if (!isPressed)
            {
                targetScale = originalScale;
                if (useColorTransition)
                    targetColor = normalColor;
            }

            if (useGlowEffect && glowImage != null)
            {
                StartCoroutine(AnimateGlow(0f));
            }

            StartAnimation();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (button != null && !button.interactable) return;

            isPressed = true;
            targetScale = originalScale * clickScale;
            if (useColorTransition)
                targetColor = clickColor;

            PlaySound(clickSound);
            StartAnimation();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPressed = false;

            if (isHovered)
            {
                targetScale = originalScale * hoverScale;
                if (useColorTransition)
                    targetColor = hoverColor;
            }
            else
            {
                targetScale = originalScale;
                if (useColorTransition)
                    targetColor = normalColor;
            }

            StartAnimation();
        }

        private void StartAnimation()
        {
            if (animationCoroutine != null)
                StopCoroutine(animationCoroutine);

            animationCoroutine = StartCoroutine(AnimateButton());
        }

        private IEnumerator AnimateButton()
        {
                        while (Vector3.Distance(transform.localScale, targetScale) > 0.01f ||
                   (buttonImage != null && useColorTransition &&
                    GetColorDistance(buttonImage.color, targetColor) > 0.01f))
            {
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale,
                    Time.deltaTime * animationSpeed);

                if (buttonImage != null && useColorTransition)
                {
                    buttonImage.color = Color.Lerp(buttonImage.color, targetColor,
                        Time.deltaTime * animationSpeed);
                }

                yield return null;
            }

            transform.localScale = targetScale;
            if (buttonImage != null && useColorTransition)
                buttonImage.color = targetColor;
        }

        private IEnumerator AnimateGlow(float targetAlpha)
        {
            if (glowImage == null) yield break;

            var canvasGroup = glowImage.GetComponent<CanvasGroup>();
            if (canvasGroup == null) yield break;

            float startAlpha = canvasGroup.alpha;
            float elapsedTime = 0f;
            float duration = 0.3f;

            while (elapsedTime < duration)
            {
                float progress = elapsedTime / duration;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
        }

        private void PlaySound(AudioClip clip)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }

        private float GetColorDistance(Color a, Color b)
        {
            return Mathf.Abs(a.r - b.r) + Mathf.Abs(a.g - b.g) + Mathf.Abs(a.b - b.b) + Mathf.Abs(a.a - b.a);
        }

        private void OnDisable()
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
                animationCoroutine = null;
            }

            transform.localScale = originalScale;
            if (buttonImage != null)
                buttonImage.color = normalColor;
        }
    }
}