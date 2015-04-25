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
            get
            {
                if (Population < 2000)
                    return "Village";
                else if (Population < 10000)
                    return "Town";
                else if (Population < 50000)
                    return "City";
                else if (Population < 100000)
                    return "Capital";
                else
                    return "Metropolis";
            }
        }
    }
}