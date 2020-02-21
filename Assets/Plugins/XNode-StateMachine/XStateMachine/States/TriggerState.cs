namespace XStateMachine.States
{
    public class TriggerState : BaseState
    {
        public void Activate()
        {
            DoActivate(true);
        }

        public void Deactivate()
        {
            DoActivate(false);
        }

        public void Toggle()
        {
            DoActivate(!IsActive);
        }
    }
}