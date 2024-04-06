using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WrysersGoPro
{
    public class TestUI : MonoBehaviour
    {
        public GameObject text;

        public void TestTheUI()
        {
            text.SetActive(!text.active);
        }

        void Start()
        {
            text = GameObject.Find("UITesting(Clone)/Canvas/succeed");
            text.SetActive(false);
            GetComponent<Button>().onClick.AddListener(TestTheUI);
        }
    }
}
