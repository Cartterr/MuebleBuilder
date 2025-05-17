using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ConfigureRayInteractors : MonoBehaviour
{
    [Tooltip("If true, the ray will always be visible, even when not pointing at interactables")]
    public bool alwaysShowRay = true;

    [Tooltip("Color of the ray when it's not pointing at an interactable")]
    public Color defaultRayColor = Color.red;

    [Tooltip("Color of the ray when it's pointing at an interactable")]
    public Color validRayColor = Color.green;

    [Tooltip("Width of the ray line")]
    public float rayWidth = 0.02f;

    // Run this once after your rays are set up
    public void ConfigureRays()
    {
        // Find all XRRayInteractors in the scene
        XRRayInteractor[] rayInteractors = FindObjectsOfType<XRRayInteractor>();

        Debug.Log($"Found {rayInteractors.Length} ray interactors to configure");

        foreach (XRRayInteractor rayInteractor in rayInteractors)
        {
            Debug.Log($"Configuring ray interactor: {rayInteractor.name}");

            // Make sure it has an XRInteractorLineVisual
            XRInteractorLineVisual lineVisual = rayInteractor.GetComponent<XRInteractorLineVisual>();
            if (lineVisual == null)
            {
                lineVisual = rayInteractor.gameObject.AddComponent<XRInteractorLineVisual>();
                Debug.Log($"Added XRInteractorLineVisual to {rayInteractor.name}");
            }

            // Configure the line visual
            lineVisual.validColorGradient = CreateGradient(validRayColor);
            lineVisual.invalidColorGradient = alwaysShowRay ? CreateGradient(defaultRayColor) : lineVisual.invalidColorGradient;
            lineVisual.lineWidth = rayWidth;

            // Make sure it's set to always be visible
            lineVisual.enabled = true;
            rayInteractor.enabled = true;            // Configure ray interactor properties
            rayInteractor.maxRaycastDistance = 10f; // Increase the distance
            rayInteractor.enableUIInteraction = true; // Allow UI interaction if needed

            Debug.Log($"Configured ray interactor: {rayInteractor.name}");
        }

        Debug.Log("Ray configuration complete!");
    }

    // Create a solid color gradient
    private Gradient CreateGradient(Color color)
    {
        Gradient gradient = new Gradient();

        // Create two color keys with the same color
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0] = new GradientColorKey(color, 0f);
        colorKeys[1] = new GradientColorKey(color, 1f);

        // Set alpha to fully opaque
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(1f, 0f);
        alphaKeys[1] = new GradientAlphaKey(1f, 1f);

        gradient.SetKeys(colorKeys, alphaKeys);

        return gradient;
    }
}
