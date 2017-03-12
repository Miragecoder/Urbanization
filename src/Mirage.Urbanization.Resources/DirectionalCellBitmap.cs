using System;
using System.Collections.Generic;

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
            Up = cellBitmapSet ?? throw new ArgumentNullException(nameof(cellBitmapSet));
            Right = Up.Generate90DegreesRotatedClone();
            Down = Right.Generate90DegreesRotatedClone();
            Left = Down.Generate90DegreesRotatedClone();
        }

        public IEnumerable<AnimatedCellBitmapSetLayers> GetAll()
        {
            yield return Up;
            yield return Down;
            yield return Left;
            yield return Right;
        }
    }
}