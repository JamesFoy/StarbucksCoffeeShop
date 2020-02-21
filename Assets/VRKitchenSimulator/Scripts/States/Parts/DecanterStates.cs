using UnityEngine;
using XStateMachine.Behaviours;
using XStateMachine.States;

namespace VRKitchenSimulator.States.Parts
{
    public class DecanterStates : MonoBehaviour
    {
        public TriggerState Filled;
        public BaseState Open;

        TriggerState filledRuntime;
        BaseState openRuntime;

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

        public BaseState OpenRuntime
        {
            get
            {
                if (openRuntime == null)
                {
                    var runtimeGraph = this.GetComponentInParent<RuntimeGraphHolder>();
                    if (runtimeGraph != null)
                    {
                        openRuntime = runtimeGraph.RuntimeGraph.FindRuntimeState(Open);
                    }
                }

                return openRuntime;
            }
        }

        public void DoneBrewingCoffee()
        {
            FilledRuntime?.Activate();
        }
    }
}