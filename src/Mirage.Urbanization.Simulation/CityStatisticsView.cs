using System;
using System.Collections.Generic;
using System.Linq;
using Mirage.Urbanization.Simulation.Datameters;

namespace Mirage.Urbanization.Simulation
{
    public class CityStatisticsView
    {
        private readonly PersistedCityStatistics _cityStatistics;
        private readonly Lazy<IList<DataMeterResult>> _dataMeterResultsLazy; 

        public CityStatisticsView(PersistedCityStatistics cityStatistics)
        {
            if (cityStatistics == null) throw new ArgumentNullException("cityStatistics");
            _cityStatistics = cityStatistics;
            _dataMeterResultsLazy = new Lazy<IList<DataMeterResult>>(() => DataMeterInstances.GetDataMeterResults(cityStatistics).ToList());
        }

        public int Population { get { return _cityStatistics.GlobalZonePopulationStatistics.Sum; } }
        public decimal AssessdValue { get { return _cityStatistics.LandValueNumbers.Sum; } }

        public string CityCategory
        {
            get { return CityCategoryDefinition.GetForPopulation(Population).Name; }
        }

        public IList<DataMeterResult> DataMeterResults { get { return _dataMeterResultsLazy.Value; } }
    }
}