using Mirage.Urbanization.Statistics;

namespace Mirage.Urbanization
{
    public interface IRoadInfrastructureStatistics
    {
        int NumberOfRoadZones { get; }
        INumberSummary TrafficNumbers { get; }
    }
}