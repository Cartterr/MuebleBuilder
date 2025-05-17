using UnityEngine;

// This file ensures the VRProject namespace is compiled first
// It helps resolve dependencies between scripts
namespace VRProject
{
    // This class is not used directly - it's just to ensure the namespace exists
    public static class VRProjectNamespace
    {
        // Empty static constructor to ensure this class is initialized
        static VRProjectNamespace()
        {
            Debug.Log("VRProject namespace initialized");
        }
    }
}
