using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WrysersGoPro.ConfigManager;
using WrysersGoPro.Core;

namespace WrysersGoPro
{
    public class StickToPlayer : MonoBehaviour
    {
        public GameObject text;
        public bool isStickingToPlayer = false;

        public void Awake()
        {
            if (GoProConfig.StickToPlayer)
            {
                StickToPlayerToggle();
            }
        }
        public void StickToPlayerToggle()
        {
            Plugin.instance.sticking = !Plugin.instance.sticking;
            GoProConfig.StickToPlayer = Plugin.instance.sticking;
            isStickingToPlayer = Plugin.instance.sticking;
            if (isStickingToPlayer)
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
            GetComponent<Button>().onClick.AddListener(StickToPlayerToggle);
            isStickingToPlayer = Plugin.instance.sticking;
            GoProConfig.StickToPlayer = Plugin.instance.sticking;
            if (isStickingToPlayer)
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
