namespace Mirage.Urbanization.Statistics
{
    public interface ICityStatistics
    {
        IPowerGridStatistics PowerGridStatistics { get; }
        IMiscCityStatistics MiscCityStatistics { get; }
        IGrowthZoneStatistics GrowthZoneStatistics { get; }
        int TimeCode { get; }
    }
}