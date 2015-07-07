using System.Collections.Generic;
using System.Linq;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Simulation
{
    public class PersistedCityStatisticsCollection
    {
        private readonly IList<PersistedCityStatistics> _persistedCityStatistics = new List<PersistedCityStatistics>();

        private PersistedCityStatistics _mostRecentStatistics;

        private readonly object _locker = new object();

        public void Add(PersistedCityStatistics statistics)
        {
            lock (_locker)
            {
                _persistedCityStatistics.Add(statistics);
                _mostRecentStatistics = statistics;
            }
        }

        public QueryResult<PersistedCityStatistics> GetMostRecentPersistedCityStatistics()
        {
            return new QueryResult<PersistedCityStatistics>(_mostRecentStatistics);
        }

        public IReadOnlyCollection<PersistedCityStatistics> GetAll()
        {
            return _persistedCityStatistics.ToList();
        }
    }
}