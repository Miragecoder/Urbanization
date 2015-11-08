using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Mirage.Urbanization.Vehicles;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Tilesets
{
    public interface ITilesetAccessor
    {
        int TileWidthAndSizeInPixels { get; set; }
        QueryResult<BitmapLayer> TryGetBitmapFor(IAreaZoneConsumption consumption);
        Size ResizeToTileWidthAndSize(Size size);

        Size GetAreaSize(IReadOnlyArea area);
        IEnumerable<VehicleBitmapAndPoint> GetBitmapsAndPointsFor(IMoveableVehicle vehicle, MiscBitmaps miscBitmapsInstance);
    }

    public class VehicleBitmapAndPoint
    {
        public VehicleBitmapAndPoint(
            Bitmap bitmap,
            IReadOnlyZoneInfo second,
            IReadOnlyZoneInfo third,
            IVehicle vehicle)
        {
            Bitmap = bitmap;
            Second = second;
            Third = third;
            Vehicle = vehicle;
        }

        public IReadOnlyZoneInfo Second { get; }
        public IReadOnlyZoneInfo Third { get; }
        public Bitmap Bitmap { get; }
        public IVehicle Vehicle { get; }
    }

    public class BitmapInfo
    {
        public Bitmap BitmapSegment { get; }
        public Bitmap BitmapParent { get; }
        public AnimatedBitmap ParentAnimatedBitmap { get; }
        public IRoadNetworkZoneTileset ParentAnimatedRoadNetworkZoneTileset { get; }

        public BitmapInfo(Bitmap bitmap) : this(bitmap, bitmap)
        {
        }

        public BitmapInfo(Bitmap bitmapSegment, Bitmap parent)
        {
            if (bitmapSegment == null) throw new ArgumentNullException(nameof(bitmapSegment));
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            BitmapSegment = bitmapSegment;
            BitmapParent = parent;
        }

        public BitmapInfo(Bitmap bitmapSegment, Bitmap parent, AnimatedBitmap parentAnimatedBitmap)
            : this(bitmapSegment, parent)
        {
            if (parentAnimatedBitmap == null) throw new ArgumentNullException(nameof(parentAnimatedBitmap));
            ParentAnimatedBitmap = parentAnimatedBitmap;
        }

        public BitmapInfo(Bitmap bitmapSegment, Bitmap parent, IRoadNetworkZoneTileset parentAnimatedRoadNetworkZoneTileset)
             : this(bitmapSegment, parent)
        {
            if (parentAnimatedRoadNetworkZoneTileset == null) throw new ArgumentNullException(nameof(parentAnimatedRoadNetworkZoneTileset));
            ParentAnimatedRoadNetworkZoneTileset = parentAnimatedRoadNetworkZoneTileset;
        }
    }


    public class BitmapLayer
    {
        public BitmapLayer(BitmapInfo layerOne, BitmapInfo layerTwo = null)
        {
            if (layerOne == null) throw new ArgumentNullException(nameof(layerOne), "The bitmap specified in 'layerOne' is not allowed to be null.");
            LayerOne = layerOne;
            LayerTwo = layerTwo;
        }

        public BitmapInfo LayerOne { get; }

        public bool IsLayerTwoSpecified => LayerTwo != null;
        public BitmapInfo LayerTwo { get; }
    }
}