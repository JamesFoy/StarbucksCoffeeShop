using UnityEngine;
using UnityEngine.Events;
using VRTK;

namespace VRKitchenSimulator.VRTK
{
    public abstract class VRTK_BaseNearTouchEvents : MonoBehaviour
    {
        int touchCount;
        public UnityEvent NearTouch => nearTouch;
        public UnityEvent NearUntouch => nearUntouch;

        void OnTouched()
        {
            try
            {
                if (touchCount == 0)
                {
                    nearTouch.Invoke();
                }
            }
            finally
            {
                touchCount += 1;
            }
        }

        void OnUntouched()
        {
            touchCount -= 1;
            if (touchCount == 0)
            {
                nearUntouch.Invoke();
            }
        }

        protected abstract VRTK_InteractableObject EnsureInteractableExists();

        void Start()
        {
            interactable = EnsureInteractableExists();
            if (interactable != null)
            {
                interactable.InteractableObjectUntouched += OnUntouch;
                interactable.InteractableObjectTouched += OnTouch;
            }
        }

        void OnDestroy()
        {
            if (interactable != null)
            {
                interactable.InteractableObjectUntouched -= OnUntouch;
                interactable.InteractableObjectTouched -= OnTouch;
            }
        }

        void OnTouch(object sender, InteractableObjectEventArgs e)
        {
            OnTouched();
        }

        void OnUntouch(object sender, InteractableObjectEventArgs e)
        {
            OnUntouched();
        }
#pragma warning disable 649
        [SerializeField] UnityEvent nearTouch;
        [SerializeField] UnityEvent nearUntouch;
        VRTK_InteractableObject interactable;
#pragma warning restore 649
    }
}