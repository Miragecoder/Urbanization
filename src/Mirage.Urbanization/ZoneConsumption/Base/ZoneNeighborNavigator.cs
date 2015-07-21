using System;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.ZoneConsumption.Base
{
    public class ZoneNeighborQuery
    {
        public IAreaZoneConsumption AreaZoneConsumption { get; }
        public int RelativeY { get; }
        public int RelativeX { get; }

        public ZoneNeighborQuery(IAreaZoneConsumption areaZoneConsumption, int relativeX, int relativeY)
        {
            AreaZoneConsumption = areaZoneConsumption;
            RelativeX = relativeX;
            RelativeY = relativeY;
        }
    }

    public class ZoneInfoFinder
    {
        private readonly Func<IAreaZoneConsumption, QueryResult<IZoneInfo>> _getZoneInfoForConsumption;

        public ZoneInfoFinder(Func<IAreaZoneConsumption, QueryResult<IZoneInfo>> getZoneInfoForConsumption)
        {
            _getZoneInfoForConsumption = getZoneInfoForConsumption;
        }

        private QueryResult<IZoneInfo> _zoneInfo;

        public QueryResult<IZoneInfo> GetZoneInfoFor(IAreaZoneConsumption consumption)
        {
            return _zoneInfo ?? (_zoneInfo = _getZoneInfoForConsumption(consumption));
        }
    }
}