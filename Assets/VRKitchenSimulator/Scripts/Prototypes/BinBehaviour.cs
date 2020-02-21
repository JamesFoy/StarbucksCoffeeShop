using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using VRTK.Controllables;

namespace VRKitchenSimulator.Prototypes
{
    public class BinBehaviour : MonoBehaviour
    {
        [ShowNonSerializedField]
        bool binOpen;

        public UnityEvent binOpened;
        public UnityEvent binClosed;

        VRTK_BaseControllable controllable;

        // Use this for initialization
        void Start()
        {
            controllable = controllable == null ? GetComponentInChildren<VRTK_BaseControllable>() : controllable;

            controllable.MinLimitReached += OnMinLimitReached;
            controllable.MinLimitExited += OnMinLimitExited;
            binOpen = controllable.AtMinLimit();
        }

        void OnMinLimitExited(object sender, ControllableEventArgs e)
        {
            if (binOpen)
            {
                binOpen = false;
                binClosed.Invoke();
            }
        }

        void OnMinLimitReached(object sender, ControllableEventArgs e)
        {
            Debug.Log("Lid Closed");
            if (binOpen != true)
            {
                binOpen = true;
                binOpened.Invoke();
            }
        }
    }
}