using NaughtyAttributes;
using UnityEngine;
using XStateMachine.Behaviours;
using XStateMachine.States;

namespace VRKitchenSimulator.States
{
    /// <summary>
    ///     A generic interface behaviour. This is used by the coffee maker scripts
    ///     to lookup the relevant states of the components that are used when making
    ///     coffee.
    ///     This behaviour should be added to the coffee pots and the filter pans.
    /// </summary>
    [RequireComponent(typeof(RuntimeGraphHolder))]
    public class InUseBehaviour : MonoBehaviour
    {
        [ResizableTextArea]
        public string Documentation;

        public BaseState InUseState;

        void Awake()
        {
            var runtimeGraph = this.GetComponentInParent<RuntimeGraphHolder>();
            if (runtimeGraph != null)
            {
                InUseState = runtimeGraph.RuntimeGraph.FindRuntimeState(InUseState);
            }
        }
    }
}