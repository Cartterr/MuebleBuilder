using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

namespace VRProject
{
    public class FurnitureSelectionManager : MonoBehaviour
    {
        [Header("UI Panels with Animators")]
        public UIAnimator mainMenuAnimator;
        public UIAnimator modelSelectionAnimator;
        public UIAnimator gameplayAnimator;

        [Header("Buttons")]
        public Button iniciarButton;
        public Button salirButton;
        public Button backToMenuButton;

        [Header("Model Selection")]
        public Transform modelGridParent;
        public GameObject modelButtonPrefab;

        [Header("Restart Button")]
        public RestartButtonSetup restartButtonSetup;

        [Header("Audio")]
        public AudioSource buttonAudioSource;
        public AudioClip buttonClickSound;
        public AudioClip menuTransitionSound;

        private MenuState currentState = MenuState.MainMenu;
        private List<FurnitureSet> furnitureSets = new List<FurnitureSet>();
        private FurnitureSet currentSelectedSet;

        private enum MenuState
        {
            MainMenu,
            ModelSelection,
            Gameplay
        }

        [System.Serializable]
        public class FurnitureSet
        {
            public string name;
            public List<GameObject> pieces;
            public Sprite previewImage;
        }

        private void Start()
        {
            SetupUI();
            HideAllFurniture();
            ShowMainMenu();
        }

        private void SetupUI()
        {
            if (iniciarButton != null)
            {
                iniciarButton.onClick.AddListener(() => {
                    PlayButtonSound();
                    TransitionToModelSelection();
                });
            }

            if (salirButton != null)
            {
                salirButton.onClick.AddListener(() => {
                    PlayButtonSound();
                    ExitApplication();
                });
            }

            if (backToMenuButton != null)
            {
                backToMenuButton.onClick.AddListener(() => {
                    PlayButtonSound();
                    ShowMainMenu();
                });
            }
        }

        public void FindAllFurnitureSets()
        {
            furnitureSets.Clear();

            // Find all furniture sets by looking for parent objects named furniture_1, furniture_2, etc.
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            Dictionary<string, FurnitureSet> setsDict = new Dictionary<string, FurnitureSet>();

            foreach (GameObject obj in allObjects)
            {
                if (obj.name.StartsWith("furniture_"))
                {
                    string[] parts = obj.name.Split('_');
                    if (parts.Length >= 2)
                    {
                        string setNumber = parts[1];
                        string setName = $"furniture_{setNumber}";

                        if (!setsDict.ContainsKey(setName))
                        {
                            setsDict[setName] = new FurnitureSet
                            {
                                name = setName,
                                pieces = new List<GameObject>()
                            };
                        }

                        setsDict[setName].pieces.Add(obj);
                    }
                }
            }

            furnitureSets.AddRange(setsDict.Values);
            Debug.Log($"Found {furnitureSets.Count} furniture sets with {GetTotalPieceCount()} total pieces");
        }

        private int GetTotalPieceCount()
        {
            int total = 0;
            foreach (FurnitureSet set in furnitureSets)
            {
                total += set.pieces.Count;
            }
            return total;
        }

        public void SetupFurnitureButtons()
        {
            if (modelGridParent == null) return;

            // Clear existing buttons
            foreach (Transform child in modelGridParent)
            {
                if (Application.isPlaying)
                    Destroy(child.gameObject);
                else
                    DestroyImmediate(child.gameObject);
            }

            // Create button for each furniture set
            for (int i = 0; i < furnitureSets.Count; i++)
            {
                FurnitureSet set = furnitureSets[i];
                CreateFurnitureButton(set, i);
            }
        }

                private void CreateFurnitureButton(FurnitureSet furnitureSet, int index)
        {
            GameObject buttonObj = new GameObject($"FurnitureButton_{furnitureSet.name}");
            buttonObj.transform.SetParent(modelGridParent);

            RectTransform rect = buttonObj.AddComponent<RectTransform>();
            rect.localScale = Vector3.one;

            MeshRenderer meshRenderer = buttonObj.AddComponent<MeshRenderer>();
            MeshFilter meshFilter = buttonObj.AddComponent<MeshFilter>();
            meshFilter.mesh = CreateSquareMesh(1f, 1f, 0.05f);

            meshRenderer.material = GetCableMaterial();

                        Button button = buttonObj.AddComponent<Button>();

            BoxCollider collider = buttonObj.AddComponent<BoxCollider>();
            collider.size = new Vector3(1f, 1f, 0.1f);

            PhysicalButton physButton = buttonObj.AddComponent<PhysicalButton>();

            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform);

            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            textRect.anchoredPosition3D = new Vector3(0, 0, -0.03f);

            Text textComponent = textObj.AddComponent<Text>();
            textComponent.text = furnitureSet.name.Replace("furniture_", "MUEBLE ");
            textComponent.fontSize = 16;
            textComponent.color = Color.white;
            textComponent.alignment = TextAnchor.MiddleCenter;
            textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            button.onClick.AddListener(() => SelectFurnitureSet(furnitureSet));

            StartCoroutine(AnimateButtonIn(buttonObj, index * 0.1f));
        }

        private Mesh CreateSquareMesh(float width, float height, float depth)
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

            vertices[4] = new Vector3(-halfWidth, halfHeight, halfDepth);
            vertices[5] = new Vector3(halfWidth, halfHeight, halfDepth);
            vertices[6] = new Vector3(halfWidth, -halfHeight, halfDepth);
            vertices[7] = new Vector3(-halfWidth, -halfHeight, halfDepth);

            vertices[8] = new Vector3(-halfWidth, halfHeight, -halfDepth);
            vertices[9] = new Vector3(-halfWidth, halfHeight, halfDepth);
            vertices[10] = new Vector3(-halfWidth, -halfHeight, halfDepth);
            vertices[11] = new Vector3(-halfWidth, -halfHeight, -halfDepth);

            vertices[12] = new Vector3(halfWidth, -halfHeight, -halfDepth);
            vertices[13] = new Vector3(halfWidth, -halfHeight, halfDepth);
            vertices[14] = new Vector3(halfWidth, halfHeight, halfDepth);
            vertices[15] = new Vector3(halfWidth, halfHeight, -halfDepth);

            vertices[16] = new Vector3(-halfWidth, -halfHeight, -halfDepth);
            vertices[17] = new Vector3(-halfWidth, -halfHeight, halfDepth);
            vertices[18] = new Vector3(halfWidth, -halfHeight, halfDepth);
            vertices[19] = new Vector3(halfWidth, -halfHeight, -halfDepth);

            vertices[20] = new Vector3(halfWidth, halfHeight, -halfDepth);
            vertices[21] = new Vector3(halfWidth, halfHeight, halfDepth);
            vertices[22] = new Vector3(-halfWidth, halfHeight, halfDepth);
            vertices[23] = new Vector3(-halfWidth, halfHeight, -halfDepth);

            mesh.vertices = vertices;

            int[] triangles = new int[36];

            triangles[0] = 0; triangles[1] = 2; triangles[2] = 1;
            triangles[3] = 0; triangles[4] = 3; triangles[5] = 2;

            triangles[6] = 4; triangles[7] = 5; triangles[8] = 6;
            triangles[9] = 4; triangles[10] = 6; triangles[11] = 7;

            triangles[12] = 8; triangles[13] = 9; triangles[14] = 10;
            triangles[15] = 8; triangles[16] = 10; triangles[17] = 11;

            triangles[18] = 12; triangles[19] = 14; triangles[20] = 13;
            triangles[21] = 12; triangles[22] = 15; triangles[23] = 14;

            triangles[24] = 16; triangles[25] = 17; triangles[26] = 18;
            triangles[27] = 16; triangles[28] = 18; triangles[29] = 19;

            triangles[30] = 20; triangles[31] = 22; triangles[32] = 21;
            triangles[33] = 20; triangles[34] = 23; triangles[35] = 22;

            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }

        private IEnumerator AnimateButtonIn(GameObject buttonObj, float delay)
        {
            buttonObj.transform.localScale = Vector3.zero;
            yield return new WaitForSeconds(delay);

            float elapsedTime = 0f;
            float duration = 0.4f;

            while (elapsedTime < duration)
            {
                float progress = elapsedTime / duration;
                buttonObj.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, progress);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            buttonObj.transform.localScale = Vector3.one;
        }

        public void SelectFurnitureSet(FurnitureSet furnitureSet)
        {
            Debug.Log($"🪑 SelectFurnitureSet called for: {furnitureSet.name}");
            Debug.Log($"🪑 Furniture set has {furnitureSet.pieces.Count} pieces");

            currentSelectedSet = furnitureSet;
            HideAllFurniture();
            ShowFurnitureSet(furnitureSet);
            StartGameplay();

            Debug.Log($"🪑 SelectFurnitureSet completed for: {furnitureSet.name}");
        }

        private void HideAllFurniture()
        {
            Debug.Log("🙈 Hiding all furniture sets");

            if (furnitureSets.Count > 0)
            {
                foreach (FurnitureSet set in furnitureSets)
                {
                    foreach (GameObject piece in set.pieces)
                    {
                        if (piece != null)
                        {
                            piece.SetActive(false);
                            Debug.Log($"🙈 Hid furniture piece: {piece.name}");
                        }
                    }
                }
                Debug.Log($"🙈 Hid {furnitureSets.Count} furniture sets via direct SetActive(false)");
            }
            else
            {
                Debug.Log("⚠️ No furniture sets found, searching for furniture_* objects...");
                GameObject[] allObjects = FindObjectsOfType<GameObject>();
                int hiddenCount = 0;
                foreach (GameObject obj in allObjects)
                {
                    if (obj.name.StartsWith("furniture_"))
                    {
                        obj.SetActive(false);
                        hiddenCount++;
                        Debug.Log($"🙈 Hid furniture: {obj.name}");
                    }
                }
                Debug.Log($"🙈 Hid {hiddenCount} furniture objects directly");
            }
        }

        private void ShowFurnitureSet(FurnitureSet furnitureSet)
        {
            Debug.Log($"👁️ Showing furniture set: {furnitureSet.name} with {furnitureSet.pieces.Count} pieces");

            foreach (GameObject piece in furnitureSet.pieces)
            {
                if (piece != null)
                {
                    piece.SetActive(true);
                    Debug.Log($"👁️ Showed furniture piece: {piece.name}");
                }
                else
                {
                    Debug.LogWarning($"⚠️ Null furniture piece found in set {furnitureSet.name}");
                }
            }

            Debug.Log($"👁️ Finished showing furniture set: {furnitureSet.name}");
        }

        public void ShowMainMenu()
        {
            if (currentState == MenuState.MainMenu) return;

            HideCurrentPanel(() => {
                mainMenuAnimator?.AnimateIn();
                currentState = MenuState.MainMenu;
                PlayTransitionSound();
            });
        }

        public void TransitionToModelSelection()
        {
            Debug.Log("🚀🚀🚀 TRANSITION TO MODEL SELECTION CALLED! 🚀🚀🚀");

            if (currentState == MenuState.ModelSelection)
            {
                Debug.Log("⚠️ Already in ModelSelection state, returning");
                return;
            }

            Debug.Log($"🔄 Current state: {currentState}");
            Debug.Log($"🔄 Current furniture sets count: {furnitureSets.Count}");

            if (furnitureSets.Count == 0)
            {
                Debug.Log("🔍 No furniture sets found, searching now...");
                FindAllFurnitureSets();
                Debug.Log($"🔍 After search, furniture sets count: {furnitureSets.Count}");
                SetupFurnitureButtons();
                Debug.Log("🔍 Furniture buttons setup completed");
            }

            Debug.Log("🔄 About to hide current panel...");
            HideCurrentPanel(() => {
                Debug.Log("🔄 Current panel hidden, now showing model selection...");

                if (modelSelectionAnimator != null)
                {
                    Debug.Log("✅ modelSelectionAnimator found!");

                    EditorVisibilityHelper editorHelper = modelSelectionAnimator.GetComponent<EditorVisibilityHelper>();
                    if (editorHelper != null)
                    {
                        Debug.Log("✅ EditorVisibilityHelper found, calling SetVisibility(true)");
                        editorHelper.SetVisibility(true);
                        Debug.Log("✅ Model selection panel shown via EditorVisibilityHelper");
                    }
                    else
                    {
                        Debug.Log("⚠️ No EditorVisibilityHelper, using AnimateIn()");
                        modelSelectionAnimator.AnimateIn();
                        Debug.Log("✅ Model selection panel animated in");
                    }
                }
                else
                {
                    Debug.LogError("❌ RED X RED - Model selection animator is null!");
                    GameObject modelPanel = GameObject.Find("ModelSelectionPanel");
                    if (modelPanel != null)
                    {
                        Debug.Log($"✅ Found ModelSelectionPanel manually: {modelPanel.name}");
                        Debug.Log($"   - Active: {modelPanel.activeSelf}");

                        EditorVisibilityHelper editorHelper = modelPanel.GetComponent<EditorVisibilityHelper>();
                        if (editorHelper != null)
                        {
                            Debug.Log("✅ EditorVisibilityHelper found on panel, calling SetVisibility(true)");
                            editorHelper.SetVisibility(true);
                        }
                        else
                        {
                            Debug.Log("⚠️ No EditorVisibilityHelper on panel, using SetActive(true)");
                            modelPanel.SetActive(true);
                        }
                        Debug.Log("⚠️ Manually activated ModelSelectionPanel");
                    }
                    else
                    {
                        Debug.LogError("❌ RED X RED - ModelSelectionPanel not found!");
                    }
                }

                currentState = MenuState.ModelSelection;
                Debug.Log($"🔄 State changed to: {currentState}");
                PlayTransitionSound();
                Debug.Log("🎵 Transition sound played");
                Debug.Log("🎉🎉🎉 TRANSITION TO MODEL SELECTION COMPLETED! 🎉🎉🎉");
            });
        }

        public void StartGameplay()
        {
            if (currentState == MenuState.Gameplay) return;

            HideCurrentPanel(() => {
                gameplayAnimator?.AnimateIn();
                currentState = MenuState.Gameplay;
                PlayTransitionSound();
                ShowRestartButton();
            });
        }

        private void HideCurrentPanel(System.Action onComplete)
        {
            switch (currentState)
            {
                case MenuState.MainMenu:
                    if (mainMenuAnimator != null)
                        mainMenuAnimator.AnimateOut(onComplete);
                    else
                        onComplete?.Invoke();
                    break;

                case MenuState.ModelSelection:
                    if (modelSelectionAnimator != null)
                        modelSelectionAnimator.AnimateOut(onComplete);
                    else
                        onComplete?.Invoke();
                    break;

                case MenuState.Gameplay:
                    if (gameplayAnimator != null)
                        gameplayAnimator.AnimateOut(onComplete);
                    else
                        onComplete?.Invoke();
                    break;

                default:
                    onComplete?.Invoke();
                    break;
            }
        }

        private void ShowRestartButton()
        {
            if (restartButtonSetup != null)
            {
                restartButtonSetup.ClearRestartButton();
                restartButtonSetup.CreateRestartButton();
                restartButtonSetup.gameObject.SetActive(true);
            }
        }

        private void HideRestartButton()
        {
            if (restartButtonSetup != null)
            {
                restartButtonSetup.ClearRestartButton();
            }
        }

        public void ReturnToMainMenu()
        {
            HideRestartButton();
            HideAllFurniture();
            currentSelectedSet = null;
            ShowMainMenu();
        }

        private void PlayButtonSound()
        {
            if (buttonAudioSource != null && buttonClickSound != null)
            {
                buttonAudioSource.PlayOneShot(buttonClickSound);
            }
        }

        private void PlayTransitionSound()
        {
            if (buttonAudioSource != null && menuTransitionSound != null)
            {
                buttonAudioSource.PlayOneShot(menuTransitionSound);
            }
        }

        public void ExitApplication()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
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
                Debug.Log("✅ Using Cable material for furniture buttons");
                return cableMaterial;
            }
            else
            {
                Debug.LogWarning("⚠️ Cable material not found, creating fallback material");
                Material fallbackMaterial = new Material(Shader.Find("Standard"));
                fallbackMaterial.color = new Color(0.3f, 0.5f, 0.8f, 1f);
                fallbackMaterial.SetFloat("_Metallic", 0.3f);
                fallbackMaterial.SetFloat("_Glossiness", 0.7f);
                return fallbackMaterial;
            }
        }
    }
}