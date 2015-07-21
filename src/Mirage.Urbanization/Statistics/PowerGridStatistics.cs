using System.Collections.Generic;

namespace Mirage.Urbanization.Statistics
{
    internal class PowerGridStatistics : BaseStatisticsCollection<IPowerGridNetworkStatistics>, IPowerGridStatistics
    {
        public PowerGridStatistics(IEnumerable<IPowerGridNetworkStatistics> powerGridNetworkStatistics)
            : base(powerGridNetworkStatistics) { }

        public IReadOnlyCollection<IPowerGridNetworkStatistics> PowerGridNetworkStatistics => NetworkStatistics;
    }
}