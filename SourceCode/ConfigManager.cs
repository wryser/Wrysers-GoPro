using System.IO;

using BepInEx;
using BepInEx.Configuration;

namespace WrysersGoPro.ConfigManager
{
    static internal class GoProConfig
    {
        public static bool FOVLock { get => FOVLockcfg.Value; set => FOVLockcfg.Value = value;}
        public static bool ControlLock { get => ControlLockcfg.Value; set => ControlLockcfg.Value = value; }
        public static bool StickToPlayer { get => StickToPlayercfg.Value; set => StickToPlayercfg.Value = value; }
        public static bool SmoothFP { get => SmoothFPcfg.Value; set => SmoothFPcfg.Value = value; }
        public static bool Grabbable { get => grabbablecfg.Value; set => grabbablecfg.Value = value; }
        public static int camMode { get => camModecfg.Value; set => camModecfg.Value = value; }
        public static float fov { get => FOVcfg.Value; set => FOVcfg.Value = value; }

        static ConfigEntry<bool> FOVLockcfg;
        static ConfigEntry<bool> ControlLockcfg;
        static ConfigEntry<bool> StickToPlayercfg;
        static ConfigEntry<bool> SmoothFPcfg;
        static ConfigEntry<bool> grabbablecfg;
        static ConfigEntry<int> camModecfg;
        static ConfigEntry<float> FOVcfg;

        static ConfigFile config;

        static GoProConfig()
        {
            config = new ConfigFile(Path.Combine(Paths.ConfigPath, "WrysersGoPro.cfg"), true);
            FOVLockcfg = config.Bind("General", "FOVLock?", false, "Change this to toggle if the FOV should be locked");
            ControlLockcfg = config.Bind("General", "ControlLock?", false, "Change this to toggle if the Controls should be locked");
            StickToPlayercfg = config.Bind("General", "Stick To Player", false, "Change this to toggle if the camera will stick to the player");
            SmoothFPcfg = config.Bind("General", "Smooth First Person?", false, "Change this to toggle if first person mode is smoothed");
            grabbablecfg = config.Bind("General", "Grabbable?", false, "Change this to toggle if the camera is grabbable");
            camModecfg = config.Bind("General", "Camera Mode", 0, "This is what the mod uses to save the position of your camera when you start the game.");
            FOVcfg = config.Bind("General", "FOV", 60f, "This is your FOV.");
        }
    }
}
