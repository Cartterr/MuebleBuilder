using UnityEngine;
using UnityEngine.UI;

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

        private void Start()
        {
            Debug.Log("🪑🪑🪑 SIMPLE FURNITURE SELECTOR STARTING 🪑🪑🪑");

            FindAllButtons();
            ConnectDirectButtons();
            HideAllFurniture();
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
                    mueble1Button = mueble1Obj.GetComponent<Button>();
            }

            if (mueble2Button == null)
            {
                GameObject mueble2Obj = GameObject.Find("Mueble2Button");
                if (mueble2Obj != null)
                    mueble2Button = mueble2Obj.GetComponent<Button>();
            }

            if (mueble3Button == null)
            {
                GameObject mueble3Obj = GameObject.Find("Mueble3Button");
                if (mueble3Obj != null)
                    mueble3Button = mueble3Obj.GetComponent<Button>();
            }

            if (salirButton == null)
            {
                GameObject salirObj = GameObject.Find("SalirButton");
                if (salirObj != null)
                    salirButton = salirObj.GetComponent<Button>();
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
                    Debug.Log("🪑🪑🪑 MUEBLE 1 SELECTED! 🪑🪑🪑");
                    SelectFurnitureSet("furniture_1");
                });
                Debug.Log("✅ Mueble1Button connected");
            }

            if (mueble2Button != null)
            {
                mueble2Button.onClick.RemoveAllListeners();
                mueble2Button.onClick.AddListener(() => {
                    Debug.Log("🪑🪑🪑 MUEBLE 2 SELECTED! 🪑🪑🪑");
                    SelectFurnitureSet("furniture_2");
                });
                Debug.Log("✅ Mueble2Button connected");
            }

            if (mueble3Button != null)
            {
                mueble3Button.onClick.RemoveAllListeners();
                mueble3Button.onClick.AddListener(() => {
                    Debug.Log("🪑🪑🪑 MUEBLE 3 SELECTED! 🪑🪑🪑");
                    SelectFurnitureSet("furniture_3");
                });
                Debug.Log("✅ Mueble3Button connected");
            }

            if (salirButton != null)
            {
                salirButton.onClick.RemoveAllListeners();
                salirButton.onClick.AddListener(() => {
                    Debug.Log("👋 SALIR PRESSED - EXITING!");
                    ExitApplication();
                });
                Debug.Log("✅ SalirButton connected");
            }
        }

        private void SelectFurnitureSet(string setPrefix)
        {
            Debug.Log($"🪑 Selecting furniture set: {setPrefix}");

            HideAllFurniture();
            ShowFurnitureSet(setPrefix);
            ShowRestartButton();

            Debug.Log($"✅ {setPrefix} furniture set activated!");
        }

        private void HideAllFurniture()
        {
            Debug.Log("🙈 Hiding all furniture...");

            FurnitureVisibilityController[] controllers = FindObjectsOfType<FurnitureVisibilityController>();
            foreach (FurnitureVisibilityController controller in controllers)
            {
                controller.HideFurniture();
            }

            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.name.StartsWith("furniture_"))
                {
                    FurnitureVisibilityController controller = obj.GetComponent<FurnitureVisibilityController>();
                    if (controller == null)
                    {
                        obj.SetActive(false);
                    }
                }
            }

            Debug.Log($"🙈 All furniture hidden");
        }

        private void ShowFurnitureSet(string setPrefix)
        {
            Debug.Log($"👁️ Showing furniture set: {setPrefix}");

            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            int shownCount = 0;

            foreach (GameObject obj in allObjects)
            {
                if (obj.name.StartsWith(setPrefix + "_"))
                {
                    FurnitureVisibilityController controller = obj.GetComponent<FurnitureVisibilityController>();
                    if (controller != null)
                    {
                        controller.ShowFurniture();
                    }
                    else
                    {
                        obj.SetActive(true);
                    }
                    shownCount++;
                    Debug.Log($"   👁️ Showed: {obj.name}");
                }
            }

            Debug.Log($"👁️ Showed {shownCount} pieces from {setPrefix}");
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

            HideAllFurniture();
            HideRestartButton();

            Debug.Log("✅ Returned to main menu state");
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

            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            int furniture1Count = 0, furniture2Count = 0, furniture3Count = 0;

            foreach (GameObject obj in allObjects)
            {
                if (obj.name.StartsWith("furniture_1_"))
                {
                    furniture1Count++;
                    Debug.Log($"   furniture_1: {obj.name} - Active: {obj.activeSelf}");
                }
                else if (obj.name.StartsWith("furniture_2_"))
                {
                    furniture2Count++;
                    Debug.Log($"   furniture_2: {obj.name} - Active: {obj.activeSelf}");
                }
                else if (obj.name.StartsWith("furniture_3_"))
                {
                    furniture3Count++;
                    Debug.Log($"   furniture_3: {obj.name} - Active: {obj.activeSelf}");
                }
            }

            Debug.Log($"🔍 Found: {furniture1Count} furniture_1, {furniture2Count} furniture_2, {furniture3Count} furniture_3");
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
    }
}