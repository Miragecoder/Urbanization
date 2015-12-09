using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Tilesets
{
    class IntersectingZoneTileAccessor
    {
        private readonly Lazy<RoadIntersections> _noneRoadIntersectionsLazy
            = new Lazy<RoadIntersections>(() => RoadIntersections.CreateFor("None", TrafficDensity.None));
        private readonly Lazy<RoadIntersections> _lowRoadIntersectionsLazy
            = new Lazy<RoadIntersections>(() => RoadIntersections.CreateFor("Low", TrafficDensity.Low));
        private readonly Lazy<RoadIntersections> _highRoadIntersectionsLazy
            = new Lazy<RoadIntersections>(() => RoadIntersections.CreateFor("High", TrafficDensity.High));

        private IEnumerable<RoadIntersections> GetIntersectionTypes()
        {
            yield return _noneRoadIntersectionsLazy.Value;
            yield return _lowRoadIntersectionsLazy.Value;
            yield return _highRoadIntersectionsLazy.Value;
        }

        private readonly Lazy<DirectionalCellBitmap> _powerNsWaterEwLazy 
            = new Lazy<DirectionalCellBitmap>(() =>
            {
                return new DirectionalCellBitmap(
                    new AnimatedCellBitmapSetLayers(
                        new AnimatedCellBitmapSet(FramerateDelay.None, new EmbeddedBitmapExtractor()
                            .GetBitmapsFromNamespace("Mirage.Urbanization.Tilesets.NetworkZones.Water.waternwes.png")
                            .Select(x => new CellBitmap(x.Bitmap))
                            .Single()
                            ),
                        new AnimatedCellBitmapSet(FramerateDelay.None, new EmbeddedBitmapExtractor()
                            .GetBitmapsFromNamespace("Mirage.Urbanization.Tilesets.IntersectingZones.powernswaterew.png")
                            .Select(x => new CellBitmap(x.Bitmap))
                            .Single()
                        )
                    )
                );
            });

        private readonly Lazy<DirectionalCellBitmap> _railNsPowerEwLazy
            = new Lazy<DirectionalCellBitmap>(() => GetDirectionalCellBitmap("railnspowerew"));
        private readonly Lazy<DirectionalCellBitmap> _railNsWaterEwLazy
            = new Lazy<DirectionalCellBitmap>(() => GetDirectionalCellBitmap("railnswaterew"));

        private static DirectionalCellBitmap GetDirectionalCellBitmap(string filename)
        {
            return new DirectionalCellBitmap(
                new AnimatedCellBitmapSetLayers(
                    new AnimatedCellBitmapSet(FramerateDelay.None, new EmbeddedBitmapExtractor()
                        .GetBitmapsFromNamespace($"Mirage.Urbanization.Tilesets.IntersectingZones.{filename}.png")
                        .Select(x => new CellBitmap(x.Bitmap))
                        .Single()
                    ),
                    null
                )
            );
        }

        public QueryResult<AnimatedCellBitmapSetLayers> GetFor(ZoneInfoSnapshot snapshot)
        {
            return snapshot
                .Pipe(x => x.AreaZoneConsumption as IntersectingZoneConsumption)
                .ToQueryResult()
                .WithResultIfHasMatch(x =>
                {
                    if (x.NorthSouthZoneConsumption is RoadZoneConsumption ||
                        x.EastWestZoneConsumption is RoadZoneConsumption)
                    {
                        return GetIntersectionTypes()
                            .Single(y => y.TrafficDensity == x.GetTrafficDensity())
                            .GetFor(snapshot);
                    }

                    if (x.NorthSouthZoneConsumption is PowerLineConsumption)
                    {
                        if (x.EastWestZoneConsumption is WaterZoneConsumption)
                        {
                            return _powerNsWaterEwLazy.Value.Up.ToQueryResult();
                        }
                        else if (x.EastWestZoneConsumption is RailRoadZoneConsumption)
                        {
                            return _railNsPowerEwLazy.Value.Right.ToQueryResult();
                        }
                        throw new InvalidOperationException();
                    }
                    else if (x.NorthSouthZoneConsumption is WaterZoneConsumption)
                    {
                        if (x.EastWestZoneConsumption is PowerLineConsumption)
                        {
                            return _powerNsWaterEwLazy.Value.Right.ToQueryResult();
                        }
                        else if (x.EastWestZoneConsumption is RailRoadZoneConsumption)
                        {
                            return _railNsWaterEwLazy.Value.Right.ToQueryResult();
                        }
                        throw new InvalidOperationException();
                    }
                    else if (x.NorthSouthZoneConsumption is RailRoadZoneConsumption)
                    {
                        if (x.EastWestZoneConsumption is PowerLineConsumption)
                        {
                            return _powerNsWaterEwLazy.Value.Right.ToQueryResult();
                        }
                        else if (x.EastWestZoneConsumption is WaterZoneConsumption)
                        {
                            return _railNsWaterEwLazy.Value.Up.ToQueryResult();
                        }
                        throw new InvalidOperationException();
                    }

                    return QueryResult<AnimatedCellBitmapSetLayers>.Empty;
                }, QueryResult<AnimatedCellBitmapSetLayers>.Empty);
        }
    }
}
