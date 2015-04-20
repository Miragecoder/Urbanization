using System.Drawing;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization.Tilesets
{
    public interface ITilesetAccessor
    {
        int TileWidthAndSizeInPixels { get; set; }
        bool TryGetBitmapFor(IAreaZoneConsumption areaZoneConsumption, out Bitmap bitmap);

        Size GetAreaSize(IReadOnlyArea area);
    }
}