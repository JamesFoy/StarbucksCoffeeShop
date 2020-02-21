using UnityEngine;
using UnityTutorialSystem.Events;
using VRTK;

namespace VRKitchenSimulator.Prototypes
{
    [RequireComponent(typeof(FlaskBehaviour))]
    [RequireComponent(typeof(VRTK_InteractableObject))]
    public class FlaskEvents : StreamEventSource
    {
        FlaskBehaviour flaskBehaviour;
        VRTK_InteractableObject interactableObject;

        void Awake()
        {
            flaskBehaviour = GetComponent<FlaskBehaviour>();
            interactableObject = GetComponent<VRTK_InteractableObject>();
        }

        void OnEnable()
        {
            flaskBehaviour.LidOpened.AddListener(OnLidOpened);
            flaskBehaviour.LidClosed.AddListener(OnLidClosed);
            flaskBehaviour.Overflowing.AddListener(OnOverflowing);
            interactableObject.InteractableObjectGrabbed += OnGrabbed;
        }

        void OnDisable()
        {
            flaskBehaviour.LidOpened.RemoveListener(OnLidOpened);
            flaskBehaviour.LidClosed.RemoveListener(OnLidClosed);
            flaskBehaviour.Overflowing.RemoveListener(OnOverflowing);
            interactableObject.InteractableObjectGrabbed -= OnGrabbed;
        }

        public override bool WillGenerateMessage(BasicEventStreamMessage msg)
        {
            if (Equals(msg, lidOpenedMessage))
            {
                return true;
            }

            if (Equals(msg, lidClosedMessage))
            {
                return true;
            }

            if (Equals(msg, grabbed))
            {
                return true;
            }

            if (Equals(msg, overflowing))
            {
                return true;
            }

            return false;
        }

        void OnOverflowing()
        {
            if (overflowing != null)
            {
                overflowing.Publish();
            }
        }

        void OnGrabbed(object sender, InteractableObjectEventArgs e)
        {
            if (grabbed != null)
            {
                grabbed.Publish();
            }
        }

        void OnLidClosed()
        {
            if (lidClosedMessage != null)
            {
                lidClosedMessage.Publish();
            }
        }

        void OnLidOpened()
        {
            if (lidOpenedMessage != null)
            {
                lidOpenedMessage.Publish();
            }
        }
#pragma warning disable 649
        [SerializeField] BasicEventStreamMessage lidOpenedMessage;
        [SerializeField] BasicEventStreamMessage lidClosedMessage;
        [SerializeField] BasicEventStreamMessage grabbed;
        [SerializeField] BasicEventStreamMessage overflowing;
#pragma warning restore 649
    }
}