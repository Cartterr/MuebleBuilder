using UnityEngine;

namespace VRProject
{
    public class RestartButtonSetup : MonoBehaviour
    {
        [Header("Button Appearance")]
        public Color buttonColor = Color.red;
        public Color glowColor = Color.yellow;
        public Color pressedColor = new Color(0.5f, 0f, 0f, 1f); // Dark red

        [Header("Button Settings")]
        public Vector3 buttonSize = new Vector3(0.2f, 0.05f, 0.2f);
        public float pressDepth = 0.02f;

        [Header("Text")]
        public bool addText = true;
        public string buttonText = "MENU";
        public Color textColor = Color.white;

        [Header("Behavior")]
        public bool returnToMenuInsteadOfRestart = true;

        [ContextMenu("Create Restart Button")]
        public void CreateRestartButton()
        {
            // Create main button GameObject
            GameObject buttonObj = new GameObject("RestartButton");
            buttonObj.transform.SetParent(transform);
            buttonObj.transform.localPosition = Vector3.zero;

            // Add collider for interaction
            BoxCollider collider = buttonObj.AddComponent<BoxCollider>();
            collider.size = buttonSize;

            // Create visual button
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            visual.name = "ButtonVisual";
            visual.transform.SetParent(buttonObj.transform);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localScale = buttonSize;

            // Remove the primitive's collider since we're using the parent's
            DestroyImmediate(visual.GetComponent<BoxCollider>());

            // Create materials using Cable material as base
            Material defaultMat = GetCableMaterial();
            Material glowMat = CreateButtonMaterial("GlowButton", glowColor);
            Material pressedMat = CreateButtonMaterial("PressedButton", pressedColor);

            // Set up renderer
            MeshRenderer renderer = visual.GetComponent<MeshRenderer>();
            renderer.material = defaultMat;

            // Add restart button component
            PhysicalRestartButton restartButton = buttonObj.AddComponent<PhysicalRestartButton>();
            restartButton.buttonTransform = visual.transform;
            restartButton.buttonRenderer = renderer;
            restartButton.defaultMaterial = defaultMat;
            restartButton.glowMaterial = glowMat;
            restartButton.pressedMaterial = pressedMat;
            restartButton.pressDepth = pressDepth;
            restartButton.returnToMenuInsteadOfRestart = returnToMenuInsteadOfRestart;

            // Add audio source
            AudioSource audioSource = buttonObj.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f; // 3D sound
            audioSource.volume = 0.5f;

            // Add light for glow effect
            GameObject lightObj = new GameObject("ButtonLight");
            lightObj.transform.SetParent(buttonObj.transform);
            lightObj.transform.localPosition = Vector3.up * (buttonSize.y * 0.6f);

            Light buttonLight = lightObj.AddComponent<Light>();
            buttonLight.type = LightType.Point;
            buttonLight.color = glowColor;
            buttonLight.intensity = 1f;
            buttonLight.range = 0.5f;
            buttonLight.enabled = false;

            restartButton.buttonLight = buttonLight;

            // Add text if requested
            if (addText)
            {
                CreateButtonText(buttonObj, buttonText, textColor);
            }

            Debug.Log("Physical restart button created successfully!");
        }

        private Material GetCableMaterial()
        {
            Material cableMaterial = Resources.Load<Material>("Assets/Material/Cable");
            if (cableMaterial == null)
            {
#if UNITY_EDITOR
                cableMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Material/Cable.mat");
#endif
            }

            if (cableMaterial != null)
            {
                Debug.Log("✅ Using Cable material for restart button");
                return cableMaterial;
            }
            else
            {
                Debug.LogWarning("⚠️ Cable material not found for restart button, creating fallback");
                return CreateButtonMaterial("DefaultButton", buttonColor);
            }
        }

        private Material CreateButtonMaterial(string name, Color color)
        {
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.name = name;
            mat.color = color;
            mat.SetFloat("_Metallic", 0.2f);
            mat.SetFloat("_Smoothness", 0.8f);
            return mat;
        }

        private void CreateButtonText(GameObject parent, string text, Color color)
        {
            GameObject textObj = new GameObject("ButtonText");
            textObj.transform.SetParent(parent.transform);
            textObj.transform.localPosition = Vector3.up * (buttonSize.y * 0.6f);
            textObj.transform.localRotation = Quaternion.Euler(90, 0, 0);

            TextMesh textMesh = textObj.AddComponent<TextMesh>();
            textMesh.text = text;
            textMesh.color = color;
            textMesh.fontSize = 20;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.characterSize = 0.01f;

            // Make text face the correct way
            textObj.transform.localScale = new Vector3(1, 1, -1);
        }

        [ContextMenu("Clear Restart Button")]
        public void ClearRestartButton()
        {
            PhysicalRestartButton existingButton = GetComponentInChildren<PhysicalRestartButton>();
            if (existingButton != null)
            {
                if (Application.isPlaying)
                    Destroy(existingButton.gameObject);
                else
                    DestroyImmediate(existingButton.gameObject);

                Debug.Log("Restart button cleared!");
            }
            else
            {
                Debug.Log("No restart button found to clear.");
            }
        }
    }
}