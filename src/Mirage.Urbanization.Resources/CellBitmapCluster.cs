using System.Collections.Generic;
using System.Drawing;

namespace Mirage.Urbanization.Tilesets
{
    public class CellBitmapCluster
    {
        public IDictionary<Point, AnimatedCellBitmapSetLayers> Bitmaps { get; }

        public CellBitmapCluster(IDictionary<Point, AnimatedCellBitmapSetLayers> bitmaps)
        {
            Bitmaps = bitmaps;
        }
    }
}