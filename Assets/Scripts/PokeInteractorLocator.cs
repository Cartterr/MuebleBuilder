using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// This utility script helps locate poke interactors in the scene and set them up correctly
public class PokeInteractorLocator : MonoBehaviour
{
    // References to the controllers (will be used to find poke interactors)
    public Transform leftControllerAnchor;
    public Transform rightControllerAnchor;

    // References to the poke interactors (found or created by script)
    [HideInInspector]
    public XRPokeInteractor leftPokeInteractor;
    [HideInInspector]
    public XRPokeInteractor rightPokeInteractor;

    // References to the interaction groups
    [HideInInspector]
    public XRInteractionGroup leftInteractionGroup;
    [HideInInspector]
    public XRInteractionGroup rightInteractionGroup;

    // Flag to indicate if we should create missing poke interactors
    public bool createMissingInteractors = true;

    public void LocatePokeInteractors()
    {
        // Try to find existing left poke interactor
        if (leftControllerAnchor != null)
        {
            Transform leftPoke = leftControllerAnchor.Find("Left Poke");
            if (leftPoke != null)
            {
                leftPokeInteractor = leftPoke.GetComponent<XRPokeInteractor>();
                if (leftPokeInteractor != null)
                {
                    Debug.Log("Found Left Poke Interactor");
                }
            }
            else if (createMissingInteractors)
            {
                // Create a new poke interactor for the left controller
                GameObject pokeObject = new GameObject("Left Poke");
                pokeObject.transform.SetParent(leftControllerAnchor);
                pokeObject.transform.localPosition = Vector3.zero;
                pokeObject.transform.localRotation = Quaternion.identity;

                leftPokeInteractor = pokeObject.AddComponent<XRPokeInteractor>();
                ConfigurePokeInteractor(leftPokeInteractor);

                Debug.Log("Created Left Poke Interactor");
            }
        }
        else
        {
            Debug.LogWarning("Left Controller Anchor not assigned!");
        }

        // Try to find existing right poke interactor
        if (rightControllerAnchor != null)
        {
            Transform rightPoke = rightControllerAnchor.Find("Right Poke");
            if (rightPoke != null)
            {
                rightPokeInteractor = rightPoke.GetComponent<XRPokeInteractor>();
                if (rightPokeInteractor != null)
                {
                    Debug.Log("Found Right Poke Interactor");
                }
            }
            else if (createMissingInteractors)
            {
                // Create a new poke interactor for the right controller
                GameObject pokeObject = new GameObject("Right Poke");
                pokeObject.transform.SetParent(rightControllerAnchor);
                pokeObject.transform.localPosition = Vector3.zero;
                pokeObject.transform.localRotation = Quaternion.identity;

                rightPokeInteractor = pokeObject.AddComponent<XRPokeInteractor>();
                ConfigurePokeInteractor(rightPokeInteractor);

                Debug.Log("Created Right Poke Interactor");
            }
        }
        else
        {
            Debug.LogWarning("Right Controller Anchor not assigned!");
        }
    }

    public void FindInteractionGroups()
    {
        XRInteractionGroup[] interactionGroups = FindObjectsOfType<XRInteractionGroup>();

        foreach (XRInteractionGroup group in interactionGroups)
        {
            // Determine if this is a left or right controller interaction group based on naming
            if (group.name.ToLower().Contains("left") ||
                (group.transform.parent != null && group.transform.parent.name.ToLower().Contains("left")))
            {
                leftInteractionGroup = group;
                Debug.Log("Found Left Interaction Group: " + group.name);
            }
            else if (group.name.ToLower().Contains("right") ||
                    (group.transform.parent != null && group.transform.parent.name.ToLower().Contains("right")))
            {
                rightInteractionGroup = group;
                Debug.Log("Found Right Interaction Group: " + group.name);
            }
        }

        // If we still haven't found the groups, try looking through the scene hierarchy
        if (leftInteractionGroup == null && leftPokeInteractor != null)
        {
            // Find all controllers up the hierarchy and see if any have an interaction group
            Transform current = leftPokeInteractor.transform.parent;
            while (current != null && leftInteractionGroup == null)
            {
                leftInteractionGroup = current.GetComponentInChildren<XRInteractionGroup>();
                current = current.parent;
            }

            if (leftInteractionGroup != null)
                Debug.Log("Found Left Interaction Group by hierarchy search: " + leftInteractionGroup.name);
        }

        if (rightInteractionGroup == null && rightPokeInteractor != null)
        {
            // Find all controllers up the hierarchy and see if any have an interaction group
            Transform current = rightPokeInteractor.transform.parent;
            while (current != null && rightInteractionGroup == null)
            {
                rightInteractionGroup = current.GetComponentInChildren<XRInteractionGroup>();
                current = current.parent;
            }

            if (rightInteractionGroup != null)
                Debug.Log("Found Right Interaction Group by hierarchy search: " + rightInteractionGroup.name);
        }
    }

    private void ConfigurePokeInteractor(XRPokeInteractor pokeInteractor)
    {
        // Configure with default values for poke interaction
        pokeInteractor.pokeDepth = 0.05f;
        pokeInteractor.pokeWidth = 0.05f;
        pokeInteractor.pokeSelectWidth = 0.15f;
        pokeInteractor.pokeHoverRadius = 0.15f;
        pokeInteractor.pokeInteractionOffset = 0.005f;
        pokeInteractor.physicsLayerMask = ~0;  // All layers
        pokeInteractor.enableUIInteraction = true;

        // Create a small visual indicator (optional)
        GameObject visualIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visualIndicator.name = "PokeVisual";
        visualIndicator.transform.SetParent(pokeInteractor.transform);
        visualIndicator.transform.localPosition = new Vector3(0, 0, pokeInteractor.pokeDepth * 0.5f);
        visualIndicator.transform.localScale = new Vector3(pokeInteractor.pokeWidth * 0.5f, pokeInteractor.pokeWidth * 0.5f, pokeInteractor.pokeWidth * 0.5f);

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

        // Hide the visual by default (can be toggled for debugging)
        visualIndicator.SetActive(false);
    }

    public void IntegrateWithInteractionGroups()
    {
        if (leftPokeInteractor != null && leftInteractionGroup != null)
        {
            // Check if the poke interactor is already in the group
            bool alreadyInGroup = false;
            var members = new System.Collections.Generic.List<IXRGroupMember>();
            leftInteractionGroup.GetGroupMembers(members);

            foreach (var member in members)
            {
                if (member as XRPokeInteractor == leftPokeInteractor)
                {
                    alreadyInGroup = true;
                    break;
                }
            }

            if (!alreadyInGroup)
            {
                leftInteractionGroup.AddGroupMember(leftPokeInteractor);
                Debug.Log("Added Left Poke Interactor to its Interaction Group");
            }
            else
            {
                Debug.Log("Left Poke Interactor is already in its Interaction Group");
            }
        }

        if (rightPokeInteractor != null && rightInteractionGroup != null)
        {
            // Check if the poke interactor is already in the group
            bool alreadyInGroup = false;
            var members = new System.Collections.Generic.List<IXRGroupMember>();
            rightInteractionGroup.GetGroupMembers(members);

            foreach (var member in members)
            {
                if (member as XRPokeInteractor == rightPokeInteractor)
                {
                    alreadyInGroup = true;
                    break;
                }
            }

            if (!alreadyInGroup)
            {
                rightInteractionGroup.AddGroupMember(rightPokeInteractor);
                Debug.Log("Added Right Poke Interactor to its Interaction Group");
            }
            else
            {
                Debug.Log("Right Poke Interactor is already in its Interaction Group");
            }
        }
    }

    // Helper method to find controller anchors if they aren't assigned
    public void FindControllerAnchors()
    {
        if (leftControllerAnchor == null || rightControllerAnchor == null)
        {
            // Try to find controller transforms in common locations/hierarchies
            Transform[] allTransforms = FindObjectsOfType<Transform>();

            foreach (Transform t in allTransforms)
            {
                // Check for common controller anchor naming patterns
                string name = t.name.ToLower();

                if (leftControllerAnchor == null &&
                   (name.Contains("left") && (name.Contains("controller") || name.Contains("hand") || name.Contains("anchor"))))
                {
                    leftControllerAnchor = t;
                    Debug.Log("Found Left Controller Anchor: " + t.name);
                }
                else if (rightControllerAnchor == null &&
                        (name.Contains("right") && (name.Contains("controller") || name.Contains("hand") || name.Contains("anchor"))))
                {
                    rightControllerAnchor = t;
                    Debug.Log("Found Right Controller Anchor: " + t.name);
                }

                if (leftControllerAnchor != null && rightControllerAnchor != null)
                    break;
            }
        }
    }

    // Full setup process
    public void SetupComplete()
    {
        FindControllerAnchors();
        LocatePokeInteractors();
        FindInteractionGroups();
        IntegrateWithInteractionGroups();
        Debug.Log("Poke Interactor setup complete!");
    }
}
