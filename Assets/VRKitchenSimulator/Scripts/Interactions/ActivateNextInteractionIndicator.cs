using UnityEngine;
using UnityTutorialSystem.Events;
using UnityTutorialSystem.Helpers;

namespace VRKitchenSimulator.Interactions
{
    public class ActivateNextInteractionIndicator : MonoBehaviour
    {
        const string IsActiveParameterName = "IsActive";
        [SerializeField] bool debug;
        NextEventSelector activeSelector;

        void Awake()
        {
            if ((sourceEventSelector == null) && autoPopulate)
            {
                activeSelector = GetComponentInParent<NextEventSelector>();
            }
            else
            {
                activeSelector = sourceEventSelector;
            }

            if (target != null)
            {
                target.SetActive(false);
            }
        }

        void OnEnable()
        {
            if (activeSelector != null)
            {
                activeSelector.EnableForNextMessage.AddListener(OnEnableForNext);
                activeSelector.DisableForNextMessage.AddListener(OnDisableForNext);
            }
        }

        void OnDisable()
        {
            if (!ReferenceEquals(activeSelector, null))
            {
                activeSelector.EnableForNextMessage.RemoveListener(OnEnableForNext);
                activeSelector.DisableForNextMessage.RemoveListener(OnDisableForNext);
            }
        }

        public void OnEnableForNext()
        {
            if (debug)
            {
                Debug.Log("Activate notification for " + gameObject.Path());
            }

            animator.SetBool(IsActiveParameterName, true);
        }

        public void OnDisableForNext()
        {
            if (debug)
            {
                Debug.Log("Deactivate notification for " + gameObject.Path());
            }

            animator.SetBool(IsActiveParameterName, false);
        }

        public void OnAnimationFinished()
        {
        }
#pragma warning disable 649
        [SerializeField] GameObject target;
        [SerializeField] NextEventSelector sourceEventSelector;
        [SerializeField] bool autoPopulate;
        [SerializeField] Animator animator;
#pragma warning restore 649
    }
}