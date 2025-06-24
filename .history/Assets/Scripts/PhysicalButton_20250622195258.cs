using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace VRProject
{
    public class PhysicalButton : MonoBehaviour
    {
        [Header("Interaction")]
        public float pressScale = 0.9f;
        public float animationSpeed = 10f;

        [Header("Audio")]
        public AudioSource audioSource;
        public AudioClip pressSound;

        private Vector3 originalScale;
        private bool isPressed = false;
        private Button button;
        private Image buttonImage;
        private Color originalColor;
        private Color pressedColor;

        private void Awake()
        {
            originalScale = transform.localScale;
            button = GetComponent<Button>();
            buttonImage = GetComponent<Image>();

            if (buttonImage != null)
            {
                originalColor = buttonImage.color;
                pressedColor = new Color(originalColor.r * 0.8f, originalColor.g * 0.8f, originalColor.b * 0.8f, originalColor.a);
            }

            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.name.Contains("Controller") || other.name.Contains("Hand"))
            {
                PressButton();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player") || other.name.Contains("Controller") || other.name.Contains("Hand"))
            {
                if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2") ||
                    Input.GetKeyDown(KeyCode.Space) || CheckForGripInput())
                {
                    ExecuteButton();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") || other.name.Contains("Controller") || other.name.Contains("Hand"))
            {
                ReleaseButton();
            }
        }

        private bool CheckForGripInput()
        {
            return Input.GetAxis("Grip") > 0.5f ||
                   Input.GetButton("Grip") ||
                   Input.GetButtonDown("Joystick button 2") ||
                   Input.GetButtonDown("Joystick button 3");
        }

        private void PressButton()
        {
            if (isPressed || (button != null && !button.interactable)) return;

            isPressed = true;
            StartCoroutine(AnimatePress());
            PlayPressSound();
        }

        private void ReleaseButton()
        {
            if (!isPressed) return;

            isPressed = false;
            StartCoroutine(AnimateRelease());
        }

        private void ExecuteButton()
        {
            if (button != null && button.interactable)
            {
                button.onClick.Invoke();
                StartCoroutine(ButtonPressEffect());
            }
        }

        private IEnumerator AnimatePress()
        {
            Vector3 targetScale = originalScale * pressScale;

            while (Vector3.Distance(transform.localScale, targetScale) > 0.01f)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);

                if (buttonImage != null)
                {
                    buttonImage.color = Color.Lerp(buttonImage.color, pressedColor, Time.deltaTime * animationSpeed);
                }

                yield return null;
            }

            transform.localScale = targetScale;
            if (buttonImage != null)
                buttonImage.color = pressedColor;
        }

        private IEnumerator AnimateRelease()
        {
            while (Vector3.Distance(transform.localScale, originalScale) > 0.01f)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * animationSpeed);

                if (buttonImage != null)
                {
                    buttonImage.color = Color.Lerp(buttonImage.color, originalColor, Time.deltaTime * animationSpeed);
                }

                yield return null;
            }

            transform.localScale = originalScale;
            if (buttonImage != null)
                buttonImage.color = originalColor;
        }

        private IEnumerator ButtonPressEffect()
        {
            Vector3 flashScale = originalScale * 1.1f;

            transform.localScale = flashScale;
            if (buttonImage != null)
                buttonImage.color = Color.white;

            yield return new WaitForSeconds(0.1f);

            float elapsed = 0f;
            float duration = 0.2f;

            while (elapsed < duration)
            {
                float progress = elapsed / duration;
                transform.localScale = Vector3.Lerp(flashScale, originalScale, progress);

                if (buttonImage != null)
                {
                    buttonImage.color = Color.Lerp(Color.white, originalColor, progress);
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localScale = originalScale;
            if (buttonImage != null)
                buttonImage.color = originalColor;
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