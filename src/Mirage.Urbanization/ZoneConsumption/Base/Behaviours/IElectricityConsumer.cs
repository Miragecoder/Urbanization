namespace Mirage.Urbanization.ZoneConsumption.Base.Behaviours
{
    public interface IElectricityConsumer : IElectricityBehaviour
    {
        int ConsumptionInUnits { get; }

        void TogglePowered(bool isPowered);

        void ToggleConnected(bool isPowered);

        bool IsConnected { get; }
    }
}