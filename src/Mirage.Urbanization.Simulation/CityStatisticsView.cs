using System;
using System.Collections.Generic;
using System.Linq;
using Mirage.Urbanization.Simulation.Datameters;
using Mirage.Urbanization.Simulation.Persistence;

namespace Mirage.Urbanization.Simulation
{
    public class CityStatisticsView
    {
        private readonly PersistedCityStatisticsWithFinancialData _cityStatistics;
        private readonly Lazy<IList<DataMeterResult>> _issueDataMeterResultsLazy; 

        public CityStatisticsView(PersistedCityStatisticsWithFinancialData cityStatistics)
        {
            if (cityStatistics == null) throw new ArgumentNullException(nameof(cityStatistics));
            _cityStatistics = cityStatistics;
            _issueDataMeterResultsLazy = new Lazy<IList<DataMeterResult>>(() => DataMeterInstances.GetDataMeterResults(cityStatistics, x => x.RepresentsIssue).ToList());
        }

        public decimal GetNegativeOpinion() => _issueDataMeterResultsLazy
            .Value
            .Average(x => x.PercentageScore);
        public decimal GetPositiveOpinion() => 1 - GetNegativeOpinion();


        public int Population => _cityStatistics.PersistedCityStatistics.GlobalZonePopulationStatistics.Sum;
        public decimal AssessedValue => _cityStatistics.PersistedCityStatistics.LandValueNumbers.Sum;

        public int CurrentAmountOfFunds => _cityStatistics.CurrentAmountOfFunds;
        public int CurrentProjectedAmountOfFunds => _cityStatistics.CurrentProjectedAmountOfFunds;

        public string CityCategory => CityCategoryDefinition.GetForPopulation(Population).Name;

        public IList<DataMeterResult> DataMeterResults => _issueDataMeterResultsLazy.Value;

        public IEnumerable<DataMeterResult> GetIssueDataMeterResults() => DataMeterResults
            .Where(x => x.ValueCategory > DataMeterValueCategory.VeryLow)
            .OrderByDescending(x => x.PercentageScore);
    }
}