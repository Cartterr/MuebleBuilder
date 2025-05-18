using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VRProject
{
    public class FurnitureAttachable : XRGrabInteractable
    {
        public float distanceAdjustSpeed = 2.0f;
        public float minDistance = 0.3f;
        public float maxDistance = 5.0f;

        // Visual feedback materials
        public Material defaultMaterial;
        public Material highlightMaterial;
        public Material connectedMaterial;

        // Attachment settings
        public List<FurnitureAttachmentPoint> attachmentPoints = new List<FurnitureAttachmentPoint>();
        public float attachmentSnapDistance = 0.15f;
        public bool autoAlignRotation = true;

        // Connection tracking
        private List<FurnitureAttachable> connectedFurniture = new List<FurnitureAttachable>();
        private Dictionary<FurnitureAttachable, FixedJoint> joints = new Dictionary<FurnitureAttachable, FixedJoint>();

        // Ray interaction variables
        private Rigidbody rb;
        private MeshRenderer meshRenderer;
        private IXRSelectInteractor selectingRayInteractorRef;
        private Transform rayOriginForDistanceAdjust;
        private float currentAdjustedDistance;
        private bool isSelectedByRayForDistanceAdjust;
        private Quaternion initialGrabRotationOffsetVsRay;
        private Vector2 lastAnalogInput;

        // Current attachment point target when being held
        private FurnitureAttachmentPoint currentSnapTarget = null;
        private FurnitureAttachmentPoint currentNearestPoint = null;

        protected override void Awake()
        {
            base.Awake();

            // Get required components
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
                Debug.Log("Added Rigidbody to FurnitureAttachable: " + gameObject.name);
            }

            meshRenderer = GetComponent<MeshRenderer>();

            // Initialize all attachment points
            foreach (FurnitureAttachmentPoint point in attachmentPoints)
            {
                if (point != null)
                {
                    point.parentFurniture = this;
                }
            }
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);

            // Change material to indicate grabbed state
            if (meshRenderer != null && highlightMaterial != null)
            {
                meshRenderer.material = highlightMaterial;
            }

            // Handle ray interactor for distance adjustment
            if (args.interactorObject is XRRayInteractor rayInteractor)
            {
                selectingRayInteractorRef = rayInteractor;
                rayOriginForDistanceAdjust = rayInteractor.rayOriginTransform;

                if (rayOriginForDistanceAdjust != null)
                {
                    currentAdjustedDistance = Vector3.Distance(rayOriginForDistanceAdjust.position, transform.position);
                    initialGrabRotationOffsetVsRay = Quaternion.Inverse(rayOriginForDistanceAdjust.rotation) * transform.rotation;
                    isSelectedByRayForDistanceAdjust = true;
                    lastAnalogInput = Vector2.zero;
                }
                else
                {
                    Debug.LogError("XRRayInteractor's rayOriginTransform is null.", rayInteractor);
                    isSelectedByRayForDistanceAdjust = false;
                }
            }
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);

            if (args.interactorObject == selectingRayInteractorRef)
            {
                isSelectedByRayForDistanceAdjust = false;
                selectingRayInteractorRef = null;
                rayOriginForDistanceAdjust = null;
            }

            // If we have a valid snap target, connect to it
            if (currentSnapTarget != null)
            {
                ConnectToAttachmentPoint(currentSnapTarget);
                currentSnapTarget = null;
            }

            // Reset material based on connection state
            UpdateVisualState();
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            // Process ray distance adjustment
            ProcessRayDistanceAdjustment(updatePhase);

            // When selected, check for potential attachment points
            if (isSelected && updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                FindNearestAttachmentPoint();
            }
        }

        private void ProcessRayDistanceAdjustment(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            if (isSelectedByRayForDistanceAdjust && selectingRayInteractorRef != null &&
                selectingRayInteractorRef.isSelectActive && rayOriginForDistanceAdjust != null)
            {
                // Process input in Dynamic phase
                if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
                {
                    lastAnalogInput = Vector2.zero; // Default to no input

                    // Use Unity's Input system
                    lastAnalogInput.y = Input.GetAxis("Vertical"); // Usually maps to controller thumbstick Y

                    // Alternative: Check if we're dealing with Oculus/Meta Quest controllers
                    if (lastAnalogInput.y == 0)
                    {
                        lastAnalogInput.y = Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickVertical");
                    }

                    // Check for disconnect button input
                    if (Input.GetButtonDown("Fire2") || Input.GetKeyDown(KeyCode.B))
                    {
                        DisconnectAll();
                    }
                }

                // Determine if we should apply the transform change
                bool applyTransformChangeThisPhase = false;
                float deltaTimeForAdjustment = Time.deltaTime;

                if (movementType == MovementType.Kinematic && updatePhase == XRInteractionUpdateOrder.UpdatePhase.Fixed)
                {
                    applyTransformChangeThisPhase = true;
                    deltaTimeForAdjustment = Time.fixedDeltaTime;
                }
                else if (movementType == MovementType.Instantaneous && updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
                {
                    applyTransformChangeThisPhase = true;
                    deltaTimeForAdjustment = Time.deltaTime;
                }

                if (applyTransformChangeThisPhase)
                {
                    float stickInputY = lastAnalogInput.y;
                    // Apply a small deadzone to prevent drift
                    if (Mathf.Abs(stickInputY) > 0.1f)
                    {
                        currentAdjustedDistance += stickInputY * distanceAdjustSpeed * deltaTimeForAdjustment;
                        currentAdjustedDistance = Mathf.Clamp(currentAdjustedDistance, minDistance, maxDistance);
                    }

                    Vector3 targetPosition = rayOriginForDistanceAdjust.position +
                                            rayOriginForDistanceAdjust.forward * currentAdjustedDistance;
                    Quaternion targetRotation = rayOriginForDistanceAdjust.rotation * initialGrabRotationOffsetVsRay;

                    if (rb != null && movementType == MovementType.Kinematic)
                    {
                        rb.MovePosition(targetPosition);
                        rb.MoveRotation(targetRotation);
                    }
                    else
                    {
                        transform.SetPositionAndRotation(targetPosition, targetRotation);
                    }
                }
            }
        }

        private void FindNearestAttachmentPoint()
        {
            float closestDistance = attachmentSnapDistance;
            FurnitureAttachmentPoint closestPoint = null;

            // If we have no attachment points, we can't connect
            if (attachmentPoints.Count == 0)
                return;

            // Find all potential furniture objects nearby
            Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, attachmentSnapDistance * 2f);

            foreach (Collider col in nearbyColliders)
            {
                // Skip our own colliders
                if (col.transform.IsChildOf(transform))
                    continue;

                // Check if it's a furniture attachment point
                FurnitureAttachable otherFurniture = col.GetComponentInParent<FurnitureAttachable>();
                if (otherFurniture == null || otherFurniture == this || IsConnectedTo(otherFurniture))
                    continue;

                // Check each of their attachment points against each of ours
                foreach (FurnitureAttachmentPoint theirPoint in otherFurniture.attachmentPoints)
                {
                    // Skip if the point is already connected
                    if (theirPoint.isConnected)
                        continue;

                    foreach (FurnitureAttachmentPoint ourPoint in attachmentPoints)
                    {
                        // Skip if our point is already connected
                        if (ourPoint.isConnected)
                            continue;

                        // Check if connection types are compatible
                        if (!ourPoint.CanConnectTo(theirPoint))
                            continue;

                        // Calculate distance
                        float distance = Vector3.Distance(ourPoint.transform.position, theirPoint.transform.position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestPoint = theirPoint;
                            currentNearestPoint = ourPoint;
                        }
                    }
                }
            }

            // Update current snap target
            currentSnapTarget = closestPoint;

            // If we found a valid target, preview alignment
            if (currentSnapTarget != null && autoAlignRotation)
            {
                // Calculate the desired rotation to align our attachment point with the target
                PreviewAlignment(currentNearestPoint, currentSnapTarget);
            }
            else
            {
                // No valid target, reset
                currentNearestPoint = null;
            }
        }

        private void PreviewAlignment(FurnitureAttachmentPoint ourPoint, FurnitureAttachmentPoint theirPoint)
        {
            // Calculate the desired position
            Vector3 targetPosition = theirPoint.transform.position;

            // Calculate the offset from our attachment point to our center
            Vector3 attachmentOffset = transform.position - ourPoint.transform.position;

            // Position with the offset applied
            targetPosition += attachmentOffset;

            // For rotation, we need to align our attachment point's forward with their attachment point's forward
            // but in opposite directions (they should face each other)
            Quaternion targetRotation = Quaternion.LookRotation(-theirPoint.transform.forward, theirPoint.transform.up);

            // Apply a slight attraction to the target position/rotation
            float snapStrength = 0.2f; // Adjust this for stronger/weaker snapping
            transform.position = Vector3.Lerp(transform.position, targetPosition, snapStrength);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, snapStrength);
        }

        private void ConnectToAttachmentPoint(FurnitureAttachmentPoint theirPoint)
        {
            if (currentNearestPoint == null || theirPoint.parentFurniture == null)
                return;

            // Get the other furniture
            FurnitureAttachable otherFurniture = theirPoint.parentFurniture;

            // Calculate the final position and rotation
            // Create the proper alignment
            Vector3 attachmentOffset = transform.position - currentNearestPoint.transform.position;
            Vector3 finalPosition = theirPoint.transform.position + attachmentOffset;

            // For rotation, align our attachment point with their attachment point
            Quaternion finalRotation = Quaternion.LookRotation(-theirPoint.transform.forward, theirPoint.transform.up);

            // Set final position and rotation before connecting
            transform.position = finalPosition;
            transform.rotation = finalRotation;

            // Create the actual physics connection using a FixedJoint
            FixedJoint joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = otherFurniture.rb;
            joint.breakForce = Mathf.Infinity;
            joint.breakTorque = Mathf.Infinity;

            // Register the connection
            connectedFurniture.Add(otherFurniture);
            joints[otherFurniture] = joint;

            // Mark the attachment points as connected
            currentNearestPoint.isConnected = true;
            theirPoint.isConnected = true;

            // Record what is connected to what
            currentNearestPoint.connectedTo = theirPoint;
            theirPoint.connectedTo = currentNearestPoint;

            // Also register the connection on the other furniture
            otherFurniture.RegisterConnection(this, joint);

            // Update visual state
            UpdateVisualState();
            otherFurniture.UpdateVisualState();

            Debug.Log($"Connected {gameObject.name} to {otherFurniture.gameObject.name} at attachment points");
        }

        public void RegisterConnection(FurnitureAttachable furniture, FixedJoint joint)
        {
            if (!connectedFurniture.Contains(furniture))
            {
                connectedFurniture.Add(furniture);
                // We don't duplicate the joint reference here
            }
        }

        // Method to disconnect from a specific furniture
        public void DisconnectFrom(FurnitureAttachable other)
        {
            if (connectedFurniture.Contains(other))
            {
                // Disconnect the attachment points
                foreach (FurnitureAttachmentPoint ourPoint in attachmentPoints)
                {
                    if (ourPoint.isConnected && ourPoint.connectedTo != null &&
                        ourPoint.connectedTo.parentFurniture == other)
                    {
                        // Found a connected point, disconnect it
                        FurnitureAttachmentPoint theirPoint = ourPoint.connectedTo;

                        ourPoint.isConnected = false;
                        ourPoint.connectedTo = null;

                        theirPoint.isConnected = false;
                        theirPoint.connectedTo = null;
                    }
                }

                // Destroy the joint if we have it
                if (joints.TryGetValue(other, out FixedJoint joint) && joint != null)
                {
                    Destroy(joint);
                    joints.Remove(other);
                }

                connectedFurniture.Remove(other);

                // Also tell the other furniture to disconnect from us
                other.UnregisterConnection(this);

                // Update visual state
                UpdateVisualState();
                other.UpdateVisualState();

                Debug.Log($"Disconnected {gameObject.name} from {other.gameObject.name}");
            }
        }

        public void UnregisterConnection(FurnitureAttachable furniture)
        {
            if (connectedFurniture.Contains(furniture))
            {
                connectedFurniture.Remove(furniture);
                // We don't need to worry about the joint here as it exists on the other object
            }
        }

        // Method to disconnect all connections
        public void DisconnectAll()
        {
            // Create a copy of the list to iterate through since we'll be modifying it
            List<FurnitureAttachable> furnitureToDisconnect = new List<FurnitureAttachable>(connectedFurniture);

            foreach (FurnitureAttachable furniture in furnitureToDisconnect)
            {
                DisconnectFrom(furniture);
            }

            // Make sure everything is cleared
            connectedFurniture.Clear();
            foreach (var joint in joints.Values)
            {
                if (joint != null)
                {
                    Destroy(joint);
                }
            }
            joints.Clear();

            // Reset all attachment points
            foreach (FurnitureAttachmentPoint point in attachmentPoints)
            {
                if (point != null)
                {
                    point.isConnected = false;
                    point.connectedTo = null;
                }
            }

            // Update visual state
            UpdateVisualState();
        }

        // Update the visual state based on connection status
        public void UpdateVisualState()
        {
            if (meshRenderer == null) return;

            bool isConnected = connectedFurniture.Count > 0;

            if (isConnected && connectedMaterial != null)
            {
                meshRenderer.material = connectedMaterial;
            }
            else if (!isSelected && defaultMaterial != null)
            {
                meshRenderer.material = defaultMaterial;
            }
        }

        // Check if this furniture is connected to another specific furniture
        public bool IsConnectedTo(FurnitureAttachable other)
        {
            return connectedFurniture.Contains(other);
        }

        // Get all furniture directly connected to this
        public List<FurnitureAttachable> GetConnectedFurniture()
        {
            return new List<FurnitureAttachable>(connectedFurniture);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            DisconnectAll();
        }

        #if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            // Visualize attachment points
            foreach (FurnitureAttachmentPoint point in attachmentPoints)
            {
                if (point != null)
                {
                    // Draw the attachment point
                    Gizmos.color = point.isConnected ? Color.green : Color.yellow;
                    Gizmos.DrawSphere(point.transform.position, 0.02f);

                    // Draw direction
                    Gizmos.color = Color.blue;
                    Gizmos.DrawRay(point.transform.position, point.transform.forward * 0.05f);
                }
            }
        }
        #endif
    }
}