using UnityEngine;

namespace WrysersGoPro
{
    public class Grabbable : MonoBehaviour
    {
        bool isGrabbed = false;
        public static Grabbable instance;

        public void Awake()
        {
            instance = this;
            gameObject.layer = 18;
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == 10)
            {
                switch(other.gameObject.name)
                {
                    case "RightHandTriggerCollider":
                        Plugin.instance.rHandInTrigger = true;
                        return;
                    case "LeftHandTriggerCollider":
                        Plugin.instance.lHandInTrigger = true;
                        return;
                }
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == 10)
            {
                switch (other.gameObject.name)
                {
                    case "RightHandTriggerCollider":
                        Plugin.instance.rHandInTrigger = false;
                        return;
                    case "LeftHandTriggerCollider":
                        Plugin.instance.lHandInTrigger = false;
                        return;
                }
            }
        }
    }
}