using System;
using UnityEngine;
using VRTK;

namespace VRKitchenSimulator.VRTK
{
    public class VRTK_SetupListener : MonoBehaviour
    {
        [NonSerialized] bool performDelayedSetup;

        protected virtual void OnEnable()
        {
            if (VRTK_SDKManager.instance != null)
            {
                VRTK_SDKManager.instance.LoadedSetupChanged += OnLoadedSetupChanged;
                OnVRTKSetupChanged();
                performDelayedSetup = false;
            }
            else
            {
                performDelayedSetup = true;
            }
        }

        void OnLoadedSetupChanged(VRTK_SDKManager sender, VRTK_SDKManager.LoadedSetupChangeEventArgs e)
        {
            OnVRTKSetupChanged();
        }

        protected virtual void OnDisable()
        {
            if (VRTK_SDKManager.instance != null)
            {
                VRTK_SDKManager.instance.LoadedSetupChanged -= OnLoadedSetupChanged;
            }
        }

        protected virtual void Update()
        {
            if (performDelayedSetup)
            {
                if (VRTK_SDKManager.instance != null)
                {
                    VRTK_SDKManager.instance.LoadedSetupChanged += OnLoadedSetupChanged;
                    OnVRTKSetupChanged();
                    performDelayedSetup = false;
                }
            }
        }

        protected virtual void OnVRTKSetupChanged()
        {
            var setup = VRTK_SDKManager.instance.loadedSetup;
            if (setup != null)
            {
                if (setup.isValid)
                {
                    OnSDKSetupInitialized();
                }
                else
                {
                    setup.Loaded += OnSetupLoadedEvent;
                }
            }
        }

        void OnSetupLoadedEvent(VRTK_SDKManager sender, VRTK_SDKSetup setup)
        {
            if (setup != null)
            {
                setup.Loaded -= OnSetupLoadedEvent;
                OnSDKSetupInitialized();
            }
        }

        protected virtual void OnSDKSetupInitialized()
        {
        }

        public static GameObject LocateHeadSet()
        {
            var sdkManager = VRTK_SDKManager.instance;
            if (sdkManager != null)
            {
                var loadedSetup = sdkManager.loadedSetup;
                if (loadedSetup != null)
                {
                    Debug.Log("SDK Manager headset is " + loadedSetup.actualHeadset);
                    return loadedSetup.actualHeadset;
                }

                Debug.Log("SDK Manager no loaded headset");
            }
            else
            {
                Debug.Log("No SDK Manager");
            }

            var main = Camera.main;
            if (main != null)
            {
                Debug.Log("Camera");
                return main.gameObject;
            }

            return null;
        }
    }
}