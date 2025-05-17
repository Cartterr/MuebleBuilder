using UnityEngine;
using VRProject; // Add the namespace reference

#if UNITY_EDITOR
using UnityEditor;
#endif

// This script helps create attachment points on a cube
public class AttachmentPointCreator : MonoBehaviour
{
    public bool createOnAwake = true;
    public float attachPointDistance = 0.51f; // Slightly more than half the cube's size
    public float attachPointSize = 0.05f;  // Size of the attachment point colliders
    public bool generateSixFaces = true;   // Create attachment points on all six faces

    private void Awake()
    {
        if (createOnAwake)
        {
            CreateAttachmentPoints();
        }
    }

    public void CreateAttachmentPoints()
    {
        // Don't create duplicates
        AttachmentPoint[] existingPoints = GetComponentsInChildren<AttachmentPoint>();
        if (existingPoints.Length > 0)
        {
            Debug.LogWarning("This object already has attachment points. Remove them before creating new ones.");
            return;
        }

        // Reference to the attachable cube component
        AttachableCube cube = GetComponent<AttachableCube>();
        if (cube == null)
        {
            Debug.LogError("This GameObject needs an AttachableCube component first.");
            return;
        }

        // Create attachment points at the center of each face
        Vector3[] directions = generateSixFaces ?
            new Vector3[] { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back } :
            new Vector3[] { Vector3.up, Vector3.forward };

        foreach (Vector3 dir in directions)
        {
            CreateAttachmentPoint(dir, cube);
        }

        Debug.Log($"Created {directions.Length} attachment points on {gameObject.name}");
    }

    private void CreateAttachmentPoint(Vector3 direction, AttachableCube cube)
    {
        // Create a new game object for the attachment point
        GameObject attachPoint = new GameObject($"AttachPoint_{direction.ToString()}");
        attachPoint.tag = "AttachmentPoint";
        attachPoint.transform.parent = transform;
        attachPoint.transform.localPosition = direction * attachPointDistance;
        attachPoint.transform.localRotation = Quaternion.LookRotation(direction);

        // Add a sphere collider
        SphereCollider collider = attachPoint.AddComponent<SphereCollider>();
        collider.radius = attachPointSize;
        collider.isTrigger = true;

        // Add the attachment point component
        AttachmentPoint point = attachPoint.AddComponent<AttachmentPoint>();
        point.parentCube = cube;

        // Add this point to the cube's list of attachment points
        System.Collections.Generic.List<AttachmentPoint> points = new System.Collections.Generic.List<AttachmentPoint>(cube.attachmentPoints);
        points.Add(point);
        cube.attachmentPoints = points;
    }
}

#if UNITY_EDITOR
// Custom editor for easier attachment point creation
[CustomEditor(typeof(AttachmentPointCreator))]
public class AttachmentPointCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AttachmentPointCreator creator = (AttachmentPointCreator)target;
        if (GUILayout.Button("Create Attachment Points"))
        {
            creator.CreateAttachmentPoints();
        }
    }
}
#endif
