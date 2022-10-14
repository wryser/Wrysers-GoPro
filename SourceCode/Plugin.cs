using BepInEx;
using Bepinject;
using System;
using UnityEngine;
using Utilla;
using UnityEngine.XR;
using System.IO;
using System.Reflection;
using WrysersGoPro.ConfigManager;
using WrysersGoPro.Core;

namespace WrysersGoPro
{
    /// <summary>
    /// This is your mod's main class.
    /// </summary>

    /* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.6.4")]
    [BepInDependency("dev.auros.bepinex.bepinject")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        bool inRoom;
        GameObject Rhand;
        GameObject Lhand;
        GameObject cam;
        Camera shoulderCamera;
        Camera fakeCamera;
        Cinemachine.CinemachineBrain cambrain;
        float nextBaka;
        float bakacooldown = 1f;
        bool canBaka;
        bool prim;
        bool sec;
        bool leftprim;
        bool stickclick;
        bool isrhand = true;
        float fov = 60;
        GameObject gopro;
        GameObject realgopro;
        private readonly XRNode rNode = XRNode.RightHand;
        private readonly XRNode lNode = XRNode.LeftHand;
        void Awake()
        {
            Zenjector.Install<MainInstaller>().OnProject();
        }

        void OnEnable()
        {
            /* Set up your mod here */
            /* Code here runs at the start and whenever your mod is enabled*/

            HarmonyPatches.ApplyHarmonyPatches();
            Utilla.Events.GameInitialized += OnGameInitialized;
        }

        void OnDisable()
        {
            /* Undo mod setup here */
            /* This provides support for toggling mods with ComputerInterface, please implement it :) */
            /* Code here runs whenever your mod is disabled (including if it disabled on startup)*/

            HarmonyPatches.RemoveHarmonyPatches();
            Utilla.Events.GameInitialized -= OnGameInitialized;
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            /* Code here runs after the game initializes (i.e. GorillaLocomotion.Player.Instance != null) */
            MakeCamera();
        }

        void FixedUpdate()
        {
            /* Code here runs every frame when the mod is enabled */
            if (!GoProConfig.ControlLock)
            {
                if (!GoProConfig.FOVLock)
                {
                    shoulderCamera.fieldOfView = fov = Mathf.Clamp(fov, 5, 160);
                    fakeCamera.fieldOfView = fov = Mathf.Clamp(fov, 5, 160);
                }

                if (Time.time > nextBaka)
                {
                    canBaka = true;
                    nextBaka = Time.time + bakacooldown;
                }
                InputDevices.GetDeviceAtXRNode(rNode).TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out prim);
                InputDevices.GetDeviceAtXRNode(rNode).TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondaryButton, out sec);
                InputDevices.GetDeviceAtXRNode(lNode).TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out leftprim);
                InputDevices.GetDeviceAtXRNode(lNode).TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxisClick, out stickclick);

                if (prim)
                {
                    if (canBaka && isrhand)
                    {
                        cam.transform.SetParent(Lhand.transform);
                        cam.transform.localPosition = new Vector3(0.0f, 0.0f, 0.1f);
                        cam.transform.localRotation = Quaternion.Euler(0f, 0f, 270f);
                        realgopro.SetActive(true);
                        canBaka = false;
                        isrhand = false;
                    }
                    else if (canBaka && !isrhand)
                    {
                        cam.transform.SetParent(Rhand.transform);
                        cam.transform.localPosition = new Vector3(-0.1f, 0.0f, 0.0f);
                        cam.transform.localRotation = Quaternion.Euler(0f, 270f, 90f);
                        realgopro.SetActive(true);
                        canBaka = false;
                        isrhand = true;
                    }
                }
                if (!GoProConfig.FOVLock)
                {
                    if (stickclick)
                    {
                        fov = 60;
                    }
                }
                if (sec)
                {
                    cam.transform.SetParent(null);
                    isrhand = false;
                    realgopro.SetActive(true);
                }
                if (leftprim)
                {
                    cam.transform.SetParent(Camera.main.transform);
                    cam.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                    cam.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    realgopro.SetActive(false);
                    isrhand = false;
                }

                Vector2 lstick;

                InputDevices.GetDeviceAtXRNode(lNode).TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out lstick);
                if (!GoProConfig.FOVLock)
                {
                    if (lstick.y > 0)
                    {
                        fov += -lstick.y;
                    }
                    if (lstick.y < 0)
                    {
                        fov += Mathf.Abs(lstick.y);
                    }
                }
            }
        }

        /* This attribute tells Utilla to call this method when a modded room is joined */
        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            /* Activate your mod here */
            /* This code will run regardless of if the mod is enabled*/

            inRoom = true;
        }

        /* This attribute tells Utilla to call this method when a modded room is left */
        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            /* Deactivate your mod here */
            /* This code will run regardless of if the mod is enabled*/

            inRoom = false;
        }

        public void MakeCamera()
        {
            GoProManager.Controlsarelocked = GoProConfig.ControlLock;
            GoProManager.FOVislock = GoProConfig.FOVLock;
            Stream str = Assembly.GetExecutingAssembly().GetManifestResourceStream("WrysersGoPro.Assets.gopro");
            AssetBundle bundle = AssetBundle.LoadFromStream(str);
            gopro = bundle.LoadAsset<GameObject>("Camera");
            realgopro = Instantiate(gopro);
            Debug.Log("GOPRO INSTIATED");
            Rhand = GameObject.Find("Global/Local VRRig/Local Gorilla Player/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/");
            Lhand = GameObject.Find("Global/Local VRRig/Local Gorilla Player/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/");
            cam = GameObject.Find("Global/Third Person Camera/Shoulder Camera");
            cam.transform.SetParent(Rhand.transform);
            realgopro.transform.SetParent(cam.transform);
            GameObject.Find("CM vcam1").SetActive(false);
            shoulderCamera = cam.GetComponent<Camera>();
            cambrain = cam.GetComponent<Cinemachine.CinemachineBrain>();
            cambrain.enabled = false;
            cam.transform.localPosition = new Vector3(-0.1f, 0.0f, 0.0f);
            cam.transform.localRotation = Quaternion.Euler(0f, 270f, 90f);
            realgopro.transform.localPosition = new Vector3(0f, 0f, 0f);
            realgopro.transform.localRotation = Quaternion.Euler(0, 90, 0);
            fakeCamera = realgopro.transform.GetChild(1).GetComponent<Camera>();
        }
    }
}
