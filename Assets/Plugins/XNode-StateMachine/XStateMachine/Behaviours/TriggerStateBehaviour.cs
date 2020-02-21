using NaughtyAttributes;
using UnityEngine;
using XStateMachine.States;

namespace XStateMachine.Behaviours
{
    /// <summary>
    ///     Trigger script that toggles the target state. Use this from Unity Event handlers.
    /// </summary>
    public class TriggerStateBehaviour : MonoBehaviour
    {
        [ResizableTextArea]
        public string Documentation;

        public TriggerState target;

        void Awake()
        {
            var runtimeGraph = this.GetComponentInParent<RuntimeGraphHolder>();
            if (runtimeGraph != null)
            {
                target = runtimeGraph.RuntimeGraph.FindRuntimeState(target);
            }
        }

        public void DoDeactivate()
        {
            if (target != null)
            {
                target.Deactivate();
            }
        }

        public void DoActivate()
        {
            if (target != null)
            {
                target.Activate();
            }
        }

        public void DoTrigger()
        {
            Debug.Log("Do Trigger Called");
            if (target != null)
            {
                target.Toggle();
            }
        }
    }
}