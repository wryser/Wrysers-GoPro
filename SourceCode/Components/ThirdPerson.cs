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
        public bool isFaceCam = false;

        public void FaceCamToggle()
        {
            GoProManager.thirdPerson = true;
            GoProManager.topDown = false;
            GoProManager.faceCam = false;
        }

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(FaceCamToggle);
        }
    }
}
