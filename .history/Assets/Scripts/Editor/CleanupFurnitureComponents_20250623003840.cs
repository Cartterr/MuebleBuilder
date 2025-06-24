using UnityEngine;
using UnityEditor;

namespace VRProject
{
    public class CleanupFurnitureComponents : EditorWindow
    {
        [MenuItem("VRProject/🧹 Remove All FurnitureVisibilityController Components")]
        public static void RemoveAllFurnitureVisibilityControllers()
        {
            if (EditorUtility.DisplayDialog("Remove FurnitureVisibilityController Components",
                "This will remove ALL FurnitureVisibilityController components from ALL GameObjects in the scene. This action cannot be undone.\n\nAre you sure?",
                "Yes, Remove All", "Cancel"))
            {
                RemoveComponents();
            }
        }

        private static void RemoveComponents()
        {
            FurnitureVisibilityController[] controllers = FindObjectsOfType<FurnitureVisibilityController>();
            int removedCount = 0;

            foreach (FurnitureVisibilityController controller in controllers)
            {
                if (controller != null)
                {
                    string objectName = controller.gameObject.name;
                    DestroyImmediate(controller);
                    Debug.Log($"🧹 Removed FurnitureVisibilityController from {objectName}");
                    removedCount++;
                }
            }

            Debug.Log($"🧹 CLEANUP COMPLETE: Removed {removedCount} FurnitureVisibilityController components");

            if (removedCount > 0)
            {
                EditorUtility.DisplayDialog("Cleanup Complete",
                    $"Successfully removed {removedCount} FurnitureVisibilityController components from the scene.",
                    "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Nothing to Clean",
                    "No FurnitureVisibilityController components were found in the scene.",
                    "OK");
            }
        }
    }
}