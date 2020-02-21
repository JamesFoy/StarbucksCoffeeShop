using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using XNode;

namespace XStateMachine.States
{
    public abstract class BaseState : Node
    {
        [NonSerialized] public readonly UnityEvent Activated;
        [NonSerialized] public readonly UnityEvent Changed;
        [NonSerialized] public readonly UnityEvent Deactivated;
        [SerializeField] bool activeOnStart;
        bool isActive;
        string logName;
        [Output] public StateReference OutputPort;

        [NonSerialized] bool reentrancyGuard;

        public BaseState()
        {
            Activated = new UnityEvent();
            Deactivated = new UnityEvent();
            Changed = new UnityEvent();
        }

        [ShowNativeProperty]
        public bool IsActive
        {
            get { return isActive; }
            private set
            {
                if (value == isActive)
                {
                    DebugLog("No Change for " + name);
                    return;
                }

                isActive = value;
                if (reentrancyGuard)
                {
                    LogError("Infinite loop in event handlers, skipping event notification.");
                    return;
                }

                try
                {
                    reentrancyGuard = true;
                    DebugLog("Notify Listeners on " + name);

                    Changed?.Invoke();
                    if (isActive)
                    {
                        Activated?.Invoke();
                    }
                    else
                    {
                        Deactivated?.Invoke();
                    }
                }
                finally
                {
                    reentrancyGuard = false;
                }
            }
        }

        public IEnumerable<NodePort> GetInputsForField(string fieldName)
        {
            return Inputs.Where(p => p.fieldName == fieldName);
        }

        public IEnumerable<T> GetConnectionsForField<T>(string fieldName) where T : Node
        {
            var en = Inputs.Where(p => p.fieldName == fieldName);
            foreach (var c in en)
            {
                foreach (var target in c.GetConnections())
                {
                    var node = target.node as T;
                    if (node != null)
                    {
                        yield return node;
                    }
                }
            }
        }

        public override void Init()
        {
            IsActive = activeOnStart;
            RefreshConnections();
        }

        protected virtual void OnDisable()
        {
            Activated.RemoveAllListeners();
            Deactivated.RemoveAllListeners();
            Changed.RemoveAllListeners();
        }

        protected void DoActivate(bool active)
        {
            DebugLog("Event " + name + " triggered to " + active);
            if (active)
            {
                IsActive = true;
            }
            else if (IsActive)
            {
                IsActive = false;
            }
        }

        public override object GetValue(NodePort port)
        {
            // not used ..
            return null;
        }

        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            RefreshConnections();
        }

        public override void OnRemoveConnection(NodePort port)
        {
            RefreshConnections();
        }

        protected virtual void RefreshConnections()
        {
        }

        protected void LogError(string message)
        {
            if (logName == null)
            {
                logName = name;
            }

            Debug.LogError("[" + logName + "]: " + message);
        }

        protected void DebugLog(string message)
        {
            if (logName == null)
            {
                logName = name;
            }

            // Debug.Log("[" + logName + "]: " + message);
        }

        [Serializable]
        public class StateReference
        {
        }
    }
}