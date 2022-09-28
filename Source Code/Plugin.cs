using BepInEx;
using System;
using UnityEngine;
using Utilla;
using UnityEngine.XR;
using Cinemachine;

namespace WrysersGoPro
{
    /// <summary>
    /// This is your mod's main class.
    /// </summary>

    /* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        bool inRoom;
        GameObject Rhand;
        GameObject Lhand;
        GameObject cam;
        Camera shoulderCamera;
        CinemachineBrain cambrain;
        float nextBaka;
        float bakacooldown = 1f;
        bool canBaka;
        bool prim;
        bool sec;
        bool leftprim;
        bool stickclick;
        bool isrhand = true;
        float fov = 60;
        Material Material1;
        GameObject gopro;

        void Start()
        {
            Events.GameInitialized += OnGameInitialized;
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            /* Code here runs after the game initializes (i.e. GorillaLocomotion.Player.Instance != null) */
            Lhand = GorillaTagger.Instance.offlineVRRig.leftHandTransform.parent.Find("palm.01.L").gameObject;
            Rhand = GorillaTagger.Instance.offlineVRRig.rightHandTransform.parent.Find("palm.01.R").gameObject;
            cam = GameObject.Find("Shoulder Camera");
            gopro = GameObject.CreatePrimitive(PrimitiveType.Cube);
            GameObject goprolens = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            goprolens.transform.GetComponent<CapsuleCollider>().enabled = false;
            gopro.transform.SetParent(cam.transform, false);
            goprolens.transform.SetParent(gopro.transform, false);
            gopro.transform.GetComponent<BoxCollider>().enabled = false;
            gopro.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            goprolens.transform.localScale = new Vector3(0.3f, 0.5f, 0.3f);
            goprolens.transform.localPosition = new Vector3(0f, 0f, 0.05f);
            goprolens.transform.localRotation = Quaternion.Euler(90f, 0.0f, 0.0f);
            cam.transform.SetParent(Rhand.transform);
            GameObject.Find("CM vcam1").SetActive(false);
            shoulderCamera = cam.GetComponent<Camera>();
            cambrain = cam.GetComponent<CinemachineBrain>();
            cambrain.enabled = false;
            cam.transform.localPosition = new Vector3(-0.1f, 0.0f, 0.0f);
            cam.transform.localRotation = Quaternion.Euler(0f, 270f, 90f);
            gopro.GetComponent<MeshRenderer>().material = Material1;
            gopro.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.black);
        }

        void FixedUpdate()
        {
            /* Code here runs every frame when the mod is enabled */
            shoulderCamera.fieldOfView = fov = Mathf.Clamp(fov, 5, 160);

            if (Time.time > nextBaka)
            {
                canBaka = true;
                nextBaka = Time.time + bakacooldown;
            }
            InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.primaryButton, out prim);
            InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.secondaryButton, out sec);
            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.primaryButton, out leftprim);
            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.primary2DAxisClick, out stickclick);

            if (prim)
            {
                if (canBaka && isrhand)
                {
                    cam.transform.SetParent(Lhand.transform);
                    cam.transform.localPosition = new Vector3(0.0f, 0.0f, 0.1f);
                    cam.transform.localRotation = Quaternion.Euler(0f, 0f, 270f);
                    gopro.SetActive(true);
                    canBaka = false;
                    isrhand = false;
                }
                else if (canBaka && !isrhand)
                {
                    cam.transform.SetParent(Rhand.transform);
                    cam.transform.localPosition = new Vector3(-0.1f, 0.0f, 0.0f);
                    cam.transform.localRotation = Quaternion.Euler(0f, 270f, 90f);
                    gopro.SetActive(true);
                    canBaka = false;
                    isrhand = true;
                }
            }
            if (stickclick)
            {
                fov = 60;
            }
            if (sec)
            {
                cam.transform.SetParent(null);
                isrhand = false;
                gopro.SetActive(true);
            }
            if (leftprim)
            {
                cam.transform.SetParent(Camera.main.transform);
                cam.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                cam.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                gopro.SetActive(false);
                isrhand = false;
            }

            Vector2 lstick;

            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out lstick);

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
