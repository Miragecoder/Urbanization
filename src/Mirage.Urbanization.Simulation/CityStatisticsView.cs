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
            if (cityStatistics == null) throw new ArgumentNullException("cityStatistics");
            _cityStatistics = cityStatistics;
            _issueDataMeterResultsLazy = new Lazy<IList<DataMeterResult>>(() => DataMeterInstances.GetDataMeterResults(cityStatistics, x => x.RepresentsIssue).ToList());
        }

        public int Population { get { return _cityStatistics.PersistedCityStatistics.GlobalZonePopulationStatistics.Sum; } }
        public decimal AssessedValue { get { return _cityStatistics.PersistedCityStatistics.LandValueNumbers.Sum; } }

        public int CurrentAmountOfFunds { get { return _cityStatistics.CurrentAmountOfFunds; } }
        public int CurrentProjectedAmountOfFunds { get { return _cityStatistics.CurrentProjectedAmountOfFunds; } }

        public string CityCategory
        {
            get { return CityCategoryDefinition.GetForPopulation(Population).Name; }
        }

        public IList<DataMeterResult> DataMeterResults { get { return _issueDataMeterResultsLazy.Value; } }
    }
}