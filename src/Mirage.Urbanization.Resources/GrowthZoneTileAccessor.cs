using System.Linq;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Tilesets
{
    class GrowthZoneTileAccessor
    {

        private readonly IBaseGrowthZoneTileAccessor[] _accessors =
        {
            new ResidentialGrowthZoneTileAccessor(),
            new IndustrialGrowthZoneTileAccessor(),
            new CommercialGrowthZoneTileAccessor()
        };

        public QueryResult<AnimatedCellBitmapSetLayers> GetFor(ZoneInfoSnapshot snapShot)
        {
            return _accessors
                .Select(x => x.GetFor(snapShot))
                .SingleOrDefault(x => x.HasMatch)
                .Pipe(match =>
                {
                    if (match != null) return match;
                    return QueryResult<AnimatedCellBitmapSetLayers>.Empty;
                });
        }
    }
}