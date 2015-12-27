using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Tilesets
{
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
                    new AnimatedCellBitmapSet(FramerateDelay.Structure, x.Select(y => new CellBitmap(y.Bitmap)).ToArray()), null));
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

        public IEnumerable<AnimatedCellBitmapSetLayers> GetAll()
        {
            return this._layerDictionaryLazy.Value.Select(x => x.Value);
        }
    }
}
