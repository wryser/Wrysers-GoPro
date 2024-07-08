using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WrysersGoPro.ConfigManager;
using WrysersGoPro.Core;


namespace WrysersGoPro
{
    public class FollowPlayer : MonoBehaviour
    {
        public GameObject text;

        public void FollowPlayerToggle()
        {
            GoProConfig.camMode = 5;
            GoProManager.followPlayer = true;
            GoProManager.faceCam = false;
            GoProManager.thirdPerson = false;
        }

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(FollowPlayerToggle);
        }
    }
}
