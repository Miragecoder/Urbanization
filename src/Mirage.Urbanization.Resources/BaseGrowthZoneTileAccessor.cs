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
                                GrowthZoneTileInfo = new GrowthZoneTileInfo(bitmap.ResourceName)
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
                            new AnimatedCellBitmapSet(500, x.Select(y => new CellBitmap(y.Bitmap)).ToArray()), null));
                });
        }

        private readonly Lazy<IDictionary<GrowthZoneTileCellInfo, AnimatedCellBitmapSetLayers>> _layerDictionaryLazy;

        public QueryResult<AnimatedCellBitmapSetLayers> GetFor(ZoneInfoSnapshot snapShot)
        {
            var maxPop = BaseGrowthZoneClusterConsumption.MaximumPopulation / _layerDictionaryLazy.Value.Max(x => x.Key.GrowthZoneTileInfo.Density);

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
                        var parent = (T) c.ParentBaseZoneClusterConsumption;
                        if (parent.PopulationDensity == 0)
                        {
                            return _layerDictionaryLazy
                                .Value
                                .Where(x => x.Key.Point == c.PositionInCluster)
                                .First(x => x.Key.GrowthZoneTileInfo.Density == 0)
                                .Value
                                .ToQueryResult();
                        }
                        return _layerDictionaryLazy
                            .Value
                            .Where(x => x.Key.Point == c.PositionInCluster)
                            .Where(x => x.Key.GrowthZoneTileInfo.Density != 0)
                            .Where(x => x.Key.GrowthZoneTileInfo.Density <= (c.PopulationDensity / maxPop) + 1)
                            .OrderByDescending(x => x.Key.GrowthZoneTileInfo.Density)
                            .ThenByDescending(x => x.Key.GrowthZoneTileInfo.Quality)
                            .First()
                            .Value
                            .ToQueryResult();
                    }, QueryResult<AnimatedCellBitmapSetLayers>.Empty);
                });
        }
    }
}