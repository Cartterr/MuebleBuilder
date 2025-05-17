using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

// This script manages the setup and configuration of poke interactors in the XR interaction system
[DefaultExecutionOrder(-10)] // Execute before other interactors to ensure proper setup
public class PokeInteractorManager : MonoBehaviour
{
    [Header("Controller References")]
    [Tooltip("The transform of the left controller")]
    public Transform leftControllerAnchor;

    [Tooltip("The transform of the right controller")]
    public Transform rightControllerAnchor;

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

    [Tooltip("Enable UI interaction with poke interactor")]
    public bool enableUIInteraction = true;

    [Tooltip("Enable debug visualizations for poke interactor")]
    public bool debugVisualizationsEnabled = false;

    [Tooltip("Should the poke visual indicator be visible?")]
    public bool showVisualIndicator = false;

    [Header("References (Auto-populated)")]
    [SerializeField, Tooltip("Reference to the left poke interactor (auto-populated)")]
    private XRPokeInteractor leftPokeInteractor;

    [SerializeField, Tooltip("Reference to the right poke interactor (auto-populated)")]
    private XRPokeInteractor rightPokeInteractor;

    [SerializeField, Tooltip("Reference to the left interaction group (auto-populated)")]
    private XRInteractionGroup leftInteractionGroup;

    [SerializeField, Tooltip("Reference to the right interaction group (auto-populated)")]
    private XRInteractionGroup rightInteractionGroup;

    // Setup result tracking
    private bool setupComplete = false;

    private void Awake()
    {
        // Auto setup on Awake if we're in play mode (optional)
        if (Application.isPlaying && !setupComplete)
        {
            SetupPokeInteractors();
        }
    }

    public void SetupPokeInteractors()
    {
        // 1. Find controller anchors if they're not already assigned
        if (leftControllerAnchor == null || rightControllerAnchor == null)
        {
            FindControllerAnchors();
        }

        // 2. Create or locate poke interactors
        CreateOrLocatePokeInteractors();

        // 3. Find interaction groups
        FindInteractionGroups();

        // 4. Integrate poke interactors with interaction groups
        SetupPokeInteractorGroups();        // 5. Configure poke interactors with the specified settings
        ApplyPokeConfiguration();

        setupComplete = true;
        Debug.Log("Poke interactor setup complete!");
    }

    private void FindControllerAnchors()
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

        if (leftControllerAnchor == null)
            Debug.LogWarning("Could not find Left Controller Anchor");

        if (rightControllerAnchor == null)
            Debug.LogWarning("Could not find Right Controller Anchor");
    }

    private void CreateOrLocatePokeInteractors()
    {
        // Try to find/create left poke interactor
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
            else
            {
                // Create a new poke interactor for the left controller
                GameObject pokeObject = new GameObject("Left Poke");
                pokeObject.transform.SetParent(leftControllerAnchor);
                pokeObject.transform.localPosition = Vector3.zero;
                pokeObject.transform.localRotation = Quaternion.identity;

                leftPokeInteractor = pokeObject.AddComponent<XRPokeInteractor>();
                CreateVisualIndicator(leftPokeInteractor);

                Debug.Log("Created Left Poke Interactor");
            }
        }

        // Try to find/create right poke interactor
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
            else
            {
                // Create a new poke interactor for the right controller
                GameObject pokeObject = new GameObject("Right Poke");
                pokeObject.transform.SetParent(rightControllerAnchor);
                pokeObject.transform.localPosition = Vector3.zero;
                pokeObject.transform.localRotation = Quaternion.identity;

                rightPokeInteractor = pokeObject.AddComponent<XRPokeInteractor>();
                CreateVisualIndicator(rightPokeInteractor);

                Debug.Log("Created Right Poke Interactor");
            }
        }
    }

    private void CreateVisualIndicator(XRPokeInteractor pokeInteractor)
    {
        // Create a small visual indicator
        GameObject visualIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visualIndicator.name = "PokeVisual";
        visualIndicator.transform.SetParent(pokeInteractor.transform);
        visualIndicator.transform.localPosition = new Vector3(0, 0, pokeDepth * 0.5f);
        visualIndicator.transform.localScale = new Vector3(pokeWidth * 0.5f, pokeWidth * 0.5f, pokeWidth * 0.5f);

        // Make the visual indicator non-interactive
        Object.Destroy(visualIndicator.GetComponent<Collider>());

        // Set material to semi-transparent
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

        visualIndicator.SetActive(showVisualIndicator);
    }

    private void FindInteractionGroups()
    {
        // First, check if the ActionBasedControllerManager is in the scene
        ActionBasedControllerManager[] controllerManagers = FindObjectsOfType<ActionBasedControllerManager>();

        if (controllerManagers.Length > 0)
        {
            Debug.Log("Found ActionBasedControllerManager(s) in the scene");

            foreach (ActionBasedControllerManager manager in controllerManagers)
            {
                // Try to find the manipulation interaction group in the controller manager
                XRInteractionGroup[] groups = manager.GetComponentsInChildren<XRInteractionGroup>();

                foreach (XRInteractionGroup group in groups)
                {
                    // Try to determine if this is a left or right controller group
                    string name = group.name.ToLower();
                    Transform parent = group.transform.parent;
                    string parentName = parent != null ? parent.name.ToLower() : "";

                    if ((name.Contains("left") || parentName.Contains("left")) && leftInteractionGroup == null)
                    {
                        leftInteractionGroup = group;
                        Debug.Log("Found Left Interaction Group: " + group.name);
                    }
                    else if ((name.Contains("right") || parentName.Contains("right")) && rightInteractionGroup == null)
                    {
                        rightInteractionGroup = group;
                        Debug.Log("Found Right Interaction Group: " + group.name);
                    }

                    if (leftInteractionGroup != null && rightInteractionGroup != null)
                        break;
                }
            }
        }

        // If we still haven't found the interaction groups, look for them elsewhere in the scene
        if (leftInteractionGroup == null || rightInteractionGroup == null)
        {
            XRInteractionGroup[] allGroups = FindObjectsOfType<XRInteractionGroup>();

            foreach (XRInteractionGroup group in allGroups)
            {
                string name = group.name.ToLower();
                Transform parent = group.transform.parent;
                string parentName = parent != null ? parent.name.ToLower() : "";

                if ((name.Contains("left") || parentName.Contains("left")) && leftInteractionGroup == null)
                {
                    leftInteractionGroup = group;
                    Debug.Log("Found Left Interaction Group: " + group.name);
                }
                else if ((name.Contains("right") || parentName.Contains("right")) && rightInteractionGroup == null)
                {
                    rightInteractionGroup = group;
                    Debug.Log("Found Right Interaction Group: " + group.name);
                }

                if (leftInteractionGroup != null && rightInteractionGroup != null)
                    break;
            }
        }

        // If we still don't have interaction groups, create them
        if (leftInteractionGroup == null && leftPokeInteractor != null)
        {
            GameObject groupObj = new GameObject("Left Interaction Group");
            groupObj.transform.SetParent(leftPokeInteractor.transform.parent);
            leftInteractionGroup = groupObj.AddComponent<XRInteractionGroup>();
            Debug.Log("Created Left Interaction Group");
        }

        if (rightInteractionGroup == null && rightPokeInteractor != null)
        {
            GameObject groupObj = new GameObject("Right Interaction Group");
            groupObj.transform.SetParent(rightPokeInteractor.transform.parent);
            rightInteractionGroup = groupObj.AddComponent<XRInteractionGroup>();
            Debug.Log("Created Right Interaction Group");
        }
    }

    private void SetupPokeInteractorGroups()
    {
        // Add left poke interactor to its group
        if (leftPokeInteractor != null && leftInteractionGroup != null)
        {
            // Check if already in group
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
                // Add to group with appropriate priority
                leftInteractionGroup.AddGroupMember(leftPokeInteractor);

                // Reorder interactors to ensure direct > poke > ray priority
                ReorderInteractorsInGroup(leftInteractionGroup);

                Debug.Log("Added Left Poke Interactor to Interaction Group");
            }
        }

        // Add right poke interactor to its group
        if (rightPokeInteractor != null && rightInteractionGroup != null)
        {
            // Check if already in group
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
                // Add to group with appropriate priority
                rightInteractionGroup.AddGroupMember(rightPokeInteractor);

                // Reorder interactors to ensure direct > poke > ray priority
                ReorderInteractorsInGroup(rightInteractionGroup);

                Debug.Log("Added Right Poke Interactor to Interaction Group");
            }
        }
    }

    private void ReorderInteractorsInGroup(XRInteractionGroup group)
    {
        // Get all members
        var members = new System.Collections.Generic.List<IXRGroupMember>();
        group.GetGroupMembers(members);

        // Temporary lists to sort by type
        var directInteractors = new System.Collections.Generic.List<IXRGroupMember>();
        var pokeInteractors = new System.Collections.Generic.List<IXRGroupMember>();
        var rayInteractors = new System.Collections.Generic.List<IXRGroupMember>();
        var otherInteractors = new System.Collections.Generic.List<IXRGroupMember>();

        // Sort interactors by type
        foreach (var member in members)
        {
            if (member is XRDirectInteractor)
                directInteractors.Add(member);
            else if (member is XRPokeInteractor)
                pokeInteractors.Add(member);
            else if (member is XRRayInteractor)
                rayInteractors.Add(member);
            else
                otherInteractors.Add(member);
        }

        // Clear the group
        foreach (var member in members)
        {
            group.RemoveGroupMember(member);
        }

        // Add them back in priority order
        foreach (var member in directInteractors)
            group.AddGroupMember(member);

        foreach (var member in pokeInteractors)
            group.AddGroupMember(member);

        foreach (var member in rayInteractors)
            group.AddGroupMember(member);

        foreach (var member in otherInteractors)
            group.AddGroupMember(member);
    }    private void ApplyPokeConfiguration()
    {
        // Configure left poke interactor
        if (leftPokeInteractor != null)
        {
            leftPokeInteractor.pokeDepth = pokeDepth;
            leftPokeInteractor.pokeWidth = pokeWidth;
            leftPokeInteractor.pokeSelectWidth = pokeSelectWidth;
            leftPokeInteractor.pokeHoverRadius = pokeHoverRadius;
            leftPokeInteractor.pokeInteractionOffset = pokeInteractionOffset;
            leftPokeInteractor.enableUIInteraction = enableUIInteraction;
            leftPokeInteractor.debugVisualizationsEnabled = debugVisualizationsEnabled;

            // Update visual indicator
            Transform visual = leftPokeInteractor.transform.Find("PokeVisual");
            if (visual != null)
            {
                visual.gameObject.SetActive(showVisualIndicator);
                visual.localPosition = new Vector3(0, 0, pokeDepth * 0.5f);
                visual.localScale = new Vector3(pokeWidth * 0.5f, pokeWidth * 0.5f, pokeWidth * 0.5f);
            }

            Debug.Log("Configured Left Poke Interactor");
        }

        // Configure right poke interactor
        if (rightPokeInteractor != null)
        {
            rightPokeInteractor.pokeDepth = pokeDepth;
            rightPokeInteractor.pokeWidth = pokeWidth;
            rightPokeInteractor.pokeSelectWidth = pokeSelectWidth;
            rightPokeInteractor.pokeHoverRadius = pokeHoverRadius;
            rightPokeInteractor.pokeInteractionOffset = pokeInteractionOffset;
            rightPokeInteractor.enableUIInteraction = enableUIInteraction;
            rightPokeInteractor.debugVisualizationsEnabled = debugVisualizationsEnabled;

            // Update visual indicator
            Transform visual = rightPokeInteractor.transform.Find("PokeVisual");
            if (visual != null)
            {
                visual.gameObject.SetActive(showVisualIndicator);
                visual.localPosition = new Vector3(0, 0, pokeDepth * 0.5f);
                visual.localScale = new Vector3(pokeWidth * 0.5f, pokeWidth * 0.5f, pokeWidth * 0.5f);
            }

            Debug.Log("Configured Right Poke Interactor");
        }
    }

    // Public methods for inspector buttons

    public void FindAnchors()
    {
        FindControllerAnchors();
    }

    public void SetupInteractors()
    {
        SetupPokeInteractors();
    }    public void UpdatePokeConfiguration()
    {
        ApplyPokeConfiguration();
    }

    public void ToggleVisualIndicators()
    {
        showVisualIndicator = !showVisualIndicator;

        if (leftPokeInteractor != null)
        {
            Transform visual = leftPokeInteractor.transform.Find("PokeVisual");
            if (visual != null)
                visual.gameObject.SetActive(showVisualIndicator);
        }

        if (rightPokeInteractor != null)
        {
            Transform visual = rightPokeInteractor.transform.Find("PokeVisual");
            if (visual != null)
                visual.gameObject.SetActive(showVisualIndicator);
        }
    }
}
