using System;
using UnityEditor;
using XNodeEditor;
using XStateMachine.States;

namespace Plugins.Editor.XStateMachine.States
{
    [CustomNodeGraphEditor(typeof(StateGraph))]
    public class StateGraphEditor : NodeGraphEditor
    {
        public override string GetNodeMenuName(Type type)
        {
            if (typeof(BaseState).IsAssignableFrom(type))
            {
                return ObjectNames.NicifyVariableName(type.Name);
            }

            return null;
        }
    }
}