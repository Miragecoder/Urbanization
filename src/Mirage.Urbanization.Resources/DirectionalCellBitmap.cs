using System;

namespace Mirage.Urbanization.Tilesets
{
    public class DirectionalCellBitmap
    {
        public AnimatedCellBitmapSetLayers Up { get; }
        public AnimatedCellBitmapSetLayers Down { get; }
        public AnimatedCellBitmapSetLayers Right { get; }
        public AnimatedCellBitmapSetLayers Left { get; }

        public DirectionalCellBitmap(AnimatedCellBitmapSetLayers cellBitmapSet)
        {
            if (cellBitmapSet == null) throw new ArgumentNullException(nameof(cellBitmapSet));
            Up = cellBitmapSet;
            Right = Up.Generate90DegreesRotatedClone();
            Down = Right.Generate90DegreesRotatedClone();
            Left = Down.Generate90DegreesRotatedClone();
        }
    }
}