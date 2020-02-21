using UnityEngine;
using UnityEngine.EventSystems;

namespace VRKitchenSimulator.Helpers
{
    public class EnsureEventSystemExists : MonoBehaviour
    {
        void Start()
        {
            var eventSystem = FindObjectOfType<EventSystem>();
            if (eventSystem == null)
            {
                gameObject.AddComponent<EventSystem>();
            }
        }
    }
}