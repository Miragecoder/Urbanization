namespace Mirage.Urbanization.ZoneConsumption.Base.Behaviours
{
    public class ElectricitySupplierBehaviour : IElectricitySupplier
    {
        private readonly int _contributionInUnits;

        public int ContributionInUnits { get { return _contributionInUnits; } }

        public ElectricitySupplierBehaviour(int contributionInUnits)
        {
            _contributionInUnits = contributionInUnits;
        }

        public bool IsPowered { get { return true; } }
    }
}