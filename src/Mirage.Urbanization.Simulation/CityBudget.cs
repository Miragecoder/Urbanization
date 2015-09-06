using System;
using System.Runtime.Remoting.Channels;
using System.Threading;
using Mirage.Urbanization.Simulation.Persistence;

namespace Mirage.Urbanization.Simulation
{
    internal class CityBudget : ICityBudget
    {
        private int _currentAmount = 50000;

        private int _projectedIncome;

        public int CurrentAmount => _currentAmount;
        public string CurrentAmountDescription => $"Current amount: {_currentAmount.ToString("C")}";

        public int ProjectedIncome => _projectedIncome;
        public string ProjectedIncomeDescription => $"Projected income: {_projectedIncome.ToString("C")}";

        private void AddProjectedIncome(int amount)
        {
            Interlocked.Add(ref _projectedIncome, amount);
            RaiseCityBudgetValueChangedEvent();
        }

        public void AddProjectedIncomeToCurrentAmount()
        {
            Interlocked.Add(ref _currentAmount, Interlocked.Exchange(ref _projectedIncome, 0));
            RaiseCityBudgetValueChangedEvent();
        }

        private void Subtract(int amount)
        {
            Interlocked.Add(ref _currentAmount, -amount);
            RaiseCityBudgetValueChangedEvent();
        }

        public void RaiseCityBudgetValueChangedEvent()
        {
            OnCityBudgetValueChanged?.Invoke(this, new CityBudgetValueChangedEventArgs(this));
        }

        public PersistedCityStatisticsWithFinancialData ProcessFinances(PersistedCityStatistics persistedCityStatistics, ICityBudgetConfiguration cityBudgetConfiguration)
        {
            var financialData = new PersistedCityStatisticsWithFinancialData(
                persistedCityStatistics: persistedCityStatistics, 
                currentAmountOfFunds: CurrentAmount,
                currentProjectedAmountOfFunds: ProjectedIncome,
                cityBudgetConfiguration: cityBudgetConfiguration
            );

            AddProjectedIncome(financialData.GetTotal());

            return financialData;
        }

        public event EventHandler<CityBudgetValueChangedEventArgs> OnCityBudgetValueChanged;
        
        public void Handle(IAreaConsumptionResult areaConsumptionResult)
        {
            if (areaConsumptionResult.Success)
            {
                Subtract(areaConsumptionResult.AreaConsumption.Cost);
            }
        }

        public void RestoreFrom(PersistedCityStatisticsWithFinancialData persistedCityStatisticsWithFinancialData)
        {
            _currentAmount = persistedCityStatisticsWithFinancialData.CurrentAmountOfFunds;
            _projectedIncome = persistedCityStatisticsWithFinancialData.CurrentProjectedAmountOfFunds;
        }
    }
}