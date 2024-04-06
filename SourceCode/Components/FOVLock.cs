using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WrysersGoPro.ConfigManager;
using WrysersGoPro.Core;

namespace WrysersGoPro
{
    public class FOVLock : MonoBehaviour
    {
        public GameObject text;
        public bool isLocked = false;

        public void LockControls()
        {
            GoProManager.FOVislock = !GoProManager.FOVislock;
            GoProConfig.FOVLock = GoProManager.FOVislock;
            isLocked = GoProConfig.FOVLock;
            if (isLocked)
            {
                gameObject.GetComponent<Image>().color = Color.green;
            }
            else
            {
                gameObject.GetComponent<Image>().color = Color.red;
            }
            isLocked = GoProConfig.FOVLock;
        }

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(LockControls);
            isLocked = GoProConfig.FOVLock;
            if (isLocked)
            {
                gameObject.GetComponent<Image>().color = Color.green;
            }
            else
            {
                gameObject.GetComponent<Image>().color = Color.red;
            }
            isLocked = GoProConfig.FOVLock;
        }
    }
}
