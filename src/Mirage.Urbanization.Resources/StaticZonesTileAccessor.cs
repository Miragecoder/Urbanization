using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Tilesets
{
    interface IBaseNetworkZoneTileAccessor
    {
        QueryResult<AnimatedCellBitmapSetLayers> GetFor(ZoneInfoSnapshot snapshot);
    }

    abstract class BaseNetworkZoneTileAccessor<T> : IBaseNetworkZoneTileAccessor
        where T : BaseNetworkZoneConsumption
    {
        protected abstract string NetworkName { get; }

        private readonly Lazy<CellBitmapNetwork> _cellBitmapNetworkLazy;

        protected BaseNetworkZoneTileAccessor()
        {
            _cellBitmapNetworkLazy = new Lazy<CellBitmapNetwork>(() => GenerateNetwork(NetworkName));
        }

        protected virtual bool Filter(ZoneInfoSnapshot snapshot) => true;

        protected virtual bool IsConnected(IAreaZoneConsumption consumption) => false;

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
                                        }, false),
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
                                        }, false),
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
                                        }, false),
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
                                        }, false)
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

        private static CellBitmapNetwork GenerateNetwork(string networkName)
        {
            return new EmbeddedBitmapExtractor()
                .GetBitmapsFromNamespace("Mirage.Urbanization.Tilesets.NetworkZones." + networkName)
                .ToList()
                .Pipe(embeddedResources =>
                {
                    return new CellBitmapNetwork(
                        center: new DirectionalCellBitmap(
                            new AnimatedCellBitmapSetLayers(
                                new AnimatedCellBitmapSet(500,
                                    embeddedResources.Where(x => string.Equals(x.FileName, networkName, StringComparison.InvariantCultureIgnoreCase))
                                        .Select(x => new CellBitmap(x.Bitmap))
                                        .ToArray()),
                                null
                            )
                        ),
                        east: new DirectionalCellBitmap(
                            new AnimatedCellBitmapSetLayers(
                                new AnimatedCellBitmapSet(500,
                                    embeddedResources.Where(x => string.Equals(x.FileName, networkName + "e", StringComparison.InvariantCultureIgnoreCase))
                                        .Select(x => new CellBitmap(x.Bitmap))
                                        .ToArray()),
                                null
                            )
                        ),
                        eastWest: new DirectionalCellBitmap(
                            new AnimatedCellBitmapSetLayers(
                                new AnimatedCellBitmapSet(500,
                                    embeddedResources.Where(x => string.Equals(x.FileName, networkName + "ew", StringComparison.InvariantCultureIgnoreCase))
                                        .Select(x => new CellBitmap(x.Bitmap))
                                        .ToArray()),
                                null
                            )
                        ),
                        northWest: new DirectionalCellBitmap(
                            new AnimatedCellBitmapSetLayers(
                                new AnimatedCellBitmapSet(500,
                                    embeddedResources.Where(x => string.Equals(x.FileName, networkName + "nw", StringComparison.InvariantCultureIgnoreCase))
                                        .Select(x => new CellBitmap(x.Bitmap))
                                        .ToArray()),
                                null
                            )
                        ),
                        northWestEast: new DirectionalCellBitmap(
                            new AnimatedCellBitmapSetLayers(
                                new AnimatedCellBitmapSet(500,
                                    embeddedResources.Where(x => string.Equals(x.FileName, networkName + "nwe", StringComparison.InvariantCultureIgnoreCase))
                                        .Select(x => new CellBitmap(x.Bitmap))
                                        .ToArray()),
                                null
                            )
                        ),
                        northWestEastSouth: new DirectionalCellBitmap(
                            new AnimatedCellBitmapSetLayers(
                                new AnimatedCellBitmapSet(500,
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

    class StaticZonesTileAccessor
    {
        private readonly Lazy<IDictionary<Tuple<Type, Point>, AnimatedCellBitmapSetLayers>> _layerDictionaryLazy =
            new Lazy<IDictionary<Tuple<Type, Point>, AnimatedCellBitmapSetLayers>>(() =>
        {
            var staticZoneTypes = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                   from assemblyType in domainAssembly.GetTypes()
                                   where typeof(StaticZoneClusterConsumption).IsAssignableFrom(assemblyType)
                                   select assemblyType).ToArray();

            return new EmbeddedBitmapExtractor()
                .GetBitmapsFromNamespace("Mirage.Urbanization.Tilesets.Tiles.Clusters.StaticZones")
                .Select(resourceName =>
                {
                    return new
                    {
                        Type = staticZoneTypes.Single(t => resourceName.ResourceName.Contains(t.Name)),
                        Bitmap = resourceName.Bitmap
                    };
                })
                .GroupBy(x => x.Type)
                .SelectMany(frames =>
                {
                    return frames
                        .SelectMany(x => x.Bitmap.GetSegments(25))
                        .Select(x => new
                        {
                            Bitmap = x.Value,
                            Point = x.Key,
                            Type = frames.Key
                        });
                })
                .GroupBy(x => new Tuple<Type, Point>(x.Type, x.Point))
                .ToDictionary(x => x.Key, x => new AnimatedCellBitmapSetLayers(
                    new AnimatedCellBitmapSet(500, x.Select(y => new CellBitmap(y.Bitmap)).ToArray()), null));
        });

        public QueryResult<AnimatedCellBitmapSetLayers> GetFor(ZoneInfoSnapshot snapShot)
        {
            return snapShot
                .Pipe(x =>
                {
                    return (snapShot.AreaZoneConsumption as ZoneClusterMemberConsumption)
                        .ToQueryResult()
                        .WithResultIfHasMatch(y => y.ParentBaseZoneClusterConsumption is StaticZoneClusterConsumption)
                        ? (x.AreaZoneConsumption as ZoneClusterMemberConsumption).ToQueryResult()
                        : default(ZoneClusterMemberConsumption).ToQueryResult();
                })
                .Pipe(clusterMember => clusterMember.WithResultIfHasMatch(x =>
                    _layerDictionaryLazy
                        .Value
                        .Single(y => y.Key.Item1 == x.ParentBaseZoneClusterConsumption.GetType() && y.Key.Item2 == x.PositionInCluster)
                        .Value
                        .ToQueryResult()
                ,
                QueryResult<AnimatedCellBitmapSetLayers>.Empty)
            );
        }
    }
}
