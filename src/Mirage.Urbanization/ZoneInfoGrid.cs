using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization
{
    internal class ZoneInfoGrid
    {
        public int ZoneWidthAndHeight { get; }

        public ZoneInfoGrid(int zoneWidthAndHeight, ILandValueCalculator landValueCalculator)
        {
            ZoneWidthAndHeight = zoneWidthAndHeight;
            ZoneInfos = (from x in Enumerable.Range(0, zoneWidthAndHeight)
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
                                  if (ZoneInfos.TryGetValue(point, out matchingZoneInfo))
                                  {
                                      return new QueryResult<IZoneInfo, RelativeZoneInfoQuery>(query, matchingZoneInfo);
                                  }
                                  return new QueryResult<IZoneInfo, RelativeZoneInfoQuery>(query);
                              },
                              landValueCalculator: landValueCalculator
                        )
                ).ToDictionary(x => x.Point, x => x);
        }

        public IReadOnlyDictionary<ZonePoint, ZoneInfo> ZoneInfos { get; }

        public QueryResult<IZoneInfo> GetZoneInfoFor(IAreaZoneConsumption zoneConsumption)
        {
            var match = zoneConsumption is BaseNetworkZoneConsumption ? ZoneInfos
                .FirstOrDefault(x =>
                {
                    var consumption = x.Value.ConsumptionState.GetZoneConsumption();

                    if (consumption == zoneConsumption)
                        return true;

                    var intersection = consumption as IntersectingZoneConsumption;
                    return intersection != null &&
                           intersection.GetIntersectingZoneConsumptions().Any(y => y == zoneConsumption);
                })
                : ZoneInfos.FirstOrDefault(x => x.Value.ConsumptionState.GetZoneConsumption() == zoneConsumption);

            return new QueryResult<IZoneInfo>(match.Value);
        }
    }
}