using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using VRKitchenSimulator.VRTK;

namespace VRKitchenSimulator.Prototypes
{
    /// <summary>
    ///     A small utility script that tries to evaluate whether a game object (like the filter spring)
    ///     would be visible to the player. This script avoids ray-casts in favor of a mathematical approximation
    ///     as we need to test objects that are occluded by other colliders and we only need a 'good enough'
    ///     solution for it to feel right.
    /// </summary>
    public class VisualCheckBehaviour : VRTK_SetupListener
    {
        GameObject headSet;
        bool objectVisible;
        bool delayedSetup;
        public UnityEvent BecameVisible;
        public UnityEvent BecameInvisible;

        public VisualCheckBehaviour()
        {
            BecameInvisible = new UnityEvent();
            BecameVisible = new UnityEvent();
        }

        [ShowNativeProperty]
        public bool ObjectVisible
        {
            get { return objectVisible; }
            set
            {
                if (value == objectVisible)
                {
                    return;
                }

                Debug.Log("Visible Changed: " + value);
                objectVisible = value;
                if (objectVisible)
                {
                    BecameVisible?.Invoke();
                }
                else
                {
                    BecameInvisible?.Invoke();
                }
            }
        }

        void Awake()
        {
            if (sourceObject == null)
            {
                sourceObject = gameObject;
            }
        }

        protected override void OnSDKSetupInitialized()
        {
            headSet = LocateHeadSet();
        }

        void OnDrawGizmos()
        {
            if (headSet != null)
            {
                var directionToHeadSet = (headSet.transform.position - sourceObject.transform.position).normalized;
                var up = sourceObject.transform.up;
                Gizmos.DrawLine(sourceObject.transform.position, sourceObject.transform.position + directionToHeadSet);
                Gizmos.DrawLine(sourceObject.transform.position, sourceObject.transform.position + up);
            }
        }

        protected override void Update()
        {
            base.Update();
            if (headSet != null)
            {
                // cannot use a raycast, as the filter's collider is convex and the spring is deep inside that filter.
                // ignoring that collider would not work either, as we need to see whether we'll collide with the filter's
                // walls. So instead of raycasts, lets check face alignment instead, and we simply assume that that is 
                // a good enough check. 
                var directionToHeadSet = (headSet.transform.position - sourceObject.transform.position).normalized;
                var up = sourceObject.transform.up;

                // Check that the headset is located within the right area to see the filter.
                // and then check that the headset is looking towards the filter.
                var alignmentWithHeadsetPosition = Vector3.Dot(directionToHeadSet, up);
                var alignmentWithHeadsetDirection = Vector3.Dot(headSet.transform.forward, up);
                // Dot product is between 1 and -1.
                var targetRange = 90 - visibleAngle;
                if ((Mathf.Abs(alignmentWithHeadsetDirection) * 90 > targetRange) &&
                    (alignmentWithHeadsetPosition * 90 > targetRange))
                {
                    ObjectVisible = true;
                }
                else
                {
                    ObjectVisible = false;
                }
            }
            else
            {
                ObjectVisible = false;
            }
        }
#pragma warning disable 649
        [SerializeField] GameObject sourceObject;
        [SerializeField] float visibleAngle;
#pragma warning restore 649
    }
}