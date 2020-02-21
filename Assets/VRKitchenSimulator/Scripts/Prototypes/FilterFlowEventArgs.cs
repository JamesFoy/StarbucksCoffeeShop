namespace VRKitchenSimulator.Prototypes
{
    public struct FilterFlowEventArgs
    {
        public readonly float WaterLevel;
        public bool HasCoffeePowder;

        public FilterFlowEventArgs(float waterLevel, bool hasCoffeePowder)
        {
            WaterLevel = waterLevel;
            HasCoffeePowder = hasCoffeePowder;
        }
    }
}