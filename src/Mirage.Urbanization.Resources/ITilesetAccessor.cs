using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Mirage.Urbanization.Vehicles;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization.Tilesets
{
    public interface ITilesetAccessor
    {
        int TileWidthAndSizeInPixels { get; set; }
        bool TryGetBitmapFor(IAreaZoneConsumption areaZoneConsumption, out BitmapLayer bitmap);
        Size ResizeToTileWidthAndSize(Size size);

        Size GetAreaSize(IReadOnlyArea area);
        IEnumerable<VehicleBitmapAndPoint> GetBitmapsAndPointsFor(IMoveableVehicle airplane, MiscBitmaps miscBitmapsInstance);
    }

    public class VehicleBitmapAndPoint
    {
        public VehicleBitmapAndPoint(Bitmap bitmap, IReadOnlyZoneInfo second, IReadOnlyZoneInfo third)
        {
            Bitmap = bitmap;
            Second = second;
            Third = third;
        }

        public IReadOnlyZoneInfo Second { get; }
        public IReadOnlyZoneInfo Third { get; }
        public Bitmap Bitmap { get; }
    }

    public class BitmapLayerOperation
    {

    }

    public class BitmapLayer
    {
        public BitmapLayer(Bitmap layerOne, Bitmap layerTwo = null)
        {
            if (layerOne == null) throw new ArgumentNullException(nameof(layerOne), "The bitmap specified in 'layerOne' is not allowed to be null.");
            LayerOne = layerOne;
            LayerTwo = layerTwo;
        }

        public Bitmap LayerOne { get; }

        public bool IsLayerTwoSpecified => LayerTwo != null;
        public Bitmap LayerTwo { get; }
    }
}