using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using XStateMachine.Behaviours;
using XStateMachine.States;

namespace VRKitchenSimulator.States
{
    public class DelayedStateDeactivationTrigger : MonoBehaviour
    {
        [Tooltip("How long after the machine has switched off before the activated state is cleared.")]
        public float CoolDownTime;

        bool activated;

        bool active;
        float deactivatedTime;

        public BaseState SourceState;

        public UnityEvent BecameActive;
        public UnityEvent BecameInactive;

        public DelayedStateDeactivationTrigger()
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
        }

        void OnChanged()
        {
            active = SourceState.IsActive;
            if (active == false)
            {
                deactivatedTime = Time.time;
                StartCoroutine(ToggleCoolDown(deactivatedTime));
            }
            else
            {
                Activated = true;
            }
        }

        IEnumerator ToggleCoolDown(float startTime)
        {
            yield return new WaitForSeconds(CoolDownTime);
            if (Mathf.Abs(deactivatedTime - startTime) <= 0.1)
            {
                Activated = false;
            }
        }
    }
}