namespace XStateMachine.States
{
    public class NotFilterState : FilterState
    {
        protected override void CheckActivation()
        {
            DoActivate(!ComputeAnyOfActivationState());
        }
    }
}