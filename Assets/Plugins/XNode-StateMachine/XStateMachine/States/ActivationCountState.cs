using NaughtyAttributes;

namespace XStateMachine.States
{
    public class ActivationCountState : FilterState
    {
        public int ActivationsToTrigger;

        [ShowNativeProperty]
        public int ActivationCount { get; protected set; }

        [ShowNativeProperty]
        public int TriggerCount { get; protected set; }

        public override void Init()
        {
            base.Init();
            ActivationCount = 0;
            TriggerCount = 0;
        }

        protected override void CheckActivation()
        {
            var state = ComputeAnyOfActivationState();
            if (state)
            {
                ActivationCount += 1;
                if (ActivationsToTrigger <= 0 ||
                    ActivationCount % ActivationsToTrigger == 0)
                {
                    // only activate if there is either no trigger count set or
                    // if we reached the desired number of activations.
                    DoActivate(true);
                    return;
                }
            }

            DoActivate(false);
        }
    }
}