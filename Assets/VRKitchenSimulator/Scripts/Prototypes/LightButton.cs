using UnityEngine;
using UnityEngine.Events;
using VRTK.Controllables;

namespace VRKitchenSimulator.Prototypes
{
    public class LightButton : MonoBehaviour
    {
        VRTK_BaseControllable controllable;

#pragma warning disable 649
        [SerializeField] UnityEvent ButtonPressed;
#pragma warning restore 649

        void OnEnable()
        {
            controllable = controllable == null ? GetComponentInChildren<VRTK_BaseControllable>() : controllable;

            if (controllable != null)
            {
                controllable.MaxLimitReached += OnMaxLimitReached;
            }
        }

        void OnDisable()
        {
            if (controllable != null)
            {
                controllable.MaxLimitReached -= OnMaxLimitReached;
            }
        }

        void OnMaxLimitReached(object sender, ControllableEventArgs e)
        {
            ButtonPressed.Invoke();
        }
    }
}