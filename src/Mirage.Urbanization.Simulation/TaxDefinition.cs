using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mirage.Urbanization.Simulation.Persistence;

namespace Mirage.Urbanization.Simulation
{
    public class TaxDefinition
    {
        private readonly string _name;
        private readonly Func<IBudget, decimal> _getCurrentRate;
        private readonly Action<IBudget, decimal> _setCurrentRate;
        private readonly Func<ISet<PersistedCityStatisticsWithFinancialData>, int> _getProjectedIncome;

        private TaxDefinition(
            string name,
            Expression<Func<IBudget, decimal>> currentRate,
            Func<ISet<PersistedCityStatisticsWithFinancialData>, int> getProjectedIncome)
        {
            _name = name;
            _getCurrentRate = currentRate.Compile();
            _setCurrentRate = (budget, rate) => ((PropertyInfo)((MemberExpression)currentRate.Body).Member).SetValue(budget, rate);
            _getProjectedIncome = getProjectedIncome;
        }

        public string Name { get { return _name; } }
        public decimal CurrentRate(IBudget budget) { return _getCurrentRate(budget); }
        public void SetCurrentRate(IBudget budget, decimal rate) { _setCurrentRate(budget, rate); }
        public int GetProjectedIncome(ISet<PersistedCityStatisticsWithFinancialData> cityStatistics)
        {
            return _getProjectedIncome(cityStatistics);
        }

        private static readonly TaxDefinition ResidentialTaxDefinition = new TaxDefinition("Residential", x => x.ResidentialTaxRate, x => x.Sum(y => y.ResidentialTaxIncome));
        private static readonly TaxDefinition CommercialTaxDefinition = new TaxDefinition("Commercial", x => x.CommercialTaxRate, x => x.Sum(y => y.CommercialTaxIncome));
        private static readonly TaxDefinition IndustrialTaxDefinition = new TaxDefinition("Industrial", x => x.IndustrialTaxRate, x => x.Sum(y => y.IndustrialTaxIncome));

        public static IEnumerable<TaxDefinition> TaxDefinitions
        {
            get
            {
                yield return ResidentialTaxDefinition;
                yield return CommercialTaxDefinition;
                yield return IndustrialTaxDefinition;
            }
        }
    }
}