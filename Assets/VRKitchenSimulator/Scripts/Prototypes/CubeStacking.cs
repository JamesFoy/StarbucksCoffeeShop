using UnityEngine;
using UnityEngine.Events;

namespace VRKitchenSimulator.Prototypes
{
    public class CubeStacking : MonoBehaviour
    {
        float CollisionStartTime;
        float CollisionSuccessTime;
        bool redCubeIsCollided;
        bool eventFired;

        void OnCollisionEnter(Collision col)
        {
            if (col.gameObject == OtherCube)
            {
                redCubeIsCollided = true;
                CollisionStartTime = Time.time;
                CollisionSuccessTime = CollisionStartTime + 2;
            }
        }

        void Update()
        {
            if (redCubeIsCollided)
            {
                if (CollisionSuccessTime <= Time.time)
                {
                    if (eventFired == false)
                    {
                        StackedCube.Invoke();
                        eventFired = true;
                    }
                }
            }
        }

        void OnCollisionExit(Collision col)
        {
            if (col.gameObject == OtherCube)
            {
                redCubeIsCollided = false;
            }
        }
#pragma warning disable 649
        [SerializeField] UnityEvent StackedCube;
        [SerializeField] GameObject OtherCube;
#pragma warning restore 649
    }
}