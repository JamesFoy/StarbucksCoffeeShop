using System.Collections;
using System.Collections.Generic;
using RoboRyanTron.SceneReference;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK.Controllables;

namespace VRKitchenSimulator.Prototypes
{
    public class ResetScene : MonoBehaviour
    {
        [SerializeField] float delay;
        VRTK_BaseControllable controllable;

        void OnEnable()
        {
            controllable = controllable == null ? GetComponentInChildren<VRTK_BaseControllable>() : controllable;
            if (controllable != null)
            {
                controllable.MaxLimitReached += OnMaxLimitReached;
                controllable.MaxLimitExited += OnMaxLimitExited;
                buttonActive = controllable.AtMaxLimit();
            }
        }

        void OnDisable()
        {
            if (controllable != null)
            {
                controllable.MaxLimitReached -= OnMaxLimitReached;
                controllable.MaxLimitExited -= OnMaxLimitExited;
            }
        }

        void OnMaxLimitExited(object sender, ControllableEventArgs e)
        {
            if (buttonActive)
            {
                buttonActive = false;
            }
        }

        void OnMaxLimitReached(object sender, ControllableEventArgs e)
        {
            Debug.Log("Button Pushed");
            if (buttonActive != true)
            {
                ActivateReset();
            }
        }

        public void ActivateReset()
        {
            if (delay > 0)
            {
                StartCoroutine(ActivateLater());
                return;
            }

            float timeScale = Time.timeScale;
            Time.timeScale = 0;
            try
            {
                SceneManager.LoadScene(scene.SceneName, LoadSceneMode.Single);
                buttonActive = true;
            }
            finally
            {
                Time.timeScale = timeScale;
            }
        }

        IEnumerator ActivateLater()
        {
            yield return new WaitForSeconds(delay);

            Debug.Log("Reset Scene started: " + scene.SceneName);
            float timeScale = Time.timeScale;
            Time.timeScale = 0;
            try
            {
                SceneManager.LoadScene(scene.SceneName, LoadSceneMode.Single);
                buttonActive = true;
            }
            finally
            {
                Time.timeScale = timeScale;
            }
            Debug.Log("Reset Scene Ended: " + scene.SceneName);
        }

#pragma warning disable 649
        [SerializeField] bool buttonActive;
        [SerializeField] SceneReference scene;
#pragma warning restore 649
    }
}