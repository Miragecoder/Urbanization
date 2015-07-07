namespace Mirage.Urbanization.Statistics
{
    public interface IRoadInfrastructureStatistics
    {
        int NumberOfRoadZones { get; }
        INumberSummary TrafficNumbers { get; }
    }
}