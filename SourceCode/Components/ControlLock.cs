using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WrysersGoPro.ConfigManager;
using WrysersGoPro.Core;

namespace WrysersGoPro
{
    public class ControlLock : MonoBehaviour
    {
        public GameObject text;
        public bool isLocked = false;

        public void LockControls()
        {
            GoProManager.Controlsarelocked = !GoProManager.Controlsarelocked;
            GoProConfig.ControlLock = GoProManager.Controlsarelocked;
            isLocked = GoProConfig.ControlLock;
            if (isLocked)
            {
                gameObject.GetComponent<Image>().color = Color.green;
            }
            else
            {
                gameObject.GetComponent<Image>().color = Color.red;
            }
            isLocked = GoProConfig.ControlLock;
        }

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(LockControls);
            isLocked = GoProConfig.ControlLock;
            if (isLocked)
            {
                gameObject.GetComponent<Image>().color = Color.green;
            }
            else
            {
                gameObject.GetComponent<Image>().color = Color.red;
            }
            isLocked = GoProConfig.ControlLock;
        }
    }
}
