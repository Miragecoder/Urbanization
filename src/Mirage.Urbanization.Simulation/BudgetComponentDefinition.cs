using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Mirage.Urbanization.Simulation
{
    public class BudgetComponentDefinition
    {
        private readonly string _name;
        private readonly Func<IBudget, decimal> _getCurrentRate;
        private readonly Action<IBudget, decimal> _setCurrentRate;

        public BudgetComponentDefinition(
            string name,
            Expression<Func<IBudget, decimal>> currentRate)
        {
            _name = name;
            _getCurrentRate = currentRate.Compile();
            _setCurrentRate = (budget, rate) => ((PropertyInfo)((MemberExpression)currentRate.Body).Member).SetValue(budget, rate);
        }

        public string Name { get { return _name; } }
        public decimal CurrentRate(IBudget budget) { return _getCurrentRate(budget); }
        public void SetCurrentRate(IBudget budget, decimal rate) { _setCurrentRate(budget, rate); }
    }
}