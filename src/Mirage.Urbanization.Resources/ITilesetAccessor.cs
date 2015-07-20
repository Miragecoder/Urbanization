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
        private readonly Bitmap _layerOne;
        private readonly Bitmap _layerTwo;

        public BitmapLayer(Bitmap layerOne, Bitmap layerTwo = null)
        {
            if (layerOne == null) throw new ArgumentNullException(nameof(layerOne), "The bitmap specified in 'layerOne' is not allowed to be null.");
            _layerOne = layerOne;
            _layerTwo = layerTwo;
        }

        public Bitmap LayerOne => _layerOne;

        public bool IsLayerTwoSpecified => _layerTwo != null;
        public Bitmap LayerTwo => _layerTwo;
    }
}