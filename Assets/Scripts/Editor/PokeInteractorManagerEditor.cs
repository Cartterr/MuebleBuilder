using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(PokeInteractorManager))]
public class PokeInteractorManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PokeInteractorManager manager = (PokeInteractorManager)target;

        // Draw the default inspector properties
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Setup Actions", EditorStyles.boldLabel);

        // Button to find controller anchors
        if (GUILayout.Button("Find Controller Anchors"))
        {
            manager.FindAnchors();
        }

        // Button to perform complete setup
        EditorGUILayout.Space();
        if (GUILayout.Button("Setup Poke Interactors"))
        {
            manager.SetupInteractors();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Configuration Actions", EditorStyles.boldLabel);

        // Button to update configuration
        if (GUILayout.Button("Update Poke Configuration"))
        {
            manager.UpdatePokeConfiguration();
        }

        // Button to toggle visual indicators
        if (GUILayout.Button("Toggle Visual Indicators"))
        {
            manager.ToggleVisualIndicators();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("1. First, assign the left and right controller anchors (or use the Find button)\n" +
                               "2. Click 'Setup Poke Interactors' to create and integrate the poke interactors\n" +
                               "3. Adjust the poke settings and click 'Update Poke Configuration' to apply changes\n" +
                               "4. Toggle the visual indicators to see where the poke interactors are positioned", MessageType.Info);
    }
}
#endif
