using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// This script is meant to be run once in the editor to set up poke interactors.
// You can attach it to any GameObject in the scene and press the button in the Inspector.
[ExecuteInEditMode]
public class SetupPokeInteractors : MonoBehaviour
{
    public Transform leftHandAnchor;
    public Transform rightHandAnchor;

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

    // Call this from the Inspector (it will show as a button)
    public void SetupInteractors()
    {
        if (leftHandAnchor != null)
        {
            SetupHandInteractor(leftHandAnchor, "Left Poke");
        }
        else
        {
            Debug.LogError("Left Hand Anchor not assigned!");
        }

        if (rightHandAnchor != null)
        {
            SetupHandInteractor(rightHandAnchor, "Right Poke");
        }
        else
        {
            Debug.LogError("Right Hand Anchor not assigned!");
        }

        Debug.Log("Poke interactors set up! You can now configure them or remove this component.");
    }

    private void SetupHandInteractor(Transform handAnchor, string pokeName)
    {
        // Check if poke already exists
        Transform existingPoke = handAnchor.Find(pokeName);
        if (existingPoke != null)
        {
            Debug.Log($"{pokeName} already exists on {handAnchor.name}");
            return;
        }

        // Create poke GameObject
        GameObject pokeObject = new GameObject(pokeName);
        pokeObject.transform.SetParent(handAnchor);
        pokeObject.transform.localPosition = Vector3.zero;
        pokeObject.transform.localRotation = Quaternion.identity;

        // Add Poke Interactor component
        XRPokeInteractor pokeInteractor = pokeObject.AddComponent<XRPokeInteractor>();

        // Configure poke interactor settings
        pokeInteractor.pokeDepth = pokeDepth;
        pokeInteractor.pokeWidth = pokeWidth;
        pokeInteractor.pokeSelectWidth = pokeSelectWidth;
        pokeInteractor.pokeHoverRadius = pokeHoverRadius;
        pokeInteractor.pokeInteractionOffset = pokeInteractionOffset;
        pokeInteractor.physicsLayerMask = physicsLayerMask;
        pokeInteractor.enableUIInteraction = true;

        // Add a small visual sphere to represent the poke point (optional)
        GameObject visualIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visualIndicator.name = "PokeVisual";
        visualIndicator.transform.SetParent(pokeObject.transform);
        visualIndicator.transform.localPosition = new Vector3(0, 0, pokeDepth * 0.5f);
        visualIndicator.transform.localScale = new Vector3(pokeWidth * 0.5f, pokeWidth * 0.5f, pokeWidth * 0.5f);

        // Make the visual indicator non-interactive
        Object.Destroy(visualIndicator.GetComponent<Collider>());

        // Optional: Set material to semi-transparent
        Renderer renderer = visualIndicator.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material material = new Material(Shader.Find("Standard"));
            material.color = new Color(0.5f, 0.5f, 1f, 0.5f);
            material.SetFloat("_Mode", 3); // Transparent mode
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = 3000;
            renderer.material = material;
        }

        // Make the visual indicator disabled by default (can be toggled for debugging)
        visualIndicator.SetActive(false);

        Debug.Log($"Created {pokeName} on {handAnchor.name}");
    }
}
