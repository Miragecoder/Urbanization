namespace Mirage.Urbanization.Simulation.Persistence
{
    public class PersistedCityStatisticsWithFinancialData
    {
        public PersistedCityStatisticsWithFinancialData() { }

        public PersistedCityStatisticsWithFinancialData(PersistedCityStatistics persistedCityStatistics, int currentAmountOfFunds)
        {
            PersistedCityStatistics = persistedCityStatistics;
            CurrentAmountOfFunds = currentAmountOfFunds;
        }

        public int CurrentAmountOfFunds { get; set; }

        public PersistedCityStatistics PersistedCityStatistics { get; set; }
    }
}