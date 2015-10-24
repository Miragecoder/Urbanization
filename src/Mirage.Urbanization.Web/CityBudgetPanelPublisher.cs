using System.Linq;
using Mirage.Urbanization.Simulation;
using Mirage.Urbanization.Web.ClientMessages;

namespace Mirage.Urbanization.Web
{
    public class CityBudgetPanelPublisher
    {
        public CityBudgetState GenerateCityBudgetState(ISimulationSession session)
        {
            var statistics = session.GetAllCityStatisticsForCurrentYear();
            return new CityBudgetState
            {
                cityServiceStates = CityServiceDefinition
                    .CityServiceDefinitions
                    .Select(x => new CityServiceState
                    {
                        name = x.Name,
                        projectedExpenses = x.GetProjectedExpenses(statistics),
                        currentRate = x.CurrentRate(session.CityBudgetConfiguration).ToString("P")
                    })
                    .ToArray(),
                taxStates = TaxDefinition
                    .TaxDefinitions
                    .Select(x => new CityTaxState
                    {
                        name = x.Name,
                        projectedIncome = x.GetProjectedIncome(statistics),
                        currentRate = x.CurrentRate(session.CityBudgetConfiguration).ToString("P")
                    })
                    .ToArray()
            };
        }
    }
}