using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace VRProject
{
    public class SimpleFurnitureSelector : MonoBehaviour
    {
                [Header("Direct Furniture Buttons")]
        public Button mueble1Button;
        public Button mueble2Button;
        public Button mueble3Button;
        public Button salirButton;

        [Header("Restart Button")]
        public RestartButtonSetup restartButtonSetup;

                [Header("Menu Management")]
        public GameObject menuCanvas;

        [Header("Furniture Spawning")]
        public FurnitureSpawner furnitureSpawner;

        private bool isProcessingSelection = false;

                private void Start()
        {
            Debug.Log("🪑🪑🪑 SIMPLE FURNITURE SELECTOR STARTING 🪑🪑🪑");

            StartCoroutine(InitializeWithDelay());
        }

                private System.Collections.IEnumerator InitializeWithDelay()
        {
            yield return new WaitForSeconds(0.1f);

            for (int attempts = 0; attempts < 5; attempts++)
            {
                Debug.Log($"🔄 Initialization attempt {attempts + 1}/5");

                FindAllButtons();

                if (mueble1Button != null && mueble2Button != null && mueble3Button != null && salirButton != null)
                {
                    Debug.Log("✅ All buttons found successfully!");
                    break;
                }

                Debug.Log($"⚠️ Not all buttons found, waiting and retrying... (attempt {attempts + 1})");
                yield return new WaitForSeconds(0.2f);
            }

                        ConnectDirectButtons();

            // Find the furniture spawner
            if (furnitureSpawner == null)
            {
                furnitureSpawner = FindObjectOfType<FurnitureSpawner>();
                Debug.Log($"🏭 FurnitureSpawner found: {(furnitureSpawner != null ? "✅" : "❌")}");
            }

            // Wait for FurnitureSpawner to finish storing data
            yield return new WaitForSeconds(0.5f);

            Debug.Log("🔄 Ensuring clean furniture state...");
            if (furnitureSpawner != null)
            {
                furnitureSpawner.ClearAllFurniture();
            }

            HideRestartButton();

            Debug.Log("✅✅✅ SIMPLE FURNITURE SELECTOR READY ✅✅✅");
        }

                private void FindAllButtons()
        {
            Debug.Log("🔍 Finding all buttons...");

            if (mueble1Button == null)
            {
                GameObject mueble1Obj = GameObject.Find("Mueble1Button");
                if (mueble1Obj != null)
                {
                    mueble1Button = mueble1Obj.GetComponent<Button>();
                    if (mueble1Button == null)
                    {
                        Debug.Log("🔧 Mueble1Button found but no Button component, adding one...");
                        mueble1Button = mueble1Obj.AddComponent<Button>();
                    }
                }
            }

            if (mueble2Button == null)
            {
                GameObject mueble2Obj = GameObject.Find("Mueble2Button");
                if (mueble2Obj != null)
                {
                    mueble2Button = mueble2Obj.GetComponent<Button>();
                    if (mueble2Button == null)
                    {
                        Debug.Log("🔧 Mueble2Button found but no Button component, adding one...");
                        mueble2Button = mueble2Obj.AddComponent<Button>();
                    }
                }
            }

            if (mueble3Button == null)
            {
                GameObject mueble3Obj = GameObject.Find("Mueble3Button");
                if (mueble3Obj != null)
                {
                    mueble3Button = mueble3Obj.GetComponent<Button>();
                    if (mueble3Button == null)
                    {
                        Debug.Log("🔧 Mueble3Button found but no Button component, adding one...");
                        mueble3Button = mueble3Obj.AddComponent<Button>();
                    }
                }
            }

            if (salirButton == null)
            {
                GameObject salirObj = GameObject.Find("SalirButton");
                if (salirObj != null)
                {
                    salirButton = salirObj.GetComponent<Button>();
                    if (salirButton == null)
                    {
                        Debug.Log("🔧 SalirButton found but no Button component, adding one...");
                        salirButton = salirObj.AddComponent<Button>();
                    }
                }
            }

            Debug.Log($"   - Mueble1Button: {(mueble1Button != null ? "✅ Found" : "❌ RED X RED - NOT FOUND")}");
            Debug.Log($"   - Mueble2Button: {(mueble2Button != null ? "✅ Found" : "❌ RED X RED - NOT FOUND")}");
            Debug.Log($"   - Mueble3Button: {(mueble3Button != null ? "✅ Found" : "❌ RED X RED - NOT FOUND")}");
            Debug.Log($"   - SalirButton: {(salirButton != null ? "✅ Found" : "❌ RED X RED - NOT FOUND")}");
        }

                private void ConnectDirectButtons()
        {
            Debug.Log("🔗 Connecting direct furniture buttons...");

            if (mueble1Button != null)
            {
                mueble1Button.onClick.RemoveAllListeners();
                mueble1Button.onClick.AddListener(() => {
                    if (!isProcessingSelection)
                    {
                        Debug.Log("🪑🪑🪑 MUEBLE 1 SELECTED! 🪑🪑🪑");
                        SelectFurnitureSet("1");
                    }
                });
                Debug.Log("✅ Mueble1Button connected");
            }

            if (mueble2Button != null)
            {
                mueble2Button.onClick.RemoveAllListeners();
                mueble2Button.onClick.AddListener(() => {
                    if (!isProcessingSelection)
                    {
                        Debug.Log("🪑🪑🪑 MUEBLE 2 SELECTED! 🪑🪑🪑");
                        SelectFurnitureSet("furniture_2");
                    }
                });
                Debug.Log("✅ Mueble2Button connected");
            }

            if (mueble3Button != null)
            {
                mueble3Button.onClick.RemoveAllListeners();
                mueble3Button.onClick.AddListener(() => {
                    if (!isProcessingSelection)
                    {
                        Debug.Log("🪑🪑🪑 MUEBLE 3 SELECTED! 🪑🪑🪑");
                        SelectFurnitureSet("furniture_3");
                    }
                });
                Debug.Log("✅ Mueble3Button connected");
            }

            if (salirButton != null)
            {
                salirButton.onClick.RemoveAllListeners();
                salirButton.onClick.AddListener(() => {
                    if (!isProcessingSelection)
                    {
                        Debug.Log("👋 SALIR PRESSED - EXITING!");
                        ExitApplication();
                    }
                });
                Debug.Log("✅ SalirButton connected");
            }
        }

                        private void SelectFurnitureSet(string setPrefix)
        {
            if (isProcessingSelection)
            {
                Debug.Log("⚠️ Already processing selection, ignoring...");
                return;
            }

            isProcessingSelection = true;
            Debug.Log($"🪑 Selecting furniture set: {setPrefix}");

            HideMenu();

            if (furnitureSpawner != null)
            {
                Debug.Log($"🏗️ Using FurnitureSpawner to spawn set {setPrefix}");
                furnitureSpawner.SpawnFurnitureSet(setPrefix);
            }
            else
            {
                Debug.LogError("❌ FurnitureSpawner not found! Cannot spawn furniture.");
            }

            ShowRestartButton();

            Debug.Log($"✅ {setPrefix} furniture set activated!");

            StartCoroutine(ResetProcessingFlag());
        }

        private System.Collections.IEnumerator ResetProcessingFlag()
        {
            yield return new WaitForSeconds(1f);
            isProcessingSelection = false;
            Debug.Log("🔓 Selection processing unlocked");
        }



        private void ShowRestartButton()
        {
            Debug.Log("🔴 Showing restart button...");

            if (restartButtonSetup == null)
            {
                restartButtonSetup = FindObjectOfType<RestartButtonSetup>();
            }

            if (restartButtonSetup != null)
            {
                restartButtonSetup.gameObject.SetActive(true);
                restartButtonSetup.ClearRestartButton();
                restartButtonSetup.CreateRestartButton();
                Debug.Log("✅ Restart button shown and created");
            }
            else
            {
                Debug.LogError("❌ RED X RED - RestartButtonSetup not found!");
            }
        }

        private void HideRestartButton()
        {
            Debug.Log("🙈 Hiding restart button...");

            if (restartButtonSetup != null)
            {
                restartButtonSetup.gameObject.SetActive(false);
                Debug.Log("✅ Restart button hidden");
            }
        }

                                public void ReturnToMainMenu()
        {
            Debug.Log("🔙 Returning to main menu...");

            if (furnitureSpawner != null)
            {
                Debug.Log("🧹 Using FurnitureSpawner to clear all furniture");
                furnitureSpawner.ClearAllFurniture();
            }

            HideRestartButton();
            ShowMenu();

            isProcessingSelection = false;

            Debug.Log("✅ Returned to main menu state");
        }

        private void HideMenu()
        {
            Debug.Log("🙈 Hiding menu...");

            if (menuCanvas == null)
            {
                menuCanvas = GameObject.Find("MenuCanvas");
            }

            if (menuCanvas != null)
            {
                menuCanvas.SetActive(false);
                Debug.Log("✅ Menu hidden");
            }
            else
            {
                Debug.LogWarning("⚠️ MenuCanvas not found!");
            }
        }

        private void ShowMenu()
        {
            Debug.Log("👁️ Showing menu...");

            if (menuCanvas == null)
            {
                menuCanvas = GameObject.Find("MenuCanvas");
            }

            if (menuCanvas != null)
            {
                menuCanvas.SetActive(true);
                Debug.Log("✅ Menu shown");
            }
            else
            {
                Debug.LogWarning("⚠️ MenuCanvas not found!");
            }
        }

        private void ExitApplication()
        {
            Debug.Log("👋 Exiting application...");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

                [ContextMenu("🔍 Debug Furniture State")]
        public void DebugFurnitureState()
        {
            Debug.Log("🔍 Debugging furniture state...");

            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            int furniture1Count = 0, furniture2Count = 0, furniture3Count = 0;
            int furniture1Active = 0, furniture2Active = 0, furniture3Active = 0;

            foreach (GameObject obj in allObjects)
            {
                if (obj.scene.IsValid() && obj.name.StartsWith("furniture_"))
                {
                    FurnitureVisibilityController controller = obj.GetComponent<FurnitureVisibilityController>();
                    string controllerInfo = controller != null ? $" (Controller: {controller.furnitureSetNumber})" : " (No Controller)";

                    if (obj.name.StartsWith("furniture_1_"))
                    {
                        furniture1Count++;
                        if (obj.activeSelf) furniture1Active++;
                        Debug.Log($"   furniture_1: {obj.name} - Active: {obj.activeSelf}{controllerInfo}");
                    }
                    else if (obj.name.StartsWith("furniture_2_"))
                    {
                        furniture2Count++;
                        if (obj.activeSelf) furniture2Active++;
                        Debug.Log($"   furniture_2: {obj.name} - Active: {obj.activeSelf}{controllerInfo}");
                    }
                    else if (obj.name.StartsWith("furniture_3_"))
                    {
                        furniture3Count++;
                        if (obj.activeSelf) furniture3Active++;
                        Debug.Log($"   furniture_3: {obj.name} - Active: {obj.activeSelf}{controllerInfo}");
                    }
                }
            }

            Debug.Log($"🔍 Found: {furniture1Count} furniture_1 ({furniture1Active} active), {furniture2Count} furniture_2 ({furniture2Active} active), {furniture3Count} furniture_3 ({furniture3Active} active)");
        }

        [ContextMenu("🪑 Force Show Furniture 1")]
        public void ForceShowFurniture1()
        {
            Debug.Log("🚀 FORCE SHOWING FURNITURE 1!");
            SelectFurnitureSet("furniture_1");
        }

        [ContextMenu("🪑 Force Show Furniture 2")]
        public void ForceShowFurniture2()
        {
            Debug.Log("🚀 FORCE SHOWING FURNITURE 2!");
            SelectFurnitureSet("furniture_2");
        }

        [ContextMenu("🪑 Force Show Furniture 3")]
        public void ForceShowFurniture3()
        {
            Debug.Log("🚀 FORCE SHOWING FURNITURE 3!");
            SelectFurnitureSet("furniture_3");
        }

        [ContextMenu("🔄 Retry Button Finding")]
        public void RetryButtonFinding()
        {
            Debug.Log("🔄 MANUALLY RETRYING BUTTON FINDING...");
            FindAllButtons();
            ConnectDirectButtons();
            Debug.Log("✅ Manual button finding completed");
        }

                        public void ForceInitialize()
        {
            Debug.Log("🚀 FORCE INITIALIZING SimpleFurnitureSelector...");

            if (menuCanvas == null)
            {
                menuCanvas = GameObject.Find("MenuCanvas");
                Debug.Log($"🎮 MenuCanvas found: {(menuCanvas != null ? "✅" : "❌")}");
            }

            if (furnitureSpawner == null)
            {
                furnitureSpawner = FindObjectOfType<FurnitureSpawner>();
                Debug.Log($"🏭 FurnitureSpawner found: {(furnitureSpawner != null ? "✅" : "❌")}");
            }

            FindAllButtons();
            ConnectDirectButtons();

            if (furnitureSpawner != null)
            {
                furnitureSpawner.ClearAllFurniture();
            }

            HideRestartButton();

            isProcessingSelection = false;

            Debug.Log("✅ Force initialization completed");
        }
    }
}