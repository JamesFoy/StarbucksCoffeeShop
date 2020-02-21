using UnityEngine;

namespace VRKitchenSimulator.Helpers
{
    public class ColliderMonitor : MonoBehaviour
    {
        void OnCollisionEnter(Collision other)
        {
            Debug.Log(name + ": Collision Enter: " + other.gameObject);
        }

        void OnCollisionExit(Collision other)
        {
            Debug.Log(name + ": Collision Exit: " + other.gameObject);
        }

        void OnTriggerEnter(Collider other)
        {
            Debug.Log(name + ": Trigger Enter: " + other.gameObject);
        }

        void OnTriggerExit(Collider other)
        {
            Debug.Log(name + ": Trigger Exit: " + other.gameObject);
        }
    }
}