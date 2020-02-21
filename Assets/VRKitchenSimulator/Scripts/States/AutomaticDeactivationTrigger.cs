using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using XStateMachine.Behaviours;
using XStateMachine.States;

namespace VRKitchenSimulator.States
{
    public class AutomaticDeactivationTrigger : MonoBehaviour
    {
        [Tooltip("How long after the state has activate before state is automatically deactivated again.")]
        public float CoolDownTime;

        bool activated;

        bool active;
        float deactivatedTime;

        public BaseState SourceState;

        public UnityEvent BecameActive;
        public UnityEvent BecameInactive;

        public AutomaticDeactivationTrigger()
        {
            BecameInactive = new UnityEvent();
            BecameActive = new UnityEvent();
        }

        [ShowNativeProperty]
        public bool Activated
        {
            get { return activated; }
            set
            {
                if (activated == value)
                {
                    return;
                }

                activated = value;
                if (activated)
                {
                    BecameActive?.Invoke();
                }
                else
                {
                    BecameInactive?.Invoke();
                }
            }
        }

        void Awake()
        {
            var runtimeGraph = this.GetComponentInParent<RuntimeGraphHolder>();
            if (runtimeGraph != null)
            {
                SourceState = runtimeGraph.RuntimeGraph.FindRuntimeState(SourceState);
            }
        }

        void OnEnable()
        {
            if (SourceState == null)
            {
                return;
            }

            SourceState.Changed.AddListener(OnChanged);
            OnChanged();
        }

        void OnChanged()
        {
            if (SourceState.IsActive)
            {
                Debug.Log("Automatic Deactivation: Detected Activation");
                deactivatedTime = Time.time;
                StartCoroutine(ToggleCoolDown(deactivatedTime));
                Activated = true;
            }
            else
            {
                // Debug.Log("Automatic Deactivation: Detected Deactivation");
                Activated = false;
            }
        }

        IEnumerator ToggleCoolDown(float startTime)
        {
            yield return new WaitForSeconds(CoolDownTime);
            if (Mathf.Abs(deactivatedTime - startTime) <= 0.1)
            {
                Debug.Log("Automatic Deactivation: Performing Deactivation");
                Activated = false;
            }
            else
            {
                Debug.Log("Automatic Deactivation: Detected Activation");
            }
        }
    }
}