using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirage.Urbanization.ZoneConsumption.Base;

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

        public DirectionalBitmap ShipFrameOne { get; }
        public DirectionalBitmap ShipFrameTwo { get; }
    }

    public class DirectionalBitmap
    {
        private readonly Bitmap _bitmapEast
            , _bitmapNorth,
            _bitmapWest,
            _bitmapSouth,
            _southEast,
            _southWest,
            _northWest,
            _northEast;

        public DirectionalBitmap(Bitmap bitmapEast)
        {
            _bitmapEast = bitmapEast;
            _bitmapSouth = _bitmapEast.Get90DegreesRotatedClone();
            _bitmapWest = _bitmapSouth.Get90DegreesRotatedClone();
            _bitmapNorth = _bitmapWest.Get90DegreesRotatedClone();

            _southEast = _bitmapEast.RotateImage(45);
            _southWest = _southEast.Get90DegreesRotatedClone();
            _northWest = _southWest.Get90DegreesRotatedClone();
            _northEast = _northWest.Get90DegreesRotatedClone();
        }

        public Bitmap GetBitmap(Orientation orientation)
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
