using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WrysersGoPro.ConfigManager;
using WrysersGoPro.Core;


namespace WrysersGoPro
{
    public class TopDown : MonoBehaviour
    {
        public GameObject text;

        public void TopDownToggle()
        {
            GoProManager.topDown = true;
            GoProManager.faceCam = false;
            GoProManager.thirdPerson = false;
        }

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(TopDownToggle);
        }
    }
}
