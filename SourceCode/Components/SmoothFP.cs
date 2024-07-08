using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WrysersGoPro.ConfigManager;
using WrysersGoPro.Core;

namespace WrysersGoPro
{
    public class SmoothFP : MonoBehaviour
    {
        public GameObject text;
        public bool isSmoothFP = false;

        public void Awake()
        {
            if (GoProConfig.SmoothFP)
            {
                ToggleSmoothFP();
            }
        }
        public void ToggleSmoothFP()
        {
            Plugin.instance.smoothFP = !Plugin.instance.smoothFP;
            GoProConfig.SmoothFP = Plugin.instance.smoothFP;
            isSmoothFP = Plugin.instance.smoothFP;
            if (isSmoothFP)
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
            GetComponent<Button>().onClick.AddListener(ToggleSmoothFP);
            isSmoothFP = Plugin.instance.smoothFP;
            GoProConfig.SmoothFP = Plugin.instance.smoothFP;
            if (isSmoothFP)
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
