using System.Collections.Generic;

namespace XStateMachine.States
{
    public class CompoundState : BaseState
    {
        [Input] public StateReference AllOf;
        [Input] public StateReference AnyOf;
        [Input] public StateReference NoneOf;

        public CompoundState()
        {
            AllOfRuntime = new HashSet<BaseState>();
            AnyOfRuntime = new HashSet<BaseState>();
            NoneOfRuntime = new HashSet<BaseState>();
        }

        public HashSet<BaseState> AnyOfRuntime { get; }
        public HashSet<BaseState> AllOfRuntime { get; }
        public HashSet<BaseState> NoneOfRuntime { get; }

        protected override void OnDisable()
        {
            base.OnDisable();
            AnyOfRuntime.Clear();
            AllOfRuntime.Clear();
            NoneOfRuntime.Clear();
        }

        protected override void RefreshConnections()
        {
            AnyOfRuntime.Clear();
            foreach (var state in GetConnectionsForField<BaseState>(nameof(AnyOf)))
            {
                DoAdd(AnyOfRuntime, state);
            }

            AllOfRuntime.Clear();
            foreach (var state in GetConnectionsForField<BaseState>(nameof(AllOf)))
            {
                DoAdd(AllOfRuntime, state);
            }

            NoneOfRuntime.Clear();
            foreach (var state in GetConnectionsForField<BaseState>(nameof(NoneOf)))
            {
                DoAdd(NoneOfRuntime, state);
            }

            DebugLog("Refreshing connections for " + name + " -> AnyOf: " +
                     AnyOfRuntime.Count + ", AllOf: " +
                     AllOfRuntime.Count + ", NoneOf: " + NoneOfRuntime.Count);
            CheckActivation();
        }

        void DoAdd(ISet<BaseState> target, BaseState state)
        {
            DebugLog("CompoundState: " + name + " Added Dependent State " + state);
            state.Changed.AddListener(CheckActivation);
            target.Add(state);
        }

        void CheckActivation()
        {
            var state = ComputeActivationState();
            DebugLog("CompoundState: " + name + " changed to " + state);
            DoActivate(state);
        }

        bool ComputeActivationState()
        {
            foreach (var state in NoneOfRuntime)
            {
                if (state.IsActive)
                {
                    return false;
                }
            }

            foreach (var state in AllOfRuntime)
            {
                if (!state.IsActive)
                {
                    return false;
                }
            }

            foreach (var state in AnyOfRuntime)
            {
                if (state.IsActive)
                {
                    return true;
                }
            }

            return AllOfRuntime.Count > 0 || NoneOfRuntime.Count > 0;
        }
    }
}