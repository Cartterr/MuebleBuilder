using UnityEngine;
using UnityEngine.XR;

namespace VRProject
{
    public class RecenterOriginPoint : MonoBehaviour
    {
        [Header("Recenter Settings")]
        [Tooltip("If true, this origin point will be set as the default recenter position on start")]
        public bool setAsDefaultOnStart = true;

        [Tooltip("If true, shows visual indicators for the origin point")]
        public bool showVisualIndicators = true;

        [Header("Manual Recenter")]
        [Tooltip("Key to manually recenter to this position (for testing in editor)")]
        public KeyCode manualRecenterKey = KeyCode.R;

        private Transform xrOrigin;
        private Transform cameraTransform;

        private void Start()
        {
            FindXRComponents();

            if (setAsDefaultOnStart)
            {
                SetAsRecenterOrigin();
            }

            if (!showVisualIndicators)
            {
                HideVisualIndicators();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(manualRecenterKey))
            {
                RecenterToThisPosition();
            }
        }

        private void FindXRComponents()
        {
            GameObject xrOriginObj = GameObject.Find("XR Origin (XR Rig)");
            if (xrOriginObj == null)
                xrOriginObj = GameObject.Find("XROrigin");

            if (xrOriginObj != null)
            {
                xrOrigin = xrOriginObj.transform;

                Camera mainCamera = Camera.main;
                if (mainCamera == null)
                    mainCamera = FindObjectOfType<Camera>();

                if (mainCamera != null)
                {
                    cameraTransform = mainCamera.transform;
                }

                Debug.Log($"🎯 XR Origin found: {xrOrigin.name}, Camera: {cameraTransform?.name}");
            }
            else
            {
                Debug.LogWarning("⚠️ XR Origin not found! Recenter functionality may not work properly.");
            }
        }

        public void SetAsRecenterOrigin()
        {
            if (xrOrigin != null)
            {
                Vector3 targetPosition = transform.position;
                Quaternion targetRotation = transform.rotation;

                Vector3 cameraOffset = Vector3.zero;
                if (cameraTransform != null)
                {
                    cameraOffset = cameraTransform.position - xrOrigin.position;
                    cameraOffset.y = 0;
                }

                Vector3 newOriginPosition = targetPosition - cameraOffset;

                xrOrigin.position = newOriginPosition;
                xrOrigin.rotation = targetRotation;

                Debug.Log($"🎯 Set recenter origin to position: {targetPosition}, rotation: {targetRotation.eulerAngles}");
            }
        }

        public void RecenterToThisPosition()
        {
            if (xrOrigin != null && cameraTransform != null)
            {
                Vector3 currentCameraPosition = cameraTransform.position;
                Vector3 targetPosition = transform.position;
                Quaternion targetRotation = transform.rotation;

                Vector3 cameraOffset = currentCameraPosition - xrOrigin.position;
                cameraOffset.y = 0;

                Vector3 newOriginPosition = targetPosition - cameraOffset;

                xrOrigin.position = newOriginPosition;

                float currentCameraY = cameraTransform.eulerAngles.y;
                float targetY = targetRotation.eulerAngles.y;
                float rotationDifference = targetY - currentCameraY;

                xrOrigin.Rotate(0, rotationDifference, 0);

                Debug.Log($"🎯 Recentered to origin point: {targetPosition}");

                TriggerXRRecenter();
            }
        }

        private void TriggerXRRecenter()
        {
            if (XRSettings.enabled)
            {
                InputTracking.Recenter();
                Debug.Log("🎯 Triggered XR InputTracking.Recenter()");
            }
        }

        private void HideVisualIndicators()
        {
            Transform indicator = transform.Find("OriginIndicator");
            if (indicator != null)
            {
                indicator.gameObject.SetActive(false);
            }
        }

        public void ShowVisualIndicators()
        {
            Transform indicator = transform.Find("OriginIndicator");
            if (indicator != null)
            {
                indicator.gameObject.SetActive(true);
            }
            showVisualIndicators = true;
        }

        [ContextMenu("🎯 Recenter to This Position")]
        public void ManualRecenter()
        {
            RecenterToThisPosition();
        }

        [ContextMenu("📍 Set Current Position as Origin")]
        public void SetCurrentPositionAsOrigin()
        {
            if (cameraTransform != null)
            {
                transform.position = cameraTransform.position;
                transform.rotation = Quaternion.LookRotation(cameraTransform.forward, Vector3.up);
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);

                Debug.Log($"🎯 Set origin to current camera position: {transform.position}");
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.5f);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * 1f);

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, transform.up * 0.5f);
        }
    }
}