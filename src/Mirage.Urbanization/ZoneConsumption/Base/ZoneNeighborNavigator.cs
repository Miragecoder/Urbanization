using System;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.ZoneConsumption.Base
{
    public class ZoneNeighborQuery
    {
        private readonly IAreaZoneConsumption _areaZoneConsumption;
        private readonly int _relativeX, _relativeY;

        public IAreaZoneConsumption AreaZoneConsumption { get { return _areaZoneConsumption; } }
        public int RelativeY { get { return _relativeY; } }
        public int RelativeX { get { return _relativeX; } }

        public ZoneNeighborQuery(IAreaZoneConsumption areaZoneConsumption, int relativeX, int relativeY)
        {
            _areaZoneConsumption = areaZoneConsumption;
            _relativeX = relativeX;
            _relativeY = relativeY;
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