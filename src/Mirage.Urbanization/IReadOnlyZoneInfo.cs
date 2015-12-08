using System;
using System.Collections.Generic;
using Mirage.Urbanization.GrowthPathFinding;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization
{
    public interface IReadOnlyZoneInfo
    {
        ZonePoint Point { get; }
        IReadOnlyZoneConsumptionState ZoneConsumptionState { get; }
        IGrowthAlgorithmHighlightState GrowthAlgorithmHighlightState { get; }
        IEnumerable<QueryResult<IZoneInfo, RelativeZoneInfoQuery>> GetSurroundingZoneInfosDiamond(int size);

        QueryResult<IQueryCrimeResult> GetLastQueryCrimeResult();
        QueryResult<IQueryFireHazardResult> GetLastQueryFireHazardResult();
        QueryResult<IQueryPollutionResult> GetLastQueryPollutionResult();
        QueryResult<IQueryLandValueResult> GetLastLandValueResult();

        int? GetLastAverageTravelDistance();
        IEnumerable<QueryResult<IZoneInfo, RelativeZoneInfoQuery>> GetNorthEastSouthWest();
        IEnumerable<IZoneInfo> CrawlAllDirections(Func<IZoneInfo, bool> predicate);
        int GetPopulationDensity();
    }
}