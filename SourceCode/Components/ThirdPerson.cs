using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WrysersGoPro.ConfigManager;
using WrysersGoPro.Core;


namespace WrysersGoPro
{
    public class ThirdPerson : MonoBehaviour
    {
        public GameObject text;

        public void FaceCamToggle()
        {
            GoProConfig.camMode = 3;
            GoProManager.thirdPerson = true;
            GoProManager.followPlayer = false;
            GoProManager.faceCam = false;
        }

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(FaceCamToggle);
        }
    }
}
