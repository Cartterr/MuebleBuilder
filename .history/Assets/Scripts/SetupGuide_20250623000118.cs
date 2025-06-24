using UnityEngine;

namespace VRProject
{
    public class SetupGuide : MonoBehaviour
    {
        [ContextMenu("📋 SHOW SETUP INSTRUCTIONS")]
        public void ShowSetupInstructions()
        {
            Debug.Log("" +
                "🚀 MUEBLE BUILDER SETUP GUIDE\n" +
                "================================\n\n" +

                "⚠️ SAFETY FIRST:\n" +
                "- The FurnitureSpawner ONLY works in PLAY MODE\n" +
                "- It will NEVER destroy objects in Editor mode\n" +
                "- All furniture destruction happens at runtime only\n\n" +

                "📋 STEP-BY-STEP SETUP:\n\n" +

                "1️⃣ CREATE THE MENU SYSTEM:\n" +
                "   - Right-click AutoMenuSetup script in Inspector\n" +
                "   - Click '🚀 CREATE COMPLETE MENU SYSTEM'\n" +
                "   - This creates everything automatically\n\n" +

                "2️⃣ VERIFY FURNITURE NAMING:\n" +
                "   - Your furniture must be named: furniture_1_1, furniture_1_2, etc.\n" +
                "   - Set 1: furniture_1_* objects\n" +
                "   - Set 2: furniture_2_* objects  \n" +
                "   - Set 3: furniture_3_* objects\n\n" +

                "3️⃣ COMPONENTS CREATED AUTOMATICALLY:\n" +
                "   - MenuManager (with FurnitureSpawner + SimpleFurnitureSelector)\n" +
                "   - MenuCanvas (with all UI buttons)\n" +
                "   - RestartButtonParent (with restart functionality)\n" +
                "   - RecenterOriginPoint (for VR recentering)\n\n" +

                "4️⃣ NO MANUAL ASSIGNMENT NEEDED:\n" +
                "   - All components auto-find each other\n" +
                "   - All buttons auto-connect their events\n" +
                "   - All furniture auto-detected by name pattern\n\n" +

                "5️⃣ HOW IT WORKS:\n" +
                "   - At START: FurnitureSpawner stores all furniture data\n" +
                "   - Then DESTROYS all furniture (clean slate)\n" +
                "   - When you select MUEBLE 1/2/3: Spawns that set fresh\n" +
                "   - Return to menu: Destroys everything again\n\n" +

                "🔧 TROUBLESHOOTING:\n" +
                "   - If buttons don't work: Use 'Force Initialize' on SimpleFurnitureSelector\n" +
                "   - If furniture missing: Check naming pattern (furniture_X_Y)\n" +
                "   - If spawning fails: Check FurnitureSpawner debug methods\n\n" +

                "⚡ TESTING:\n" +
                "   - ONLY test in PLAY MODE\n" +
                "   - Press Play, use menu buttons\n" +
                "   - Furniture will be destroyed/recreated as needed\n\n" +

                "🎯 THAT'S IT! No manual setup required!"
            );
        }

        [ContextMenu("🔍 CHECK CURRENT SETUP")]
        public void CheckCurrentSetup()
        {
            Debug.Log("🔍 CHECKING CURRENT SETUP...\n");

            // Check for MenuManager
            GameObject menuManager = GameObject.Find("MenuManager");
            if (menuManager != null)
            {
                FurnitureSpawner spawner = menuManager.GetComponent<FurnitureSpawner>();
                SimpleFurnitureSelector selector = menuManager.GetComponent<SimpleFurnitureSelector>();

                Debug.Log($"✅ MenuManager found: {menuManager.name}");
                Debug.Log($"   - FurnitureSpawner: {(spawner != null ? "✅" : "❌")}");
                Debug.Log($"   - SimpleFurnitureSelector: {(selector != null ? "✅" : "❌")}");

                if (spawner != null)
                {
                    Debug.Log($"   - Stored furniture data: {spawner.originalFurnitureData.Count} pieces");
                }
            }
            else
            {
                Debug.LogError("❌ MenuManager NOT FOUND! Run 'CREATE COMPLETE MENU SYSTEM' first!");
            }

            // Check for Canvas
            GameObject canvas = GameObject.Find("MenuCanvas");
            Debug.Log($"📱 MenuCanvas: {(canvas != null ? "✅" : "❌")}");

            // Check for RestartButton
            GameObject restartParent = GameObject.Find("RestartButtonParent");
            Debug.Log($"🔴 RestartButtonParent: {(restartParent != null ? "✅" : "❌")}");

            // Check for Origin Point
            GameObject originPoint = GameObject.Find("RecenterOriginPoint");
            Debug.Log($"🎯 RecenterOriginPoint: {(originPoint != null ? "✅" : "❌")}");

            // Count furniture
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            int furniture1 = 0, furniture2 = 0, furniture3 = 0;

            foreach (GameObject obj in allObjects)
            {
                if (obj.name.StartsWith("furniture_1_")) furniture1++;
                else if (obj.name.StartsWith("furniture_2_")) furniture2++;
                else if (obj.name.StartsWith("furniture_3_")) furniture3++;
            }

            Debug.Log($"🪑 FURNITURE COUNT:");
            Debug.Log($"   - Set 1 (furniture_1_*): {furniture1} pieces");
            Debug.Log($"   - Set 2 (furniture_2_*): {furniture2} pieces");
            Debug.Log($"   - Set 3 (furniture_3_*): {furniture3} pieces");

            if (furniture1 == 0 && furniture2 == 0 && furniture3 == 0)
            {
                Debug.LogError("❌ NO FURNITURE FOUND! Make sure your furniture is named: furniture_1_1, furniture_2_1, etc.");
            }
        }

        [ContextMenu("🚨 EMERGENCY: Show All Furniture")]
        public void EmergencyShowAllFurniture()
        {
            Debug.Log("🚨 EMERGENCY: Showing all furniture...");

            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            int shownCount = 0;

            foreach (GameObject obj in allObjects)
            {
                if (obj.name.StartsWith("furniture_"))
                {
                    obj.SetActive(true);
                    shownCount++;
                    Debug.Log($"   👁️ Shown: {obj.name}");
                }
            }

            Debug.Log($"🚨 Emergency showed {shownCount} furniture pieces");
        }
    }
}