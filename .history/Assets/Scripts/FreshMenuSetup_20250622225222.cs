using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VRProject
{
    public class FreshMenuSetup : MonoBehaviour
    {
        [ContextMenu("🚀 CREATE FRESH MENU SYSTEM")]
        public void CreateFreshMenuSystem()
        {
            Debug.Log("🚀🚀🚀 CREATING FRESH MENU SYSTEM FROM SCRATCH 🚀🚀🚀");

            ClearExistingMenus();
            CreateCanvas();
            CreateMainMenu();
            CreateModelSelectionMenu();
            AddMenuController();

            Debug.Log("🎉🎉🎉 FRESH MENU SYSTEM CREATED! 🎉🎉🎉");
        }

        private void ClearExistingMenus()
        {
            Debug.Log("🗑️ Clearing existing menus...");

            GameObject[] toDelete = {
                GameObject.Find("MenuCanvas"),
                GameObject.Find("MenuManager"),
                GameObject.Find("SimpleMenuSystem")
            };

            foreach (GameObject obj in toDelete)
            {
                if (obj != null)
                {
                    if (Application.isPlaying)
                        Destroy(obj);
                    else
                        DestroyImmediate(obj);
                }
            }
        }

        private void CreateCanvas()
        {
            Debug.Log("📱 Creating canvas...");

            GameObject canvasObj = new GameObject("MenuCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasObj.AddComponent<GraphicRaycaster>();

            canvasObj.transform.position = Vector3.forward * 2f + Vector3.up * 1.5f;
            canvasObj.transform.localScale = Vector3.one * 0.001f;

            RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(2000, 1500);
        }

        private void CreateMainMenu()
        {
            Debug.Log("🏠 Creating main menu...");

            GameObject canvas = GameObject.Find("MenuCanvas");

            GameObject mainPanel = new GameObject("MainMenuPanel");
            mainPanel.transform.SetParent(canvas.transform);

            RectTransform rect = mainPanel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            mainPanel.AddComponent<CanvasGroup>();

            CreateButton("IniciarButton", "INICIAR", mainPanel.transform, new Vector2(0, 100), new Vector2(400, 100));
            CreateButton("SalirButton", "SALIR", mainPanel.transform, new Vector2(0, -100), new Vector2(400, 100));
        }

        private void CreateModelSelectionMenu()
        {
            Debug.Log("📋 Creating model selection menu...");

            GameObject canvas = GameObject.Find("MenuCanvas");

            GameObject modelPanel = new GameObject("ModelSelectionPanel");
            modelPanel.transform.SetParent(canvas.transform);

            RectTransform rect = modelPanel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            CanvasGroup cg = modelPanel.AddComponent<CanvasGroup>();

            EditorVisibilityHelper helper = modelPanel.AddComponent<EditorVisibilityHelper>();
            helper.hideAtRuntime = true;
            helper.editorAlpha = 0.7f;

            CreateText("Selecciona tu Mueble", modelPanel.transform, new Vector2(0, 300), 60);
            CreateButton("BackButton", "◀ VOLVER", modelPanel.transform, new Vector2(-400, 300), new Vector2(300, 80));

            CreateText("Mueble 1", modelPanel.transform, new Vector2(-200, 0), 40);
            CreateText("Mueble 2", modelPanel.transform, new Vector2(200, 0), 40);
        }

        private void CreateButton(string name, string text, Transform parent, Vector2 position, Vector2 size)
        {
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent);

            RectTransform rect = buttonObj.AddComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            MeshRenderer meshRenderer = buttonObj.AddComponent<MeshRenderer>();
            MeshFilter meshFilter = buttonObj.AddComponent<MeshFilter>();
            meshFilter.mesh = CreateButtonMesh(size.x / 1000f, size.y / 1000f, 0.05f);

            meshRenderer.material = GetCableMaterial();

            Button button = buttonObj.AddComponent<Button>();
            BoxCollider collider = buttonObj.AddComponent<BoxCollider>();
            collider.size = new Vector3(size.x / 1000f, size.y / 1000f, 0.1f);

            PhysicalButton physButton = buttonObj.AddComponent<PhysicalButton>();

            CreateButtonText(text, buttonObj.transform, size);
        }

        private void CreateButtonText(string text, Transform parent, Vector2 buttonSize)
        {
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(parent);

            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            textRect.anchoredPosition3D = new Vector3(0, 0, -0.03f);

            TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.text = text;
            textComponent.fontSize = buttonSize.y * 0.3f;
            textComponent.color = Color.white;
            textComponent.alignment = TextAlignmentOptions.Center;
        }

        private void CreateText(string text, Transform parent, Vector2 position, float fontSize)
        {
            GameObject textObj = new GameObject("Text_" + text.Replace(" ", ""));
            textObj.transform.SetParent(parent);

            RectTransform rect = textObj.AddComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(800, 100);

            TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.text = text;
            textComponent.fontSize = fontSize;
            textComponent.color = Color.white;
            textComponent.alignment = TextAlignmentOptions.Center;
        }

        private void AddMenuController()
        {
            Debug.Log("🎮 Adding menu controller...");

            GameObject controllerObj = new GameObject("UltraSimpleMenuController");
            controllerObj.AddComponent<UltraSimpleMenu>();
        }

        private Material GetCableMaterial()
        {
            Material cableMaterial = null;
#if UNITY_EDITOR
            cableMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Material/Cable.mat");
#endif

            if (cableMaterial != null)
            {
                return cableMaterial;
            }
            else
            {
                Material fallback = new Material(Shader.Find("Standard"));
                fallback.color = new Color(0.2f, 0.3f, 0.8f, 1f);
                return fallback;
            }
        }

        private Mesh CreateButtonMesh(float width, float height, float depth)
        {
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[24];
            float halfWidth = width * 0.5f;
            float halfHeight = height * 0.5f;
            float halfDepth = depth * 0.5f;

            vertices[0] = new Vector3(-halfWidth, -halfHeight, -halfDepth);
            vertices[1] = new Vector3(halfWidth, -halfHeight, -halfDepth);
            vertices[2] = new Vector3(halfWidth, halfHeight, -halfDepth);
            vertices[3] = new Vector3(-halfWidth, halfHeight, -halfDepth);

            vertices[4] = new Vector3(-halfWidth, -halfHeight, halfDepth);
            vertices[5] = new Vector3(halfWidth, -halfHeight, halfDepth);
            vertices[6] = new Vector3(halfWidth, halfHeight, halfDepth);
            vertices[7] = new Vector3(-halfWidth, halfHeight, halfDepth);

            for (int i = 0; i < 8; i++)
            {
                vertices[i + 8] = vertices[i];
                vertices[i + 16] = vertices[i];
            }

            mesh.vertices = vertices;

            int[] triangles = new int[36];
            triangles[0] = 0; triangles[1] = 2; triangles[2] = 1;
            triangles[3] = 0; triangles[4] = 3; triangles[5] = 2;
            triangles[6] = 4; triangles[7] = 5; triangles[8] = 6;
            triangles[9] = 4; triangles[10] = 6; triangles[11] = 7;
            triangles[12] = 0; triangles[13] = 1; triangles[14] = 5;
            triangles[15] = 0; triangles[16] = 5; triangles[17] = 4;
            triangles[18] = 1; triangles[19] = 2; triangles[20] = 6;
            triangles[21] = 1; triangles[22] = 6; triangles[23] = 5;
            triangles[24] = 2; triangles[25] = 3; triangles[26] = 7;
            triangles[27] = 2; triangles[28] = 7; triangles[29] = 6;
            triangles[30] = 3; triangles[31] = 0; triangles[32] = 4;
            triangles[33] = 3; triangles[34] = 4; triangles[35] = 7;

            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }
    }
}