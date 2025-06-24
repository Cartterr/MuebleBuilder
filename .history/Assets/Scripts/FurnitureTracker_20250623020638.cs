using UnityEngine;

namespace VRProject
{
    public class FurnitureTracker : MonoBehaviour
    {
        [Header("Tracking")]
        public InstructionPanelController instructionController;

        private bool isGrabbed = false;
        private bool wasGrabbedLastFrame = false;
        private Rigidbody rb;
        private Vector3 lastPosition;
        private float grabThreshold = 0.1f;
        private float instructionShownTime = 0f;
        private bool instructionShown = false;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            lastPosition = transform.position;

            if (instructionController == null)
            {
                instructionController = FindObjectOfType<InstructionPanelController>();
            }

            Debug.Log($"📍 FurnitureTracker initialized on {gameObject.name}");
        }

        void Update()
        {
            CheckGrabState();
        }

        void CheckGrabState()
        {
            if (rb == null) return;

            float movementSpeed = Vector3.Distance(transform.position, lastPosition) / Time.deltaTime;
            bool isMovingFast = movementSpeed > grabThreshold;
            bool isBeingHeld = rb.velocity.magnitude > 0.01f || isMovingFast;

            if (isBeingHeld && !wasGrabbedLastFrame)
            {
                OnGrabbed();
            }
            else if (!isBeingHeld && wasGrabbedLastFrame)
            {
                OnReleased();
            }

            wasGrabbedLastFrame = isBeingHeld;
            lastPosition = transform.position;

            if (instructionShown)
            {
                instructionShownTime += Time.deltaTime;
                if (instructionShownTime > 4f)
                {
                    instructionShown = false;
                    instructionShownTime = 0f;
                }
            }
        }

        void OnGrabbed()
        {
            if (instructionShown) return;

            Debug.Log($"🎯 FurnitureTracker detected grab on {gameObject.name}");

            if (instructionController != null)
            {
                instructionController.ShowInstructionForObject(gameObject.name);
                instructionShown = true;
                instructionShownTime = 0f;
            }
            else
            {
                Debug.LogWarning("⚠️ No InstructionPanelController found!");
            }
        }

        void OnReleased()
        {
            Debug.Log($"🎯 FurnitureTracker detected release on {gameObject.name}");
        }

        void OnMouseDown()
        {
            OnGrabbed();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.name.Contains("Hand") || other.name.Contains("Controller"))
            {
                OnGrabbed();
            }
        }

        [ContextMenu("🧪 Test Grab")]
        public void TestGrab()
        {
            OnGrabbed();
        }
    }
}