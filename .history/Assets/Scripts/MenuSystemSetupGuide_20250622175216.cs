using UnityEngine;

namespace VRProject
{
    public class MenuSystemSetupGuide : MonoBehaviour
    {
        [Header("Setup Instructions")]
        [TextArea(15, 30)]
        public string setupInstructions = @"
MUEBLEBUILDER MENU SYSTEM SETUP GUIDE
=====================================

STEP 1: CREATE CANVAS
---------------------
1. Create a Canvas (UI → Canvas)
2. Set Canvas Render Mode to 'World Space'
3. Position canvas in front of player spawn
4. Scale down to reasonable size (e.g., 0.01, 0.01, 0.01)

STEP 2: CREATE UI PANELS
------------------------
Under Canvas, create these panels:

A) MainMenuPanel:
   - Add UIAnimator component
   - Add CanvasGroup component
   - Add child objects:
     * Title Text: 'MuebleBuilder'
     * IniciarButton (with ModernButtonEffect)
     * SalirButton (with ModernButtonEffect)

B) ModelSelectionPanel:
   - Add UIAnimator component
   - Add CanvasGroup component
   - Set initially inactive
   - Add child objects:
     * Title Text: 'Selecciona tu Mueble'
     * BackButton (with ModernButtonEffect)
     * ScrollView with Grid Layout Group

C) GameplayPanel:
   - Add UIAnimator component
   - Add CanvasGroup component
   - Set initially inactive
   - This can be empty or contain game UI

STEP 3: CREATE MENU MANAGER
---------------------------
1. Create empty GameObject 'MenuManager'
2. Add AnimatedMenuManager component
3. Assign all UI references in inspector
4. Add MenuSetupHelper component
5. Add FurniturePrefabGenerator component

STEP 4: CREATE RESTART BUTTON SETUP
-----------------------------------
1. Create empty GameObject 'RestartButtonParent'
2. Position where you want button to appear in gameplay
3. Add RestartButtonSetup component
4. Set initially inactive
5. Link this to MenuManager's restartButtonSetup field

STEP 5: CONFIGURE COMPONENTS
---------------------------
In MenuSetupHelper:
- Assign AnimatedMenuManager reference
- Assign FurniturePrefabGenerator reference
- Right-click → 'Create Sample Materials'
- Right-click → 'Setup Menu with Generated Furniture'

STEP 6: TEST THE FLOW
--------------------
Play mode should show this flow:
1. Main Menu appears
2. Press 'Iniciar' → Model selection grid
3. Select model → Menu disappears, model spawns, restart button appears
4. Press restart button → Return to main menu, model clears

TROUBLESHOOTING
--------------
- Ensure all UI elements have correct parent/child relationships
- Check Canvas render mode is World Space
- Verify all component references are assigned
- Make sure RestartButtonSetup is initially inactive
";

        [ContextMenu("Print Setup Instructions")]
        public void PrintInstructions()
        {
            Debug.Log(setupInstructions);
        }

        [Header("Quick Setup")]
        [SerializeField] private bool autoSetupOnStart = false;

        private void Start()
        {
            if (autoSetupOnStart)
            {
                Debug.Log("=== MUEBLEBUILDER SETUP GUIDE ===");
                Debug.Log(setupInstructions);
            }
        }

        [ContextMenu("Validate Current Setup")]
        public void ValidateSetup()
        {
            Debug.Log("=== SETUP VALIDATION ===");

            // Check for required components
            AnimatedMenuManager menuManager = FindObjectOfType<AnimatedMenuManager>();
            if (menuManager == null)
            {
                Debug.LogError("❌ AnimatedMenuManager not found!");
            }
            else
            {
                Debug.Log("✅ AnimatedMenuManager found");

                if (menuManager.mainMenuAnimator == null)
                    Debug.LogWarning("⚠️ Main Menu Animator not assigned");
                if (menuManager.modelSelectionAnimator == null)
                    Debug.LogWarning("⚠️ Model Selection Animator not assigned");
                if (menuManager.gameplayAnimator == null)
                    Debug.LogWarning("⚠️ Gameplay Animator not assigned");
                if (menuManager.restartButtonSetup == null)
                    Debug.LogWarning("⚠️ Restart Button Setup not assigned");
            }

            MenuSetupHelper setupHelper = FindObjectOfType<MenuSetupHelper>();
            if (setupHelper == null)
            {
                Debug.LogWarning("⚠️ MenuSetupHelper not found - furniture generation may not work");
            }
            else
            {
                Debug.Log("✅ MenuSetupHelper found");
            }

            FurniturePrefabGenerator generator = FindObjectOfType<FurniturePrefabGenerator>();
            if (generator == null)
            {
                Debug.LogWarning("⚠️ FurniturePrefabGenerator not found - no furniture will be available");
            }
            else
            {
                Debug.Log("✅ FurniturePrefabGenerator found");
                if (generator.baseCubePrefab == null)
                    Debug.LogWarning("⚠️ Base Cube Prefab not assigned in FurniturePrefabGenerator");
            }

            Canvas[] canvases = FindObjectsOfType<Canvas>();
            bool foundWorldSpaceCanvas = false;
            foreach (Canvas canvas in canvases)
            {
                if (canvas.renderMode == RenderMode.WorldSpace)
                {
                    foundWorldSpaceCanvas = true;
                    break;
                }
            }

            if (!foundWorldSpaceCanvas)
            {
                Debug.LogWarning("⚠️ No World Space Canvas found - UI may not display correctly in VR");
            }
            else
            {
                Debug.Log("✅ World Space Canvas found");
            }

            Debug.Log("=== VALIDATION COMPLETE ===");
        }
    }
}