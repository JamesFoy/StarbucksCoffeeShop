using UnityEngine;
using VRTK;

namespace VRKitchenSimulator.VRTK
{
    public static class VRTKHelpers
    {
        public static void InitiateTouch(this VRTK_InteractGrab grabber, VRTK_InteractableObject target)
        {
            grabber.interactTouch.ForceTouch(target.gameObject);
            if (target.gameObject != grabber.interactTouch.GetTouchedObject())
            {
                Debug.Log("Failed to touch object");
            }
        }

        public static void InitiateTouchAndGrab(this VRTK_InteractGrab grabber, VRTK_InteractableObject target)
        {
            if (target.IsGrabbed())
            {
                Debug.LogWarning("Given object is already grabbed.");
            }

            grabber.InitiateTouch(target);
            grabber.AttemptGrab();
        }

        public static void StopTouchAndGrab(this VRTK_InteractGrab grabber, VRTK_InteractableObject target)
        {
            grabber.ForceRelease();
            grabber.interactTouch.ForceStopTouching();
        }

        public static VRTK_InteractGrab FindGrabController(this VRTK_InteractableObject grabbed)
        {
            var left = VRTK_SDKManager.instance.scriptAliasLeftController;
            var grabberLeft = TryFindGrabber(grabbed, left);
            if (grabberLeft != null)
            {
                return grabberLeft;
            }

            return TryFindGrabber(grabbed, VRTK_SDKManager.instance.scriptAliasRightController);
        }

        static VRTK_InteractGrab TryFindGrabber(VRTK_InteractableObject grabbed, GameObject left)
        {
            if (left != null)
            {
                var grabber = left.GetComponent<VRTK_InteractGrab>();
                if (grabber.GetGrabbedObject() == grabbed.gameObject)
                {
                    return grabber;
                }
            }

            return null;
        }
    }
}