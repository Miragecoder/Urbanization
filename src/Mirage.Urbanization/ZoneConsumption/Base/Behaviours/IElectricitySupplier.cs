namespace Mirage.Urbanization.ZoneConsumption.Base.Behaviours
{
    public interface IElectricitySupplier : IElectricityBehaviour
    {
        int ContributionInUnits { get; }
    }
}