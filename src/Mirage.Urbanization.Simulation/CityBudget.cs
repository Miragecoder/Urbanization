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

        public int CurrentAmount { get { return _currentAmount; } }

        public int ProjectedIncome { get { return _projectedIncome; } }

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

        private void Add(int amount)
        {
            Interlocked.Add(ref _currentAmount, amount);
            RaiseCityBudgetValueChangedEvent();
        }

        private void Subtract(int amount)
        {
            Interlocked.Add(ref _currentAmount, -amount);
            RaiseCityBudgetValueChangedEvent();
        }

        public void RaiseCityBudgetValueChangedEvent()
        {
            var onCityBudgetValueChanged = OnCityBudgetValueChanged;
            if (onCityBudgetValueChanged != null)
            {
                onCityBudgetValueChanged(this, new CityBudgetValueChangedEventArgs(this));
            }
        }

        public PersistedCityStatisticsWithFinancialData ProcessFinances(PersistedCityStatistics persistedCityStatistics)
        {
            return new PersistedCityStatisticsWithFinancialData(persistedCityStatistics, CurrentAmount);
        }

        public event EventHandler<CityBudgetValueChangedEventArgs> OnCityBudgetValueChanged;
        
        public void Handle(IAreaConsumptionResult areaConsumptionResult)
        {
            if (areaConsumptionResult.Success)
            {
                Subtract(areaConsumptionResult.AreaConsumption.Cost);
            }
        }
    }
}