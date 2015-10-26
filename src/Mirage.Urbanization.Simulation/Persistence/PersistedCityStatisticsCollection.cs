using System.Collections.Generic;
using System.Linq;
using Mirage.Urbanization.ZoneStatisticsQuerying;
using System.Collections.Immutable;

namespace Mirage.Urbanization.Simulation.Persistence
{
    public class PersistedCityStatisticsCollection
    {
        private ImmutableQueue<PersistedCityStatisticsWithFinancialData> _persistedCityStatistics = ImmutableQueue<PersistedCityStatisticsWithFinancialData>.Empty;

        private PersistedCityStatisticsWithFinancialData _mostRecentStatistics;

        public void Add(PersistedCityStatisticsWithFinancialData statistics)
        {
            _persistedCityStatistics = _persistedCityStatistics.Enqueue(statistics);
            if (_persistedCityStatistics.Count() > 20800)
                _persistedCityStatistics = _persistedCityStatistics.Dequeue();

            _mostRecentStatistics = statistics;
        }

        public QueryResult<PersistedCityStatisticsWithFinancialData> GetMostRecentPersistedCityStatistics()
        {
            return QueryResult<PersistedCityStatisticsWithFinancialData>.Create(_mostRecentStatistics);
        }

        public IEnumerable<PersistedCityStatisticsWithFinancialData> GetAll()
        {
            return _persistedCityStatistics;
        }
    }
}