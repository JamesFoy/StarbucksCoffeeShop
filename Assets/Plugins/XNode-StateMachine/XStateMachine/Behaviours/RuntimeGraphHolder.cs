using NaughtyAttributes;
using UnityEngine;
using XStateMachine.States;

namespace XStateMachine.Behaviours
{
    public class RuntimeGraphHolder : MonoBehaviour
    {
        [ResizableTextArea]
        public string Documentation;

        public StateGraph Graph;
        RuntimeStateGraph runtimeGraph;

        public RuntimeStateGraph RuntimeGraph
        {
            get
            {
                if (Graph == null)
                {
                    return null;
                }

                if (runtimeGraph == null)
                {
                    runtimeGraph = new RuntimeStateGraph(Graph);
                }

                return runtimeGraph;
            }
        }
    }
}