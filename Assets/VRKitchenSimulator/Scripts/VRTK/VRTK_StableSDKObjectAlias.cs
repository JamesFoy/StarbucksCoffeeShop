using UnityEngine;
using VRTK;

namespace VRKitchenSimulator.VRTK
{
    /// <summary>
    ///     VRTK's object alias script cannot handle cases where the SDK isn't initialised when that
    ///     script gets enabled. This script fixes that.
    /// </summary>
    public class VRTK_StableSDKObjectAlias : VRTK_SetupListener
    {
        [Tooltip("The specific SDK Object to child this GameObject to.")]
        public VRTK_SDKObjectAlias.SDKObject sdkObject = VRTK_SDKObjectAlias.SDKObject.Boundary;

        protected override void OnSDKSetupInitialized()
        {
            if (VRTK_SDKManager.ValidInstance() && gameObject.activeInHierarchy)
            {
                ChildToSDKObject();
            }
        }

        protected virtual void ChildToSDKObject()
        {
            var currentPosition = transform.localPosition;
            var currentRotation = transform.localRotation;
            var currentScale = transform.localScale;
            Transform newParent = null;

            switch (sdkObject)
            {
                case VRTK_SDKObjectAlias.SDKObject.Boundary:
                    newParent = VRTK_DeviceFinder.PlayAreaTransform();
                    break;
                case VRTK_SDKObjectAlias.SDKObject.Headset:
                    newParent = VRTK_DeviceFinder.HeadsetTransform();
                    break;
            }

            transform.SetParent(newParent);
            transform.localPosition = currentPosition;
            transform.localRotation = currentRotation;
            transform.localScale = currentScale;
        }
    }
}