using System;

namespace Mirage.Urbanization.ZoneConsumption.Base.Behaviours
{
    public class ElectricityConsumerBehaviour : IElectricityConsumer
    {
        private readonly int _consumptionInUnits;

        public int ConsumptionInUnits { get { return _consumptionInUnits; } }

        public ElectricityConsumerBehaviour(int consumptionInUnits)
        {
            _consumptionInUnits = consumptionInUnits;
        }

        public bool IsPowered
        {
            get { return _isPowered; }
        }

        public bool IsConnected
        {
            get { return _isConnected; }
        }

        private bool _isPowered;
        private bool _isConnected;

        public void TogglePowered(bool isPowered)
        {
            _isPowered = isPowered;
        }

        public void ToggleConnected(bool isConnected)
        {
            _isConnected = isConnected;
        }
    }
}