using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WrysersGoPro.ConfigManager;
using WrysersGoPro.Core;

namespace WrysersGoPro
{
    public class GrabbableButton : MonoBehaviour
    {
        public GameObject text;
        public bool grabbable = false;

        public void Awake()
        {
            if (GoProConfig.Grabbable)
            {
                ToggleGrabbable();
            }
        }
        public void ToggleGrabbable()
        {
            Plugin.instance.grabbable = !Plugin.instance.grabbable;
            GoProConfig.Grabbable = Plugin.instance.grabbable;
            grabbable = Plugin.instance.grabbable;
            if (grabbable)
            {
                gameObject.GetComponent<Image>().color = Color.green;
            }
            else
            {
                gameObject.GetComponent<Image>().color = Color.red;
            }
        }

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(ToggleGrabbable);
            grabbable = Plugin.instance.grabbable;
            GoProConfig.Grabbable = Plugin.instance.grabbable;
            if (grabbable)
            {
                gameObject.GetComponent<Image>().color = Color.green;
            }
            else
            {
                gameObject.GetComponent<Image>().color = Color.red;
            }
        }
    }
}
