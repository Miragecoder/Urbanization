namespace Mirage.Urbanization.Statistics
{
    internal class PowerGridNetworkStatistics : IPowerGridNetworkStatistics
    {
        private readonly int _amountOfSuppliers, _amountOfConsumers, _supplyInUnits, _consumptionInUnits;

        public int AmountOfSuppliers => _amountOfSuppliers;
        public int AmountOfConsumers => _amountOfConsumers;
        public int SupplyInUnits => _supplyInUnits;
        public int ConsumptionInUnits => _consumptionInUnits;

        public PowerGridNetworkStatistics(int amountOfSuppliers, int amountOfConsumers, int supplyInUnits,
            int consumptionInUnits)
        {
            _amountOfSuppliers = amountOfSuppliers;
            _amountOfConsumers = amountOfConsumers;
            _supplyInUnits = supplyInUnits;
            _consumptionInUnits = consumptionInUnits;
        }
    }
}