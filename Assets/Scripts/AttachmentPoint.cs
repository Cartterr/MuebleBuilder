using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VRProject
{
// This script handles attachment points for the simplified cube attachment approach
// Last modified: May 17, 2025 - Fixed namespace issues
public class AttachmentPoint : MonoBehaviour
{
    public AttachableCube parentCube;
    public AttachmentPoint connectedTo;
    public float snapDistance = 0.05f; // Distance threshold for snapping

    // The shared parent object that will hold all connected objects
    private GameObject sharedAnchor = null;

    private void Awake()
    {
        // Make sure this object has a collider and is properly tagged
        if (GetComponent<Collider>() == null)
        {
            Debug.LogWarning("AttachmentPoint needs a collider to work properly.", this);
        }

        if (!gameObject.CompareTag("AttachmentPoint"))
        {
            gameObject.tag = "AttachmentPoint";
            Debug.Log("Set tag to 'AttachmentPoint' automatically. Make sure this tag exists in your project.", this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TryConnect(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryConnect(other);
    }

    private void TryConnect(Collider other)
    {
        // Only proceed if we're not already connected
        if (connectedTo != null)
            return;

        if (other.CompareTag("AttachmentPoint"))
        {
            AttachmentPoint otherPoint = other.GetComponent<AttachmentPoint>();
            if (otherPoint != null && otherPoint.parentCube != parentCube && otherPoint.connectedTo == null)
            {
                float distance = Vector3.Distance(transform.position, other.transform.position);
                if (distance <= snapDistance)
                {
                    Connect(otherPoint);
                }
            }
        }
    }

    private void Connect(AttachmentPoint otherPoint)
    {
        // Create a shared anchor if none exists
        if (sharedAnchor == null && otherPoint.sharedAnchor == null)
        {
            // Create a new shared anchor at the midpoint between the two cubes
            Vector3 midPoint = (parentCube.transform.position + otherPoint.parentCube.transform.position) * 0.5f;
            sharedAnchor = new GameObject("SharedAnchor_" + parentCube.name + "_" + otherPoint.parentCube.name);
            sharedAnchor.transform.position = midPoint;

            // Parent both cubes to this shared anchor
            ParentToAnchor(parentCube.gameObject, sharedAnchor);
            ParentToAnchor(otherPoint.parentCube.gameObject, sharedAnchor);
        }
        else if (sharedAnchor != null && otherPoint.sharedAnchor == null)
        {
            // We already have a shared anchor, parent the other cube to our anchor
            ParentToAnchor(otherPoint.parentCube.gameObject, sharedAnchor);
            otherPoint.sharedAnchor = sharedAnchor;
        }
        else if (sharedAnchor == null && otherPoint.sharedAnchor != null)
        {
            // Other point has a shared anchor, parent our cube to their anchor
            ParentToAnchor(parentCube.gameObject, otherPoint.sharedAnchor);
            sharedAnchor = otherPoint.sharedAnchor;
        }
        else if (sharedAnchor != null && otherPoint.sharedAnchor != null && sharedAnchor != otherPoint.sharedAnchor)
        {
            // Both have different shared anchors - need to merge them
            MergeAnchors(sharedAnchor, otherPoint.sharedAnchor);
        }

        // Update connection references
        connectedTo = otherPoint;
        otherPoint.connectedTo = this;

        // Disable colliders on attachment points to prevent further unwanted connections
        GetComponent<Collider>().enabled = false;
        otherPoint.GetComponent<Collider>().enabled = false;

        Debug.Log($"Connected {parentCube.name} to {otherPoint.parentCube.name} with shared anchor");
    }

    // Helper to parent an object to the shared anchor while preserving its world position/rotation
    private void ParentToAnchor(GameObject obj, GameObject anchor)
    {
        // Store original world position/rotation
        Vector3 worldPos = obj.transform.position;
        Quaternion worldRot = obj.transform.rotation;

        // Parent to the anchor
        obj.transform.SetParent(anchor.transform, false);

        // Restore world position/rotation
        obj.transform.position = worldPos;
        obj.transform.rotation = worldRot;

        // Disable the object's rigidbody since the shared anchor will handle physics
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    // Merge two shared anchors into one
    private void MergeAnchors(GameObject anchor1, GameObject anchor2)
    {
        // Move all children from anchor2 to anchor1
        while (anchor2.transform.childCount > 0)
        {
            Transform child = anchor2.transform.GetChild(0);
            ParentToAnchor(child.gameObject, anchor1);
        }

        // Destroy the now-empty anchor2
        Destroy(anchor2);

        // Update all AttachmentPoints that were using anchor2
        AttachmentPoint[] allPoints = GameObject.FindObjectsOfType<AttachmentPoint>();
        foreach (AttachmentPoint point in allPoints)
        {
            if (point.sharedAnchor == anchor2)
            {
                point.sharedAnchor = anchor1;
            }
        }
    }

    // Public method to disconnect attachment point
    public void Disconnect()
    {
        if (connectedTo == null || sharedAnchor == null)
            return;

        // Get reference to the other point before clearing
        AttachmentPoint otherPoint = connectedTo;

        // Clear connection reference
        connectedTo = null;
        otherPoint.connectedTo = null;

        // Re-enable colliders
        GetComponent<Collider>().enabled = true;
        otherPoint.GetComponent<Collider>().enabled = true;

        // Handle disconnection from the shared anchor
        HandleDisconnection(parentCube.gameObject);
        HandleDisconnection(otherPoint.parentCube.gameObject);

        Debug.Log($"Disconnected {parentCube.name} from {otherPoint.parentCube.name}");
    }

    // Handle disconnection of an object from the shared anchor
    private void HandleDisconnection(GameObject obj)
    {
        // Only proceed if we have a shared anchor
        if (sharedAnchor == null)
            return;

        // Unparent from shared anchor
        obj.transform.SetParent(null);

        // Re-enable rigidbody
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        // Check if the shared anchor has any children left
        if (sharedAnchor.transform.childCount == 0)
        {
            // No more children, destroy the anchor
            Destroy(sharedAnchor);
              // Clear the reference
            AttachmentPoint[] allPoints = GameObject.FindObjectsOfType<AttachmentPoint>();
            foreach (AttachmentPoint point in allPoints)
            {
                if (point.sharedAnchor == sharedAnchor)
                {
                    point.sharedAnchor = null;
                }
            }
        }
    }
      // Clean up when the object is destroyed
    private void OnDestroy()
    {
        // Disconnect if connected to another object
        if (connectedTo != null)
        {
            Disconnect();
        }
    }
}
}