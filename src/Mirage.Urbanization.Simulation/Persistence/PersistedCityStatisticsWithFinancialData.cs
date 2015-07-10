namespace Mirage.Urbanization.Simulation.Persistence
{
    public class PersistedCityStatisticsWithFinancialData
    {
        public PersistedCityStatisticsWithFinancialData() { }

        public PersistedCityStatisticsWithFinancialData(
            PersistedCityStatistics persistedCityStatistics,
            int currentAmountOfFunds,
            int currentProjectedAmountOfFunds
        )
        {
            PersistedCityStatistics = persistedCityStatistics;
            CurrentAmountOfFunds = currentAmountOfFunds;
            CurrentProjectedAmountOfFunds = currentProjectedAmountOfFunds;

            ResidentialTaxIncome = persistedCityStatistics.ResidentialZonePopulationStatistics.Sum;
            CommercialTaxIncome = persistedCityStatistics.CommercialZonePopulationStatistics.Sum;
            IndustrialTaxIncome = persistedCityStatistics.IndustrialZonePopulationStatistics.Sum;

            PoliceServiceExpenses = persistedCityStatistics.NumberOfPoliceStations * 50;
            FireServiceExpenses = persistedCityStatistics.NumberOfFireStations * 50;
            RoadInfrastructureExpenses = persistedCityStatistics.NumberOfRoadZones;
            RailroadInfrastructureExpenses = (persistedCityStatistics.NumberOfRailRoadZones*2) +
                                             (persistedCityStatistics.NumberOfTrainStations*500);
        }

        public int CurrentAmountOfFunds { get; set; }
        public int CurrentProjectedAmountOfFunds { get; set; }

        public int ResidentialTaxIncome { get; set; }
        public int CommercialTaxIncome { get; set; }
        public int IndustrialTaxIncome { get; set; }

        public int PoliceServiceExpenses { get; set; }
        public int FireServiceExpenses { get; set; }
        public int RoadInfrastructureExpenses { get; set; }
        public int RailroadInfrastructureExpenses { get; set; }

        public int GetTotal()
        {
            var income = ResidentialTaxIncome + CommercialTaxIncome + IndustrialTaxIncome;
            var expenses = PoliceServiceExpenses
                + FireServiceExpenses
                + RoadInfrastructureExpenses
                + RailroadInfrastructureExpenses;

            return income - expenses;
        }

        public PersistedCityStatistics PersistedCityStatistics { get; set; }
    }
}