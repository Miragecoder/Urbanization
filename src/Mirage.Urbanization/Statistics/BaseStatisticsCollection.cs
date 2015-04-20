using System.Collections.Generic;
using System.Linq;

namespace Mirage.Urbanization.Statistics
{
    internal abstract class BaseStatisticsCollection<TNetworkStatistics>
        where TNetworkStatistics : INetworkStatistics
    {
        protected readonly IReadOnlyList<TNetworkStatistics> NetworkStatistics;

        protected BaseStatisticsCollection(IEnumerable<TNetworkStatistics> networkStatistics)
        {
            NetworkStatistics = networkStatistics.ToList();
        }
    }
}