using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using VRKitchenSimulator.Helpers;
using VRTK.Controllables;
using VRTK.Controllables.ArtificialBased;

namespace VRKitchenSimulator.Prototypes
{
    public class FlaskBehaviour : MonoBehaviour
    {
        public bool LidOpen => lidOpen;

        public float FillLevel
        {
            get { return fillLevel; }
            set
            {
                fillLevel = value;
                if (fillLevel > maximumFillLevel)
                {
                    overflowing.Invoke();
                }

                if (waterLevel != null)
                {
                    waterLevel.UpdateFillLevel(NormalizedFillLevel);
                }
            }
        }

        public float NormalizedFillLevel => maximumFillLevel <= 0 ? 0 : fillLevel / maximumFillLevel;
        public bool IsOverflowing => FillLevel > maximumFillLevel;
        public UnityEvent LidOpened => lidOpened;
        public UnityEvent LidClosed => lidClosed;
        public UnityEvent Overflowing => overflowing;


        void Start()
        {
            lidHinge = GetComponentInChildren<VRTK_ArtificialRotator>();

            if (lidHinge != null)
            {
                lidHinge.MinLimitReached += OnMinLimitReached;
                lidHinge.MinLimitExited += OnMinLimitExited;
                lidOpen = lidHinge.AtMaxLimit();
            }
        }

        void OnMinLimitExited(object sender, ControllableEventArgs e)
        {
            if (lidOpen)
            {
                lidOpen = false;
                lidClosed.Invoke();
            }
        }

        void OnMinLimitReached(object sender, ControllableEventArgs e)
        {
            if (lidOpen != true)
            {
                lidOpen = true;
                lidOpened.Invoke();
            }
        }

        public void ReceiveLiquidTick(float waterLevel, bool isCoffee)
        {
            FillLevel += waterLevel;
            this.isCoffee |= isCoffee;
        }
#pragma warning disable 649
        [Tooltip("How much liquid can possibly fit into this flask")]
        [SerializeField]
        float maximumFillLevel;

        [Tooltip("How much liquid is currently in this flask")]
        [SerializeField]
        float fillLevel;

        [Tooltip("Is the lid open?")]
        [SerializeField]
        [ReadOnly]
        bool lidOpen;

        [Tooltip("Does the flask contain coffee? (If water runs without filter that has coffee powder, we get hot water instead.)")]
        [SerializeField]
        [ReadOnly]
        bool isCoffee;

        [BoxGroup("Events")]
        [SerializeField]
        UnityEvent lidOpened;

        [BoxGroup("Events")]
        [SerializeField]
        UnityEvent lidClosed;

        [BoxGroup("Events")]
        [SerializeField]
        UnityEvent overflowing;

        [BoxGroup("Internal Properties")] [SerializeField]
        VRTK_ArtificialRotator lidHinge;

        [BoxGroup("Internal Properties")] [SerializeField]
        FillLevelAnimationController waterLevel;
#pragma warning restore 649
    }
}