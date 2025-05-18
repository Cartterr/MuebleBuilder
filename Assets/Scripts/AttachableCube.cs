using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AttachableCube : XRGrabInteractable
{
    public float distanceAdjustSpeed = 2.0f;
    public float minDistance = 0.3f;
    public float maxDistance = 5.0f;

    // Attachment settings
    public float attachmentRadius = 0.1f;
    public string attachmentTag = "AttachableCube";

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

    // Connection tracking
    private List<AttachableCube> connectedCubes = new List<AttachableCube>();
    private Dictionary<AttachableCube, FixedJoint> joints = new Dictionary<AttachableCube, FixedJoint>();

    protected override void Awake()
    {
        base.Awake();

        // Get required components
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            Debug.Log("Added Rigidbody to AttachableCube.");
        }

        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogWarning("AttachableCube doesn't have a MeshRenderer component.", this);
        }

        // Make sure this object has the correct tag for easier identification
        gameObject.tag = attachmentTag;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

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

        // Change material to indicate grabbed state
        if (meshRenderer != null && highlightMaterial != null)
        {
            meshRenderer.material = highlightMaterial;
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

        // Update material based on connection state
        UpdateVisualState();

        // Check for potential attachments when released
        CheckForAttachment();
    }

    private void CheckForAttachment()
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, attachmentRadius);

        foreach (Collider col in nearbyColliders)
        {
            // Skip self or already connected cubes
            if (col.gameObject == gameObject) continue;

            AttachableCube otherCube = col.GetComponent<AttachableCube>();
            if (otherCube != null && !IsConnectedTo(otherCube))
            {
                // Connect to this cube
                ConnectTo(otherCube);
                break; // Only connect to one cube at a time for simplicity
            }
        }
    }

    private bool IsConnectedTo(AttachableCube otherCube)
    {
        return connectedCubes.Contains(otherCube);
    }

    private void ConnectTo(AttachableCube otherCube)
    {
        // Create a fixed joint to physically connect the two cubes
        FixedJoint joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = otherCube.rb;
        joint.breakForce = Mathf.Infinity; // Make it unbreakable (you can adjust this)
        joint.breakTorque = Mathf.Infinity;

        // Register the connection
        connectedCubes.Add(otherCube);
        joints[otherCube] = joint;

        // Also register the connection on the other cube
        otherCube.RegisterConnection(this, joint);

        // Update visual state
        UpdateVisualState();
        otherCube.UpdateVisualState();

        Debug.Log($"Connected {gameObject.name} to {otherCube.gameObject.name}");
    }

    public void RegisterConnection(AttachableCube cube, FixedJoint joint)
    {
        if (!connectedCubes.Contains(cube))
        {
            connectedCubes.Add(cube);
            // Note: we don't duplicate the joint reference here, as the joint exists on the other object
        }
    }

    // Method to disconnect from a specific cube
    public void DisconnectFrom(AttachableCube otherCube)
    {
        if (connectedCubes.Contains(otherCube))
        {
            // Destroy the joint if we have it
            if (joints.TryGetValue(otherCube, out FixedJoint joint) && joint != null)
            {
                Destroy(joint);
                joints.Remove(otherCube);
            }

            connectedCubes.Remove(otherCube);

            // Also tell the other cube to disconnect from us
            otherCube.UnregisterConnection(this);

            // Update visual state
            UpdateVisualState();
            otherCube.UpdateVisualState();

            Debug.Log($"Disconnected {gameObject.name} from {otherCube.gameObject.name}");
        }
    }

    public void UnregisterConnection(AttachableCube cube)
    {
        if (connectedCubes.Contains(cube))
        {
            connectedCubes.Remove(cube);
            // We don't need to worry about the joint here as it exists on the other object
        }
    }

    // Method to disconnect all connections
    public void DisconnectAll()
    {
        // Create a copy of the list to iterate through since we'll be modifying it
        List<AttachableCube> cubesToDisconnect = new List<AttachableCube>(connectedCubes);

        foreach (AttachableCube cube in cubesToDisconnect)
        {
            DisconnectFrom(cube);
        }

        // Make sure everything is cleared
        connectedCubes.Clear();
        foreach (var joint in joints.Values)
        {
            if (joint != null)
            {
                Destroy(joint);
            }
        }
        joints.Clear();

        // Update visual state
        UpdateVisualState();
    }

    // Update the visual state based on connection status
    private void UpdateVisualState()
    {
        if (meshRenderer == null) return;

        if (connectedCubes.Count > 0 && connectedMaterial != null)
        {
            meshRenderer.material = connectedMaterial;
        }
        else if (defaultMaterial != null)
        {
            meshRenderer.material = defaultMaterial;
        }
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        // Let the base class do its processing first
        base.ProcessInteractable(updatePhase);

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

                Vector3 targetPosition = rayOriginForDistanceAdjust.position +
                                        rayOriginForDistanceAdjust.forward * currentAdjustedDistance;
                Quaternion targetRotation = rayOriginForDistanceAdjust.rotation * initialGrabRotationOffsetVsRay;

                if (rb != null && movementType == MovementType.Kinematic)
                {
                    rb.MovePosition(targetPosition);
                    rb.MoveRotation(targetRotation);
                }
                else // Handles Instantaneous movement type
                {
                    transform.SetPositionAndRotation(targetPosition, targetRotation);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the attachment radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attachmentRadius);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        DisconnectAll();
    }

    // Get all cubes directly connected to this cube
    public List<AttachableCube> GetConnectedCubes()
    {
        return new List<AttachableCube>(connectedCubes);
    }
}