using WrysersGoPro.ConfigManager;
using UnityEngine;

namespace WrysersGoPro.Core
{
    public class GoProManager : MonoBehaviour
	{
		public static bool FOVislock = false;
		public static bool Controlsarelocked = false;
		public static bool faceCam = false;
		public static bool topDown = false;
		public static bool thirdPerson = false;
		//For people looking through my code this script was meant to be for using a void that Toggles FOVLock but I couldn't get it to work
	}
}