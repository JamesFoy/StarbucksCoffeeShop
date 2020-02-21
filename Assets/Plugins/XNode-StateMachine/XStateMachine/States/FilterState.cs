using System.Collections.Generic;

namespace XStateMachine.States
{
    public abstract class FilterState : BaseState
    {
        [Input] public StateReference Input;

        protected FilterState()
        {
            AnyOfRuntime = new HashSet<BaseState>();
        }

        public HashSet<BaseState> AnyOfRuntime { get; }

        protected override void OnDisable()
        {
            base.OnDisable();
            AnyOfRuntime.Clear();
        }

        protected override void RefreshConnections()
        {
            AnyOfRuntime.Clear();
            foreach (var state in GetConnectionsForField<BaseState>(nameof(Input)))
            {
                DoAdd(AnyOfRuntime, state);
            }

            CheckActivation();
        }

        protected abstract void CheckActivation();

        void DoAdd(ISet<BaseState> target, BaseState state)
        {
            state.Activated.AddListener(CheckActivation);
            target.Add(state);
        }

        protected bool ComputeAnyOfActivationState()
        {
            foreach (var state in AnyOfRuntime)
            {
                if (state.IsActive)
                {
                    return true;
                }
            }

            return false;
        }

        protected bool ComputeAllOfActivationState()
        {
            if (AnyOfRuntime.Count == 0)
            {
                return false;
            }

            foreach (var state in AnyOfRuntime)
            {
                if (!state.IsActive)
                {
                    return false;
                }
            }

            return true;
        }
    }
}