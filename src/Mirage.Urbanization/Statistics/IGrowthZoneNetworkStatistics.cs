namespace Mirage.Urbanization.Statistics
{
    public interface IGrowthZoneStatistics
    {
        INumberSummary ResidentialZonePopulationNumbers { get; }
        INumberSummary CommercialZonePopulationNumbers { get; }
        INumberSummary IndustrialZonePopulationNumbers { get; }
        INumberSummary GlobalZonePopulationNumbers { get; }

        IRoadInfrastructureStatistics RoadInfrastructureStatistics { get; }
        IRailroadInfrastructureStatistics RailroadInfrastructureStatistics { get; }
    }
}