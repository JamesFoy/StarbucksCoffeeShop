using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using VRKitchenSimulator.Helpers;
using VRKitchenSimulator.States;
using VRKitchenSimulator.VRTK;
using VRTK;
using VRTK.Controllables;
using VRTK.Controllables.ArtificialBased;
using XStateMachine.Behaviours;

namespace VRKitchenSimulator.Prototypes
{
    public class CoffeeMakerBehaviour : MonoBehaviour
    {
        readonly ActivationTimer timer = new ActivationTimer();

        [BoxGroup("Button Events")]
        public UnityEvent StopButtonActivated;

        [BoxGroup("Button Events")]
        [Tooltip("Half Button")]
        public UnityEvent HalfButtonActivated;

        [BoxGroup("Button Events")]
        [Tooltip("Full Button")]
        public UnityEvent FullButtonActivated;

        [BoxGroup("Flask Events")]
        public UnityEvent FlaskAttached;

        [BoxGroup("Flask Events")]
        public UnityEvent FlaskDetached;

        [BoxGroup("Filter Events")]
        public UnityEvent FilterAttached;

        [BoxGroup("Filter Events")]
        public UnityEvent FilterDetached;

        [BoxGroup("Filter Events")]
        public UnityEvent FilterFullyInserted;

        [BoxGroup("Filter Events")]
        public UnityEvent FilterNoLongerInserted;

        [BoxGroup("Brewing Events")]
        public UnityEvent BrewingStartedEvent;

        [BoxGroup("Brewing Events")]
        public UnityEvent BrewingFinishedEvent;

        FlaskBehaviour currentFlask;
        FilterBehaviour currentFilter;
        bool flaskSafeToReceiveLiquid;

        public bool FlaskSafeToReceiveLiquid
        {
            get { return flaskSafeToReceiveLiquid; }
            private set
            {
                flaskSafeToReceiveLiquid = value;
                if (currentFilter != null)
                {
                    currentFilter.CoffeeFilterSafeToUse = flaskSafeToReceiveLiquid;
                }
            }
        }

        public bool Brewing { get; set; }

        public bool HasFlask => currentFlask != null;
        public bool FlaskOpen => (currentFlask != null) && currentFlask.LidOpen;
        public bool HasFilter => currentFilter != null;
        public bool HasCoffeePowderInFilter => (currentFilter != null) && currentFilter.CoffeePowderInserted;
        public bool HasUnsafeFilter => (currentFilter != null) && currentFilter.IsBlocked;

        public bool FilterSafeToRemove
        {
            get
            {
                var filter = GetAttachedFilter();
                if (filter == null)
                {
                    return true;
                }

                return !filter.ContainsWater;
            }
        }

        void Awake()
        {
            if (halfBrewTimer != null)
            {
                halfBrewTimer.CoolDownTime = halfBrewDuration;
            }

            if (fullBrewTimer != null)
            {
                fullBrewTimer.CoolDownTime = fullBrewDuration;
            }
        }

        void OnFlaskUnsnapped(object sender, SnapDropZoneEventArgs e)
        {
            FlaskDetached.Invoke();
            if (currentFlask != null)
            {
                currentFlask.LidOpened.RemoveListener(OnFlaskLidOpened);
                currentFlask.LidClosed.RemoveListener(OnFlaskLidClosed);
                FlaskSafeToReceiveLiquid = false;
            }

            currentFlask = null;
            flaskInsertedTrigger.DoDeactivate();
        }

        void OnFlaskSnapped(object sender, SnapDropZoneEventArgs e)
        {
            currentFlask = e.snappedObject.GetComponent<FlaskBehaviour>();
            if (currentFlask != null)
            {
                currentFlask.LidOpened.AddListener(OnFlaskLidOpened);
                currentFlask.LidClosed.AddListener(OnFlaskLidClosed);
                FlaskSafeToReceiveLiquid = currentFlask.LidOpen;
            }
            else
            {
                FlaskSafeToReceiveLiquid = false;
            }

            flaskInsertedTrigger.DoActivate();
            FlaskAttached.Invoke();
        }

        void OnFlaskLidClosed()
        {
            FlaskSafeToReceiveLiquid = false;
        }

        void OnFlaskLidOpened()
        {
            FlaskSafeToReceiveLiquid = true;
        }

        void OnEnable()
        {
            ButtonBL.MaxLimitReached += ButtonBlOnMaxLimitReached;
            ButtonBM.MaxLimitReached += ButtonBmOnMaxLimitReached;
            ButtonBR.MaxLimitReached += ButtonBrOnMaxLimitReached;

            flaskSnapDropZone.ObjectSnappedToDropZone += OnFlaskSnapped;
            flaskSnapDropZone.ObjectUnsnappedFromDropZone += OnFlaskUnsnapped;

            sliderController.SliderLimitExited.AddListener(OnFilterNoLongerFullyInserted);
            sliderController.SliderLimitReached.AddListener(OnFilterFullyInserted);
            sliderController.ObjectAttached.AddListener(OnFilterAttached);
            sliderController.ObjectDetached.AddListener(OnFilterDetached);
        }

        void OnDisable()
        {
            ButtonBL.MaxLimitReached -= ButtonBlOnMaxLimitReached;
            ButtonBM.MaxLimitReached -= ButtonBmOnMaxLimitReached;
            ButtonBR.MaxLimitReached -= ButtonBrOnMaxLimitReached;

            flaskSnapDropZone.ObjectSnappedToDropZone -= OnFlaskSnapped;
            flaskSnapDropZone.ObjectUnsnappedFromDropZone -= OnFlaskUnsnapped;

            sliderController.SliderLimitExited.RemoveListener(OnFilterNoLongerFullyInserted);
            sliderController.SliderLimitReached.RemoveListener(OnFilterFullyInserted);
            sliderController.ObjectAttached.RemoveListener(OnFilterAttached);
            sliderController.ObjectDetached.RemoveListener(OnFilterDetached);
        }

        void OnFilterDetached()
        {
            FilterDetached.Invoke();
        }

        void OnFilterAttached()
        {
            FilterAttached.Invoke();
        }

        void OnFilterNoLongerFullyInserted()
        {
            if (currentFilter != null)
            {
                currentFilter.WaterPassingThrough -= OnWaterPassingThroughFilter;

                filterInsertedTrigger.DoDeactivate();
                FilterNoLongerInserted.Invoke();
            }

            Debug.Log("Filter Removed: " + currentFilter);
            currentFilter = null;
        }

        FilterBehaviour GetAttachedFilter()
        {
            var currentSnappedObject = sliderController.MountedObject;
            if (currentSnappedObject == null)
            {
                return null;
            }

            return currentSnappedObject.GetComponent<FilterBehaviour>();
        }

        void OnFilterFullyInserted()
        {
            var currentSnappedObject = sliderController.MountedObject;
            if (currentSnappedObject != null)
            {
                currentFilter = currentSnappedObject.GetComponent<FilterBehaviour>();
                if (currentFilter != null)
                {
                    currentFilter.CoffeeFilterSafeToUse = FlaskSafeToReceiveLiquid;
                    currentFilter.WaterPassingThrough += OnWaterPassingThroughFilter;
                }
            }
            else
            {
                currentFilter = null;
            }

            if (currentFilter != null)
            {
                filterInsertedTrigger.DoActivate();
                FilterFullyInserted.Invoke();
            }
        }

        void OnWaterPassingThroughFilter(object sender, FilterFlowEventArgs e)
        {
            if (currentFlask != null)
            {
                currentFlask.ReceiveLiquidTick(e.WaterLevel, e.HasCoffeePowder);
            }
        }

        void ButtonBrOnMaxLimitReached(object sender, ControllableEventArgs e)
        {
            FullButtonActivated.Invoke();
        }

        void ButtonBmOnMaxLimitReached(object sender, ControllableEventArgs e)
        {
            HalfButtonActivated.Invoke();
        }

        void ButtonBlOnMaxLimitReached(object sender, ControllableEventArgs e)
        {
            StopButtonActivated.Invoke();
        }

        void Update()
        {
            timer.Update();

            if (!Brewing)
            {
                if (!waterParticleSystem.isStopped)
                {
                    waterParticleSystem.Stop();
                }

                return;
            }

            if (currentFilter != null)
            {
                if (!waterParticleSystem.isStopped)
                {
                    waterParticleSystem.Stop();
                }

                currentFilter.ReceiveLiquidTick(flowRate * timer.DeltaTime);
            }
            else
            {
                if (waterParticleSystem.isStopped)
                {
                    waterParticleSystem.Play();
                }

                if (currentFlask != null)
                {
                    currentFlask.ReceiveLiquidTick(flowRate * timer.DeltaTime, false);
                }
            }
        }

        public void BrewingStarted()
        {
            Brewing = true;
            BrewingStartedEvent.Invoke();
            timer.Start();
        }

        public void BrewingFinished()
        {
            Brewing = false;
            BrewingFinishedEvent.Invoke();
        }

#pragma warning disable 649
        [BoxGroup("Internal Settings")]
        [SerializeField]
        bool showInternalEditors;

        [BoxGroup("Internal Settings")]
        [ShowIf(nameof(showInternalEditors))]
        [Required("This component is mandatory")]
        [SerializeField]
        VRTK_ArtificialPusher ButtonBL;

        [BoxGroup("Internal Settings")]
        [ShowIf(nameof(showInternalEditors))]
        [Required("This component is mandatory")]
        [SerializeField]
        VRTK_ArtificialPusher ButtonBM;

        [BoxGroup("Internal Settings")]
        [ShowIf(nameof(showInternalEditors))]
        [Required("This component is mandatory")]
        [SerializeField]
        VRTK_ArtificialPusher ButtonBR;

        [BoxGroup("Internal Settings")]
        [ShowIf(nameof(showInternalEditors))]
        [Required("This component is mandatory")]
        [SerializeField]
        SliderMountController sliderController;

        [BoxGroup("Internal Settings")]
        [ShowIf(nameof(showInternalEditors))]
        [Required("This component is mandatory")]
        [SerializeField]
        VRTK_SnapDropZone flaskSnapDropZone;

        [BoxGroup("Internal Settings")]
        [ShowIf(nameof(showInternalEditors))]
        [Required("This component is mandatory")]
        [SerializeField]
        ParticleSystem waterParticleSystem;

        [BoxGroup("Internal State Connectors")]
        [ShowIf(nameof(showInternalEditors))]
        [Required("This component is mandatory")]
        [SerializeField]
        TriggerStateBehaviour filterInsertedTrigger;

        [BoxGroup("Internal State Connectors")]
        [ShowIf(nameof(showInternalEditors))]
        [Required("This component is mandatory")]
        [SerializeField]
        TriggerStateBehaviour flaskInsertedTrigger;

        [BoxGroup("Internal State Connectors")]
        [ShowIf(nameof(showInternalEditors))]
        [Required("This component is mandatory")]
        [SerializeField]
        AutomaticDeactivationTrigger halfBrewTimer;

        [BoxGroup("Internal State Connectors")]
        [ShowIf(nameof(showInternalEditors))]
        [Required("This component is mandatory")]
        [SerializeField]
        AutomaticDeactivationTrigger fullBrewTimer;

        [BoxGroup("Behaviour")]
        [SerializeField]
        float flowRate;

        [BoxGroup("Behaviour")]
        [SerializeField]
        float halfBrewDuration;

        [BoxGroup("Behaviour")]
        [SerializeField]
        float fullBrewDuration;
#pragma warning restore 649
    }
}