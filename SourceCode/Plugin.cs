using BepInEx;
using System;
using UnityEngine;
using UnityEngine.XR;
using System.IO;
using System.Reflection;
using HarmonyLib;
using GorillaNetworking;
using WrysersGoPro.ConfigManager;
using WrysersGoPro.Core;
using Valve.VR;
using UnityEngine.EventSystems;
using GorillaLocomotion;
using UnityEngine.UI;
using GorillaMenu;

namespace WrysersGoPro
{
    /// <summary>
    /// This is your mod's main class.
    /// </summary>

    /* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
    [BepInDependency("com.wryser.gorillatag.gorillamenu", "1.0.0")]
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
        bool IsSteamVR;

        public AssetBundle bundle;
        public static Plugin instance;
        GameObject stinkymonkeyface;
        public bool faceCam = false;
        bool hasChangedFOV = false;
        public bool smoothFP = false;
        public bool sticking = false;
        bool FP = false;
        public bool hideCam;
        bool initialized = false;
        bool setCamMode = false;
        float distanceFromPlayer;
        Transform camFollower;
        public bool grabbable = false;
        public bool lHandInTrigger = false;
        public bool rHandInTrigger = false;
        bool inLhand = false;
        bool inRhand = false;
        bool lPressing = false;
        bool rPressing = false;
        bool justPressedFP = false;

        void Awake()
        {
            instance = this;
            HarmonyPatches.ApplyHarmonyPatches();
        }

        public void PlayerSpawned()
        {
            /* Code here runs after the game initializes (i.e. GorillaLocomotion.Player.Instance != null) */
            IsSteamVR = Traverse.Create(PlayFabAuthenticator.instance).Field("platform").GetValue().ToString().ToLower() == "steam";
            MakeCamera();
        }

        void FixedUpdate()
        {
            /* Code here runs every frame when the mod is enabled */
            if (initialized)
            {
                if (!setCamMode)
                {
                    GetConfig();
                }
                if (hideCam)
                {
                    realgopro.SetActive(false);
                }
                shoulderCamera.fieldOfView = fov = Mathf.Clamp(fov, 5, 140);
                fakeCamera.fieldOfView = fov = Mathf.Clamp(fov, 5, 140);
                fov = GoProConfig.fov;
                if (!GoProConfig.ControlLock)
                {
                    GetInputs();

                    if (Time.time > nextBaka)
                    {
                        canBaka = true;
                        nextBaka = Time.time + bakacooldown;
                    }

                    if (GoProManager.followPlayer)
                    {
                        hasChangedFOV = false;
                        cam.transform.SetParent(null, false);
                        cam.transform.LookAt(camFollower.position - camFollower.position + Player.Instance.headCollider.transform.position);
                        distanceFromPlayer = Vector3.Distance(camFollower.position, cam.transform.position);
                        if (distanceFromPlayer > 3.5f)
                        {
                            cam.transform.position = Vector3.Lerp(cam.transform.position, camFollower.position, 2 * Time.deltaTime);
                        }
                        fov = Mathf.Abs(60 - (Vector3.Distance(transform.InverseTransformPoint(camFollower.position), transform.InverseTransformPoint(cam.transform.position)) * 4.5f));
                        FP = false;
                        realgopro.SetActive(true);
                    }

                    if (prim && !grabbable)
                    {
                        if (canBaka && isrhand)
                        {
                            GoProConfig.camMode = 1;
                            cam.transform.SetParent(Lhand.transform);
                            cam.transform.localPosition = new Vector3(-0.1f, 0.05f, 0.0f);
                            cam.transform.localRotation = Quaternion.Euler(0f, 270f, 270f);
                            realgopro.SetActive(true);
                            canBaka = false;
                            isrhand = false;
                            GoProManager.faceCam = false;
                            GoProManager.followPlayer = false;
                            GoProManager.thirdPerson = false;
                            FP = false;
                            hasChangedFOV = false;
                        }
                        else if (canBaka && !isrhand)
                        {
                            GoProConfig.camMode = 0;
                            cam.transform.SetParent(Rhand.transform);
                            cam.transform.localPosition = new Vector3(0.11f, 0.05f, 0.0f);
                            cam.transform.localRotation = Quaternion.Euler(0f, 90f, 90f);
                            realgopro.SetActive(true);
                            canBaka = false;
                            isrhand = true;
                            GoProManager.faceCam = false;
                            GoProManager.followPlayer = false;
                            GoProManager.thirdPerson = false;
                            FP = false;
                            hasChangedFOV = false;
                        }
                    }
                    if (!GoProConfig.FOVLock)
                    {
                        if (stickclick)
                        {
                            fov = 60;
                            GoProConfig.fov = fov;
                        }
                    }
                    if (sec && !grabbable)
                    {
                        if (sticking)
                        {
                            cam.transform.SetParent(Player.Instance.bodyCollider.transform);
                        }
                        else
                        {
                            cam.transform.SetParent(null);
                        }
                        isrhand = false;
                        realgopro.SetActive(true);
                        GoProManager.faceCam = false;
                        GoProManager.followPlayer = false;
                        GoProManager.thirdPerson = false;
                        FP = false;
                        hasChangedFOV = false;
                    }
                    if (leftprim && !grabbable)
                    {
                        FP = true;
                        GoProConfig.camMode = 2;
                        if (!smoothFP)
                        {
                            cam.transform.SetParent(Camera.main.transform);
                            cam.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                            cam.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                            realgopro.SetActive(false);
                            GoProManager.faceCam = false;
                            GoProManager.followPlayer = false;
                            GoProManager.thirdPerson = false;
                            hasChangedFOV = false;
                            isrhand = false;
                        }
                    }
                    else if(grabbable && leftprim && !justPressedFP)
                    {
                        if (!FP)
                        {
                            FP = true;
                            GoProConfig.camMode = 2;
                            if (!smoothFP)
                            {
                                cam.transform.SetParent(Camera.main.transform);
                                cam.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                                cam.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                                realgopro.SetActive(false);
                                GoProManager.faceCam = false;
                                GoProManager.followPlayer = false;
                                GoProManager.thirdPerson = false;
                                hasChangedFOV = false;
                                isrhand = false;
                            }
                            justPressedFP = true;
                        }
                        else
                        {
                            GoProConfig.camMode = 0;
                            cam.transform.SetParent(Rhand.transform);
                            cam.transform.localPosition = new Vector3(0.11f, 0.05f, 0.0f);
                            cam.transform.localRotation = Quaternion.Euler(0f, 90f, 90f);
                            realgopro.SetActive(true);
                            canBaka = false;
                            isrhand = true;
                            inRhand = true;
                            inLhand = false;
                            rPressing = true;
                            GoProManager.faceCam = false;
                            GoProManager.followPlayer = false;
                            GoProManager.thirdPerson = false;
                            FP = false;
                            hasChangedFOV = false;
                            justPressedFP = true;
                        }
                    }
                    else if (!leftprim)
                    {
                        justPressedFP = false;
                    }

                    if (GoProManager.faceCam)
                    {
                        if (!hasChangedFOV)
                        {
                            fov = 60;
                            GoProConfig.fov = fov;
                            hasChangedFOV = true;
                        }
                        cam.transform.position = new Vector3(0, 0, 0);
                        cam.transform.SetParent(stinkymonkeyface.transform, false);
                        cam.transform.localPosition = new Vector3(0.0f, 0.08f, 0.3f);
                        cam.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                        realgopro.SetActive(false);
                        FP = false;
                        isrhand = false;
                    }
                    if (GoProManager.thirdPerson)
                    {
                        if (!hasChangedFOV)
                        {
                            fov = 60;
                            GoProConfig.fov = fov;
                            hasChangedFOV = true;
                        }
                        cam.transform.SetParent(Camera.main.transform, false);
                        cam.transform.localPosition = new Vector3(0.0f, 0.3f, -2.0f);
                        cam.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                        FP = false;
                    }

                    Vector2 lstick;

                    lstick = ControllerInputPoller.instance.rightControllerPrimary2DAxis;
                    if (!GoProConfig.FOVLock)
                    {
                        if (lstick.y > 0)
                        {
                            fov += -lstick.y;
                            GoProConfig.fov = fov;
                        }
                        if (lstick.y < 0)
                        {
                            fov += Mathf.Abs(lstick.y);
                            GoProConfig.fov = fov;
                        }
                    }
                }
                if (smoothFP && FP)
                {
                    cam.transform.SetParent(null);
                    cam.transform.position = Camera.main.transform.position;
                    cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, camFollower.rotation, 0.05f);
                    realgopro.SetActive(false);
                    GoProManager.faceCam = false;
                    GoProManager.followPlayer = false;
                    GoProManager.thirdPerson = false;
                    hasChangedFOV = false;
                    isrhand = false;
                }
                if (grabbable)
                {
                    if (ControllerInputPoller.instance.rightGrab && rHandInTrigger && !inRhand && !rPressing && !FP)
                    {
                        GoProConfig.camMode = 0;
                        cam.transform.SetParent(Rhand.transform);
                        cam.transform.localPosition = new Vector3(0.11f, 0.05f, 0.0f);
                        cam.transform.localRotation = Quaternion.Euler(0f, 90f, 90f);
                        realgopro.SetActive(true);
                        canBaka = false;
                        isrhand = true;
                        inRhand = true;
                        inLhand = false;
                        rPressing = true;
                        GoProManager.faceCam = false;
                        GoProManager.followPlayer = false;
                        GoProManager.thirdPerson = false;
                        FP = false;
                        hasChangedFOV = false;
                    }
                    else if (!ControllerInputPoller.instance.rightGrab)
                    {
                        rPressing = false;
                    }

                    if (ControllerInputPoller.instance.rightGrab && inRhand && !rPressing && !FP)
                    {
                        if (sticking)
                        {
                            cam.transform.SetParent(Player.Instance.bodyCollider.transform);
                        }
                        else
                        {
                            cam.transform.SetParent(null);
                        }
                        isrhand = false;
                        realgopro.SetActive(true);
                        GoProManager.faceCam = false;
                        GoProManager.followPlayer = false;
                        GoProManager.thirdPerson = false;
                        inLhand = false;
                        inRhand = false;
                        rPressing = true;
                        FP = false;
                        hasChangedFOV = false;
                    }
                    else if (!ControllerInputPoller.instance.rightGrab)
                    {
                        rPressing = false;
                    }

                    if (ControllerInputPoller.instance.leftGrab && lHandInTrigger && !inLhand && !lPressing && !FP)
                    {
                        GoProConfig.camMode = 1;
                        cam.transform.SetParent(Lhand.transform);
                        cam.transform.localPosition = new Vector3(-0.1f, 0.05f, 0.0f);
                        cam.transform.localRotation = Quaternion.Euler(0f, 270f, 270f);
                        realgopro.SetActive(true);
                        canBaka = false;
                        isrhand = false;
                        inLhand = true;
                        inRhand = false;
                        GoProManager.faceCam = false;
                        GoProManager.followPlayer = false;
                        GoProManager.thirdPerson = false;
                        lPressing = true;
                        FP = false;
                        hasChangedFOV = false;
                    }
                    else if (!ControllerInputPoller.instance.leftGrab)
                    {
                        lPressing = false;
                    }

                    if (ControllerInputPoller.instance.leftGrab && inLhand && !lPressing && !FP)
                    {
                        if (sticking)
                        {
                            cam.transform.SetParent(Player.Instance.bodyCollider.transform);
                        }
                        else
                        {
                            cam.transform.SetParent(null);
                        }
                        isrhand = false;
                        realgopro.SetActive(true);
                        GoProManager.faceCam = false;
                        GoProManager.followPlayer = false;
                        GoProManager.thirdPerson = false;
                        inLhand = false;
                        inRhand = false;
                        FP = false;
                        lPressing = true;
                        hasChangedFOV = false;
                    }
                    else if (!ControllerInputPoller.instance.leftGrab)
                    {
                        lPressing = false;
                    }
                }
            }
        }

        private void GetInputs()
        {
            leftprim = ControllerInputPoller.instance.leftControllerPrimaryButton;
            prim = ControllerInputPoller.instance.rightControllerPrimaryButton;
            sec = ControllerInputPoller.instance.rightControllerSecondaryButton;
            if (IsSteamVR) { stickclick = SteamVR_Actions.gorillaTag_RightJoystickClick.GetState(SteamVR_Input_Sources.RightHand); }
            else { ControllerInputPoller.instance.rightControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out stickclick); }
        }

        private void GetConfig()
        {
            setCamMode = true;
            fov = GoProConfig.fov;
            shoulderCamera.fieldOfView = fov = Mathf.Clamp(fov, 5, 140);
            fakeCamera.fieldOfView = fov = Mathf.Clamp(fov, 5, 140);
            switch (GoProConfig.camMode)
            {
                case 0:
                    cam.transform.SetParent(Rhand.transform);
                    cam.transform.localPosition = new Vector3(0.11f, 0.05f, 0.0f);
                    cam.transform.localRotation = Quaternion.Euler(0f, 90f, 90f);
                    realgopro.SetActive(true);
                    canBaka = false;
                    isrhand = true;
                    GoProManager.faceCam = false;
                    GoProManager.followPlayer = false;
                    GoProManager.thirdPerson = false;
                    FP = false;
                    hasChangedFOV = false;
                    return;
                case 1:
                    cam.transform.SetParent(Lhand.transform);
                    cam.transform.localPosition = new Vector3(-0.1f, 0.05f, 0.0f);
                    cam.transform.localRotation = Quaternion.Euler(0f, 270f, 270f);
                    realgopro.SetActive(true);
                    canBaka = false;
                    isrhand = false;
                    GoProManager.faceCam = false;
                    GoProManager.followPlayer = false;
                    GoProManager.thirdPerson = false;
                    FP = false;
                    hasChangedFOV = false;
                    return;
                case 2:
                    FP = true;
                    if (!smoothFP)
                    {
                        cam.transform.SetParent(Camera.main.transform);
                        cam.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                        cam.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                        realgopro.SetActive(false);
                        GoProManager.faceCam = false;
                        GoProManager.followPlayer = false;
                        GoProManager.thirdPerson = false;
                        hasChangedFOV = false;
                        isrhand = false;
                    }
                    return;
                case 3:
                    GoProManager.thirdPerson = true;
                    GoProManager.followPlayer = false;
                    GoProManager.faceCam = false;
                    return;
                case 4:
                    GoProManager.thirdPerson = false;
                    GoProManager.followPlayer = false;
                    GoProManager.faceCam = true;
                    return;
                case 5:
                    GoProManager.thirdPerson = false;
                    GoProManager.followPlayer = true;
                    GoProManager.faceCam = false;
                    return;
            }
        }

        void MakeCamera()
        {
            camFollower = GameObject.Find("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/Main Camera/Camera Follower").transform;
            GoProManager.Controlsarelocked = GoProConfig.ControlLock;
            GoProManager.FOVislock = GoProConfig.FOVLock;
            Stream str = Assembly.GetExecutingAssembly().GetManifestResourceStream("WrysersGoPro.Assets.gopro");
            AssetBundle bundle = AssetBundle.LoadFromStream(str);
            gopro = bundle.LoadAsset<GameObject>("Camera");
            realgopro = Instantiate(gopro);
            Debug.Log("GOPRO INSTIATED");
            realgopro.transform.Find("CamGrab").AddComponent<Grabbable>();
            Rhand = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/");
            Lhand = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/");
            cam = GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera");
            cam.transform.SetParent(Rhand.transform);
            realgopro.transform.SetParent(cam.transform);
            cam.transform.Find("CM vcam1").gameObject.SetActive(false);
            shoulderCamera = cam.GetComponent<Camera>();
            cambrain = cam.GetComponent<Cinemachine.CinemachineBrain>();
            cambrain.enabled = false;
            cam.transform.localPosition = new Vector3(0.11f, 0.05f, 0.0f);
            cam.transform.localRotation = Quaternion.Euler(0f, 90f, 90f);
            realgopro.transform.localPosition = new Vector3(0f, 0f, 0f);
            realgopro.transform.localRotation = Quaternion.Euler(0, 90, 0);
            fakeCamera = realgopro.transform.GetChild(1).GetComponent<Camera>();
            stinkymonkeyface = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/head/");
            Stream strr = Assembly.GetExecutingAssembly().GetManifestResourceStream("WrysersGoPro.Assets.gopromenu");
            AssetBundle bundlee = AssetBundle.LoadFromStream(strr);
            GameObject asset = bundlee.LoadAsset<GameObject>("GoProMenu");
            GameObject board = Instantiate(asset);
            GameObject.Find("GoProMenu(Clone)/Canvas/Settings/ControlLock").AddComponent<ControlLock>();
            GameObject.Find("GoProMenu(Clone)/Canvas/Settings/FOVLock").AddComponent<FOVLock>();
            GameObject.Find("GoProMenu(Clone)/Canvas/Settings/FaceCam").AddComponent<FaceCam>();
            GameObject.Find("GoProMenu(Clone)/Canvas/Settings/FollowCam").AddComponent<FollowPlayer>();
            GameObject.Find("GoProMenu(Clone)/Canvas/Settings/TPC").AddComponent<ThirdPerson>();
            GameObject.Find("GoProMenu(Clone)/Canvas/Settings/Grabbable").AddComponent<GrabbableButton>();
            GameObject.Find("GoProMenu(Clone)/Canvas/Settings/SmoothFP").AddComponent<SmoothFP>();
            GameObject.Find("GoProMenu(Clone)/Canvas/Settings/Stick To Player").AddComponent<StickToPlayer>();
            GameObject.Find("GoProMenu(Clone)/Canvas/Settings/HideCam").AddComponent<HideCamera>();
            GorillaMenu.Utils.ModUtils.AddMod("Wrysers Go Pro", board);
            initialized = true;
        }
    }
}