using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirage.Urbanization.Tilesets.Tiles.Clusters.StaticZones;
using Mirage.Urbanization.Vehicles;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Tilesets
{
    public interface ITilesetAccessor
    {
        int TileWidthAndSizeInPixels { get; set; }
        Size GetAreaSize(IReadOnlyArea area);
        QueryResult<AnimatedCellBitmapSetLayers> TryGetBitmapFor(IReadOnlyZoneInfo zoneInfo);
        IEnumerable<VehicleBitmapAndPoint> GetBitmapsAndPointsFor(IMoveableVehicle vehicle);
        Size ResizeToTileWidthAndSize(Size size);
    }
    public class TilesetAccessor : ITilesetAccessor
    {
        public int TileWidthAndSizeInPixels { get; set; } = 25;
        public Size GetAreaSize(IReadOnlyArea area)
        {
            return new Size(area.AmountOfZonesX * TileWidthAndSizeInPixels, area.AmountOfZonesY * TileWidthAndSizeInPixels);
        }

        public QueryResult<AnimatedCellBitmapSetLayers> TryGetBitmapFor(IReadOnlyZoneInfo zoneInfo)
        {
            return _staticZonesTileAccessor.GetFor(zoneInfo);
        }

        private readonly StaticZonesTileAccessor _staticZonesTileAccessor = new StaticZonesTileAccessor();

        public IEnumerable<VehicleBitmapAndPoint> GetBitmapsAndPointsFor(IMoveableVehicle vehicle)
        {
            throw new NotImplementedException();
        }

        public Size ResizeToTileWidthAndSize(Size size)
        {
            decimal resizeMultiplier = (decimal)TileWidthAndSizeInPixels / 25;
            return new Size(
                Convert.ToInt32(Math.Round(size.Width * resizeMultiplier)),
                Convert.ToInt32(Math.Round(size.Height * resizeMultiplier))
            );
        }
    }

    public static class AnimatedCellBitmapSetLayersDefinitions
    {
        private static IReadOnlyCollection<AnimatedCellBitmapSetLayers> _definitions = new[]
        {
            new AnimatedCellBitmapSetLayers(null,null)
        }.ToList();

        public static IEnumerable<AnimatedCellBitmapSetLayers> Definitions => _definitions;
    }
}
