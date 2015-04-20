namespace Mirage.Urbanization.Statistics
{
    public interface IPowerGridNetworkStatistics : INetworkStatistics
    {
        int AmountOfSuppliers { get; }
        int AmountOfConsumers { get; }

        int SupplyInUnits { get; }
        int ConsumptionInUnits { get; }
    }
}