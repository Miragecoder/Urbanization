using System.Linq;

namespace Mirage.Urbanization.Web.ClientMessages
{
    public class CityBudgetState
    {
        public CityTaxState[] taxStates { get; set; }
        public CityServiceState[] cityServiceStates { get; set; }

        public CityTaxState totalTaxState => new CityTaxState
        {
            name = "Total tax income",
            projectedIncome = taxStates?.Sum(x => x.projectedIncome) ?? 0M
        };

        public CityServiceState totalCityServiceState => new CityServiceState
        {
            name = "Total city service expenses",
            projectedExpenses = cityServiceStates?.Sum(x => x.projectedExpenses) ?? 0M
        };

        public decimal TotalProjectedIncome => totalTaxState.projectedIncome - totalCityServiceState.projectedExpenses;
    }
}