using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WrysersGoPro.ConfigManager;
using WrysersGoPro.Core;


namespace WrysersGoPro
{
    public class FaceCam : MonoBehaviour
    {
        public GameObject text;

        public void FaceCamToggle()
        {
            GoProManager.faceCam = true;
            GoProManager.topDown = false;
            GoProManager.thirdPerson = false;
        }

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(FaceCamToggle);
        }
    }
}
