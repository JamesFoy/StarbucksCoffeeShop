using Plugins.XStateMachine;
using XStateMachine.Behaviours;
using XStateMachine.States;

namespace VRKitchenSimulator.States.Parts
{
    public class DecanterBinding : BindingBase<DecanterStates>
    {
        public TriggerState FilledTarget;
        public TriggerState OpenTarget;

        StateBinding filledBinding;
        StateBinding openBinding;

        protected override void Awake()
        {
            base.Awake();
            var runtimeGraph = this.GetComponentInParent<RuntimeGraphHolder>();
            if (runtimeGraph != null)
            {
                FilledTarget = runtimeGraph.RuntimeGraph.FindRuntimeState(FilledTarget);
                OpenTarget = runtimeGraph.RuntimeGraph.FindRuntimeState(OpenTarget);
            }
        }

        protected override void OnDeactivate(DecanterStates state)
        {
            filledBinding?.Dispose();
            openBinding?.Dispose();
        }

        protected override void OnActivate(DecanterStates state)
        {
            if (FilledTarget != null)
            {
                filledBinding = new StateBinding(state.FilledRuntime, FilledTarget);
            }

            if (OpenTarget != null)
            {
                openBinding = new StateBinding(state.OpenRuntime, OpenTarget);
            }
        }

        public void DoneBrewingCoffee()
        {
            SourceRuntime?.DoneBrewingCoffee();
        }
    }
}