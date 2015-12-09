using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirage.Urbanization.Vehicles;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Tilesets
{
    public interface ITilesetAccessor
    {
        int TileWidthAndSizeInPixels { get; set; }
        Size GetAreaSize(IReadOnlyArea area);
        QueryResult<AnimatedCellBitmapSetLayers> TryGetBitmapFor(ZoneInfoSnapshot snapShot);
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

        public QueryResult<AnimatedCellBitmapSetLayers> TryGetBitmapFor(ZoneInfoSnapshot snapShot)
        {
            return _staticZonesTileAccessor.GetFor(snapShot)
                .Pipe(result =>
                {
                    if (result.HasMatch)
                        return result;
                    else
                        return _growthZoneTileAccessor.GetFor(snapShot);
                })
                .Pipe(result =>
                {
                    if (result.HasMatch)
                        return result;
                    else
                        return _networkZoneTileAccessor.GetFor(snapShot);
                })
                .Pipe(result =>
                {
                    if (result.HasMatch)
                        return result;
                    return _intersectingZoneTileAccessor.GetFor(snapShot);
                });
        }

        private readonly GrowthZoneTileAccessor _growthZoneTileAccessor = new GrowthZoneTileAccessor();
        private readonly StaticZonesTileAccessor _staticZonesTileAccessor = new StaticZonesTileAccessor();
        private readonly NetworkZoneTileAccessor _networkZoneTileAccessor = new NetworkZoneTileAccessor();
        private readonly IntersectingZoneTileAccessor _intersectingZoneTileAccessor = new IntersectingZoneTileAccessor();

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
}
