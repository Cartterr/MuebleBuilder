using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// This script is meant to be run once in the editor to set up ray interactors.
// You can attach it to any GameObject in the scene and press the button in the Inspector.
[ExecuteInEditMode]
public class SetupRayInteractors : MonoBehaviour
{
    public Transform leftHandAnchor;
    public Transform rightHandAnchor;

    // Call this from the Inspector (it will show as a button)
    public void SetupInteractors()
    {
        if (leftHandAnchor != null)
        {
            SetupHandInteractor(leftHandAnchor, "Left Ray");
        }
        else
        {
            Debug.LogError("Left Hand Anchor not assigned!");
        }

        if (rightHandAnchor != null)
        {
            SetupHandInteractor(rightHandAnchor, "Right Ray");
        }
        else
        {
            Debug.LogError("Right Hand Anchor not assigned!");
        }

        Debug.Log("Ray interactors set up! You can now remove this component or GameObject.");
    }

    private void SetupHandInteractor(Transform handAnchor, string rayName)
    {
        // Check if ray already exists
        Transform existingRay = handAnchor.Find(rayName);
        if (existingRay != null)
        {
            Debug.Log($"{rayName} already exists on {handAnchor.name}");
            return;
        }

        // Create ray GameObject
        GameObject rayObject = new GameObject(rayName);
        rayObject.transform.SetParent(handAnchor);
        rayObject.transform.localPosition = Vector3.zero;
        rayObject.transform.localRotation = Quaternion.identity;

        // Add Ray Interactor components
        XRRayInteractor rayInteractor = rayObject.AddComponent<XRRayInteractor>();
        rayObject.AddComponent<XRInteractorLineVisual>();        // Configure ray to be always visible
        var lineVisual = rayObject.GetComponent<XRInteractorLineVisual>();
        if (lineVisual != null)
        {
            lineVisual.invalidColorGradient = lineVisual.validColorGradient; // Make invalid color same as valid
            // Set other properties based on your XR Interaction Toolkit version
            // We'll use simpler settings that work across versions

            // For older versions of XR Interaction Toolkit, you might need to configure:
            // - lineWidth (set to a visible value like 0.02f)
            // - validColorGradient (to make it more visible)
            if (lineVisual.GetType().GetProperty("lineWidth") != null)
            {
                lineVisual.lineWidth = 0.02f;
            }
        }

        Debug.Log($"Created {rayName} on {handAnchor.name}");
    }
}
