using UnityEngine;
using XStateMachine.Behaviours;
using XStateMachine.States;

namespace VRKitchenSimulator.States.Parts
{
    public class FilterPanStates : MonoBehaviour
    {
        public TriggerState Filled;
        public TriggerState InUse;
        TriggerState filledRuntime;
        TriggerState inUseRuntime;

        public TriggerState FilledRuntime
        {
            get
            {
                if (filledRuntime == null)
                {
                    var runtimeGraph = this.GetComponentInParent<RuntimeGraphHolder>();
                    if (runtimeGraph != null)
                    {
                        filledRuntime = runtimeGraph.RuntimeGraph.FindRuntimeState(Filled);
                    }
                }

                return filledRuntime;
            }
        }

        public TriggerState InUseRuntime
        {
            get
            {
                if (inUseRuntime == null)
                {
                    var runtimeGraph = this.GetComponentInParent<RuntimeGraphHolder>();
                    if (runtimeGraph != null)
                    {
                        inUseRuntime = runtimeGraph.RuntimeGraph.FindRuntimeState(InUse);
                    }
                }

                return inUseRuntime;
            }
        }

        public void DoneBrewingCoffee()
        {
            FilledRuntime?.Deactivate();
        }
    }
}