using UnityEngine;

namespace VRKitchenSimulator.Helpers
{
    /// <summary>
    ///  Requires a trigger-collider that has a RespawnZone attached.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class RespawnObjects: MonoBehaviour
    {
        new Rigidbody rigidbody;
        bool recordedSleep;
        Vector3 recordedPosition;
        Quaternion recordedRotation;

        void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            if (recordedSleep && rigidbody.IsSleeping())
            {
                return;
            }

            // record the position once the body is falling asleep. 
            // once the rigidbody is sleeping, those positions wont change.
            // never record a moving body, as we cannot tell whether the
            // resting position for it would be valid.
            if (rigidbody.IsSleeping())
            {
                recordedSleep = rigidbody.IsSleeping();
                recordedPosition = rigidbody.position;
                recordedRotation = rigidbody.rotation;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (!other.GetComponent<RespawnZone>())
            {
                return;
            }

            Debug.Log($"Restored position: {name} to {recordedPosition}; {recordedRotation.eulerAngles}");
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.position = recordedPosition;
            rigidbody.rotation = recordedRotation;
        }
    }
}