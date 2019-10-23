using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirage.Urbanization.ZoneConsumption.Base;
using SixLabors.ImageSharp;

namespace Mirage.Urbanization.Tilesets.Vehicles
{
    public class VehicleBitmaps
    {
        public VehicleBitmaps()
        {
            var bitmaps = new EmbeddedBitmapExtractor()
                .GetBitmapsFromNamespace("Mirage.Urbanization.Tilesets.Vehicles")
                .ToList();

            Plane = new DirectionalBitmap(bitmaps
                .Single(x => string.Equals(x.FileName, "airplane", StringComparison.InvariantCultureIgnoreCase))
                .Bitmap
            );
            Train = new DirectionalBitmap(bitmaps
                .Single(x => string.Equals(x.FileName, "train", StringComparison.InvariantCultureIgnoreCase))
                .Bitmap
            );

            ShipFrameOne = new DirectionalBitmap(bitmaps
                .Single(x => string.Equals(x.FileName, "shipanim1", StringComparison.InvariantCultureIgnoreCase))
                .Bitmap
            );

            ShipFrameTwo = new DirectionalBitmap(bitmaps
                .Single(x => string.Equals(x.FileName, "shipanim2", StringComparison.InvariantCultureIgnoreCase))
                .Bitmap
            );

        }

        public DirectionalBitmap Plane { get; }
        public DirectionalBitmap Train { get; }

        public DirectionalBitmap GetShipBitmapFrame()
        {
            return DateTime.Now.Millisecond % 400 > 200 ? ShipFrameOne : ShipFrameTwo;
        }

        public IEnumerable<DirectionalBitmap> GetAllVehicleTiles()
        {
            yield return Plane;
            yield return Train;
            yield return ShipFrameOne;
            yield return ShipFrameTwo;
        }

        public DirectionalBitmap ShipFrameOne { get; }
        public DirectionalBitmap ShipFrameTwo { get; }
    }

    public class DirectionalBitmap
    {
        private readonly VehicleBitmap _bitmapEast
            , _bitmapNorth,
            _bitmapWest,
            _bitmapSouth,
            _southEast,
            _southWest,
            _northWest,
            _northEast;

        public DirectionalBitmap(Image bitmapEast)
        {
            _bitmapEast = new VehicleBitmap(bitmapEast);
            _bitmapSouth = new VehicleBitmap(_bitmapEast.Bitmap.RotateImage(45));
            _bitmapWest = new VehicleBitmap(_bitmapSouth.Bitmap.RotateImage(45));
            _bitmapNorth = new VehicleBitmap(_bitmapWest.Bitmap.RotateImage(45));

            _southEast = new VehicleBitmap(_bitmapEast.Bitmap.RotateImage(45));
            _southWest = new VehicleBitmap(_southEast.Bitmap.RotateImage(45));
            _northWest = new VehicleBitmap(_southWest.Bitmap.RotateImage(45));
            _northEast = new VehicleBitmap(_northWest.Bitmap.RotateImage(45));
        }

        public VehicleBitmap GetBitmap(Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.East:
                    return _bitmapEast;
                case Orientation.West:
                    return _bitmapWest;
                case Orientation.North:
                    return _bitmapNorth;
                case Orientation.South:
                    return _bitmapSouth;
                case Orientation.NorthEast:
                    return _northEast;
                case Orientation.NorthWest:
                    return _northWest;
                case Orientation.SouthEast:
                    return _southEast;
                case Orientation.SouthWest:
                    return _southWest;
            }
            throw new InvalidOperationException();
        }
    }
}
