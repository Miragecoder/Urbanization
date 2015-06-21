using System.Threading;

namespace Mirage.Urbanization.Simulation
{
    internal class CityBudget
    {
        private int _currentAmount = 50000;

        public int CurrentAmount { get { return _currentAmount; } }

        public void Add(int amount)
        {
            Interlocked.Add(ref _currentAmount, amount);
        }

        public void Subtract(int amount)
        {
            Interlocked.Add(ref _currentAmount, -amount);
        }
    }
}