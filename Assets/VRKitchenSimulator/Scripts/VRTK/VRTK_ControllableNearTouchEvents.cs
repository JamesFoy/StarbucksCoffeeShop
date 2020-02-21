using VRTK;

namespace VRKitchenSimulator.VRTK
{
    public class VRTK_ControllableNearTouchEvents : VRTK_BaseNearTouchEvents
    {
        protected override VRTK_InteractableObject EnsureInteractableExists()
        {
            var parent = transform.parent;
            if (parent == null)
            {
                return null;
            }

            return parent.GetComponent<VRTK_InteractableObject>();
        }
    }
}