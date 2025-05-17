using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ConfigurePokeInteractors : MonoBehaviour
{
    [Header("Poke Interactor Settings")]
    [Tooltip("The depth of the poke interactor (how far the poke extends)")]
    public float pokeDepth = 0.05f;

    [Tooltip("The width of the poke interactor")]
    public float pokeWidth = 0.05f;

    [Tooltip("The width for selection of the poke interactor")]
    public float pokeSelectWidth = 0.15f;

    [Tooltip("The hover radius of the poke interactor")]
    public float pokeHoverRadius = 0.15f;

    [Tooltip("The interaction offset of the poke interactor")]
    public float pokeInteractionOffset = 0.005f;

    [Tooltip("The physics layer mask for the poke interactor")]
    public LayerMask physicsLayerMask = ~0;  // Default to all layers

    [Tooltip("Enable UI interaction with poke interactor")]
    public bool enableUIInteraction = true;

    [Tooltip("Enable debug visualizations for poke interactor")]
    public bool debugVisualizationsEnabled = false;

    [Tooltip("Should the poke visual indicator be visible?")]
    public bool showVisualIndicator = false;    // Run this once after your poke interactors are set up
    public void UpdatePokeInteractors()
    {
        // Find all XRPokeInteractors in the scene
        XRPokeInteractor[] pokeInteractors = FindObjectsOfType<XRPokeInteractor>();

        Debug.Log($"Found {pokeInteractors.Length} poke interactors to configure");

        foreach (XRPokeInteractor pokeInteractor in pokeInteractors)
        {
            Debug.Log($"Configuring poke interactor: {pokeInteractor.name}");

            // Configure the poke interactor
            pokeInteractor.pokeDepth = pokeDepth;
            pokeInteractor.pokeWidth = pokeWidth;
            pokeInteractor.pokeSelectWidth = pokeSelectWidth;
            pokeInteractor.pokeHoverRadius = pokeHoverRadius;
            pokeInteractor.pokeInteractionOffset = pokeInteractionOffset;
            pokeInteractor.physicsLayerMask = physicsLayerMask;
            pokeInteractor.enableUIInteraction = enableUIInteraction;
            pokeInteractor.debugVisualizationsEnabled = debugVisualizationsEnabled;

            // Find and configure visual indicator if it exists
            Transform visualIndicator = pokeInteractor.transform.Find("PokeVisual");
            if (visualIndicator != null)
            {
                visualIndicator.gameObject.SetActive(showVisualIndicator);

                // Update the visual indicator position and scale based on new settings
                visualIndicator.localPosition = new Vector3(0, 0, pokeDepth * 0.5f);
                visualIndicator.localScale = new Vector3(pokeWidth * 0.5f, pokeWidth * 0.5f, pokeWidth * 0.5f);
            }

            Debug.Log($"Configured poke interactor: {pokeInteractor.name}");
        }

        Debug.Log("Poke interactor configuration complete!");
    }
}
