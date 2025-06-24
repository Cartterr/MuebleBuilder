using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace VRProject
{
    public class UIAnimator : MonoBehaviour
    {
        [Header("Animation Settings")]
        public float animationDuration = 0.3f;
        public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Scale Animation")]
        public Vector3 startScale = Vector3.zero;
        public Vector3 targetScale = Vector3.one;

        [Header("Fade Animation")]
        public bool animateFade = true;

        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();

            if (canvasGroup == null && animateFade)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        public void AnimateIn()
        {
            gameObject.SetActive(true);
            StartCoroutine(AnimateInCoroutine());
        }

        public void AnimateOut(System.Action onComplete = null)
        {
            StartCoroutine(AnimateOutCoroutine(onComplete));
        }

        private IEnumerator AnimateInCoroutine()
        {
            float elapsedTime = 0f;

            if (rectTransform != null)
                rectTransform.localScale = startScale;

            if (canvasGroup != null)
                canvasGroup.alpha = 0f;

            while (elapsedTime < animationDuration)
            {
                float progress = elapsedTime / animationDuration;

                if (rectTransform != null)
                {
                    Vector3 currentScale = Vector3.Lerp(startScale, targetScale, scaleCurve.Evaluate(progress));
                    rectTransform.localScale = currentScale;
                }

                if (canvasGroup != null && animateFade)
                {
                    canvasGroup.alpha = fadeCurve.Evaluate(progress);
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (rectTransform != null)
                rectTransform.localScale = targetScale;

            if (canvasGroup != null)
                canvasGroup.alpha = 1f;
        }

        private IEnumerator AnimateOutCoroutine(System.Action onComplete)
        {
            float elapsedTime = 0f;

            while (elapsedTime < animationDuration)
            {
                float progress = elapsedTime / animationDuration;
                float reverseProgress = 1f - progress;

                if (rectTransform != null)
                {
                    Vector3 currentScale = Vector3.Lerp(startScale, targetScale, scaleCurve.Evaluate(reverseProgress));
                    rectTransform.localScale = currentScale;
                }

                if (canvasGroup != null && animateFade)
                {
                    canvasGroup.alpha = fadeCurve.Evaluate(reverseProgress);
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (rectTransform != null)
                rectTransform.localScale = startScale;

            if (canvasGroup != null)
                canvasGroup.alpha = 0f;

            gameObject.SetActive(false);
            onComplete?.Invoke();
        }
    }
}