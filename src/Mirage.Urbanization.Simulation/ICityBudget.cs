using System;
using Mirage.Urbanization.Simulation.Persistence;

namespace Mirage.Urbanization.Simulation
{
    public interface ICityBudget
    {
        int CurrentAmount { get; }
        int ProjectedIncome { get; }
        PersistedCityStatisticsWithFinancialData ProcessFinances(PersistedCityStatistics persistedCityStatistics);
        void AddProjectedIncomeToCurrentAmount();
        event EventHandler<CityBudgetValueChangedEventArgs> OnCityBudgetValueChanged;
        void RaiseCityBudgetValueChangedEvent();

        void Handle(IAreaConsumptionResult areaConsumptionResult);
    }
}