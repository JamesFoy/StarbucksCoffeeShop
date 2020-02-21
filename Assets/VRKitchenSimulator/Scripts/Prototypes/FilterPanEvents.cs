using UnityEngine;
using UnityTutorialSystem.Events;
using VRTK;

namespace VRKitchenSimulator.Prototypes
{
    [RequireComponent(typeof(VRTK_InteractableObject))]
    [RequireComponent(typeof(FilterBehaviour))]
    [RequireComponent(typeof(VisualCheckBehaviour))]
    public class FilterPanEvents : StreamEventSource
    {
        VRTK_InteractableObject interactableObject;
        FilterBehaviour filterBehaviour;
        VisualCheckBehaviour filterSpringVisualCheckBehaviour;

        void Awake()
        {
            interactableObject = GetComponent<VRTK_InteractableObject>();
            filterBehaviour = GetComponent<FilterBehaviour>();
            filterSpringVisualCheckBehaviour = GetComponent<VisualCheckBehaviour>();
        }

        void OnEnable()
        {
            interactableObject.InteractableObjectGrabbed += OnGrabbed;
            filterBehaviour.springStateFixed.AddListener(OnSpringFixed);
            filterBehaviour.filterPaperInserted.AddListener(OnPaperInserted);
            filterBehaviour.filterPaperRemoved.AddListener(OnPaperRemoved);
            filterBehaviour.coffeeAdded.AddListener(OnCoffeeAdded);
            filterBehaviour.Overflowing.AddListener(OnOverflowing);
            filterBehaviour.waterPassingFinished.AddListener(OnWaterPassingFinished);
            filterSpringVisualCheckBehaviour.BecameVisible.AddListener(OnSpringVisible);
        }

        void OnDisable()
        {
            interactableObject.InteractableObjectGrabbed -= OnGrabbed;
            filterBehaviour.springStateFixed.RemoveListener(OnSpringFixed);
            filterBehaviour.filterPaperInserted.RemoveListener(OnPaperInserted);
            filterBehaviour.filterPaperRemoved.RemoveListener(OnPaperRemoved);
            filterBehaviour.coffeeAdded.RemoveListener(OnCoffeeAdded);
            filterBehaviour.Overflowing.RemoveListener(OnOverflowing);
            filterBehaviour.waterPassingFinished.RemoveListener(OnWaterPassingFinished);
            filterSpringVisualCheckBehaviour.BecameVisible.RemoveListener(OnSpringVisible);
        }

        public override bool WillGenerateMessage(BasicEventStreamMessage msg)
        {
            if (Equals(msg, grabbedMessage))
            {
                return true;
            }

            if (Equals(msg, springInspectedMessage))
            {
                return true;
            }

            if (Equals(msg, springInspectedMessage))
            {
                return true;
            }

            if (Equals(msg, springFittedMessage))
            {
                return true;
            }

            if (Equals(msg, paperInsertedMessage))
            {
                return true;
            }

            if (Equals(msg, paperRemovedMessage))
            {
                return true;
            }

            if (Equals(msg, filterWaterLevelCheck))
            {
                return true;
            }

            if (Equals(msg, coffeeAddedMessage))
            {
                return true;
            }

            if (Equals(msg, overflowingMessage))
            {
                return true;
            }

            if (Equals(msg, unsafeUsageMessage))
            {
                return true;
            }

            return false;
        }

        void OnOverflowing()
        {
            if (overflowingMessage != null)
            {
                overflowingMessage.Publish();
            }
        }

        void OnCoffeeAdded()
        {
            if (coffeeAddedMessage != null)
            {
                coffeeAddedMessage.Publish();
            }
        }

        void OnPaperInserted()
        {
            if (paperInsertedMessage != null)
            {
                paperInsertedMessage.Publish();
            }
        }

        void OnPaperRemoved()
        {
            if (paperRemovedMessage != null)
            {
                paperRemovedMessage.Publish();
            }
        }

        void OnSpringFixed()
        {
            if (springFittedMessage != null)
            {
                springFittedMessage.Publish();
            }
        }

        /// <summary>
        ///     This is a general check on whether the player has looked into the filter pan.
        /// </summary>
        void OnSpringVisible()
        {
            if (springInspectedMessage != null)
            {
                springInspectedMessage.Publish();
            }

            if (filterWaterLevelCheck != null)
            {
                filterWaterLevelCheck.Publish();
            }

            if (filterBehaviour.SpringState == FilterBehaviour.FilterSpringState.Fixed)
            {
                OnSpringFixed();
            }
        }

        /// <summary>
        ///     Informs the world that this filter has been picked up. This method also checks for cascading
        ///     events. If the spring is visible while the pan is picked up, this counts as inspection, and if
        ///     the spring is already fixed in place, this counts as spring fixed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnGrabbed(object sender, InteractableObjectEventArgs e)
        {
            grabbedMessage.Publish();
            if (filterSpringVisualCheckBehaviour.ObjectVisible)
            {
                OnSpringVisible();
            }
        }

        void OnWaterPassingFinished()
        {
            if (filterBehaviour.IsBlocked && (unsafeUsageMessage != null))
            {
                unsafeUsageMessage.Publish();
            }
        }
#pragma warning disable 649
        [SerializeField] BasicEventStreamMessage grabbedMessage;
        [SerializeField] BasicEventStreamMessage springInspectedMessage;
        [SerializeField] BasicEventStreamMessage springFittedMessage;
        [SerializeField] BasicEventStreamMessage paperInsertedMessage;
        [SerializeField] BasicEventStreamMessage paperRemovedMessage;
        [SerializeField] BasicEventStreamMessage filterWaterLevelCheck;
        [SerializeField] BasicEventStreamMessage coffeeAddedMessage;
        [SerializeField] BasicEventStreamMessage overflowingMessage;
        [SerializeField] BasicEventStreamMessage unsafeUsageMessage;
#pragma warning restore 649
    }
}