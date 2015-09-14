using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Mirage.Urbanization.Simulation.Persistence;
using System.Linq;

namespace Mirage.Urbanization.Simulation
{
    public class CityServiceDefinition : BudgetComponentDefinition
    {
        private readonly Func<IEnumerable<PersistedCityStatisticsWithFinancialData>, int> _getProjectedExpensesFunc;

        private CityServiceDefinition(
            string name,
            Expression<Func<ICityBudgetConfiguration, decimal>> currentRate,
            Func<IEnumerable<PersistedCityStatisticsWithFinancialData>, int> getProjectedExpensesFunc)
            : base(name, currentRate)
        {
            _getProjectedExpensesFunc = getProjectedExpensesFunc;
        }

        public int GetProjectedExpenses(IEnumerable<PersistedCityStatisticsWithFinancialData> cityStatistics)
        {
            return _getProjectedExpensesFunc(cityStatistics);
        }

        private static readonly IReadOnlyList<CityServiceDefinition> CityServiceDefinitionInstances = new List<CityServiceDefinition>
        {
            new CityServiceDefinition("Road infrastructure", x => x.RoadInfrastructureServiceRate, x => x.Sum(y => y.RoadInfrastructureExpenses)),
            new CityServiceDefinition("Railroad infrastructure", x => x.RailroadInfrastructureServiceRate, x => x.Sum(y => y.RailroadInfrastructureExpenses)),
            new CityServiceDefinition("Police department", x => x.PoliceServiceRate, x => x.Sum(y => y.PoliceServiceExpenses)),
            new CityServiceDefinition("Fire department", x => x.FireDepartmentServiceRate, x => x.Sum(y => y.FireServiceExpenses)),
        };

        public static IEnumerable<CityServiceDefinition> CityServiceDefinitions => CityServiceDefinitionInstances;

        public override IEnumerable<decimal> GetSelectableRatePercentages()
        {
            return Enumerable.Range(0, 101)
                .Where(x => x % 5 == 0)
                .Select(x => (decimal)x / 100);
        }
    }
}