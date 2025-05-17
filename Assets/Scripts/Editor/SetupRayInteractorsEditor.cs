using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SetupRayInteractors))]
public class SetupRayInteractorsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // Get the target script
        SetupRayInteractors script = (SetupRayInteractors)target;

        // Add a button to call the setup method
        if (GUILayout.Button("Setup Ray Interactors", GUILayout.Height(30)))
        {
            script.SetupInteractors();
        }
    }
}
