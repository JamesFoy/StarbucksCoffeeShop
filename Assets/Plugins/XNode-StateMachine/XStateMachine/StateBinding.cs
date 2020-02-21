using System;
using XStateMachine.States;

namespace Plugins.XStateMachine
{
    public class StateBinding
    {
        public StateBinding(BaseState source, TriggerState target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            Target = target;
            Source = source;

            Source.Changed.AddListener(OnChanged);
            OnChanged();
        }

        public TriggerState Target { get; }
        public BaseState Source { get; }

        void OnChanged()
        {
            if (Source.IsActive)
            {
                Target.Activate();
            }
            else
            {
                Target.Deactivate();
            }
        }

        public void Dispose()
        {
            Source.Changed.RemoveListener(OnChanged);
        }
    }
}