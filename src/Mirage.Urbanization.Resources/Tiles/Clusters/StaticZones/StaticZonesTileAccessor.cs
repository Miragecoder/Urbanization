using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Tilesets.Tiles.Clusters.StaticZones
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

            return Assembly
                .GetExecutingAssembly()
                .GetManifestResourceNames()
                .Where(x => x.Contains("Mirage.Urbanization.Tilesets.Tiles.Clusters.StaticZones"))
                .Select(resourceName =>
                {
                    return new
                    {
                        Type = staticZoneTypes.Single(t => resourceName.Contains(t.Name)),
                        Bitmap = new Bitmap(
                            Image.FromStream(
                                Assembly
                                    .GetExecutingAssembly()
                                    .GetManifestResourceStream(resourceName)
                                )
                            )
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

        public QueryResult<AnimatedCellBitmapSetLayers> GetFor(IReadOnlyZoneInfo readOnlyZoneInfo)
        {
            return readOnlyZoneInfo
                .ZoneConsumptionState
                .GetZoneConsumption()
                .Pipe(consumption => (consumption as ZoneClusterMemberConsumption).ToQueryResult())
                .Pipe(clusterMember => clusterMember
                    .WithResultIfHasMatch(x => 
                        new Point(
                            clusterMember.MatchingObject.PositionInClusterX - 1,
                            clusterMember.MatchingObject.PositionInClusterY - 1
                        )
                        .Pipe(location =>
                            _layerDictionaryLazy.Value
                               .Single(y => y.Key.Item1 == x.ParentBaseZoneClusterConsumption.GetType() 
                                && y.Key.Item2 == location)
                               .Value
                               .ToQueryResult()
                        ), 
                    QueryResult<AnimatedCellBitmapSetLayers>.Empty
                )
            );
        }
    }
}
