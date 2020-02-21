using System;
using UnityEngine;
using VRTK;
using VRTK.Controllables;
using VRTK.Controllables.PhysicsBased;

namespace VRKitchenSimulator.Prototypes
{
    [Obsolete]
    public class FilterInteraction : MonoBehaviour
    {
        VRTK_PhysicsSlider slider;
        VRTK_BaseControllable controllable;
        bool ignoreNextEvent;

#pragma warning disable 649
        [SerializeField]
        VRTK_SnapDropZone dropZone;
#pragma warning restore 649

        public void Start()
        {
            slider = gameObject.GetComponent<VRTK_PhysicsSlider>();
            controllable = controllable == null ? GetComponent<VRTK_BaseControllable>() : controllable;
            controllable.MaxLimitReached += OnMaxLimitReached;
            controllable.MaxLimitExited += OnMaxLimitExited;
            dropZone.enabled = false;
            ignoreNextEvent = false;
        }

        void OnMaxLimitExited(object sender, ControllableEventArgs e)
        {
            slider.enabled = true;
            dropZone.enabled = true;
        }

        void OnMaxLimitReached(object sender, ControllableEventArgs e)
        {
            if (ignoreNextEvent)
            {
                ignoreNextEvent = false;
                return;
            }

            slider.enabled = false;
            dropZone.enabled = true;
        }

        public void OnSnap()
        {
            //ignoreNextEvent = true;
            slider.enabled = true;
            slider.SetValue(0);
            dropZone.enabled = false;
        }
    }
}