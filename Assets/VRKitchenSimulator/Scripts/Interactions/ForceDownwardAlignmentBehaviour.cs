using UnityEngine;

namespace VRKitchenSimulator.Interactions
{
    public class ForceDownwardAlignmentBehaviour : MonoBehaviour
    {
        void FixedUpdate()
        {
            var target = transform.position + Vector3.forward;
            transform.LookAt(target, Vector3.up);
        }
    }
}