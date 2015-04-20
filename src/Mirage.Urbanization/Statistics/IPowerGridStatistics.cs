using System.Collections.Generic;

namespace Mirage.Urbanization.Statistics
{
    public interface IPowerGridStatistics
    {
        IReadOnlyCollection<IPowerGridNetworkStatistics> PowerGridNetworkStatistics { get; }
    }
}