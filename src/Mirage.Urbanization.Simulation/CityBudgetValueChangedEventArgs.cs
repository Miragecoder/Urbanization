using System;

namespace Mirage.Urbanization.Simulation
{
    public class CityBudgetValueChangedEventArgs : EventArgs
    {
        private readonly int _newValue;

        public CityBudgetValueChangedEventArgs(int newValue)
        {
            _newValue = newValue;
        }

        public int NewValue { get { return _newValue; } }
    }
}