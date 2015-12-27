using System.Collections.Generic;
using System.Drawing;
using Mirage.Urbanization.Vehicles;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Tilesets
{
    public interface ITilesetAccessor
    {
        int TileWidthAndSizeInPixels { get; set; }
        Size GetAreaSize(IReadOnlyArea area);
        QueryResult<AnimatedCellBitmapSetLayers> TryGetBitmapFor(ZoneInfoSnapshot snapShot, bool includeNoElectricity);
        IEnumerable<VehicleBitmapAndPoint> GetBitmapsAndPointsFor(IMoveableVehicle vehicle);
        Size ResizeToTileWidthAndSize(Size size);
        IEnumerable<AnimatedCellBitmapSetLayers> GetAll();
    }
}