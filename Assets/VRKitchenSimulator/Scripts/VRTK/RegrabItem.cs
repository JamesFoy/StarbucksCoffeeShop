using UnityEngine;
using VRTK;

namespace VRKitchenSimulator.VRTK
{
    /// <summary>
    ///     A debug helper that regrabs the given item
    /// </summary>
    public class RegrabItem : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] VRTK_InteractableObject interactiveObject;
#pragma warning restore 649

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                Regrab();
            }
        }

        void Regrab()
        {
            var grabber = interactiveObject.FindGrabController();
            if (grabber != null)
            {
                interactiveObject.grabAttachMechanicScript.StartGrab(grabber.gameObject,
                                                                     grabber.GetGrabbedObject(),
                                                                     grabber.controllerAttachPoint);
            }
        }
    }
}