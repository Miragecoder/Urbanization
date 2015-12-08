using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Tilesets.Tiles.Clusters.StaticZones
{
    public class GrowthZoneTileInfo
    {
        public GrowthZoneTileInfo(string resourceName)
        {
            var fileName = resourceName.Split('.').Reverse().Skip(1).Take(1).First();
            //d0_q0_n1_a1.png
            var segments = fileName.Split('_');

            Density = Convert.ToInt32(segments[0].Substring(1));
            Quality = Convert.ToInt32(segments[1].Substring(1));
            Id = Convert.ToInt32(segments[2].Substring(1));
        }

        public int Density { get; }
        public int Quality { get; }
        public int Id { get; }
        public string GroupId => string.Join("_", Density, Quality, Id);

        public override bool Equals(object obj)
        {
            return (obj as GrowthZoneTileInfo).ToQueryResult()
                .WithResultIfHasMatch(x => GroupId == x.GroupId, false);
        }

        public override int GetHashCode()
        {
            return GroupId.GetHashCode();
        }
    }

    public class GrowthZoneTileCellInfo
    {
        public GrowthZoneTileCellInfo(GrowthZoneTileInfo growthZoneTileInfo, Point point)
        {
            GrowthZoneTileInfo = growthZoneTileInfo;
            Point = point;
        }

        public GrowthZoneTileInfo GrowthZoneTileInfo { get; }
        public Point Point { get; }

        public override int GetHashCode()
        {
            return (GrowthZoneTileInfo.GroupId + Point.X + Point.Y).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return (obj as GrowthZoneTileCellInfo)
                .ToQueryResult()
                .WithResultIfHasMatch(x => x.GetHashCode() == GetHashCode());
        }
    }

    class CommercialGrowthZoneTileAccessor : BaseGrowthZoneTileAccessor<CommercialZoneClusterConsumption>
    {
        public override string Namespace => "Commercial";
    }
    class ResidentialGrowthZoneTileAccessor : BaseGrowthZoneTileAccessor<ResidentialZoneClusterConsumption>
    {
        public override string Namespace => "Residential";
    }
    class IndustrialGrowthZoneTileAccessor : BaseGrowthZoneTileAccessor<IndustrialZoneClusterConsumption>
    {
        public override string Namespace => "Industrial";
    }

    class GrowthZoneTileAccessor
    {

        private readonly IBaseGrowthZoneTileAccessor[] _accessors =
        {
            new ResidentialGrowthZoneTileAccessor(),
            new IndustrialGrowthZoneTileAccessor(),
            new CommercialGrowthZoneTileAccessor()
        };

        public QueryResult<AnimatedCellBitmapSetLayers> GetFor(IReadOnlyZoneInfo readOnlyZoneInfo)
        {
            return _accessors
                .Select(x => x.GetFor(readOnlyZoneInfo))
                .SingleOrDefault(x => x.HasMatch)
                .Pipe(match =>
                {
                    if (match != null) return match;
                    return QueryResult<AnimatedCellBitmapSetLayers>.Empty;
                });
        }
    }

    interface IBaseGrowthZoneTileAccessor
    {
        QueryResult<AnimatedCellBitmapSetLayers> GetFor(IReadOnlyZoneInfo readOnlyZoneInfo);
    }

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

        public QueryResult<AnimatedCellBitmapSetLayers> GetFor(IReadOnlyZoneInfo readOnlyZoneInfo)
        {
            var maxPop = BaseGrowthZoneClusterConsumption.MaximumPopulation / _layerDictionaryLazy.Value.Max(x => x.Key.GrowthZoneTileInfo.Density);

            return readOnlyZoneInfo
                .ZoneConsumptionState
                .Pipe(x => x.GetIfZoneClusterAndParent<T>())
                .Pipe(clusterMember =>
                {
                    return clusterMember.WithResultIfHasMatch(c =>
                    {
                        if (c.BaseZoneClusterConsumption.PopulationDensity == 0)
                        {
                            return _layerDictionaryLazy
                                .Value
                                .Where(x => x.Key.Point == c.ZoneClusterMemberConsumption.PositionInCluster)
                                .First(x => x.Key.GrowthZoneTileInfo.Density == 0)
                                .Value
                                .ToQueryResult();
                        }
                        return _layerDictionaryLazy
                            .Value
                            .Where(x => x.Key.Point == c.ZoneClusterMemberConsumption.PositionInCluster)
                            .Where(x => x.Key.GrowthZoneTileInfo.Density != 0)
                            .Where(x => x.Key.GrowthZoneTileInfo.Density <= (c.BaseZoneClusterConsumption.PopulationDensity / maxPop) + 1)
                            .OrderByDescending(x => x.Key.GrowthZoneTileInfo.Density)
                            .ThenByDescending(x => x.Key.GrowthZoneTileInfo.Quality)
                            .First()
                            .Value
                            .ToQueryResult();
                    }, QueryResult<AnimatedCellBitmapSetLayers>.Empty);
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

        public QueryResult<AnimatedCellBitmapSetLayers> GetFor(IReadOnlyZoneInfo readOnlyZoneInfo)
        {
            return readOnlyZoneInfo
                .ZoneConsumptionState
                .Pipe(x => x.GetIfZoneClusterAndParent<StaticZoneClusterConsumption>())
                .Pipe(clusterMember => clusterMember.WithResultIfHasMatch(x =>
                    _layerDictionaryLazy
                        .Value
                        .Single(y => y.Key.Item1 == x.BaseZoneClusterConsumption.GetType() && y.Key.Item2 == x.ZoneClusterMemberConsumption.PositionInCluster)
                        .Value
                        .ToQueryResult()
                ,
                QueryResult<AnimatedCellBitmapSetLayers>.Empty)
            );
        }
    }
}
