using UnityEngine;
using UnityEngine.Events;

namespace VRKitchenSimulator.Interactions
{
    public class TutorialEnterTargetZoneHandler : MonoBehaviour
    {
        public UnityEvent ZoneEntered;
        public Collider targetZone;

        void OnTriggerEnter(Collider other)
        {
            if (other == targetZone)
            {
                ZoneEntered.Invoke();
            }
        }
    }
}