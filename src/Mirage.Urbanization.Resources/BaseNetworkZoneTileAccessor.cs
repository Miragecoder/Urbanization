using System;
using System.Linq;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Tilesets
{
    abstract class BaseNetworkZoneTileAccessor<T> : IBaseNetworkZoneTileAccessor
        where T : BaseNetworkZoneConsumption
    {
        protected abstract string NetworkName { get; }

        private readonly Lazy<CellBitmapNetwork> _cellBitmapNetworkLazy;

        protected BaseNetworkZoneTileAccessor()
        {
            _cellBitmapNetworkLazy = new Lazy<CellBitmapNetwork>(() => GenerateNetwork(NetworkName, Delay));
        }

        protected virtual bool Filter(ZoneInfoSnapshot snapshot) => true;

        protected virtual bool IsConnected(IAreaZoneConsumption consumption) => false;

        protected virtual FramerateDelay Delay => FramerateDelay.None;

        protected virtual bool ConnectToEdgeOfMap => false;

        public QueryResult<AnimatedCellBitmapSetLayers> GetFor(ZoneInfoSnapshot snapshot)
        {
            return snapshot
                .AreaZoneConsumption
                .Pipe(consumption =>
                {
                    if (consumption.GetType() == typeof (T) && Filter(snapshot))
                    {
                        return snapshot.GetNorthEastSouthWest()
                            .ToList()
                            .Pipe(vicinity =>
                            {
                                var compass = new
                                {
                                    North = vicinity
                                        .Single(x => x.QueryObject.RelativeX == 0 && x.QueryObject.RelativeY == -1)
                                        .WithResultIfHasMatch(match =>
                                        {
                                            return match.ConsumptionState.GetZoneConsumption().Pipe(matchingConsumption =>
                                            {
                                                if (matchingConsumption is T || IsConnected(matchingConsumption))
                                                    return true;

                                                return (matchingConsumption as IntersectingZoneConsumption)
                                                    .ToQueryResult()
                                                    .WithResultIfHasMatch(x => x.NorthSouthZoneConsumption is T);
                                            });
                                        }, ConnectToEdgeOfMap),
                                    South = vicinity
                                        .Single(x => x.QueryObject.RelativeX == 0 && x.QueryObject.RelativeY == 1)
                                        .WithResultIfHasMatch(match =>
                                        {
                                            return match.ConsumptionState.GetZoneConsumption().Pipe(matchingConsumption =>
                                            {
                                                if (matchingConsumption is T || IsConnected(matchingConsumption))
                                                    return true;

                                                return (matchingConsumption as IntersectingZoneConsumption)
                                                    .ToQueryResult()
                                                    .WithResultIfHasMatch(x => x.NorthSouthZoneConsumption is T);
                                            });
                                        }, ConnectToEdgeOfMap),
                                    East = vicinity
                                        .Single(x => x.QueryObject.RelativeX == 1 && x.QueryObject.RelativeY == 0)
                                        .WithResultIfHasMatch(match =>
                                        {
                                            return match.ConsumptionState.GetZoneConsumption().Pipe(matchingConsumption =>
                                            {
                                                if (matchingConsumption is T || IsConnected(matchingConsumption))
                                                    return true;

                                                return (matchingConsumption as IntersectingZoneConsumption)
                                                    .ToQueryResult()
                                                    .WithResultIfHasMatch(x => x.EastWestZoneConsumption is T);
                                            });
                                        }, ConnectToEdgeOfMap),
                                    West = vicinity
                                        .Single(x => x.QueryObject.RelativeX == -1 && x.QueryObject.RelativeY == 0)
                                        .WithResultIfHasMatch(match =>
                                        {
                                            return match.ConsumptionState.GetZoneConsumption().Pipe(matchingConsumption =>
                                            {
                                                if (matchingConsumption is T || IsConnected(matchingConsumption))
                                                    return true;

                                                return (matchingConsumption as IntersectingZoneConsumption)
                                                    .ToQueryResult()
                                                    .WithResultIfHasMatch(x => x.EastWestZoneConsumption is T);
                                            });
                                        }, ConnectToEdgeOfMap)
                                };

                                return _cellBitmapNetworkLazy.Value.GetForDirections(
                                    compass.North,
                                    compass.East,
                                    compass.South,
                                    compass.West
                                    ).ToQueryResult();
                            });
                        
                    }

                    return QueryResult<AnimatedCellBitmapSetLayers>.Empty;
                });
        }

        private static CellBitmapNetwork GenerateNetwork(string networkName, FramerateDelay delay)
        {
            return new EmbeddedBitmapExtractor()
                .GetBitmapsFromNamespace("Mirage.Urbanization.Tilesets.NetworkZones." + networkName)
                .ToList()
                .Pipe(embeddedResources =>
                {
                    return new CellBitmapNetwork(
                        center: new DirectionalCellBitmap(
                            new AnimatedCellBitmapSetLayers(
                                new AnimatedCellBitmapSet(delay,
                                    embeddedResources.Where(x => string.Equals(x.FileName, networkName, StringComparison.InvariantCultureIgnoreCase))
                                        .Select(x => new CellBitmap(x.Bitmap))
                                        .ToArray()),
                                null
                                )
                            ),
                        east: new DirectionalCellBitmap(
                            new AnimatedCellBitmapSetLayers(
                                new AnimatedCellBitmapSet(delay,
                                    embeddedResources.Where(x => string.Equals(x.FileName, networkName + "e", StringComparison.InvariantCultureIgnoreCase))
                                        .Select(x => new CellBitmap(x.Bitmap))
                                        .ToArray()),
                                null
                                )
                            ),
                        eastWest: new DirectionalCellBitmap(
                            new AnimatedCellBitmapSetLayers(
                                new AnimatedCellBitmapSet(delay,
                                    embeddedResources.Where(x => string.Equals(x.FileName, networkName + "ew", StringComparison.InvariantCultureIgnoreCase))
                                        .Select(x => new CellBitmap(x.Bitmap))
                                        .ToArray()),
                                null
                                )
                            ),
                        northWest: new DirectionalCellBitmap(
                            new AnimatedCellBitmapSetLayers(
                                new AnimatedCellBitmapSet(delay,
                                    embeddedResources.Where(x => string.Equals(x.FileName, networkName + "nw", StringComparison.InvariantCultureIgnoreCase))
                                        .Select(x => new CellBitmap(x.Bitmap))
                                        .ToArray()),
                                null
                                )
                            ),
                        northWestEast: new DirectionalCellBitmap(
                            new AnimatedCellBitmapSetLayers(
                                new AnimatedCellBitmapSet(delay,
                                    embeddedResources.Where(x => string.Equals(x.FileName, networkName + "nwe", StringComparison.InvariantCultureIgnoreCase))
                                        .Select(x => new CellBitmap(x.Bitmap))
                                        .ToArray()),
                                null
                                )
                            ),
                        northWestEastSouth: new DirectionalCellBitmap(
                            new AnimatedCellBitmapSetLayers(
                                new AnimatedCellBitmapSet(delay,
                                    embeddedResources.Where(x => string.Equals(x.FileName, networkName + "nwes", StringComparison.InvariantCultureIgnoreCase))
                                        .Select(x => new CellBitmap(x.Bitmap))
                                        .ToArray()),
                                null
                                )
                            )
                        );
                });
        }
    }
}