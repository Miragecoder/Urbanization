using System;
using System.Collections.Generic;
using System.Linq;

namespace Mirage.Urbanization.Simulation.Persistence
{
    public class PersistedCityStatisticsWithFinancialData
    {
        public PersistedCityStatisticsWithFinancialData() { }

        public PersistedCityStatisticsWithFinancialData(
            PersistedCityStatistics persistedCityStatistics,
            int currentAmountOfFunds,
            int currentProjectedAmountOfFunds,
            ICityBudgetConfiguration cityBudgetConfiguration)
        {
            PersistedCityStatistics = persistedCityStatistics;
            CurrentAmountOfFunds = currentAmountOfFunds;
            CurrentProjectedAmountOfFunds = currentProjectedAmountOfFunds;

            ResidentialTaxIncome = Convert.ToInt32(persistedCityStatistics.ResidentialZonePopulationStatistics.Sum * cityBudgetConfiguration.ResidentialTaxRate);
            CommercialTaxIncome = Convert.ToInt32(persistedCityStatistics.CommercialZonePopulationStatistics.Sum * cityBudgetConfiguration.CommercialTaxRate);
            IndustrialTaxIncome = Convert.ToInt32(persistedCityStatistics.IndustrialZonePopulationStatistics.Sum * cityBudgetConfiguration.IndustrialTaxRate);

            PoliceServiceExpenses = Convert.ToInt32((persistedCityStatistics.NumberOfPoliceStations * 10) * cityBudgetConfiguration.PoliceServiceRate);
            FireServiceExpenses = Convert.ToInt32((persistedCityStatistics.NumberOfFireStations * 10) * cityBudgetConfiguration.FireDepartmentServiceRate);

            RoadInfrastructureExpenses = Convert.ToInt32((persistedCityStatistics.NumberOfRoadZones) * cityBudgetConfiguration.RoadInfrastructureServiceRate);
            RailroadInfrastructureExpenses = Convert.ToInt32((persistedCityStatistics.NumberOfRailRoadZones) * cityBudgetConfiguration.RailroadInfrastructureServiceRate)
                + Convert.ToInt32((persistedCityStatistics.NumberOfTrainStations * 10) * cityBudgetConfiguration.RailroadInfrastructureServiceRate);
        }

        public IEnumerable<PersistedCityStatisticsWithFinancialData> CombineWithYearMates(
            IEnumerable<PersistedCityStatisticsWithFinancialData> persistedCityStatisticsWithFinancialDatas)
        {
            yield return this;
            foreach (var match in persistedCityStatisticsWithFinancialDatas
                .Where(x => x.PersistedCityStatistics.SharesYearWith(PersistedCityStatistics)))
                yield return match;
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

        public int GetIncome() { return ResidentialTaxIncome + CommercialTaxIncome + IndustrialTaxIncome; }

        public int GetExpenses()
        {
            return PoliceServiceExpenses
                + FireServiceExpenses
                + RoadInfrastructureExpenses
                + RailroadInfrastructureExpenses;
        }

        public int GetTotal()
        {
            return GetIncome() - GetExpenses();
        }

        public PersistedCityStatistics PersistedCityStatistics { get; set; }
    }
}