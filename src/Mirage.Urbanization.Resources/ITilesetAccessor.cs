using System;
using System.Drawing;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization.Tilesets
{
    public interface ITilesetAccessor
    {
        int TileWidthAndSizeInPixels { get; set; }
        bool TryGetBitmapFor(IAreaZoneConsumption areaZoneConsumption, out BitmapLayer bitmap);
        Size ResizeToTileWidthAndSize(Size size);

        Size GetAreaSize(IReadOnlyArea area);
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