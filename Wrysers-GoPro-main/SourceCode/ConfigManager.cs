using System.IO;

using BepInEx;
using BepInEx.Configuration;

namespace WrysersGoPro.ConfigManager
{
    static internal class GoProConfig
    {
        public static bool FOVLock { get => FOVLockcfg.Value; set => FOVLockcfg.Value = value;}
        public static bool ControlLock { get => ControlLockcfg.Value; set => ControlLockcfg.Value = value; }

        static ConfigEntry<bool> FOVLockcfg;
        static ConfigEntry<bool> ControlLockcfg;

        static ConfigFile config;

        static GoProConfig()
        {
            config = new ConfigFile(Path.Combine(Paths.ConfigPath, "WrysersGoPro.cfg"), true);
            FOVLockcfg = config.Bind("General", "FOVLock?", false, "Change this to toggle if the FOV should be locked");
            ControlLockcfg = config.Bind("General", "ControlLock?", false, "Change this to toggle if the Controls should be locked");
        }
    }
}
