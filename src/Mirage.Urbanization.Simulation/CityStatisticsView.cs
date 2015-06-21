using System;

namespace Mirage.Urbanization.Simulation
{
    public class CityStatisticsView
    {
        private readonly PersistedCityStatistics _cityStatistics;

        public CityStatisticsView(PersistedCityStatistics cityStatistics)
        {
            if (cityStatistics == null) throw new ArgumentNullException("cityStatistics");
            _cityStatistics = cityStatistics;
        }

        public int Population { get { return _cityStatistics.GlobalZonePopulationStatistics.Sum; } }
        public decimal AssessdValue { get { return _cityStatistics.LandValueNumbers.Sum; } }

        public string CityCategory
        {
            get { return CityCategoryDefinition.GetForPopulation(Population).Name; }
        }
    }
}