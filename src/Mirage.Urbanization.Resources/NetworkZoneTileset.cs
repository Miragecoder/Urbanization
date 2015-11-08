using System;
using System.Collections.Generic;
using System.Drawing;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization.Tilesets
{
    public class NetworkZoneTileset : INetworkZoneTileset
    {
        private readonly Bitmap _bitmapEastWest, _bitmapNorthWest, _bitmapNorthWestEast, _bitmapNorthEastSouthWest, _bitmapEast, _bitmapNoDirection;

        private readonly Lazy<Bitmap>
            _bitmapNorthSouth,

            _bitmapNorthEast,
            _bitmapSouthWest,
            _bitmapSouthEast,

            _bitmapNorthEastSouth,
            _bitmapEastSouthWest,
            _bitmapSouthWestNorth,

            _bitmapNorth,
            _bitmapWest,
            _bitmapSouth;

        public NetworkZoneTileset(
            Bitmap bitmapEastWest, 
            Bitmap bitmapNorthWest, 
            Bitmap bitmapWestNorthEast, 
            Bitmap bitmapNorthEastSouthWest, 
            Bitmap bitmapEast, 
            Bitmap bitmapNoDirection)
        {
            _bitmapNorthEastSouthWest = bitmapNorthEastSouthWest;

            _bitmapEastWest = bitmapEastWest;
            _bitmapNorthSouth = new Lazy<Bitmap>(() => CloneBitmap(bitmapEastWest, RotateFlipType.Rotate90FlipNone));

            _bitmapNorthWest = bitmapNorthWest;
            _bitmapNorthEast = new Lazy<Bitmap>(() => CloneBitmap(bitmapNorthWest, RotateFlipType.Rotate90FlipNone));
            _bitmapSouthWest = new Lazy<Bitmap>(() => CloneBitmap(bitmapNorthWest, RotateFlipType.Rotate270FlipNone));
            _bitmapSouthEast = new Lazy<Bitmap>(() => CloneBitmap(bitmapNorthWest, RotateFlipType.Rotate180FlipNone));

            _bitmapNorthWestEast = bitmapWestNorthEast;

            _bitmapNorthEastSouth = new Lazy<Bitmap>(() => CloneBitmap(_bitmapNorthWestEast, RotateFlipType.Rotate90FlipNone));
            _bitmapEastSouthWest = new Lazy<Bitmap>(() => CloneBitmap(_bitmapNorthWestEast, RotateFlipType.Rotate180FlipNone));
            _bitmapSouthWestNorth = new Lazy<Bitmap>(() => CloneBitmap(_bitmapNorthWestEast, RotateFlipType.Rotate270FlipNone));

            _bitmapEast = bitmapEast;
            _bitmapNoDirection = bitmapNoDirection;

            _bitmapSouth = new Lazy<Bitmap>(() => CloneBitmap(_bitmapEast, RotateFlipType.Rotate90FlipNone));
            _bitmapWest = new Lazy<Bitmap>(() => CloneBitmap(_bitmapEast, RotateFlipType.Rotate180FlipNone));
            _bitmapNorth = new Lazy<Bitmap>(() => CloneBitmap(_bitmapEast, RotateFlipType.Rotate270FlipNone));
        }

        private static Bitmap CloneBitmap(Bitmap bitmap, RotateFlipType rotateFlipType)
        {
            var clone = bitmap.Clone() as Bitmap;
            clone.RotateFlip(rotateFlipType);
            return clone;
        }
        

        private Bitmap GetBitmapFor(BaseNetworkZoneConsumption networkConsumption)
        {
            var orientation = networkConsumption.GetOrientation();

            if (orientation.HasValue)
            {
                switch (orientation.Value)
                {
                    case Orientation.East:
                        return _bitmapEast;
                    case Orientation.North:
                        return _bitmapNorth.Value;
                    case Orientation.West:
                        return _bitmapWest.Value;
                    case Orientation.South:
                        return _bitmapSouth.Value;

                    case Orientation.NorthSouth:
                        return _bitmapNorthSouth.Value;
                    case Orientation.EastWest:
                        return _bitmapEastWest;

                    case Orientation.NorthEast:
                        return _bitmapNorthEast.Value;
                    case Orientation.NorthWest:
                        return _bitmapNorthWest;
                    case Orientation.SouthWest:
                        return _bitmapSouthWest.Value;
                    case Orientation.SouthEast:
                        return _bitmapSouthEast.Value;

                    case Orientation.NorthWestEast:
                        return _bitmapNorthWestEast;
                    case Orientation.NorthEastSouth:
                        return _bitmapNorthEastSouth.Value;
                    case Orientation.EastSouthWest:
                        return _bitmapEastSouthWest.Value;
                    case Orientation.SouthWestNorth:
                        return _bitmapSouthWestNorth.Value;

                    case Orientation.AllDirections:
                        return _bitmapNorthEastSouthWest;
                }
            }
            else
                return _bitmapNoDirection;
            throw new InvalidOperationException();
        }

        public BitmapInfo GetBitmapInfoFor(BaseNetworkZoneConsumption intersection)
        {
            return GetBitmapFor(intersection).ToBitmapInfo();
        }
    }
}