using UnityEngine;
using UnityTutorialSystem.Events;
using VRTK;

namespace VRKitchenSimulator.Prototypes
{
    [RequireComponent(typeof(VRTK_InteractableObject))]
    [RequireComponent(typeof(CoffeeSachetBehaviour))]
    public class CoffeeSachetEvents : StreamEventSource
    {
        VRTK_InteractableObject interactableObject;
        CoffeeSachetBehaviour coffeeSachetBehaviour;
        public BasicEventStreamMessage Grabbed;
        public BasicEventStreamMessage Opened;

        void Awake()
        {
            interactableObject = GetComponent<VRTK_InteractableObject>();
            coffeeSachetBehaviour = GetComponent<CoffeeSachetBehaviour>();
        }

        void OnEnable()
        {
            interactableObject.InteractableObjectGrabbed += OnGrabbed;
            coffeeSachetBehaviour.Opened.AddListener(OnSachetOpened);
        }

        void OnDisable()
        {
            interactableObject.InteractableObjectGrabbed -= OnGrabbed;
            coffeeSachetBehaviour.Opened.RemoveListener(OnSachetOpened);
        }

        public override bool WillGenerateMessage(BasicEventStreamMessage msg)
        {
            if (Equals(msg, Opened))
            {
                return true;
            }

            if (Equals(msg, Grabbed))
            {
                return true;
            }

            return false;
        }

        void OnSachetOpened()
        {
            Opened.Publish();
        }

        void OnGrabbed(object sender, InteractableObjectEventArgs e)
        {
            Grabbed.Publish();
        }
    }
}