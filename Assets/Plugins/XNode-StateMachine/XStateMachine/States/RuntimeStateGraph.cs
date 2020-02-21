using System;
using UnityEngine;
using XNode;

namespace XStateMachine.States
{
    public class RuntimeStateGraph
    {
        readonly StateGraph baseGraph;
        readonly StateGraph runtimeGraph;

        public RuntimeStateGraph(StateGraph baseGraph)
        {
            if (baseGraph == null)
            {
                throw new ArgumentNullException(nameof(baseGraph));
            }

            this.baseGraph = baseGraph;
            runtimeGraph = (StateGraph) baseGraph.Copy();
        }

        public TNode FindRuntimeState<TNode>(TNode node) where TNode : Node
        {
            if (node == null)
            {
                return null;
            }

            var nodeIdx = baseGraph.nodes.IndexOf(node);
            if (nodeIdx == -1)
            {
                Debug.LogError("Unable to locate node " + node.name + " in runtime graph " + baseGraph.name);
                return node;
            }

            return (TNode) runtimeGraph.nodes[nodeIdx];
        }
    }
}