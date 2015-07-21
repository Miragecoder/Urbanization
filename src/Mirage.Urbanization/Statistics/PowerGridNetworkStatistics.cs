namespace Mirage.Urbanization.Statistics
{
    internal class PowerGridNetworkStatistics : IPowerGridNetworkStatistics
    {
        public int AmountOfSuppliers { get; }

        public int AmountOfConsumers { get; }

        public int SupplyInUnits { get; }

        public int ConsumptionInUnits { get; }

        public PowerGridNetworkStatistics(int amountOfSuppliers, int amountOfConsumers, int supplyInUnits,
            int consumptionInUnits)
        {
            AmountOfSuppliers = amountOfSuppliers;
            AmountOfConsumers = amountOfConsumers;
            SupplyInUnits = supplyInUnits;
            ConsumptionInUnits = consumptionInUnits;
        }
    }
}