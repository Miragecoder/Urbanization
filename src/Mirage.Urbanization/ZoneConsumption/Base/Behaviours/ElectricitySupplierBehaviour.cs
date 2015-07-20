namespace Mirage.Urbanization.ZoneConsumption.Base.Behaviours
{
    public class ElectricitySupplierBehaviour : IElectricitySupplier
    {
        public int ContributionInUnits { get; }

        public ElectricitySupplierBehaviour(int contributionInUnits)
        {
            ContributionInUnits = contributionInUnits;
        }

        public bool IsPowered => true;
    }
}