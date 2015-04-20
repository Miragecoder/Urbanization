using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization
{
    internal class ZoneInfoGrid
    {
        private readonly IReadOnlyDictionary<ZonePoint, ZoneInfo> _zoneInfos;
        private readonly int _zoneWidthAndHeight;

        public int ZoneWidthAndHeight { get { return _zoneWidthAndHeight; } }

        public ZoneInfoGrid(int zoneWidthAndHeight)
        {
            _zoneWidthAndHeight = zoneWidthAndHeight;
            _zoneInfos = (from x in Enumerable.Range(0, zoneWidthAndHeight)
                          from y in Enumerable.Range(0, zoneWidthAndHeight)
                          let localX = x
                          let localY = y
                          select new ZoneInfo(
                              zonePoint: new ZonePoint
                              {
                                  X = localX,
                                  Y = localY
                              },
                              getRelativeZoneInfo: (query) =>
                              {
                                  var point = new ZonePoint
                                  {
                                      X = (localX + query.RelativeX),
                                      Y = (localY + query.RelativeY)
                                  };

                                  ZoneInfo matchingZoneInfo;
                                  if (_zoneInfos.TryGetValue(point, out matchingZoneInfo))
                                  {
                                      return new QueryResult<IZoneInfo, RelativeZoneInfoQuery>(query, matchingZoneInfo);
                                  }
                                  return new QueryResult<IZoneInfo, RelativeZoneInfoQuery>(query);
                              })
                ).ToDictionary(x => x.Point, x => x);
        }

        public IReadOnlyDictionary<ZonePoint, ZoneInfo> ZoneInfos { get { return _zoneInfos; } }

        public QueryResult<IZoneInfo> GetZoneInfoFor(IAreaZoneConsumption zoneConsumption)
        {
            var match = zoneConsumption is BaseNetworkZoneConsumption ? _zoneInfos
                .FirstOrDefault(x =>
                {
                    var consumption = x.Value.ConsumptionState.GetZoneConsumption();

                    if (consumption == zoneConsumption)
                        return true;

                    var intersection = consumption as IntersectingZoneConsumption;
                    if (intersection != null &&
                        intersection.GetIntersectingZoneConsumptions().Any(y => y == zoneConsumption))
                        return true;
                    return false;
                })
                : _zoneInfos.FirstOrDefault(x => x.Value.ConsumptionState.GetZoneConsumption() == zoneConsumption);

            return new QueryResult<IZoneInfo>(match.Value);
        }
    }
}