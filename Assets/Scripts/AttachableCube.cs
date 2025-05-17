using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using VRProject; // Add the namespace reference

public class AttachableCube : XRGrabInteractable
{
    public float distanceAdjustSpeed = 2.0f;
    public float minDistance = 0.3f;
    public float maxDistance = 5.0f;

    // Attachment settings
    public bool autoSearchForAttachmentPoints = true;
    public List<AttachmentPoint> attachmentPoints = new List<AttachmentPoint>();
    public float proximityHighlightDistance = 0.15f;

    // Optional visual feedback
    public Material defaultMaterial;
    public Material highlightMaterial;
    public Material connectedMaterial;

    private Rigidbody rb;
    private MeshRenderer meshRenderer;

    private IXRSelectInteractor selectingRayInteractorRef;
    private Transform rayOriginForDistanceAdjust;
    private float currentAdjustedDistance;
    private bool isSelectedByRayForDistanceAdjust;
    private Quaternion initialGrabRotationOffsetVsRay;
    private Vector2 lastAnalogInput;

    // Connection state tracking
    private bool isConnectedToAnything = false;    protected override void Awake()
    {
        base.Awake();

        // Get required components
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("AttachableCube requires a Rigidbody component.", this);
        }

        meshRenderer = GetComponent<MeshRenderer>();

        // Auto-find attachment points if enabled
        if (autoSearchForAttachmentPoints && attachmentPoints.Count == 0)
        {
            AttachmentPoint[] points = GetComponentsInChildren<AttachmentPoint>();
            attachmentPoints.AddRange(points);

            if (attachmentPoints.Count == 0)
            {
                Debug.LogWarning("No attachment points found on " + gameObject.name + ". You may need to add AttachmentPoint components to child GameObjects.", this);
            }
        }

        // Initialize all attachment points
        foreach (AttachmentPoint point in attachmentPoints)
        {
            if (point != null)
            {
                point.parentCube = this;
            }
        }

        // Make sure this object has the "AttachableCube" tag for easier identification
        if (!CompareTag("AttachableCube"))
        {
            gameObject.tag = "AttachableCube";
        }
    }    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        // Handle ray interactor for distance adjustment
        if (args.interactorObject is XRRayInteractor rayInteractor)
        {
            selectingRayInteractorRef = rayInteractor;
            rayOriginForDistanceAdjust = rayInteractor.rayOriginTransform; // Typically the controller's transform

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

        // When grabbed, temporarily disable all attachment points to prevent accidental connections during movement
        foreach (AttachmentPoint point in attachmentPoints)
        {
            if (point != null && point.connectedTo == null)  // Only disable free attachment points
            {
                point.GetComponent<Collider>().enabled = false;
            }
        }

        // Change material to indicate grabbed state
        if (meshRenderer != null && highlightMaterial != null)
        {
            meshRenderer.material = highlightMaterial;
        }
    }    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        if (args.interactorObject == selectingRayInteractorRef)
        {
            isSelectedByRayForDistanceAdjust = false;
            selectingRayInteractorRef = null;
            rayOriginForDistanceAdjust = null;
        }

        // Re-enable all attachment points that aren't already connected
        foreach (AttachmentPoint point in attachmentPoints)
        {
            if (point != null && point.connectedTo == null)
            {
                point.GetComponent<Collider>().enabled = true;
            }
        }

        // Reset material based on connection state
        UpdateVisualState();

        // Check for nearby attachment points when released
        CheckForPotentialConnections();
    }

    // Update the visual state based on connection status
    private void UpdateVisualState()
    {
        if (meshRenderer == null) return;

        // Check if any attachment points are connected
        isConnectedToAnything = false;
        foreach (AttachmentPoint point in attachmentPoints)
        {
            if (point != null && point.connectedTo != null)
            {
                isConnectedToAnything = true;
                break;
            }
        }

        // Apply appropriate material
        if (isConnectedToAnything && connectedMaterial != null)
        {
            meshRenderer.material = connectedMaterial;
        }
        else if (defaultMaterial != null)
        {
            meshRenderer.material = defaultMaterial;
        }
    }

    // Check for nearby attachment points when the cube is released
    private void CheckForPotentialConnections()
    {
        foreach (AttachmentPoint point in attachmentPoints)
        {
            if (point != null && point.connectedTo == null)
            {
                // Use a sphere cast to find nearby attachment points
                Collider[] colliders = Physics.OverlapSphere(point.transform.position, point.snapDistance);
                foreach (Collider col in colliders)
                {
                    if (col.CompareTag("AttachmentPoint"))
                    {
                        AttachmentPoint otherPoint = col.GetComponent<AttachmentPoint>();
                        if (otherPoint != null && otherPoint.parentCube != this && otherPoint.connectedTo == null)
                        {
                            // Found a potential connection, trigger it
                            point.GetComponent<Collider>().enabled = true;  // Ensure collider is enabled
                            break;
                        }
                    }
                }
            }
        }
    }    // Method to disconnect all attachment points
    public void DisconnectAll()
    {
        foreach (AttachmentPoint point in attachmentPoints)
        {
            if (point != null && point.connectedTo != null)
            {
                point.Disconnect();
            }
        }

        UpdateVisualState();
    }

    // Update is called once per frame
    protected override void OnDestroy()
    {
        base.OnDestroy();
        DisconnectAll();
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        // Let the base class do its processing first.
        // This handles standard grab attachment, events, etc.
        base.ProcessInteractable(updatePhase);

        if (isSelectedByRayForDistanceAdjust && selectingRayInteractorRef != null && selectingRayInteractorRef.isSelectActive && rayOriginForDistanceAdjust != null)
        {
            // Process input in Dynamic phase
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                lastAnalogInput = Vector2.zero; // Default to no input

                // Use Unity's Input system instead of trying to query the XR device directly
                // This is a much more reliable approach across Unity versions
                lastAnalogInput.y = Input.GetAxis("Vertical"); // Usually maps to controller thumbstick Y

                // Alternative: Check if we're dealing with Oculus/Meta Quest controllers
                if (lastAnalogInput.y == 0)
                {
                    // Try Oculus specific input if standard input didn't work
                    lastAnalogInput.y = Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickVertical");
                }

                // Check for disconnect button input (e.g., B button)
                // You could add a configurable button for disconnection
                if (Input.GetButtonDown("Fire2") || Input.GetKeyDown(KeyCode.B))  // Customize this for your input setup
                {
                    DisconnectAll();
                }
            }

            // Determine if we should apply the transform change in this phase
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

                Vector3 targetPosition = rayOriginForDistanceAdjust.position + rayOriginForDistanceAdjust.forward * currentAdjustedDistance;
                Quaternion targetRotation = rayOriginForDistanceAdjust.rotation * initialGrabRotationOffsetVsRay;

                if (rb != null && movementType == MovementType.Kinematic)
                {
                    rb.MovePosition(targetPosition);
                    rb.MoveRotation(targetRotation);
                }
                else // Handles Instantaneous movement type, or if somehow not kinematic
                {
                    transform.SetPositionAndRotation(targetPosition, targetRotation);
                }
            }
        }

        // In the late update phase, check for potential connections if not held
        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Late && !isSelected)
        {
            // Only highlight nearby attachment points if we're not already connected
            if (!isConnectedToAnything)
            {
                CheckForNearbyAttachmentPoints();
            }
        }
    }

    // Check for nearby attachment points for visual highlighting
    private void CheckForNearbyAttachmentPoints()
    {
        // Implementation of proximity highlighting logic
        // This is a more basic version that doesn't change materials
        // You can expand this with visual feedback if needed
        foreach (AttachmentPoint point in attachmentPoints)
        {
            if (point != null && point.connectedTo == null)
            {
                Collider[] colliders = Physics.OverlapSphere(point.transform.position, proximityHighlightDistance);
                foreach (Collider col in colliders)
                {
                    if (col.CompareTag("AttachmentPoint"))
                    {
                        AttachmentPoint otherPoint = col.GetComponent<AttachmentPoint>();
                        if (otherPoint != null && otherPoint.parentCube != this && otherPoint.connectedTo == null)
                        {
                            // Found a potential connection, could add visual feedback here
                            break;
                        }
                    }
                }
            }
        }
    }

    // Check if this cube is connected to another specific cube
    public bool IsConnectedToCube(AttachableCube otherCube)
    {
        if (otherCube == null) return false;

        // Check if any of our attachment points are connected to any of the other cube's attachment points
        foreach (AttachmentPoint point in attachmentPoints)
        {
            if (point != null && point.connectedTo != null && point.connectedTo.parentCube == otherCube)
            {
                return true;
            }
        }

        return false;
    }

    // Get all cubes directly connected to this cube
    public List<AttachableCube> GetConnectedCubes()
    {
        List<AttachableCube> connectedCubes = new List<AttachableCube>();

        foreach (AttachmentPoint point in attachmentPoints)
        {
            if (point != null && point.connectedTo != null && point.connectedTo.parentCube != null)
            {
                // Add the connected cube if not already in the list
                if (!connectedCubes.Contains(point.connectedTo.parentCube))
                {
                    connectedCubes.Add(point.connectedTo.parentCube);
                }
            }
        }

        return connectedCubes;
    }
}
