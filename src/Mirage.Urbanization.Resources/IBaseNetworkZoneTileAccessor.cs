using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Tilesets
{
    interface IBaseNetworkZoneTileAccessor
    {
        QueryResult<AnimatedCellBitmapSetLayers> GetFor(ZoneInfoSnapshot snapshot);
    }
}