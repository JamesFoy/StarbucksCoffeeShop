using Plugins.XStateMachine;
using XStateMachine.Behaviours;
using XStateMachine.States;

namespace VRKitchenSimulator.States.Parts
{
    public class FilterPanBinding : BindingBase<FilterPanStates>
    {
        public TriggerState FilledTarget;
        public BaseState BrewingState;
        StateBinding fillBinding;
        StateBinding brewingBinding;

        protected override void Awake()
        {
            base.Awake();
            var runtimeGraph = this.GetComponentInParent<RuntimeGraphHolder>();
            if (runtimeGraph != null)
            {
                FilledTarget = runtimeGraph.RuntimeGraph.FindRuntimeState(FilledTarget);
                BrewingState = runtimeGraph.RuntimeGraph.FindRuntimeState(BrewingState);
            }
        }

        protected override void OnDeactivate(FilterPanStates state)
        {
            fillBinding?.Dispose();
            brewingBinding?.Dispose();
        }

        protected override void OnActivate(FilterPanStates state)
        {
            if (FilledTarget != null)
            {
                fillBinding = new StateBinding(state.FilledRuntime, FilledTarget);
            }

            if (BrewingState != null)
            {
                brewingBinding = new StateBinding(BrewingState, state.InUseRuntime);
            }
        }

        public void DoneBrewingCoffee()
        {
            SourceRuntime?.DoneBrewingCoffee();
        }
    }
}