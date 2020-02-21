using VRTK;

namespace VRKitchenSimulator.VRTK
{
    public class VRTK_InteractableNearTouchEvents : VRTK_BaseNearTouchEvents
    {
        protected override VRTK_InteractableObject EnsureInteractableExists()
        {
            return GetComponent<VRTK_InteractableObject>();
        }
    }
}