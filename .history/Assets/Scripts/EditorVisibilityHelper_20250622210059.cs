using UnityEngine;

namespace VRProject
{
    public class EditorVisibilityHelper : MonoBehaviour
    {
        [Header("Visibility Settings")]
        [Tooltip("If true, this object will be hidden when the game starts")]
        public bool hideAtRuntime = true;

        [Tooltip("Alpha value to use in editor for semi-transparent preview")]
        [Range(0.1f, 1f)]
        public float editorAlpha = 0.7f;

        private CanvasGroup canvasGroup;
        private bool wasVisibleInEditor;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            wasVisibleInEditor = gameObject.activeSelf;
        }

        private void Start()
        {
            if (Application.isPlaying && hideAtRuntime)
            {
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0f;
                    canvasGroup.interactable = false;
                    canvasGroup.blocksRaycasts = false;
                }

                UIAnimator animator = GetComponent<UIAnimator>();
                if (animator != null)
                {
                    animator.gameObject.SetActive(true);
                    animator.SetHidden();
                }
                else
                {
                    gameObject.SetActive(false);
                }

                Debug.Log($"🙈 Hidden {gameObject.name} at runtime (was visible in editor for positioning)");
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                CanvasGroup cg = GetComponent<CanvasGroup>();
                if (cg == null)
                {
                    cg = gameObject.AddComponent<CanvasGroup>();
                }

                if (hideAtRuntime)
                {
                    cg.alpha = editorAlpha;
                    cg.interactable = false;
                    cg.blocksRaycasts = false;
                }
                else
                {
                    cg.alpha = 1f;
                    cg.interactable = true;
                    cg.blocksRaycasts = true;
                }
            }
        }

        private void Reset()
        {
            hideAtRuntime = true;
            editorAlpha = 0.7f;
        }
#endif

        [ContextMenu("👁️ Show in Runtime")]
        public void ShowInRuntime()
        {
            if (Application.isPlaying)
            {
                gameObject.SetActive(true);
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f;
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                }
                Debug.Log($"👁️ Showing {gameObject.name} in runtime");
            }
        }

        [ContextMenu("🙈 Hide in Runtime")]
        public void HideInRuntime()
        {
            if (Application.isPlaying)
            {
                gameObject.SetActive(false);
                Debug.Log($"🙈 Hidden {gameObject.name} in runtime");
            }
        }

        public void SetVisibility(bool visible)
        {
            if (Application.isPlaying)
            {
                gameObject.SetActive(visible);
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = visible ? 1f : 0f;
                    canvasGroup.interactable = visible;
                    canvasGroup.blocksRaycasts = visible;
                }
            }
        }
    }
}