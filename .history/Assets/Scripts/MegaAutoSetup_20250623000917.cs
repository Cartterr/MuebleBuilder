using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;

namespace VRProject
{
    public class MegaAutoSetup : MonoBehaviour
    {
        [Header("🤖 MEGA AUTO DETECTION")]
        [SerializeField] private bool debugMode = true;
        [SerializeField] private List<GameObject> detectedFurniture = new List<GameObject>();
        [SerializeField] private List<GameObject> detectedMenus = new List<GameObject>();
        [SerializeField] private List<GameObject> detectedButtons = new List<GameObject>();
        [SerializeField] private List<GameObject> detectedCanvases = new List<GameObject>();
        [SerializeField] private List<GameObject> detectedXRRigs = new List<GameObject>();
        [SerializeField] private List<GameObject> detectedCameras = new List<GameObject>();

        private Dictionary<string, List<GameObject>> furnitureSetMap = new Dictionary<string, List<GameObject>>();
        private Dictionary<string, GameObject> componentMap = new Dictionary<string, GameObject>();
        private List<string> furniturePatterns = new List<string>();
        private List<string> menuPatterns = new List<string>();
        private List<string> buttonPatterns = new List<string>();

        [ContextMenu("🚀 MEGA AUTO SETUP EVERYTHING")]
        public void MegaAutoSetupEverything()
        {
            StartCoroutine(MegaAutoSetupCoroutine());
        }

        private IEnumerator MegaAutoSetupCoroutine()
        {
            Log("🤖 MEGA AUTO SETUP INITIATED - SCANNING ENTIRE HIERARCHY...");

            InitializePatterns();
            yield return new WaitForEndOfFrame();

            ScanEntireHierarchy();
            yield return new WaitForEndOfFrame();

            AnalyzeFurnitureStructure();
            yield return new WaitForEndOfFrame();

            DetectAndCreateMenuSystem();
            yield return new WaitForEndOfFrame();

            AutoWireEverything();
            yield return new WaitForEndOfFrame();

            ValidateAndFixEverything();
            yield return new WaitForEndOfFrame();

            Log("✅ MEGA AUTO SETUP COMPLETE - EVERYTHING IS WIRED AND READY!");
        }

        private void InitializePatterns()
        {
            Log("🔍 Initializing detection patterns...");

            furniturePatterns.AddRange(new string[] {
                @"furniture_\d+_\d+",
                @"mueble_\d+_\d+",
                @"chair_\d+",
                @"table_\d+",
                @"sofa_\d+",
                @"bed_\d+",
                @"desk_\d+",
                @"cabinet_\d+",
                @"shelf_\d+",
                @"lamp_\d+",
                @".*furniture.*",
                @".*mueble.*",
                @".*chair.*",
                @".*table.*",
                @".*sofa.*"
            });

            menuPatterns.AddRange(new string[] {
                @".*menu.*",
                @".*canvas.*",
                @".*ui.*",
                @".*panel.*",
                @".*screen.*",
                @".*interface.*"
            });

            buttonPatterns.AddRange(new string[] {
                @".*button.*",
                @".*btn.*",
                @".*mueble.*",
                @".*iniciar.*",
                @".*start.*",
                @".*salir.*",
                @".*exit.*",
                @".*quit.*",
                @".*restart.*",
                @".*back.*",
                @".*return.*"
            });
        }

        private void ScanEntireHierarchy()
        {
            Log("🔍 SCANNING ENTIRE HIERARCHY FOR ALL OBJECTS...");

            detectedFurniture.Clear();
            detectedMenus.Clear();
            detectedButtons.Clear();
            detectedCanvases.Clear();
            detectedXRRigs.Clear();
            detectedCameras.Clear();

            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                if (!obj.scene.IsValid()) continue;

                string objName = obj.name.ToLower();

                // Detect furniture with multiple patterns
                if (IsFurnitureObject(obj, objName))
                {
                    detectedFurniture.Add(obj);
                    Log($"   🪑 FURNITURE DETECTED: {obj.name}");
                }

                // Detect menu-related objects
                if (IsMenuObject(obj, objName))
                {
                    detectedMenus.Add(obj);
                    Log($"   📱 MENU DETECTED: {obj.name}");
                }

                // Detect buttons
                if (IsButtonObject(obj, objName))
                {
                    detectedButtons.Add(obj);
                    Log($"   🔘 BUTTON DETECTED: {obj.name}");
                }

                // Detect canvases
                if (obj.GetComponent<Canvas>() != null)
                {
                    detectedCanvases.Add(obj);
                    Log($"   🖼️ CANVAS DETECTED: {obj.name}");
                }

                // Detect XR Rigs
                if (IsXRRig(obj, objName))
                {
                    detectedXRRigs.Add(obj);
                    Log($"   🥽 XR RIG DETECTED: {obj.name}");
                }

                // Detect cameras
                if (obj.GetComponent<Camera>() != null)
                {
                    detectedCameras.Add(obj);
                    Log($"   📷 CAMERA DETECTED: {obj.name}");
                }
            }

            Log($"🔍 SCAN COMPLETE: Furniture:{detectedFurniture.Count}, Menus:{detectedMenus.Count}, Buttons:{detectedButtons.Count}, Canvases:{detectedCanvases.Count}, XRRigs:{detectedXRRigs.Count}, Cameras:{detectedCameras.Count}");
        }

        private bool IsFurnitureObject(GameObject obj, string objName)
        {
            // Check if object has furniture-like components
            bool hasMeshRenderer = obj.GetComponent<MeshRenderer>() != null;
            bool hasCollider = obj.GetComponent<Collider>() != null;
            bool hasXRGrab = obj.GetComponent<XRGrabInteractable>() != null;

            // Check name patterns
            bool matchesPattern = false;
            foreach (string pattern in furniturePatterns)
            {
                if (Regex.IsMatch(objName, pattern, RegexOptions.IgnoreCase))
                {
                    matchesPattern = true;
                    break;
                }
            }

            // Check parent/child structure
            bool hasParentWithFurnitureName = false;
            Transform parent = obj.transform.parent;
            while (parent != null)
            {
                if (parent.name.ToLower().Contains("furniture") || parent.name.ToLower().Contains("mueble"))
                {
                    hasParentWithFurnitureName = true;
                    break;
                }
                parent = parent.parent;
            }

            // Check if it has furniture-like children
            bool hasFurnitureChildren = false;
            foreach (Transform child in obj.transform)
            {
                string childName = child.name.ToLower();
                if (childName.Contains("mesh") || childName.Contains("model") || childName.Contains("geometry"))
                {
                    hasFurnitureChildren = true;
                    break;
                }
            }

            return matchesPattern || (hasMeshRenderer && hasCollider && hasXRGrab) || hasParentWithFurnitureName || hasFurnitureChildren;
        }

        private bool IsMenuObject(GameObject obj, string objName)
        {
            bool hasCanvas = obj.GetComponent<Canvas>() != null;
            bool hasCanvasGroup = obj.GetComponent<CanvasGroup>() != null;
            bool hasLayoutGroup = obj.GetComponent<LayoutGroup>() != null;

            bool matchesPattern = false;
            foreach (string pattern in menuPatterns)
            {
                if (Regex.IsMatch(objName, pattern, RegexOptions.IgnoreCase))
                {
                    matchesPattern = true;
                    break;
                }
            }

            return matchesPattern || hasCanvas || hasCanvasGroup || hasLayoutGroup;
        }

        private bool IsButtonObject(GameObject obj, string objName)
        {
            bool hasButton = obj.GetComponent<Button>() != null;
            bool hasXRButton = obj.GetComponent<XRSimpleInteractable>() != null;
            bool hasPhysicalButton = obj.GetComponent<PhysicalButton>() != null;

            bool matchesPattern = false;
            foreach (string pattern in buttonPatterns)
            {
                if (Regex.IsMatch(objName, pattern, RegexOptions.IgnoreCase))
                {
                    matchesPattern = true;
                    break;
                }
            }

            bool hasButtonLikeChildren = false;
            foreach (Transform child in obj.transform)
            {
                if (child.GetComponent<Text>() != null || child.GetComponent<TMPro.TextMeshProUGUI>() != null)
                {
                    hasButtonLikeChildren = true;
                    break;
                }
            }

            return matchesPattern || hasButton || hasXRButton || hasPhysicalButton || hasButtonLikeChildren;
        }

        private bool IsXRRig(GameObject obj, string objName)
        {
            return objName.Contains("xr") && (objName.Contains("rig") || objName.Contains("origin")) ||
                   objName.Contains("player") && objName.Contains("spawn") ||
                   obj.GetComponent<UnityEngine.XR.XRNode>() != null ||
                   obj.transform.Find("Camera Offset") != null ||
                   obj.transform.Find("Main Camera") != null;
        }

        private void AnalyzeFurnitureStructure()
        {
            Log("🧠 ANALYZING FURNITURE STRUCTURE AND GROUPING...");

            furnitureSetMap.Clear();

            foreach (GameObject furniture in detectedFurniture)
            {
                string setKey = ExtractFurnitureSetKey(furniture.name);

                if (!furnitureSetMap.ContainsKey(setKey))
                {
                    furnitureSetMap[setKey] = new List<GameObject>();
                }

                furnitureSetMap[setKey].Add(furniture);
                Log($"   📦 GROUPED: {furniture.name} → Set '{setKey}'");
            }

            // Auto-detect set patterns if none found
            if (furnitureSetMap.Count == 0)
            {
                AutoDetectFurnitureSets();
            }

            // Create missing sets if we have less than 3
            while (furnitureSetMap.Count < 3)
            {
                string newSetKey = (furnitureSetMap.Count + 1).ToString();
                furnitureSetMap[newSetKey] = new List<GameObject>();

                // Try to distribute furniture evenly
                int furniturePerSet = Mathf.Max(1, detectedFurniture.Count / 3);
                int startIndex = furnitureSetMap.Count * furniturePerSet;

                for (int i = startIndex; i < Mathf.Min(startIndex + furniturePerSet, detectedFurniture.Count); i++)
                {
                    if (i < detectedFurniture.Count)
                    {
                        furnitureSetMap[newSetKey].Add(detectedFurniture[i]);
                    }
                }

                Log($"   🆕 CREATED SET: '{newSetKey}' with {furnitureSetMap[newSetKey].Count} pieces");
            }

            Log($"🧠 FURNITURE ANALYSIS COMPLETE: {furnitureSetMap.Count} sets detected");
        }

        private string ExtractFurnitureSetKey(string furnitureName)
        {
            // Try multiple extraction patterns
            Match match;

            // Pattern: furniture_X_Y
            match = Regex.Match(furnitureName, @"furniture_(\d+)_\d+", RegexOptions.IgnoreCase);
            if (match.Success) return match.Groups[1].Value;

            // Pattern: mueble_X_Y
            match = Regex.Match(furnitureName, @"mueble_(\d+)_\d+", RegexOptions.IgnoreCase);
            if (match.Success) return match.Groups[1].Value;

            // Pattern: anything_X
            match = Regex.Match(furnitureName, @".*_(\d+)$", RegexOptions.IgnoreCase);
            if (match.Success) return match.Groups[1].Value;

            // Pattern: anythingX
            match = Regex.Match(furnitureName, @".*(\d+)$", RegexOptions.IgnoreCase);
            if (match.Success) return match.Groups[1].Value;

            // Default grouping by name similarity
            if (furnitureName.ToLower().Contains("chair")) return "1";
            if (furnitureName.ToLower().Contains("table")) return "2";
            if (furnitureName.ToLower().Contains("sofa")) return "3";

            return "1"; // Default to set 1
        }

        private void AutoDetectFurnitureSets()
        {
            Log("🔍 AUTO-DETECTING FURNITURE SETS BY SIMILARITY...");

            // Group by name similarity
            var similarityGroups = new Dictionary<string, List<GameObject>>();

            foreach (GameObject furniture in detectedFurniture)
            {
                string baseType = ExtractFurnitureBaseType(furniture.name);

                if (!similarityGroups.ContainsKey(baseType))
                {
                    similarityGroups[baseType] = new List<GameObject>();
                }

                similarityGroups[baseType].Add(furniture);
            }

            // Convert similarity groups to sets
            int setIndex = 1;
            foreach (var group in similarityGroups)
            {
                furnitureSetMap[setIndex.ToString()] = group.Value;
                Log($"   🎯 AUTO-DETECTED SET {setIndex}: {group.Key} ({group.Value.Count} pieces)");
                setIndex++;
            }
        }

        private string ExtractFurnitureBaseType(string name)
        {
            string cleanName = Regex.Replace(name, @"\d+", "", RegexOptions.IgnoreCase);
            cleanName = Regex.Replace(cleanName, @"[_\-\s]+", "_", RegexOptions.IgnoreCase);
            cleanName = cleanName.Trim('_');

            if (string.IsNullOrEmpty(cleanName)) return "furniture";
            return cleanName.ToLower();
        }

        private void DetectAndCreateMenuSystem()
        {
            Log("🏗️ DETECTING AND CREATING MENU SYSTEM...");

            // Find or create menu manager
            GameObject menuManager = FindOrCreateMenuManager();

            // Find or create canvas
            GameObject canvas = FindOrCreateCanvas();

            // Find or create XR setup
            GameObject xrRig = FindOrCreateXRRig();

            // Create furniture spawner if needed
            FurnitureSpawner spawner = menuManager.GetComponent<FurnitureSpawner>();
            if (spawner == null)
            {
                spawner = menuManager.AddComponent<FurnitureSpawner>();
                Log("   🏭 CREATED: FurnitureSpawner");
            }

            // Create furniture selector if needed
            SimpleFurnitureSelector selector = menuManager.GetComponent<SimpleFurnitureSelector>();
            if (selector == null)
            {
                selector = menuManager.AddComponent<SimpleFurnitureSelector>();
                Log("   🎛️ CREATED: SimpleFurnitureSelector");
            }

            // Link components
            selector.furnitureSpawner = spawner;
            selector.menuCanvas = canvas;

            // Store references
            componentMap["MenuManager"] = menuManager;
            componentMap["Canvas"] = canvas;
            componentMap["XRRig"] = xrRig;
            componentMap["FurnitureSpawner"] = spawner.gameObject;
            componentMap["FurnitureSelector"] = selector.gameObject;
        }

        private GameObject FindOrCreateMenuManager()
        {
            // Try to find existing menu manager
            GameObject existing = GameObject.Find("MenuManager");
            if (existing != null)
            {
                Log("   ✅ FOUND: Existing MenuManager");
                return existing;
            }

            // Check detected menus for suitable candidates
            foreach (GameObject menu in detectedMenus)
            {
                if (menu.name.ToLower().Contains("manager") || menu.name.ToLower().Contains("control"))
                {
                    menu.name = "MenuManager";
                    Log($"   🔄 RENAMED: {menu.name} → MenuManager");
                    return menu;
                }
            }

            // Create new one
            GameObject newManager = new GameObject("MenuManager");
            Log("   🆕 CREATED: New MenuManager");
            return newManager;
        }

        private GameObject FindOrCreateCanvas()
        {
            // Try to find existing menu canvas
            foreach (GameObject canvas in detectedCanvases)
            {
                if (canvas.name.ToLower().Contains("menu"))
                {
                    Log($"   ✅ FOUND: Existing menu canvas: {canvas.name}");
                    return canvas;
                }
            }

            // Use any canvas
            if (detectedCanvases.Count > 0)
            {
                GameObject canvas = detectedCanvases[0];
                canvas.name = "MenuCanvas";
                Log($"   🔄 REPURPOSED: Canvas → MenuCanvas");
                return canvas;
            }

            // Create new canvas
            return CreateNewCanvas();
        }

        private GameObject CreateNewCanvas()
        {
            Log("   🆕 CREATING: New Canvas System");

            GameObject canvasObj = new GameObject("MenuCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            GraphicRaycaster raycaster = canvasObj.AddComponent<GraphicRaycaster>();

            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = GetMainCamera();

            // Position canvas intelligently
            Vector3 canvasPosition = Vector3.zero;
            if (detectedXRRigs.Count > 0)
            {
                canvasPosition = detectedXRRigs[0].transform.position + Vector3.forward * 2f + Vector3.up * 1.5f;
            }
            else if (detectedCameras.Count > 0)
            {
                canvasPosition = detectedCameras[0].transform.position + Vector3.forward * 2f;
            }

            canvasObj.transform.position = canvasPosition;
            canvasObj.transform.localScale = Vector3.one * 0.00025f;

            RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(1920, 1080);

            CreateMenuButtons(canvasObj.transform);

            return canvasObj;
        }

        private void CreateMenuButtons(Transform parent)
        {
            Log("   🔘 CREATING: Menu Buttons");

            // Create main menu panel
            GameObject mainPanel = CreatePanel("MainMenuPanel", parent);

            // Create buttons based on detected furniture sets
            float yPos = 0.6f;
            float spacing = 0.2f;

            foreach (var furnitureSet in furnitureSetMap)
            {
                string buttonName = $"Mueble{furnitureSet.Key}Button";
                string buttonText = $"MUEBLE {furnitureSet.Key}";

                CreatePhysicalButton(buttonName, buttonText, mainPanel.transform,
                    new Vector2(0, yPos), new Vector2(0.6f, 0.15f));

                yPos -= spacing;
            }

            // Create exit button
            CreatePhysicalButton("SalirButton", "SALIR", mainPanel.transform,
                new Vector2(0, yPos), new Vector2(0.6f, 0.15f));
        }

        private GameObject CreatePanel(string name, Transform parent)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent);

            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            return panel;
        }

        private GameObject CreatePhysicalButton(string name, string text, Transform parent, Vector2 position, Vector2 size)
        {
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent);

            RectTransform rect = buttonObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position * 1000f;
            rect.sizeDelta = size * 1000f;

            // Add mesh components
            MeshFilter meshFilter = buttonObj.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = buttonObj.AddComponent<MeshRenderer>();
            BoxCollider boxCollider = buttonObj.AddComponent<BoxCollider>();

            // Create button mesh
            meshFilter.mesh = CreateButtonMesh(size.x, size.y, 0.02f);

            // Add materials
            Material buttonMaterial = GetCableMaterial();
            if (buttonMaterial != null)
            {
                meshRenderer.material = buttonMaterial;
            }
            else
            {
                Material defaultMat = new Material(Shader.Find("Standard"));
                defaultMat.color = new Color(0.2f, 0.2f, 0.2f, 1f);
                meshRenderer.material = defaultMat;
            }

            // Add Unity Button component first
            Button unityButton = buttonObj.AddComponent<Button>();
            unityButton.interactable = true;

            // Add XR interaction
            XRSimpleInteractable interactable = buttonObj.AddComponent<XRSimpleInteractable>();
            PhysicalButton physicalButton = buttonObj.AddComponent<PhysicalButton>();

            // Create text
            CreateButtonText(text, buttonObj.transform, Vector2.zero, 0.1f);

            return buttonObj;
        }

        private Mesh CreateButtonMesh(float width, float height, float depth)
        {
            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[8];
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

            mesh.vertices = vertices;

            int[] triangles = new int[36];
            // Front face
            triangles[0] = 0; triangles[1] = 2; triangles[2] = 1;
            triangles[3] = 0; triangles[4] = 3; triangles[5] = 2;
            // Back face
            triangles[6] = 4; triangles[7] = 5; triangles[8] = 6;
            triangles[9] = 4; triangles[10] = 6; triangles[11] = 7;
            // Left face
            triangles[12] = 0; triangles[13] = 4; triangles[14] = 7;
            triangles[15] = 0; triangles[16] = 7; triangles[17] = 3;
            // Right face
            triangles[18] = 1; triangles[19] = 2; triangles[20] = 6;
            triangles[21] = 1; triangles[22] = 6; triangles[23] = 5;
            // Top face
            triangles[24] = 2; triangles[25] = 3; triangles[26] = 7;
            triangles[27] = 2; triangles[28] = 7; triangles[29] = 6;
            // Bottom face
            triangles[30] = 0; triangles[31] = 1; triangles[32] = 5;
            triangles[33] = 0; triangles[34] = 5; triangles[35] = 4;

            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }

        private void CreateButtonText(string text, Transform parent, Vector2 position, float fontSize)
        {
            GameObject textObj = new GameObject("ButtonText");
            textObj.transform.SetParent(parent);

            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            textRect.anchoredPosition = position;

            Text textComponent = textObj.AddComponent<Text>();
            textComponent.text = text;
            textComponent.fontSize = Mathf.RoundToInt(fontSize * 1000);
            textComponent.color = Color.white;
            textComponent.alignment = TextAnchor.MiddleCenter;
            textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        private GameObject FindOrCreateXRRig()
        {
            if (detectedXRRigs.Count > 0)
            {
                Log($"   ✅ FOUND: XR Rig: {detectedXRRigs[0].name}");
                return detectedXRRigs[0];
            }

            Log("   ⚠️ NO XR RIG DETECTED - Menu will use world coordinates");
            return null;
        }

        private Camera GetMainCamera()
        {
            if (detectedCameras.Count > 0)
            {
                return detectedCameras[0].GetComponent<Camera>();
            }

            Camera mainCam = Camera.main;
            if (mainCam != null) return mainCam;

            return FindObjectOfType<Camera>();
        }

        private Material GetCableMaterial()
        {
            #if UNITY_EDITOR
            string[] guids = UnityEditor.AssetDatabase.FindAssets("Cable t:Material");
            if (guids.Length > 0)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                return UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(path);
            }
            #endif
            return null;
        }

        private void AutoWireEverything()
        {
            Log("🔌 AUTO-WIRING ALL COMPONENTS...");

            // Wire furniture spawner
            if (componentMap.ContainsKey("FurnitureSpawner"))
            {
                FurnitureSpawner spawner = componentMap["FurnitureSpawner"].GetComponent<FurnitureSpawner>();
                if (spawner != null)
                {
                    // Populate furniture data
                    spawner.originalFurnitureData.Clear();

                    foreach (var furnitureSet in furnitureSetMap)
                    {
                        foreach (GameObject furniture in furnitureSet.Value)
                        {
                            var data = new FurnitureSpawner.FurnitureData();
                            data.name = furniture.name;
                            data.position = furniture.transform.position;
                            data.rotation = furniture.transform.rotation;
                            data.scale = furniture.transform.localScale;
                            data.prefab = furniture;
                            data.setNumber = furnitureSet.Key;

                            spawner.originalFurnitureData.Add(data);
                        }
                    }

                    Log($"   🏭 WIRED: FurnitureSpawner with {spawner.originalFurnitureData.Count} furniture pieces");
                }
            }

            // Wire furniture selector
            if (componentMap.ContainsKey("FurnitureSelector"))
            {
                SimpleFurnitureSelector selector = componentMap["FurnitureSelector"].GetComponent<SimpleFurnitureSelector>();
                if (selector != null)
                {
                    selector.menuCanvas = componentMap.ContainsKey("Canvas") ? componentMap["Canvas"] : null;
                    selector.furnitureSpawner = componentMap.ContainsKey("FurnitureSpawner") ?
                        componentMap["FurnitureSpawner"].GetComponent<FurnitureSpawner>() : null;

                    Log("   🎛️ WIRED: SimpleFurnitureSelector with all references");
                }
            }

            // Wire buttons
            WireAllButtons();

            // Create restart system
            CreateRestartSystem();

            // Create origin point
            CreateOriginPoint();
        }

                private void WireAllButtons()
        {
            Log("   🔘 WIRING ALL BUTTONS...");

            PhysicalButton[] allPhysicalButtons = FindObjectsOfType<PhysicalButton>();
            Button[] allUnityButtons = FindObjectsOfType<Button>();

            SimpleFurnitureSelector selector = componentMap.ContainsKey("FurnitureSelector") ?
                componentMap["FurnitureSelector"].GetComponent<SimpleFurnitureSelector>() : null;

            if (selector == null)
            {
                Log("   ⚠️ NO SELECTOR FOUND FOR BUTTON WIRING");
                return;
            }

            // Wire PhysicalButtons through their Unity Button components
            foreach (PhysicalButton physicalButton in allPhysicalButtons)
            {
                Button unityButton = physicalButton.GetComponent<Button>();
                if (unityButton == null)
                {
                    unityButton = physicalButton.gameObject.AddComponent<Button>();
                    Log($"   🔧 ADDED: Unity Button component to {physicalButton.name}");
                }

                WireUnityButton(unityButton, selector);
            }

            // Wire any standalone Unity Buttons (avoid duplicates)
            foreach (Button unityButton in allUnityButtons)
            {
                // Skip if this button is already handled by a PhysicalButton
                bool alreadyHandled = false;
                foreach (PhysicalButton physicalButton in allPhysicalButtons)
                {
                    if (physicalButton.GetComponent<Button>() == unityButton)
                    {
                        alreadyHandled = true;
                        break;
                    }
                }

                if (!alreadyHandled)
                {
                    WireUnityButton(unityButton, selector);
                }
            }
        }

                private void WireUnityButton(Button unityButton, SimpleFurnitureSelector selector)
        {
            if (unityButton == null || selector == null) return;

            string buttonName = unityButton.name.ToLower();

            if (buttonName.Contains("mueble1") || buttonName.Contains("furniture1"))
            {
                unityButton.onClick.RemoveAllListeners();
                unityButton.onClick.AddListener(() => {
                    if (selector != null) selector.SelectMueble1();
                });
                Log($"   🔗 WIRED: {unityButton.name} → SelectMueble1()");
            }
            else if (buttonName.Contains("mueble2") || buttonName.Contains("furniture2"))
            {
                unityButton.onClick.RemoveAllListeners();
                unityButton.onClick.AddListener(() => {
                    if (selector != null) selector.SelectMueble2();
                });
                Log($"   🔗 WIRED: {unityButton.name} → SelectMueble2()");
            }
            else if (buttonName.Contains("mueble3") || buttonName.Contains("furniture3"))
            {
                unityButton.onClick.RemoveAllListeners();
                unityButton.onClick.AddListener(() => {
                    if (selector != null) selector.SelectMueble3();
                });
                Log($"   🔗 WIRED: {unityButton.name} → SelectMueble3()");
            }
            else if (buttonName.Contains("salir") || buttonName.Contains("exit") || buttonName.Contains("quit"))
            {
                unityButton.onClick.RemoveAllListeners();
                unityButton.onClick.AddListener(() => {
                    if (selector != null) selector.ExitApplication();
                });
                Log($"   🔗 WIRED: {unityButton.name} → ExitApplication()");
            }
        }

        private void CreateRestartSystem()
        {
            Log("   🔴 CREATING: Restart System");

            GameObject restartParent = GameObject.Find("RestartButtonParent");
            if (restartParent == null)
            {
                restartParent = new GameObject("RestartButtonParent");

                // Position near XR rig if available
                if (detectedXRRigs.Count > 0)
                {
                    restartParent.transform.position = detectedXRRigs[0].transform.position + Vector3.right * 1f;
                }

                RestartButtonSetup restartSetup = restartParent.AddComponent<RestartButtonSetup>();

                // Wire to selector
                if (componentMap.ContainsKey("FurnitureSelector"))
                {
                    SimpleFurnitureSelector selector = componentMap["FurnitureSelector"].GetComponent<SimpleFurnitureSelector>();
                    selector.restartButtonSetup = restartSetup;
                }

                Log("   🔴 CREATED: RestartButtonSetup");
            }
        }

        private void CreateOriginPoint()
        {
            Log("   🎯 CREATING: Origin Point");

            GameObject originPoint = GameObject.Find("RecenterOriginPoint");
            if (originPoint == null)
            {
                originPoint = new GameObject("RecenterOriginPoint");

                // Position at XR rig if available
                if (detectedXRRigs.Count > 0)
                {
                    originPoint.transform.position = detectedXRRigs[0].transform.position;
                    originPoint.transform.rotation = detectedXRRigs[0].transform.rotation * Quaternion.Euler(0, 180, 0);
                }

                RecenterOriginPoint recenterScript = originPoint.AddComponent<RecenterOriginPoint>();

                Log("   🎯 CREATED: RecenterOriginPoint");
            }
        }

        private void ValidateAndFixEverything()
        {
            Log("🔧 VALIDATING AND FIXING EVERYTHING...");

            // Validate furniture spawner
            ValidateFurnitureSpawner();

            // Validate furniture selector
            ValidateFurnitureSelector();

            // Validate canvas and UI
            ValidateCanvasSystem();

            // Validate XR setup
            ValidateXRSetup();

            // Final initialization
            FinalInitialization();
        }

        private void ValidateFurnitureSpawner()
        {
            if (!componentMap.ContainsKey("FurnitureSpawner")) return;

            FurnitureSpawner spawner = componentMap["FurnitureSpawner"].GetComponent<FurnitureSpawner>();
            if (spawner == null) return;

            if (spawner.originalFurnitureData.Count == 0)
            {
                Log("   ⚠️ FIXING: Empty furniture data, repopulating...");
                // Repopulate from detected furniture
                foreach (GameObject furniture in detectedFurniture)
                {
                    var data = new FurnitureSpawner.FurnitureData();
                    data.name = furniture.name;
                    data.position = furniture.transform.position;
                    data.rotation = furniture.transform.rotation;
                    data.scale = furniture.transform.localScale;
                    data.prefab = furniture;
                    data.setNumber = ExtractFurnitureSetKey(furniture.name);

                    spawner.originalFurnitureData.Add(data);
                }
            }

            Log($"   ✅ VALIDATED: FurnitureSpawner ({spawner.originalFurnitureData.Count} pieces)");
        }

        private void ValidateFurnitureSelector()
        {
            if (!componentMap.ContainsKey("FurnitureSelector")) return;

            SimpleFurnitureSelector selector = componentMap["FurnitureSelector"].GetComponent<SimpleFurnitureSelector>();
            if (selector == null) return;

            if (selector.furnitureSpawner == null && componentMap.ContainsKey("FurnitureSpawner"))
            {
                selector.furnitureSpawner = componentMap["FurnitureSpawner"].GetComponent<FurnitureSpawner>();
                Log("   🔧 FIXED: Missing FurnitureSpawner reference");
            }

            if (selector.menuCanvas == null && componentMap.ContainsKey("Canvas"))
            {
                selector.menuCanvas = componentMap["Canvas"];
                Log("   🔧 FIXED: Missing MenuCanvas reference");
            }

            Log("   ✅ VALIDATED: SimpleFurnitureSelector");
        }

        private void ValidateCanvasSystem()
        {
            if (!componentMap.ContainsKey("Canvas")) return;

            GameObject canvas = componentMap["Canvas"];
            Canvas canvasComponent = canvas.GetComponent<Canvas>();

            if (canvasComponent != null && canvasComponent.worldCamera == null)
            {
                canvasComponent.worldCamera = GetMainCamera();
                Log("   🔧 FIXED: Missing Canvas camera reference");
            }

            Log("   ✅ VALIDATED: Canvas System");
        }

        private void ValidateXRSetup()
        {
            // Ensure EventSystem exists for UI interaction
            UnityEngine.EventSystems.EventSystem eventSystem = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
            if (eventSystem == null)
            {
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemObj.AddComponent<UnityEngine.XR.Interaction.Toolkit.UI.XRUIInputModule>();
                Log("   🔧 CREATED: EventSystem with XRUIInputModule");
            }

            Log("   ✅ VALIDATED: XR Setup");
        }

        private void FinalInitialization()
        {
            Log("🚀 FINAL INITIALIZATION...");

            // Force initialize selector if available
            if (componentMap.ContainsKey("FurnitureSelector"))
            {
                SimpleFurnitureSelector selector = componentMap["FurnitureSelector"].GetComponent<SimpleFurnitureSelector>();
                if (selector != null)
                {
                    StartCoroutine(DelayedInitialization(selector));
                }
            }
        }

        private IEnumerator DelayedInitialization(SimpleFurnitureSelector selector)
        {
            yield return new WaitForSeconds(0.1f);
            selector.ForceInitialize();
            Log("   🚀 EXECUTED: Final selector initialization");
        }

        private void Log(string message)
        {
            if (debugMode)
            {
                Debug.Log($"[MegaAutoSetup] {message}");
            }
        }

        [ContextMenu("🔍 DEBUG: Show Detection Results")]
        public void ShowDetectionResults()
        {
            Log("🔍 DETECTION RESULTS:");
            Log($"   Furniture: {detectedFurniture.Count}");
            Log($"   Menus: {detectedMenus.Count}");
            Log($"   Buttons: {detectedButtons.Count}");
            Log($"   Canvases: {detectedCanvases.Count}");
            Log($"   XR Rigs: {detectedXRRigs.Count}");
            Log($"   Cameras: {detectedCameras.Count}");
            Log($"   Furniture Sets: {furnitureSetMap.Count}");
        }
    }
}