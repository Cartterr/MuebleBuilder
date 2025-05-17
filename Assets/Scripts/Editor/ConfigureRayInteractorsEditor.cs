using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ConfigureRayInteractors))]
public class ConfigureRayInteractorsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ConfigureRayInteractors script = (ConfigureRayInteractors)target;

        if (GUILayout.Button("Configure Ray Interactors", GUILayout.Height(30)))
        {
            script.ConfigureRays();
        }
    }
}
