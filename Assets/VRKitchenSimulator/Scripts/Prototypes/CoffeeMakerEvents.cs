using UnityEngine;
using UnityTutorialSystem.Events;

namespace VRKitchenSimulator.Prototypes
{
    [RequireComponent(typeof(CoffeeMakerBehaviour))]
    public class CoffeeMakerEvents : StreamEventSource
    {
        CoffeeMakerBehaviour coffeeMaker;

        void Awake()
        {
            coffeeMaker = GetComponent<CoffeeMakerBehaviour>();
        }

        void OnEnable()
        {
            coffeeMaker.FilterAttached.AddListener(OnFilterAttached);
            coffeeMaker.FilterFullyInserted.AddListener(OnFilterLockedIn);
            coffeeMaker.FilterDetached.AddListener(OnFilterDetached);
            coffeeMaker.FilterNoLongerInserted.AddListener(OnFilterNotLockedIn);
            coffeeMaker.FlaskAttached.AddListener(OnFlaskAttached);
            coffeeMaker.FlaskDetached.AddListener(OnFlaskDetached);
            coffeeMaker.BrewingStartedEvent.AddListener(OnBrewingStarted);
            coffeeMaker.BrewingFinishedEvent.AddListener(OnBrewingFinished);
        }

        void OnDisable()
        {
            coffeeMaker.FilterAttached.RemoveListener(OnFilterAttached);
            coffeeMaker.FilterFullyInserted.RemoveListener(OnFilterLockedIn);
            coffeeMaker.FilterDetached.RemoveListener(OnFilterDetached);
            coffeeMaker.FilterNoLongerInserted.RemoveListener(OnFilterNotLockedIn);
            coffeeMaker.FlaskAttached.RemoveListener(OnFlaskAttached);
            coffeeMaker.FlaskDetached.RemoveListener(OnFlaskDetached);
        }

        public override bool WillGenerateMessage(BasicEventStreamMessage msg)
        {
            if (Equals(msg, FilterPanAttached))
            {
                return true;
            }

            if (Equals(msg, FilterPanLocked))
            {
                return true;
            }

            if (Equals(msg, FilterPanRemoved))
            {
                return true;
            }

            if (Equals(msg, FilterPanNotLocked))
            {
                return true;
            }

            if (Equals(msg, FlaskAttached))
            {
                return true;
            }

            if (Equals(msg, FlaskDetached))
            {
                return true;
            }

            if (Equals(msg, BrewingStarted))
            {
                return true;
            }

            if (Equals(msg, BrewingFinished))
            {
                return true;
            }

            if (Equals(msg, CoffeePotMissing))
            {
                return true;
            }

            if (Equals(msg, CoffeePotLidClosed))
            {
                return true;
            }

            if (Equals(msg, CoffeePowderMissing))
            {
                return true;
            }

            if (Equals(msg, FilterMissing))
            {
                return true;
            }

            if (Equals(msg, UnsafeFilterUsed))
            {
                return true;
            }

            if (Equals(msg, UnsafeFilterRemoved))
            {
                return true;
            }

            return false;
        }

        void OnBrewingFinished()
        {
            if (BrewingFinished != null)
            {
                BrewingFinished.Publish();
            }
        }

        void OnBrewingStarted()
        {
            if (BrewingStarted != null)
            {
                BrewingStarted.Publish();
            }

            if (!coffeeMaker.HasFlask)
            {
                if (CoffeePotMissing != null)
                {
                    CoffeePotMissing.Publish();
                }
            }
            else if (!coffeeMaker.FlaskOpen)
            {
                if (CoffeePotLidClosed != null)
                {
                    CoffeePotLidClosed.Publish();
                }
            }

            if (coffeeMaker.HasFilter)
            {
                if (!coffeeMaker.HasCoffeePowderInFilter)
                {
                    if (CoffeePowderMissing != null)
                    {
                        CoffeePowderMissing.Publish();
                    }
                }

                if (coffeeMaker.HasUnsafeFilter)
                {
                    if (UnsafeFilterUsed != null)
                    {
                        UnsafeFilterUsed.Publish();
                    }
                }
            }
            else
            {
                if (FilterMissing != null)
                {
                    FilterMissing.Publish();
                }
            }
        }

        void OnFlaskAttached()
        {
            if (FlaskAttached != null)
            {
                FlaskAttached.Publish();
            }
        }

        void OnFlaskDetached()
        {
            if (FlaskDetached != null)
            {
                FlaskDetached.Publish();
            }
        }

        void OnFilterNotLockedIn()
        {
            if (FilterPanNotLocked != null)
            {
                FilterPanNotLocked.Publish();
            }
        }

        void OnFilterDetached()
        {
            if (FilterPanRemoved != null)
            {
                FilterPanRemoved.Publish();
            }

            if (!coffeeMaker.FilterSafeToRemove)
            {
                if (UnsafeFilterRemoved != null)
                {
                    UnsafeFilterRemoved.Publish();
                }
            }
        }

        void OnFilterLockedIn()
        {
            if (FilterPanLocked != null)
            {
                FilterPanLocked.Publish();
            }
        }

        void OnFilterAttached()
        {
            if (FilterPanAttached != null)
            {
                FilterPanAttached.Publish();
            }
        }
#pragma warning disable 649
        [SerializeField] BasicEventStreamMessage FilterPanAttached;
        [SerializeField] BasicEventStreamMessage FilterPanLocked;
        [SerializeField] BasicEventStreamMessage FilterPanRemoved;
        [SerializeField] BasicEventStreamMessage FilterPanNotLocked;
        [SerializeField] BasicEventStreamMessage FlaskAttached;
        [SerializeField] BasicEventStreamMessage FlaskDetached;
        [SerializeField] BasicEventStreamMessage BrewingStarted;
        [SerializeField] BasicEventStreamMessage BrewingFinished;
        [SerializeField] BasicEventStreamMessage CoffeePotMissing;
        [SerializeField] BasicEventStreamMessage CoffeePotLidClosed;
        [SerializeField] BasicEventStreamMessage CoffeePowderMissing;
        [SerializeField] BasicEventStreamMessage FilterMissing;
        [SerializeField] BasicEventStreamMessage UnsafeFilterUsed;
        [SerializeField] BasicEventStreamMessage UnsafeFilterRemoved;
#pragma warning restore 649
    }
}