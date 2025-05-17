using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// This script provides a quick setup guide for poke interactors in the scene.
/// It serves as documentation and a convenient entry point for setting up poke interactions.
/// </summary>
public class PokeInteractorsGuide : MonoBehaviour
{
    [TextArea(10, 20)]
    public string setupInstructions =
        "=== POKE INTERACTORS SETUP GUIDE ===\n\n" +
        "1. Add the PokeInteractorManager component to any GameObject in your scene\n\n" +
        "2. Either manually assign controller anchors or click \"Find Controller Anchors\"\n\n" +
        "3. Click \"Setup Poke Interactors\" to create and configure everything\n\n" +
        "4. Adjust settings as needed and click \"Update Poke Configuration\"\n\n" +
        "5. Toggle visual indicators to see where the poke points are\n\n" +
        "For more details, see the PokeInteractors_README.md file.";

    [TextArea(5, 10)]
    public string controllerSetup =
        "=== CONTROLLER HIERARCHY ===\n\n" +
        "Poke interactors work alongside Ray and Direct interactors with the following priority:\n" +
        "1. Direct interactors (highest priority)\n" +
        "2. Poke interactors (medium priority)\n" +
        "3. Ray interactors (lowest priority)\n\n" +
        "This allows for seamless interaction using all three methods.";

    [Header("Poke Settings Reference")]
    [Tooltip("How far the poke extends from the controller")]
    public float recommendedPokeDepth = 0.05f;

    [Tooltip("Width of the poke interactor")]
    public float recommendedPokeWidth = 0.05f;

    [Tooltip("Width used for selection")]
    public float recommendedPokeSelectWidth = 0.15f;

    [Tooltip("Radius for hover detection")]
    public float recommendedPokeHoverRadius = 0.15f;

    [Tooltip("Offset used for interaction")]
    public float recommendedPokeInteractionOffset = 0.005f;

    // These buttons are for the editor only
#if UNITY_EDITOR
    [Header("Quick Actions")]
    [SerializeField] private bool setupManagerComponent = false;

    private void OnValidate()
    {
        if (setupManagerComponent)
        {
            setupManagerComponent = false;

            // Check if we already have a PokeInteractorManager component
            PokeInteractorManager existingManager = FindObjectOfType<PokeInteractorManager>();
            if (existingManager == null)
            {
                // Create new game object with the manager
                GameObject managerObj = new GameObject("Poke Interactors Manager");
                PokeInteractorManager manager = managerObj.AddComponent<PokeInteractorManager>();
                Debug.Log("Created Poke Interactors Manager GameObject in the scene");
            }
            else
            {
                Debug.Log("Poke Interactors Manager already exists in the scene");
            }
        }
    }
#endif
}
