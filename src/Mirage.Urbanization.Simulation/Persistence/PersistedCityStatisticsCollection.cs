using System.Collections.Generic;
using System.Linq;
using Mirage.Urbanization.ZoneStatisticsQuerying;
using System.Collections.Immutable;

namespace Mirage.Urbanization.Simulation.Persistence
{
    public class PersistedCityStatisticsCollection
    {
        private ImmutableList<PersistedCityStatisticsWithFinancialData> _persistedCityStatistics = ImmutableList<PersistedCityStatisticsWithFinancialData>.Empty;

        private PersistedCityStatisticsWithFinancialData _mostRecentStatistics;

        public void Add(PersistedCityStatisticsWithFinancialData statistics)
        {
            _persistedCityStatistics = _persistedCityStatistics.Add(statistics);
            _mostRecentStatistics = statistics;
        }

        public QueryResult<PersistedCityStatisticsWithFinancialData> GetMostRecentPersistedCityStatistics()
        {
            return new QueryResult<PersistedCityStatisticsWithFinancialData>(_mostRecentStatistics);
        }

        public IReadOnlyCollection<PersistedCityStatisticsWithFinancialData> GetAll()
        {
            return _persistedCityStatistics;
        }
    }
}