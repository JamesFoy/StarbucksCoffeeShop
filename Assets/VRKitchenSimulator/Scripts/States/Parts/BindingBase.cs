using UnityEngine;
using XStateMachine.Behaviours;
using XStateMachine.States;

namespace VRKitchenSimulator.States.Parts
{
    public abstract class BindingBase<TSourceState> : MonoBehaviour where TSourceState : MonoBehaviour
    {
        public TSourceState Source;
        public TriggerState ObjectInserted;

        TSourceState sourceRuntime;

        public TSourceState SourceRuntime
        {
            get { return sourceRuntime; }
            set
            {
                if (sourceRuntime == value)
                {
                    return;
                }

                if (enabled && (sourceRuntime != null))
                {
                    ObjectInserted?.Deactivate();
                    OnDeactivate(sourceRuntime);
                }

                sourceRuntime = value;

                if (enabled && (sourceRuntime != null))
                {
                    ObjectInserted?.Activate();
                    OnActivate(sourceRuntime);
                }
            }
        }

        protected virtual void Awake()
        {
            var runtimeGraph = this.GetComponentInParent<RuntimeGraphHolder>();
            if (runtimeGraph != null)
            {
                ObjectInserted = runtimeGraph.RuntimeGraph.FindRuntimeState(ObjectInserted);
            }
            else
            {
                Debug.LogWarning("No runtime graph for " + name);
            }
        }

        void Start()
        {
            SourceRuntime = Source;
        }

        protected virtual void OnEnable()
        {
            if (SourceRuntime != null)
            {
                ObjectInserted?.Activate();
            }
        }

        protected virtual void OnDisable()
        {
            if (SourceRuntime != null)
            {
                ObjectInserted?.Deactivate();
            }
        }

        protected virtual void OnDeactivate(TSourceState state)
        {
        }

        protected virtual void OnActivate(TSourceState state)
        {
        }
    }
}