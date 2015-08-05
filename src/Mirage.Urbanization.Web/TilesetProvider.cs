using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirage.Urbanization.Tilesets;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Web
{
    static class TilesetProvider
    {
        private static readonly ITilesetAccessor TilesetAccessor = new TilesetAccessor();

        private static readonly Dictionary<int, Bitmap> BitmapsByHashCode = new Dictionary<int, Bitmap>();

        public static Bitmap GetBitmapForHashcode(int hashCode)
        {
            return BitmapsByHashCode[hashCode];
        }

        public static int GetTilePathFor(
            IAreaZoneConsumption consumption, 
            Func<BitmapLayer, Bitmap> bitmapSelector)
        {
            BitmapLayer bitmapLayer;
            if (!TilesetAccessor.TryGetBitmapFor(consumption, out bitmapLayer))
                return default(int);

            var bitmap = bitmapSelector(bitmapLayer);
            if (bitmap == null)
                return default(int);

            if (!BitmapsByHashCode.ContainsKey(bitmap.GetHashCode()))
                BitmapsByHashCode.Add(bitmap.GetHashCode(), bitmap);

            return bitmap.GetHashCode();
        }
    }
}
