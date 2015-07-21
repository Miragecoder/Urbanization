using System;

namespace Mirage.Urbanization.ZoneConsumption.Base.Behaviours
{
    public class ElectricityConsumerBehaviour : IElectricityConsumer
    {
        public int ConsumptionInUnits { get; }

        public ElectricityConsumerBehaviour(int consumptionInUnits)
        {
            ConsumptionInUnits = consumptionInUnits;
        }

        public bool IsPowered { get; private set; }

        public bool IsConnected { get; private set; }

        public void TogglePowered(bool isPowered)
        {
            IsPowered = isPowered;
        }

        public void ToggleConnected(bool isConnected)
        {
            IsConnected = isConnected;
        }
    }
}