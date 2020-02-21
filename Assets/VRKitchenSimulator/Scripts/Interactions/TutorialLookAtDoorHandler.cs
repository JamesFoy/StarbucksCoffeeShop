using UnityEngine;
using UnityEngine.Events;
using VRKitchenSimulator.VRTK;

namespace VRKitchenSimulator.Interactions
{
    public class TutorialLookAtDoorHandler : VRTK_SetupListener
    {
        public Collider TargetCollider;
        public UnityEvent PlayerIsLookingAtMe;
#pragma warning disable 649
        [SerializeField] float rateLimit;
#pragma warning restore 649
        GameObject headSet;
        float lastEventFired;


        protected override void Update()
        {
            base.Update();
            if ((headSet != null) && (lastEventFired + rateLimit < Time.time))
            {
                var viewDirection = headSet.transform.forward;
                var viewOrigin = headSet.transform.position;
                RaycastHit hitInfo;
                if (Physics.Raycast(new Ray(viewOrigin, viewDirection), out hitInfo, 100f))
                {
                    if (hitInfo.collider == TargetCollider)
                    {
                        PlayerIsLookingAtMe.Invoke();
                        lastEventFired = rateLimit;
                    }
                    else
                    {
                        lastEventFired = rateLimit;
                    }
                }
            }
        }

        protected override void OnSDKSetupInitialized()
        {
            headSet = LocateHeadSet();
        }
    }
}