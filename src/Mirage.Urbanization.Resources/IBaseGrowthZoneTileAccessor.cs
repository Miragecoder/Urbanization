using System.Collections.Generic;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Tilesets
{
    interface IBaseGrowthZoneTileAccessor
    {
        QueryResult<AnimatedCellBitmapSetLayers> GetFor(ZoneInfoSnapshot snapShot);
        IEnumerable<AnimatedCellBitmapSetLayers> GetAll();
    }
}