using System;
using System.Collections.Generic;
using Mirage.Urbanization.GrowthPathFinding;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization
{
    public struct ZoneInfoSnapshot
    {
        public ZoneInfoSnapshot(
            ZonePoint point, 
            IAreaZoneConsumption areaZoneConsumption, 
            Func<IEnumerable<QueryResult<IZoneInfo, RelativeZoneInfoQuery>>> getNorthEastSouthWestFunc, 
            TrafficDensity trafficDensity)
        {
            Point = point;
            AreaZoneConsumption = areaZoneConsumption;
            _getNorthEastSouthWestFunc = getNorthEastSouthWestFunc;
            TrafficDensity = trafficDensity;
        }

        private readonly Func<IEnumerable<QueryResult<IZoneInfo, RelativeZoneInfoQuery>>> _getNorthEastSouthWestFunc;

        public ZonePoint Point { get; }
        public IAreaZoneConsumption AreaZoneConsumption { get; }

        public TrafficDensity TrafficDensity { get; }

        public IEnumerable<QueryResult<IZoneInfo, RelativeZoneInfoQuery>> GetNorthEastSouthWest()
        {
            return _getNorthEastSouthWestFunc();
        }
    }

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
        ZoneInfoSnapshot TakeSnapshot();
    }
}