using System.Collections.Generic;
using System.Linq;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Simulation.Persistence
{
    public class PersistedCityStatisticsCollection
    {
        private readonly IList<PersistedCityStatisticsWithFinancialData> _persistedCityStatistics = new List<PersistedCityStatisticsWithFinancialData>();

        private PersistedCityStatisticsWithFinancialData _mostRecentStatistics;

        private readonly object _locker = new object();

        public void Add(PersistedCityStatisticsWithFinancialData statistics)
        {
            lock (_locker)
            {
                _persistedCityStatistics.Add(statistics);
                _mostRecentStatistics = statistics;
            }
        }

        public QueryResult<PersistedCityStatisticsWithFinancialData> GetMostRecentPersistedCityStatistics()
        {
            return new QueryResult<PersistedCityStatisticsWithFinancialData>(_mostRecentStatistics);
        }

        public IReadOnlyCollection<PersistedCityStatisticsWithFinancialData> GetAll()
        {
            return _persistedCityStatistics.ToList();
        }
    }
}