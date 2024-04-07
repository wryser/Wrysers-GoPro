using BepInEx;
using Bepinject;
using System;
using UnityEngine;
using Utilla;
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

namespace WrysersGoPro
{
    /// <summary>
    /// This is your mod's main class.
    /// </summary>

    /* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.6.9")]
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
        bool IsSteamVR;
        public LineRenderer lr;
        Vector3 uiHitPos;
        bool trigger;
        bool canPress = true;
        public AssetBundle bundle;
        public static Plugin instance;
        Transform rHand;
        GameObject testCube;
        int layer_mask;
        GameObject stinkymonkeyface;
        public bool faceCam = false;
        bool hasChangedFOV = false;

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
            IsSteamVR = Traverse.Create(PlayFabAuthenticator.instance).Field("platform").GetValue().ToString().ToLower() == "steam";
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
                leftprim = ControllerInputPoller.instance.leftControllerPrimaryButton;
                prim = ControllerInputPoller.instance.rightControllerPrimaryButton;
                sec = ControllerInputPoller.instance.rightControllerSecondaryButton;
                if (IsSteamVR) { stickclick = SteamVR_Actions.gorillaTag_RightJoystickClick.GetState(SteamVR_Input_Sources.RightHand); }
                else { ControllerInputPoller.instance.rightControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out stickclick); }

                if (prim)
                {
                    if (canBaka && isrhand)
                    {
                        cam.transform.SetParent(Lhand.transform);
                        cam.transform.localPosition = new Vector3(-0.1f, 0.05f, 0.0f);
                        cam.transform.localRotation = Quaternion.Euler(0f, 270f, 270f);
                        realgopro.SetActive(true);
                        canBaka = false;
                        isrhand = false;
                        GoProManager.faceCam = false;
                        GoProManager.topDown = false;
                        GoProManager.thirdPerson = false;
                        hasChangedFOV = false;
                    }
                    else if (canBaka && !isrhand)
                    {
                        cam.transform.SetParent(Rhand.transform);
                        cam.transform.localPosition = new Vector3(0.11f, 0.05f, 0.0f);
                        cam.transform.localRotation = Quaternion.Euler(0f, 90f, 90f);
                        realgopro.SetActive(true);
                        canBaka = false;
                        isrhand = true;
                        GoProManager.faceCam = false;
                        GoProManager.topDown = false;
                        GoProManager.thirdPerson = false;
                        hasChangedFOV = false;
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
                    GoProManager.faceCam = false;
                    GoProManager.topDown = false;
                    GoProManager.thirdPerson = false;
                    hasChangedFOV = false;
                }
                if (leftprim)
                {
                    cam.transform.SetParent(Camera.main.transform);
                    cam.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                    cam.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    realgopro.SetActive(false);
                    GoProManager.faceCam = false;
                    GoProManager.topDown = false;
                    GoProManager.thirdPerson = false;
                    hasChangedFOV = false;
                    isrhand = false;
                }
                if (GoProManager.faceCam)
                {
                    if (!hasChangedFOV)
                    {
                        fov = 60;
                        hasChangedFOV = true;
                    }
                    cam.transform.position = new Vector3(0, 0, 0);
                    cam.transform.SetParent(stinkymonkeyface.transform, false);
                    cam.transform.localPosition = new Vector3(0.0f, 0.08f, 0.3f);
                    cam.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                    realgopro.SetActive(false);
                    isrhand = false;
                }
                if (GoProManager.topDown)
                {
                    hasChangedFOV = false;
                    cam.transform.SetParent(null, false);
                    cam.transform.position = new Vector3(-62f, 28f, -66f);
                    cam.transform.LookAt(Player.Instance.headCollider.transform);
                    Vector3 headpos = new Vector3(Player.Instance.headCollider.transform.position.x, Player.Instance.headCollider.transform.position.y, Player.Instance.headCollider.transform.position.z);
                    Vector3 campos = new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z);
                    cam.GetComponent<Camera>().fieldOfView = Mathf.Abs(60 - Vector3.Distance(transform.InverseTransformPoint(headpos), transform.InverseTransformPoint(campos)));
                    realgopro.SetActive(true);
                }
                if (GoProManager.thirdPerson)
                {
                    if (!hasChangedFOV)
                    {
                        fov = 60;
                        hasChangedFOV = true;
                    }
                    cam.transform.SetParent(Camera.main.transform, false);
                    cam.transform.localPosition = new Vector3(0.0f, 0.3f, -2.0f);
                    cam.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                }

                Vector2 lstick;

                lstick = ControllerInputPoller.instance.rightControllerPrimary2DAxis;
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

        void Update()
        {
            /* Code here runs every frame when the mod is enabled */
            trigger = ControllerInputPoller.instance.rightGrab;
            RaycastHit hit;
            if (Physics.Raycast(testCube.transform.position, testCube.transform.right, out hit, 100f, layer_mask))
            {
                uiHitPos = hit.point;

                if (hit.collider.gameObject.layer == 5 && hit.collider.gameObject.name != "BG")
                {
                    lr.positionCount = 2;
                    DrawLine();
                    if (hit.collider.gameObject.GetComponent<Button>() && canPress && trigger)
                    {
                        hit.collider.gameObject.GetComponent<Button>().onClick.Invoke();
                        canPress = false;
                    }
                    if (!trigger)
                    {
                        canPress = true;
                    }
                }
                else
                {
                    RemoveLine();
                }
            }
            else
            {
                RemoveLine();
            }
        }

        void DrawLine()
        {
            lr.SetPosition(0, testCube.transform.position);
            lr.SetPosition(1, uiHitPos);
        }

        void RemoveLine()
        {
            lr.positionCount = 0;
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
            Rhand = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R/");
            Lhand = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L/");
            cam = GameObject.Find("Player Objects/Third Person Camera/Shoulder Camera");
            cam.transform.SetParent(Rhand.transform);
            realgopro.transform.SetParent(cam.transform);
            GameObject.Find("CM vcam1").SetActive(false);
            shoulderCamera = cam.GetComponent<Camera>();
            cambrain = cam.GetComponent<Cinemachine.CinemachineBrain>();
            cambrain.enabled = false;
            cam.transform.localPosition = new Vector3(0.11f, 0.05f, 0.0f);
            cam.transform.localRotation = Quaternion.Euler(0f, 90f, 90f);
            realgopro.transform.localPosition = new Vector3(0f, 0f, 0f);
            realgopro.transform.localRotation = Quaternion.Euler(0, 90, 0);
            fakeCamera = realgopro.transform.GetChild(1).GetComponent<Camera>();
            stinkymonkeyface = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/head/");
            Stream strr = Assembly.GetExecutingAssembly().GetManifestResourceStream("WrysersGoPro.Assets.gorillaui");
            AssetBundle bundlee = AssetBundle.LoadFromStream(strr);
            GameObject asset = bundlee.LoadAsset<GameObject>("UITesting");
            GameObject board = Instantiate(asset);
            lr = Player.Instance.gameObject.AddComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = Color.white;
            lr.endColor = Color.white;
            lr.startWidth = 0.02f;
            lr.endWidth = 0.02f;
            GameObject.Find("UITesting(Clone)/Canvas/ControlLock").AddComponent<ControlLock>();
            GameObject.Find("UITesting(Clone)/Canvas/FOVLock").AddComponent<FOVLock>();
            GameObject.Find("UITesting(Clone)/Canvas/FaceCam").AddComponent<FaceCam>();
            GameObject.Find("UITesting(Clone)/Canvas/TopDown").AddComponent<TopDown>();
            GameObject.Find("UITesting(Clone)/Canvas/TPC").AddComponent<ThirdPerson>();
            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            rHand = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.R/upper_arm.R/forearm.R/hand.R").GetComponent<Transform>();
            testCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testCube.GetComponent<BoxCollider>().enabled = false;
            testCube.GetComponent<Transform>().localScale = new Vector3(0.3f, 0.3f, 0.3f);
            testCube.transform.localPosition = new Vector3(1f, 0f, 0f);
            testCube.transform.position = rHand.position;
            testCube.transform.localRotation = Quaternion.Euler(0f, 270f, 270f);
            testCube.GetComponent<Transform>().SetParent(rHand);
            layer_mask = LayerMask.GetMask("UI");
        }
    }
}