using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirage.Urbanization.Tilesets.Vehicles;
using Mirage.Urbanization.Vehicles;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Tilesets
{
    public class TilesetAccessor : ITilesetAccessor
    {
        public int TileWidthAndSizeInPixels { get; set; } = 25;
        public Size GetAreaSize(IReadOnlyArea area)
        {
            return new Size(area.AmountOfZonesX * TileWidthAndSizeInPixels, area.AmountOfZonesY * TileWidthAndSizeInPixels);
        }

        public QueryResult<AnimatedCellBitmapSetLayers> TryGetNoElectricityIcon(ZoneInfoSnapshot snapShot)
        {
            return (snapShot.AreaZoneConsumption as ZoneClusterMemberConsumption)
                .ToQueryResult()
                .WithResultIfHasMatch(x =>
                {
                    if (x.IsCentralClusterMember && !x.ParentBaseZoneClusterConsumption.HasPower && DateTime.Now.Millisecond > 500)
                        return _noElectricity.ToQueryResult();
                    return QueryResult<AnimatedCellBitmapSetLayers>.Empty;
                }, QueryResult<AnimatedCellBitmapSetLayers>.Empty);
        } 

        public QueryResult<AnimatedCellBitmapSetLayers> TryGetBitmapFor(ZoneInfoSnapshot snapShot, bool includeNoElectricity)
        {
            if (includeNoElectricity)
            {
                var x = TryGetNoElectricityIcon(snapShot);
                if (x.HasMatch)
                    return x;
            }

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
                })
                .Pipe(result =>
                {
                    if (result.HasMatch)
                        return result;
                    if (snapShot.AreaZoneConsumption is RubbishZoneConsumption)
                        return _rubbish.ToQueryResult();

                    return QueryResult<AnimatedCellBitmapSetLayers>.Empty;
                });
        }

        private readonly GrowthZoneTileAccessor _growthZoneTileAccessor = new GrowthZoneTileAccessor();
        private readonly StaticZonesTileAccessor _staticZonesTileAccessor = new StaticZonesTileAccessor();
        private readonly NetworkZoneTileAccessor _networkZoneTileAccessor = new NetworkZoneTileAccessor();
        private readonly IntersectingZoneTileAccessor _intersectingZoneTileAccessor = new IntersectingZoneTileAccessor();
        private readonly VehicleBitmaps _vehicleBitmaps = new VehicleBitmaps();

        private readonly AnimatedCellBitmapSetLayers _rubbish = new EmbeddedBitmapExtractor()
            .GetBitmapsFromNamespace("Mirage.Urbanization.Tilesets.Tiles.Misc.rubbish")
            .ToList()
            .Single()
            .Bitmap.Pipe(bitmap =>
            {
                return
                    new AnimatedCellBitmapSetLayers(
                        new AnimatedCellBitmapSet(FramerateDelay.None, new[] {bitmap}.Select(x => new CellBitmap(x)).ToArray()), null);
            });

        private readonly AnimatedCellBitmapSetLayers _noElectricity = new EmbeddedBitmapExtractor()
            .GetBitmapsFromNamespace("Mirage.Urbanization.Tilesets.Tiles.Misc.noelectricity.png")
            .ToList()
            .Single()
            .Bitmap.Pipe(bitmap =>
            {
                return
                    new AnimatedCellBitmapSetLayers(
                        new AnimatedCellBitmapSet(FramerateDelay.None, new[] { bitmap }.Select(x => new CellBitmap(x)).ToArray()), null);
            });

        public IEnumerable<VehicleBitmapAndPoint> GetBitmapsAndPointsFor(IMoveableVehicle vehicle)
        {
            if (vehicle.PreviousPreviousPreviousPreviousPosition == null)
                yield break;

            foreach (var pair in new[]
            {
                new
                {
                    Render = (vehicle is ITrain),
                    First = vehicle.CurrentPosition,
                    Second = vehicle.PreviousPosition,
                    Third = vehicle.PreviousPreviousPosition,
                    Head = true
                },
                new
                {
                    Render = true,
                    First = vehicle.PreviousPosition,
                    Second = vehicle.PreviousPreviousPosition,
                    Third = vehicle.PreviousPreviousPreviousPosition,
                    Head = false
                },
                new
                {
                    Render = (vehicle is ITrain),
                    First = vehicle.PreviousPreviousPosition,
                    Second = vehicle.PreviousPreviousPreviousPosition,
                    Third = vehicle.PreviousPreviousPreviousPreviousPosition,
                    Head = false
                }
            })
            {
                var orientation = (pair.Third.Point != pair.First.Point)
                    ? pair.Third.Point.OrientationTo(pair.First.Point)
                    : pair.Second.Point.OrientationTo(pair.First.Point);

                Bitmap bitmap;

                if (vehicle is IAirplane)
                    bitmap = _vehicleBitmaps.Plane.GetBitmap(orientation);
                else if (vehicle is ITrain)
                    bitmap = _vehicleBitmaps.Train.GetBitmap(orientation);
                else if (vehicle is IShip)
                    bitmap = _vehicleBitmaps.GetShipBitmapFrame().GetBitmap(orientation);
                else
                    throw new InvalidOperationException();

                if (pair.Render)
                {
                    yield return new VehicleBitmapAndPoint(new VehicleBitmap(bitmap), pair.Second, pair.Third, vehicle);
                }
            }
        }

        public Size ResizeToTileWidthAndSize(Size size)
        {
            decimal resizeMultiplier = (decimal)TileWidthAndSizeInPixels / 25;
            return new Size(
                Convert.ToInt32(Math.Round(size.Width * resizeMultiplier)),
                Convert.ToInt32(Math.Round(size.Height * resizeMultiplier))
            );
        }

        public IEnumerable<AnimatedCellBitmapSetLayers> GetAll()
        {
            return _growthZoneTileAccessor.GetAll()
                .Concat(_intersectingZoneTileAccessor.GetAll())
                .Concat(_networkZoneTileAccessor.GetAll())
                .Concat(_staticZonesTileAccessor.GetAll())
                .Concat(new[]
                {
                    _noElectricity,
                    _rubbish
                });
        }
    }
}
