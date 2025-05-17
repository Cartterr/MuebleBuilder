using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

// This script integrates poke interactors with the ActionBasedControllerManager
[RequireComponent(typeof(ActionBasedControllerManager))]
public class IntegratePokeInteractors : MonoBehaviour
{
    [Tooltip("The GameObject containing the left hand poke interactor.")]
    public XRPokeInteractor leftPokeInteractor;

    [Tooltip("The GameObject containing the right hand poke interactor.")]
    public XRPokeInteractor rightPokeInteractor;

    [Tooltip("The interaction group for the left controller.")]
    public XRInteractionGroup leftInteractionGroup;

    [Tooltip("The interaction group for the right controller.")]
    public XRInteractionGroup rightInteractionGroup;

    private ActionBasedControllerManager controllerManager;

    private void Awake()
    {
        controllerManager = GetComponent<ActionBasedControllerManager>();
    }

    public void IntegratePokeWithControllerManager()
    {
        Debug.Log("Integrating poke interactors with controller manager...");

        // Add poke interactors to their respective interaction groups
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
                // Add to group - ensuring direct interactors have priority over poke interactors,
                // and poke interactors have priority over ray interactors for interaction
                leftInteractionGroup.AddGroupMember(leftPokeInteractor);
                Debug.Log("Left poke interactor added to left interaction group");
            }
            else
            {
                Debug.Log("Left poke interactor already in left interaction group");
            }
        }
        else if (leftPokeInteractor == null)
        {
            Debug.LogWarning("Left poke interactor not assigned");
        }
        else if (leftInteractionGroup == null)
        {
            Debug.LogWarning("Left interaction group not assigned");
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
                // Add to group - ensuring direct interactors have priority over poke interactors,
                // and poke interactors have priority over ray interactors for interaction
                rightInteractionGroup.AddGroupMember(rightPokeInteractor);
                Debug.Log("Right poke interactor added to right interaction group");
            }
            else
            {
                Debug.Log("Right poke interactor already in right interaction group");
            }
        }
        else if (rightPokeInteractor == null)
        {
            Debug.LogWarning("Right poke interactor not assigned");
        }
        else if (rightInteractionGroup == null)
        {
            Debug.LogWarning("Right interaction group not assigned");
        }

        Debug.Log("Poke interactors integration complete!");
    }

    // Helper method to find interaction groups in the scene
    public void AutoFindInteractionGroups()
    {
        if (leftPokeInteractor != null && leftInteractionGroup == null)
        {
            // Try to find the interaction group in the hierarchy
            XRInteractionGroup[] groups = leftPokeInteractor.transform.root.GetComponentsInChildren<XRInteractionGroup>();
            foreach (var group in groups)
            {
                // Find the group that contains the left controller interactors
                var members = new System.Collections.Generic.List<IXRGroupMember>();
                group.GetGroupMembers(members);

                foreach (var member in members)
                {                    if (member is XRDirectInteractor directInteractor || member is XRRayInteractor rayInteractor)
                    {
                        // Get the right component to access transform
                        Component interactorComponent = member as Component;
                        if (interactorComponent != null &&
                           (interactorComponent.transform.root.name.ToLower().Contains("left") ||
                            interactorComponent.transform.parent.name.ToLower().Contains("left")))
                        {
                            leftInteractionGroup = group;
                            Debug.Log("Found left interaction group: " + group.name);
                            break;
                        }
                    }
                }

                if (leftInteractionGroup != null)
                    break;
            }
        }

        if (rightPokeInteractor != null && rightInteractionGroup == null)
        {
            // Try to find the interaction group in the hierarchy
            XRInteractionGroup[] groups = rightPokeInteractor.transform.root.GetComponentsInChildren<XRInteractionGroup>();
            foreach (var group in groups)
            {
                // Find the group that contains the right controller interactors
                var members = new System.Collections.Generic.List<IXRGroupMember>();
                group.GetGroupMembers(members);

                foreach (var member in members)
                {                    if (member is XRDirectInteractor directInteractor || member is XRRayInteractor rayInteractor)
                    {
                        // Get the right component to access transform
                        Component interactorComponent = member as Component;
                        if (interactorComponent != null &&
                           (interactorComponent.transform.root.name.ToLower().Contains("right") ||
                            interactorComponent.transform.parent.name.ToLower().Contains("right")))
                        {
                            rightInteractionGroup = group;
                            Debug.Log("Found right interaction group: " + group.name);
                            break;
                        }
                    }
                }

                if (rightInteractionGroup != null)
                    break;
            }
        }
    }
}
