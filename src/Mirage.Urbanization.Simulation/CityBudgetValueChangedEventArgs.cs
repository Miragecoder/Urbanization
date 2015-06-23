using System;

namespace Mirage.Urbanization.Simulation
{
    public class CityBudgetValueChangedEventArgs : EventArgsWithData<ICityBudget>
    {
        public CityBudgetValueChangedEventArgs(ICityBudget cityBudget)
            : base(cityBudget)
        {

        }
    }
}