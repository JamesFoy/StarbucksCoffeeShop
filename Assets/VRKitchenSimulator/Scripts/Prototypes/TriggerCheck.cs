using UnityEngine;

namespace VRKitchenSimulator.Prototypes
{
    public class TriggerCheck : MonoBehaviour
    {
        Rigidbody body;
        Vector3 storedPos;
        Quaternion storedRot;

        // Use this for initialization
        void Awake()
        {
            body = GetComponent<Rigidbody>();

            if (body == null)
            {
                Debug.LogError("Missing Rigidbody");
                return;
            }

            storedPos = body.position;
            storedRot = body.rotation;
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("BoundingVolume"))
            {
                body.position = storedPos;
                body.rotation = storedRot;
                body.velocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
            }
        }
    }
}