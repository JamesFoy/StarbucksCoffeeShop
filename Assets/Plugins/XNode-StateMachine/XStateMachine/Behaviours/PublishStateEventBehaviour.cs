using UnityEngine;
using UnityEngine.Events;
using XStateMachine.States;

namespace XStateMachine.Behaviours
{
    public class PublishStateEventBehaviour : MonoBehaviour
    {
        public UnityEvent Activated;
        public UnityEvent Changed;
        public UnityEvent Deactivated;
        public BaseState State;

        public PublishStateEventBehaviour()
        {
            Activated = new UnityEvent();
            Deactivated = new UnityEvent();
            Changed = new UnityEvent();
        }

        void Awake()
        {
            var runtimeGraph = this.GetComponentInParent<RuntimeGraphHolder>();
            if (runtimeGraph != null)
            {
                State = runtimeGraph.RuntimeGraph.FindRuntimeState(State);
            }
        }

        void OnEnable()
        {
            if (State != null)
            {
                DebugLog("PublishState for : " + State.name);

                State.Activated.AddListener(OnActivated);
                State.Deactivated.AddListener(OnDeactivated);
                State.Changed.AddListener(OnChanged);

                OnChanged();
                if (State.IsActive)
                {
                    OnActivated();
                }
                else
                {
                    OnDeactivated();
                }
            }
        }

        void OnChanged()
        {
            DebugLog("PublishState Change: " + State.name);
            Changed?.Invoke();
        }

        void OnDeactivated()
        {
            DebugLog("PublishState Deactivate: " + State.name);
            Deactivated?.Invoke();
        }

        void OnActivated()
        {
            DebugLog("PublishState Activate: " + State.name);
            Activated?.Invoke();
        }

        void DebugLog(string message)
        {
            // Debug.Log(message);
        }
    }
}