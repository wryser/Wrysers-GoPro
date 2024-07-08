using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WrysersGoPro.ConfigManager;
using WrysersGoPro.Core;

namespace WrysersGoPro
{
    public class HideCamera : MonoBehaviour
    {
        public GameObject text;
        public bool cameraHidden = false;

        public void HideCamToggle()
        {
            Plugin.instance.hideCam = !Plugin.instance.hideCam;
            cameraHidden = Plugin.instance.hideCam;
            if (cameraHidden)
            {
                gameObject.GetComponent<Image>().color = Color.green;
            }
            else
            {
                gameObject.GetComponent<Image>().color = Color.red;
            }
            cameraHidden = Plugin.instance.hideCam;
        }

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(HideCamToggle);
            cameraHidden = Plugin.instance.hideCam;
            if (cameraHidden)
            {
                gameObject.GetComponent<Image>().color = Color.green;
            }
            else
            {
                gameObject.GetComponent<Image>().color = Color.red;
            }
            cameraHidden = Plugin.instance.hideCam;
        }
    }
}
