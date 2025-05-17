using UnityEngine;
using System.Collections.Generic;
using VRProject; // Add the namespace reference

// This class provides visual feedback for attachment points
[RequireComponent(typeof(AttachableCube))]
public class AttachmentPointVisualizer : MonoBehaviour
{
    public bool showVisualizers = true;
    public Material connectedMaterial;
    public Material readyMaterial;
    public Material standardMaterial;
    public float visualizerScale = 0.04f;

    private AttachableCube cube;
    private Dictionary<AttachmentPoint, GameObject> visualizers = new Dictionary<AttachmentPoint, GameObject>();

    private void Start()
    {
        cube = GetComponent<AttachableCube>();

        if (cube == null)
        {
            Debug.LogError("AttachmentPointVisualizer requires AttachableCube component.");
            enabled = false;
            return;
        }

        CreateVisualizers();
    }

    private void CreateVisualizers()
    {
        if (!showVisualizers) return;

        foreach (AttachmentPoint point in cube.attachmentPoints)
        {
            if (point == null) continue;

            GameObject visualizer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            visualizer.name = "Visualizer_" + point.gameObject.name;
            visualizer.transform.parent = point.transform;
            visualizer.transform.localPosition = Vector3.zero;
            visualizer.transform.localScale = Vector3.one * visualizerScale;

            // Make visualizer non-interactive
            Collider collider = visualizer.GetComponent<Collider>();
            if (collider != null) Destroy(collider);

            // Apply default material
            Renderer renderer = visualizer.GetComponent<Renderer>();
            if (renderer != null && standardMaterial != null)
            {
                renderer.material = standardMaterial;
            }

            visualizers.Add(point, visualizer);
        }
    }

    private void Update()
    {
        if (!showVisualizers) return;

        foreach (AttachmentPoint point in cube.attachmentPoints)
        {
            if (point == null) continue;

            // Skip if no visualizer for this point
            if (!visualizers.ContainsKey(point)) continue;

            GameObject visualizer = visualizers[point];
            Renderer renderer = visualizer.GetComponent<Renderer>();
            if (renderer == null) continue;

            // Update material based on connection status
            if (point.connectedTo != null)
            {
                // Connected state
                if (connectedMaterial != null)
                    renderer.material = connectedMaterial;
            }
            else
            {
                // Check for nearby attachment points
                Collider[] colliders = Physics.OverlapSphere(point.transform.position, point.snapDistance);
                bool nearbyPoint = false;

                foreach (Collider col in colliders)
                {
                    if (col.CompareTag("AttachmentPoint"))
                    {
                        AttachmentPoint otherPoint = col.GetComponent<AttachmentPoint>();
                        if (otherPoint != null && otherPoint.parentCube != cube && otherPoint.connectedTo == null)
                        {
                            nearbyPoint = true;
                            break;
                        }
                    }
                }

                if (nearbyPoint && readyMaterial != null)
                {
                    // Ready to connect
                    renderer.material = readyMaterial;
                }
                else if (standardMaterial != null)
                {
                    // Standard state
                    renderer.material = standardMaterial;
                }
            }
        }
    }

    // Toggle visibility
    public void SetVisualizersVisible(bool visible)
    {
        showVisualizers = visible;
        foreach (var kvp in visualizers)
        {
            if (kvp.Value != null)
                kvp.Value.SetActive(visible);
        }
    }

    private void OnDestroy()
    {
        // Clean up visualizers
        foreach (var kvp in visualizers)
        {
            if (kvp.Value != null)
                Destroy(kvp.Value);
        }
    }
}
