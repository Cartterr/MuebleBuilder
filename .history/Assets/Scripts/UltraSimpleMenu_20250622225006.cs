using UnityEngine;
using UnityEngine.UI;

namespace VRProject
{
    public class UltraSimpleMenu : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log("🚀 ULTRA SIMPLE MENU STARTING");
            SetupMenus();
        }

        private void SetupMenus()
        {
            Debug.Log("🔧 Setting up menus...");

            GameObject mainMenu = GameObject.Find("MainMenuPanel");
            GameObject modelMenu = GameObject.Find("ModelSelectionPanel");

            if (mainMenu != null && modelMenu != null)
            {
                Debug.Log("✅ Found both menu panels");

                mainMenu.SetActive(true);
                modelMenu.SetActive(false);

                GameObject iniciarBtn = GameObject.Find("IniciarButton");
                if (iniciarBtn != null)
                {
                    Button btn = iniciarBtn.GetComponent<Button>();
                    if (btn != null)
                    {
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(() => {
                            Debug.Log("🚀🚀🚀 INICIAR PRESSED - SWITCHING MENUS! 🚀🚀🚀");

                            mainMenu.SetActive(false);

                            EditorVisibilityHelper helper = modelMenu.GetComponent<EditorVisibilityHelper>();
                            if (helper != null)
                            {
                                helper.enabled = false;
                                Debug.Log("🔧 Disabled EditorVisibilityHelper");
                            }

                            modelMenu.SetActive(true);

                            CanvasGroup cg = modelMenu.GetComponent<CanvasGroup>();
                            if (cg != null)
                            {
                                cg.alpha = 1f;
                                cg.interactable = true;
                                cg.blocksRaycasts = true;
                                Debug.Log("🔧 Set CanvasGroup to visible");
                            }

                            modelMenu.transform.localScale = Vector3.one;

                            Debug.Log("✅ MODEL SELECTION MENU SHOULD NOW BE VISIBLE!");
                        });
                        Debug.Log("✅ INICIAR button connected!");
                    }
                    else
                    {
                        Debug.LogError("❌ IniciarButton has no Button component!");
                    }
                }
                else
                {
                    Debug.LogError("❌ IniciarButton not found!");
                }

                GameObject backBtn = GameObject.Find("BackButton");
                if (backBtn != null)
                {
                    Button btn = backBtn.GetComponent<Button>();
                    if (btn != null)
                    {
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(() => {
                            Debug.Log("🔙 BACK PRESSED - RETURNING TO MAIN MENU!");
                            modelMenu.SetActive(false);
                            mainMenu.SetActive(true);
                        });
                        Debug.Log("✅ BACK button connected!");
                    }
                }

                GameObject salirBtn = GameObject.Find("SalirButton");
                if (salirBtn != null)
                {
                    Button btn = salirBtn.GetComponent<Button>();
                    if (btn != null)
                    {
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(() => {
                            Debug.Log("👋 SALIR PRESSED - EXITING!");
#if UNITY_EDITOR
                            UnityEditor.EditorApplication.isPlaying = false;
#else
                            Application.Quit();
#endif
                        });
                        Debug.Log("✅ SALIR button connected!");
                    }
                }
            }
            else
            {
                Debug.LogError("❌ Could not find menu panels!");
                Debug.LogError($"   MainMenuPanel: {mainMenu != null}");
                Debug.LogError($"   ModelSelectionPanel: {modelMenu != null}");
            }
        }

        [ContextMenu("🔍 Debug Menu State")]
        public void DebugMenuState()
        {
            GameObject mainMenu = GameObject.Find("MainMenuPanel");
            GameObject modelMenu = GameObject.Find("ModelSelectionPanel");

            Debug.Log("🔍 CURRENT MENU STATE:");
            Debug.Log($"   MainMenuPanel: {(mainMenu != null ? $"Found, Active={mainMenu.activeSelf}" : "NOT FOUND")}");
            Debug.Log($"   ModelSelectionPanel: {(modelMenu != null ? $"Found, Active={modelMenu.activeSelf}" : "NOT FOUND")}");

            if (modelMenu != null)
            {
                CanvasGroup cg = modelMenu.GetComponent<CanvasGroup>();
                if (cg != null)
                    Debug.Log($"   ModelSelection CanvasGroup: Alpha={cg.alpha}, Interactable={cg.interactable}");

                EditorVisibilityHelper helper = modelMenu.GetComponent<EditorVisibilityHelper>();
                if (helper != null)
                    Debug.Log($"   ModelSelection EditorHelper: Enabled={helper.enabled}, HideAtRuntime={helper.hideAtRuntime}");
            }
        }

        [ContextMenu("🚀 Force Show Model Selection")]
        public void ForceShowModelSelection()
        {
            GameObject mainMenu = GameObject.Find("MainMenuPanel");
            GameObject modelMenu = GameObject.Find("ModelSelectionPanel");

            if (mainMenu != null) mainMenu.SetActive(false);

            if (modelMenu != null)
            {
                EditorVisibilityHelper helper = modelMenu.GetComponent<EditorVisibilityHelper>();
                if (helper != null) helper.enabled = false;

                modelMenu.SetActive(true);

                CanvasGroup cg = modelMenu.GetComponent<CanvasGroup>();
                if (cg != null)
                {
                    cg.alpha = 1f;
                    cg.interactable = true;
                    cg.blocksRaycasts = true;
                }

                modelMenu.transform.localScale = Vector3.one;

                Debug.Log("🚀 FORCED MODEL SELECTION TO SHOW!");
            }
        }
    }
}