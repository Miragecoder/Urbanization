using System;
using System.Collections.Generic;
using System.Linq;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Tilesets
{
    abstract class BaseGrowthZoneTileAccessor<T> : IBaseGrowthZoneTileAccessor
        where T : BaseGrowthZoneClusterConsumption
    {
        public abstract string Namespace { get; }

        protected BaseGrowthZoneTileAccessor()
        {
            _layerDictionaryLazy =
                new Lazy<IDictionary<GrowthZoneTileCellInfo, AnimatedCellBitmapSetLayers>>(() =>
                {
                    return new EmbeddedBitmapExtractor()
                        .GetBitmapsFromNamespace("Mirage.Urbanization.Tilesets.Tiles.Clusters.GrowthZones." + Namespace)
                        .Select(bitmap =>
                        {
                            return new
                            {
                                bitmap.Bitmap,
                                GrowthZoneTileInfo = new GrowthZoneClusterTileInfo(bitmap.ResourceName)
                            };
                        })
                        .GroupBy(x => x.GrowthZoneTileInfo.GroupId)
                        .SelectMany(frames =>
                        {
                            return frames
                                .SelectMany(x => x.Bitmap.GetSegments(25))
                                .Select(x => new
                                {
                                    Bitmap = x.Value,
                                    GrowthZoneTileCellInfo = new GrowthZoneTileCellInfo(frames.First().GrowthZoneTileInfo, x.Key)
                                });
                        })
                        .GroupBy(x => x.GrowthZoneTileCellInfo)
                        .ToDictionary(x => x.Key, x => new AnimatedCellBitmapSetLayers(
                            new AnimatedCellBitmapSet(FramerateDelay.Structure, x.Select(y => new CellBitmap(y.Bitmap)).ToArray()), null));
                });

            _houseLayerDictionaryLazy = new Lazy<IDictionary<GrowthZoneHouseTileInfo, AnimatedCellBitmapSetLayers>>(
                () =>
                {
                    return new EmbeddedBitmapExtractor()
                        .GetBitmapsFromNamespace("Mirage.Urbanization.Tilesets.Tiles.Cells." + Namespace)
                        .Select(bitmap =>
                        {
                            return new
                            {
                                bitmap.Bitmap,
                                GrowthZoneTileInfo = new GrowthZoneHouseTileInfo(bitmap.ResourceName)
                            };
                        })
                        .ToDictionary(x => x.GrowthZoneTileInfo, x => new AnimatedCellBitmapSetLayers(
                            new AnimatedCellBitmapSet(FramerateDelay.Structure, new [] { x.Bitmap }.Select(y => new CellBitmap(y)).ToArray()), null));
                }
            );
        }

        private readonly Lazy<IDictionary<GrowthZoneTileCellInfo, AnimatedCellBitmapSetLayers>> _layerDictionaryLazy;
        private readonly Lazy<IDictionary<GrowthZoneHouseTileInfo, AnimatedCellBitmapSetLayers>> _houseLayerDictionaryLazy;

        public QueryResult<AnimatedCellBitmapSetLayers> GetFor(ZoneInfoSnapshot snapShot)
        {
            var maxPop = BaseGrowthZoneClusterConsumption.MaximumPopulation / _layerDictionaryLazy.Value.Max(x => x.Key.GrowthZoneClusterTileInfo.Density);

            return snapShot
                .Pipe(x =>
                {
                    return (snapShot.AreaZoneConsumption as ZoneClusterMemberConsumption)
                        .ToQueryResult()
                        .WithResultIfHasMatch(y => y.ParentBaseZoneClusterConsumption is T)
                        ? (x.AreaZoneConsumption as ZoneClusterMemberConsumption).ToQueryResult()
                        : default(ZoneClusterMemberConsumption).ToQueryResult();
                })
                .Pipe(clusterMember =>
                {
                    return clusterMember.WithResultIfHasMatch(c =>
                    {
                        var parent = (T)c.ParentBaseZoneClusterConsumption;
                        if (parent.PopulationDensity < 9)
                        {
                            if (parent.RenderAsHouse(c))
                            {
                                return _houseLayerDictionaryLazy
                                    .Value
                                    .ToArray()
                                    .Pipe(set => set[c.Id % set.Length])
                                    .Value
                                    .ToQueryResult();
                            }
                            return _layerDictionaryLazy
                                .Value
                                .Where(x => x.Key.Point == c.PositionInCluster)
                                .First(x => x.Key.GrowthZoneClusterTileInfo.Density == 0)
                                .Value
                                .ToQueryResult();
                        }
                        return _layerDictionaryLazy
                            .Value
                            .Where(x => x.Key.Point == c.PositionInCluster)
                            .Where(x => x.Key.GrowthZoneClusterTileInfo.Density == Math.Ceiling(c.PopulationDensity / (decimal)maxPop))
                            .ToArray()
                            .Pipe(set =>
                            {
                                var id = (clusterMember
                                    .MatchingObject
                                    .ParentBaseZoneClusterConsumption as BaseGrowthZoneClusterConsumption)
                                    .Id;

                                return set[id % set.Length];
                            })
                            .Value
                            .ToQueryResult();
                    }, QueryResult<AnimatedCellBitmapSetLayers>.Empty);
                });
        }

        public IEnumerable<AnimatedCellBitmapSetLayers> GetAll()
        {
            return this._layerDictionaryLazy.Value.Select(x => x.Value)
                .Concat(this._houseLayerDictionaryLazy.Value.Select(x => x.Value));
        }
    }
}