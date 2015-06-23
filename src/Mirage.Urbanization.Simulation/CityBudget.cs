using System;
using System.Runtime.Remoting.Channels;
using System.Threading;

namespace Mirage.Urbanization.Simulation
{
    public interface ICityBudget
    {
        int CurrentAmount { get; }
        int ProjectedIncome { get; }
    }

    internal class CityBudget : ICityBudget
    {
        private int _currentAmount = 50000;

        private int _projectedIncome;

        public int CurrentAmount { get { return _currentAmount; } }

        public int ProjectedIncome { get { return _projectedIncome; } }

        public CityBudget(IYearAndMonth yearAndMonth)
        {
            if (yearAndMonth == null) throw new ArgumentNullException("yearAndMonth");
            yearAndMonth.OnWeekElapsed += (sender, e) =>
            {
                if (e.EventData.IsAtBeginningOfNewYear)
                    AddProjectedIncomeToCurrentAmount();
            };
        }

        public void AddProjectedIncome(int amount)
        {
            Interlocked.Add(ref _projectedIncome, amount);
            RaiseCityBudgetValueChangedEvent();
        }

        private void AddProjectedIncomeToCurrentAmount()
        {
            Interlocked.Add(ref _currentAmount, Interlocked.Exchange(ref _projectedIncome, 0));
            RaiseCityBudgetValueChangedEvent();
        }

        public void Add(int amount)
        {
            Interlocked.Add(ref _currentAmount, amount);
            RaiseCityBudgetValueChangedEvent();
        }

        public void Subtract(int amount)
        {
            Interlocked.Add(ref _currentAmount, -amount);
            RaiseCityBudgetValueChangedEvent();
        }

        private void RaiseCityBudgetValueChangedEvent()
        {
            var onCityBudgetValueChanged = OnCityBudgetValueChanged;
            if (onCityBudgetValueChanged != null)
            {
                onCityBudgetValueChanged(this, new CityBudgetValueChangedEventArgs(this));
            }
        }

        public event EventHandler<CityBudgetValueChangedEventArgs> OnCityBudgetValueChanged;
    }
}