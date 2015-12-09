using System;
using System.Linq;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Tilesets
{
    public class RoadIntersections
    {
        public static RoadIntersections CreateFor(string suffix, TrafficDensity trafficDensity)
        {
            return new EmbeddedBitmapExtractor()
                .GetBitmapsFromNamespace("Mirage.Urbanization.Tilesets.IntersectingZones.Road" + suffix)
                .ToArray()
                .Pipe(bitmaps =>
                {
                    return new RoadIntersections(
                        trafficDensity: trafficDensity,
                        powerNsRoadEw: new DirectionalCellBitmap(
                            cellBitmapSet: new AnimatedCellBitmapSetLayers(
                                new AnimatedCellBitmapSet(
                                    FramerateDelay.TrafficFramerate,
                                    bitmaps
                                        .Where(x => string.Equals(x.FileName, "powernsroadew", StringComparison.CurrentCultureIgnoreCase))
                                        .Select(x => new CellBitmap(x.Bitmap))
                                        .ToArray()
                                    ),
                                null
                                )
                            ),
                        railNsRoadEw: new DirectionalCellBitmap(
                            cellBitmapSet: new AnimatedCellBitmapSetLayers(
                                new AnimatedCellBitmapSet(
                                    FramerateDelay.TrafficFramerate,
                                    bitmaps
                                        .Where(x => string.Equals(x.FileName, "railnsroadew", StringComparison.CurrentCultureIgnoreCase))
                                        .Select(x => new CellBitmap(x.Bitmap))
                                        .ToArray()
                                    ),
                                null
                                )
                            ),
                        waterNsRoadEw: new DirectionalCellBitmap(
                            cellBitmapSet: new AnimatedCellBitmapSetLayers(
                                new AnimatedCellBitmapSet(
                                    FramerateDelay.TrafficFramerate,
                                    new EmbeddedBitmapExtractor()
                                        .GetBitmapsFromNamespace("Mirage.Urbanization.Tilesets.NetworkZones.Water.waternwes.png")
                                        .Select(x => new CellBitmap(x.Bitmap))
                                        .Single()
                                    ),
                                new AnimatedCellBitmapSet(
                                    FramerateDelay.TrafficFramerate,
                                    bitmaps
                                        .Where(x => string.Equals(x.FileName, "waternsroadew", StringComparison.CurrentCultureIgnoreCase))
                                        .Select(x => new CellBitmap(x.Bitmap))
                                        .ToArray()
                                    )
                                )
                            ));
                });
        }

        public TrafficDensity TrafficDensity { get; }
        private readonly DirectionalCellBitmap _powerNsRoadEw;
        private readonly DirectionalCellBitmap _railNsRoadEw;
        private readonly DirectionalCellBitmap _waterNsRoadEw;

        public RoadIntersections(
            TrafficDensity trafficDensity,
            DirectionalCellBitmap powerNsRoadEw,
            DirectionalCellBitmap railNsRoadEw,
            DirectionalCellBitmap waterNsRoadEw)
        {
            TrafficDensity = trafficDensity;
            _powerNsRoadEw = powerNsRoadEw;
            _railNsRoadEw = railNsRoadEw;
            _waterNsRoadEw = waterNsRoadEw;
        }

        public QueryResult<AnimatedCellBitmapSetLayers> GetFor(ZoneInfoSnapshot snapshot)
        {
            return snapshot
                .Pipe(x => x.AreaZoneConsumption as IntersectingZoneConsumption)
                .ToQueryResult()
                .WithResultIfHasMatch(x =>
                {
                    if (x.NorthSouthZoneConsumption is RoadZoneConsumption)
                    {
                        if (x.EastWestZoneConsumption is RailRoadZoneConsumption)
                            return _railNsRoadEw.Right.ToQueryResult();
                        else if (x.EastWestZoneConsumption is WaterZoneConsumption)
                            return _waterNsRoadEw.Right.ToQueryResult();
                        else if (x.EastWestZoneConsumption is PowerLineConsumption)
                            return _powerNsRoadEw.Right.ToQueryResult();
                        throw new InvalidOperationException();
                    }
                    else if (x.EastWestZoneConsumption is RoadZoneConsumption)
                    {
                        if (x.NorthSouthZoneConsumption is RailRoadZoneConsumption)
                            return _railNsRoadEw.Up.ToQueryResult();
                        else if (x.NorthSouthZoneConsumption is WaterZoneConsumption)
                            return _waterNsRoadEw.Up.ToQueryResult();
                        else if (x.NorthSouthZoneConsumption is PowerLineConsumption)
                            return _powerNsRoadEw.Up.ToQueryResult();
                        throw new InvalidOperationException();
                    }
                    return QueryResult<AnimatedCellBitmapSetLayers>.Empty;
                },
                    QueryResult<AnimatedCellBitmapSetLayers>.Empty
                );
        }
    }
}