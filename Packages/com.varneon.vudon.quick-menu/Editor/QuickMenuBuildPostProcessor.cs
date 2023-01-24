using JetBrains.Annotations;
using System.Linq;
using UnityEditor.Callbacks;
using UnityEngine;
using Varneon.VUdon.QuickMenu.Abstract;
using Varneon.VUdon.Menus.Abstract;

namespace Varneon.VUdon.QuickMenu.Editor
{
    public static class QuickMenuBuildPostProcessor
    {
        [UsedImplicitly]
        [PostProcessScene(-2)]
        private static void PostProcessQuickMenus()
        {
            QuickMenu[] quickMenus = Resources.FindObjectsOfTypeAll<QuickMenu>().Where(q => q.gameObject.scene.IsValid()).ToArray();
        
            foreach(QuickMenu quickMenu in quickMenus)
            {
                PostProcessQuickMenu(quickMenu);
            }
        }

        private static void PostProcessQuickMenu(QuickMenu quickMenu)
        {
            quickMenu.items = new QuickMenuItem[][] { new QuickMenuItem[0] };
            quickMenu.folderPaths = new string[] { string.Empty };
            quickMenu.folders = new QuickMenuFolderContainer[] { quickMenu.GetComponentInChildren<QuickMenuFolderContainer>(true) };

            //quickMenu.TryRegisterFolder("Quick Actions", "Quick Actions");
            //quickMenu.TryRegisterFolder("Settings", "Audio, Graphics, Gameplay, etc.");
            //quickMenu.TryRegisterFolder("Teleport", "Teleport to different locations");

            //quickMenu.TryRegisterToggle("Quick Actions/Seat Calibration Mode", null, false, tooltip: "Toggle seat calibration mode");
            //quickMenu.TryRegisterToggle("Quick Actions/Noclip", null, false, tooltip: "Toggle noclip");
            //quickMenu.TryRegisterSlider("Settings/Audio/Ambience", null, 80f, 0f, 100f, tooltip: "Ambience volume");
            //quickMenu.TryRegisterSlider("Settings/Audio/Music", null, 80f, 0f, 100f, tooltip: "Music volume");
            //quickMenu.TryRegisterSlider("Settings/Audio/SFX", null, 80f, 0f, 100f, tooltip: "SFX volume");
            //quickMenu.TryRegisterSlider("Settings/Audio/Footsteps", null, 80f, 0f, 100f, tooltip: "Footsteps volume");
            //quickMenu.TryRegisterOption("Settings/Graphics/Post Processing", null, new string[] { "None", "Low", "Medium", "High", "Ultra" }, 3, tooltip: "");
            //quickMenu.TryRegisterSlider("Settings/Graphics/Bloom Intensity", null, 60f, 0f, 100f, tooltip: "FLASHBANG!");
            //quickMenu.TryRegisterSlider("Settings/Graphics/Brightness", null, 80f, 0f, 100f, tooltip: "");
            //quickMenu.TryRegisterOption("Settings/Graphics/Advanced/Realtime Reflections", null, new string[] { "Disabled", "Sliced", "Full" }, 0, tooltip: "");
            //quickMenu.TryRegisterSlider("Settings/Noclip/Speed", null, 15f, 5f, 50f, 9, "m/s", tooltip: "How many meters per second can you fly when noclip is enabled");
            //quickMenu.TryRegisterSlider("Settings/Quick Menu/Vertical Position", null, 0f, -100f, 100f, 10, "mm", tooltip: "Vertical position of this menu");
            //quickMenu.TryRegisterButton("Teleport/Spawn", null, tooltip: "Teleport to Spawn");
            //quickMenu.TryRegisterButton("Teleport/Garage", null, tooltip: "Teleport to Garage");
            //quickMenu.TryRegisterButton("Teleport/Hangar", null, tooltip: "Teleport to Hangar");
            //quickMenu.TryRegisterButton("Teleport/Cabin", null, tooltip: "Teleport to Cabin");
            //quickMenu.TryRegisterButton("Teleport/Gas Station", null, tooltip: "Teleport to Gas Station");
            //quickMenu.TryRegisterButton("Teleport/Drift Track", null, tooltip: "Teleport to Drift Track");
            //quickMenu.TryRegisterButton("Teleport/Race Track", null, tooltip: "Teleport to Race Track");
        }
    }
}
