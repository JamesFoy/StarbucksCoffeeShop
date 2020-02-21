using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using VRKitchenSimulator.Helpers;
using VRTK;
using VRTK.Controllables;
using VRTK.Controllables.ArtificialBased;

namespace VRKitchenSimulator.Prototypes
{
    public class FilterBehaviour : MonoBehaviour, ICoffeePowderReceiver
    {
        public enum FilterSpringState
        {
            None,
            Loose,
            Fixed
        }

        public enum FilterPaperState
        {
            None,
            Clean,
            Dirty
        }

        IEnumerator tickCoroutine;

        public bool CoffeePowderInserted => coffeePowderInserted;

        float FillLevelNormalized => MaximumFillLevel <= 0 ? 0 : Mathf.Clamp01(FillLevel / MaximumFillLevel);

        public float FillLevel
        {
            get { return fillLevel; }
            set
            {
                fillLevel = value;
                if (waterPlane != null)
                {
                    waterPlane.UpdateFillLevel(FillLevelNormalized);
                }
            }
        }

        /// <summary>
        ///     An internal flag that is maintained by the coffee maker. If set to true,
        ///     then any "water passing through" event will be handled by a flask.
        ///     If this flag is false, the filter should play a water running animation instead.
        /// </summary>
        public bool CoffeeFilterSafeToUse { get; set; }

        public bool IsBlocked => (filterPaperState != FilterPaperState.None) && (SpringState == FilterSpringState.None);

        public FilterSpringState SpringState
        {
            get { return springState; }
            set
            {
                if (value == springState)
                {
                    return;
                }

                springState = value;
                if (springState == FilterSpringState.Fixed)
                {
                    springStateFixed.Invoke();
                }
                else
                {
                    springStateLoose.Invoke();
                }
            }
        }

        public bool ContainsWater => fillLevel > 0;

        public void ReceivedCoffeePowder()
        {
            if (filterPaperState != FilterPaperState.None)
            {
                if (coffeePowderInserted == false)
                {
                    coffeeAdded.Invoke();
                    coffeePowderInserted = true;
                    coffeeFillAnimation.gameObject.SetActive(true);
                    coffeeFillAnimation.Play();
                }
            }
        }

        public event EventHandler<FilterFlowEventArgs> WaterPassingThrough;

        void Awake()
        {
            filterPaperState = FilterPaperState.None;
            paperUsed.SetActive(false);
            coffeeFillAnimation.gameObject.SetActive(false);
            if (FilterDelay <= 0)
            {
                FilterDelay = 1;
            }

            waterPlane.UpdateFillLevel(FillLevelNormalized);
            dropzoneParent.SetActive(true);
        }

        void OnEnable()
        {
            if (springPush != null)
            {
                springPush.MaxLimitReached += OnMaxLimitReached;
                springPush.MaxLimitExited += OnMaxLimitExited;
                dropZone.gameObject.SetActive(springPush.AtMaxLimit());
                dropZone.ObjectSnappedToDropZone += OnPaperSnap;
                SpringState = springPush.AtMaxLimit() ? FilterSpringState.Fixed : FilterSpringState.Loose;
            }
            else
            {
                SpringState = FilterSpringState.None;
                dropZone.gameObject.SetActive(true);
                dropZone.ObjectSnappedToDropZone += OnPaperSnap;
            }
        }

        void OnDisable()
        {
            if (springPush != null)
            {
                springPush.MaxLimitReached -= OnMaxLimitReached;
                springPush.MaxLimitExited -= OnMaxLimitExited;
            }

            if (dropZone != null)
            {
                dropZone.ObjectSnappedToDropZone -= OnPaperSnap;
            }
        }

        void Update()
        {
            if (filterPaperState == FilterPaperState.None)
            {
                return;
            }

            RaycastHit hit;
            var direction = (dropTarget.transform.position - raycastSource.transform.position).normalized;
            var dotResult = Vector3.Dot(direction, Vector3.down);
            var isDroppingContents = dotResult > 0.8f;

            if (Physics.Raycast(raycastSource.transform.position, Vector3.down, out hit, 50))
            {
                if (isDroppingContents)
                {
                    StartCoroutine(SpawnFilterPaperGarbage());
                    drop.Invoke();
                }
            }
        }

        IEnumerator SpawnFilterPaperGarbage()
        {
            Debug.Log("Spawn Object");
            paperUsed.SetActive(false);
            paperUsedWet.SetActive(false);
            coffeePowderInserted = false;
            coffeeFillAnimation.gameObject.SetActive(false);
            filterPaperState = FilterPaperState.None;
            filterPaperRemoved.Invoke();

            Instantiate(spawnedObj, dropTarget.transform.position, dropTarget.transform.rotation);
            yield return new WaitForEndOfFrame();
            dropzoneParent.gameObject.SetActive(true);
            stopDrop.Invoke();
        }

        void OnMaxLimitReached(object sender, ControllableEventArgs e)
        {
            SpringState = FilterSpringState.Fixed;
            dropZone.gameObject.SetActive(true);
        }

        void OnMaxLimitExited(object sender, ControllableEventArgs e)
        {
            SpringState = FilterSpringState.Loose;
            dropZone.gameObject.SetActive(false);
        }

        public void OnPaperSnap(object sender, SnapDropZoneEventArgs snapDropZoneEventArgs)
        {
            filterPaperState = FilterPaperState.Clean;
            filterPaperInserted.Invoke();
            var snappedObject = dropZone.GetCurrentSnappedObject();
            if (snappedObject != null)
            {
                Debug.Log("Snapped object removed");
                Destroy(snappedObject);
            }

            dropzoneParent.SetActive(false);
            paperUsed.SetActive(true);
        }

        public void ReceiveLiquidTick(float flow)
        {
            coffeeFillAnimation.gameObject.SetActive(false);

            if (filterPaperState == FilterPaperState.Clean)
            {
                filterPaperState = FilterPaperState.Dirty;
                paperUsed.SetActive(false);
                paperUsedWet.SetActive(true);
            }

            FillLevel += flow;
            if (FillLevel > MaximumFillLevel)
            {
                Overflowing.Invoke();
            }

            if (tickCoroutine == null)
            {
                tickCoroutine = WaterPassingThroughFilter();
                StartCoroutine(tickCoroutine);
            }
        }

        public IEnumerator WaterPassingThroughFilter()
        {
            var timer = new ActivationTimer();
            waterPassingStarted.Invoke();

            // Delay the first tick for a certain amount of time.
            // The delay depends on whether there is filter paper in the pan.
            var initialDelay = filterPaperState != FilterPaperState.None ? FilterDelay : 0;
            // Debug.Log("Waiting for first filter water passing through: " + initialDelay);
            yield return new WaitForSeconds(initialDelay);
            timer.Start();

            // After the initial delay, send out the water.
            while (FillLevel > 0)
            {
                yield return new WaitForEndOfFrame();
                timer.Update();

                var waterFlowingOut = ComputeWaterFlowingOut(timer);
                FillLevel -= waterFlowingOut;

                WaterPassingThrough?.Invoke(this, new FilterFlowEventArgs(waterFlowingOut, coffeePowderInserted));
                UpdateWaterEmitter();
            }

            FillLevel = 0;
            waterParticleSystem.Stop();
            tickCoroutine = null;
            waterPassingFinished.Invoke();
        }

        void UpdateWaterEmitter()
        {
            if (CoffeeFilterSafeToUse)
            {
                if (!waterParticleSystem.isStopped)
                {
                    waterParticleSystem.Stop();
                }
            }
            else
            {
                if (waterParticleSystem.isStopped)
                {
                    waterParticleSystem.Play();
                }
            }
        }

        float ComputeWaterFlowingOut(ActivationTimer timer)
        {
            float waterFlowingOut;
            if (IsBlocked)
            {
                waterFlowingOut = timer.DeltaTime * BlockedFlowRate;
            }
            else
            {
                waterFlowingOut = timer.DeltaTime * FlowRate;
            }

            waterFlowingOut = Math.Min(FillLevel, waterFlowingOut);
            return waterFlowingOut;
        }

#pragma warning disable 649
        [BoxGroup("Filter State")] [SerializeField] [ReadOnly]
        FilterPaperState filterPaperState;

        [BoxGroup("Filter State")] [SerializeField] [ReadOnly]
        bool coffeePowderInserted;

        [BoxGroup("Filter State")] [SerializeField] [ReadOnly]
        FilterSpringState springState;

        [BoxGroup("Water Flow")] public float MaximumFillLevel;

        [BoxGroup("Water Flow")] [SerializeField]
        float fillLevel;

        [BoxGroup("Water Flow")] [Tooltip("How long (in seconds) does it take to pass the water through the filter when filter paper is inserted")]
        public float FilterDelay;

        [BoxGroup("Water Flow")]
        [Tooltip(
            "How much water can flow through the filter when it is not blocked. This should be the same amount as the coffee maker's water production or you will get an overflow.")]
        public float FlowRate;

        [BoxGroup("Water Flow")] [Tooltip("How much water can flow through the filter when it is blocked")]
        public float BlockedFlowRate;

        [BoxGroup("Internal Components")] [SerializeField]
        ParticleSystem waterParticleSystem;

        [BoxGroup("Internal Components")] [SerializeField]
        VRTK_ArtificialPusher springPush;

        [BoxGroup("Internal Components")] [SerializeField]
        GameObject spawnedObj;

        [BoxGroup("Internal Components")] [SerializeField]
        GameObject dropTarget;

        [BoxGroup("Internal Components")] [SerializeField]
        GameObject dropzoneParent;

        [BoxGroup("Internal Components")] [SerializeField]
        VRTK_SnapDropZone dropZone;

        [BoxGroup("Internal Components")] [SerializeField]
        GameObject raycastSource;

        [Tooltip("The prefab for instantiating a paper filter after it has been inserted (but before being used).")] [BoxGroup("Internal Components")] [SerializeField]
        GameObject paperUsed;

        [Tooltip("The prefab for instantiating a paper filter after it has had water run through.")] [BoxGroup("Internal Components")] [SerializeField]
        GameObject paperUsedWet;

        [BoxGroup("Internal Components")] [SerializeField]
        Animation coffeeFillAnimation;

        [BoxGroup("Internal Components")] [SerializeField]
        FillLevelAnimationController waterPlane;

        [BoxGroup("Events")] [Tooltip("Invoked when there is more liquid in the filter than the filter can hold.")]
        public UnityEvent Overflowing;

        [BoxGroup("Events")] [Tooltip("Invoked when the filter will drop out of the pan.")]
        public UnityEvent drop;

        [BoxGroup("Events")] [Tooltip("Invoked when the filter has left the pan.")]
        public UnityEvent stopDrop;

        [BoxGroup("Events")] [Tooltip("Invoked when the spring state changed to fixed.")]
        public UnityEvent springStateFixed;

        [BoxGroup("Events")] [Tooltip("Invoked when the spring state changed to loose.")]
        public UnityEvent springStateLoose;

        [BoxGroup("Events")] [Tooltip("Invoked when filter paper has been inserted.")]
        public UnityEvent filterPaperInserted;

        [BoxGroup("Events")] [Tooltip("Invoked when filter paper has been inserted.")]
        public UnityEvent filterPaperRemoved;

        [BoxGroup("Events")] [Tooltip("Invoked when coffee has been added.")]
        public UnityEvent coffeeAdded;

        [BoxGroup("Events")] [Tooltip("Invoked when water starts to leave the filter.")]
        public UnityEvent waterPassingStarted;

        [BoxGroup("Events")] [Tooltip("Invoked when water finished to leave the filter.")]
        public UnityEvent waterPassingFinished;
#pragma warning restore 649
    }
}