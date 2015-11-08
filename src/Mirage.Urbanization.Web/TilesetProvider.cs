using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirage.Urbanization.Tilesets;
using Mirage.Urbanization.Vehicles;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Web
{
    static class TilesetProvider
    {
        private static readonly ITilesetAccessor TilesetAccessor = new TilesetAccessor();
        private static readonly MiscBitmaps MiscBitmaps = new MiscBitmaps();

        private static readonly Dictionary<int, Bitmap> BitmapsByHashCode = new Dictionary<int, Bitmap>();

        public static Bitmap GetBitmapForHashcode(int hashCode)
        {
            return BitmapsByHashCode[hashCode];
        }

        public static IEnumerable<VehicleBitmapAndPoint> GetBitmapsAndPointsFor(IMoveableVehicle vehicle)
        {
            foreach (var x in TilesetAccessor.GetBitmapsAndPointsFor(vehicle, MiscBitmaps))
            {
                RegisterBitmap(x.Bitmap);

                yield return x;
            }
        }

        private static void RegisterBitmap(Bitmap bitmap)
        {
            if (!BitmapsByHashCode.ContainsKey(bitmap.GetHashCode()))
                BitmapsByHashCode.Add(bitmap.GetHashCode(), bitmap);
        }

        public static int GetTilePathFor(
            IAreaZoneConsumption consumption, 
            Func<BitmapLayer, Bitmap> bitmapSelector)
        {
            var result = TilesetAccessor.TryGetBitmapFor(consumption);

            if (result.HasNoMatch)
                return default(int);

            var bitmap = bitmapSelector(result.MatchingObject);
            if (bitmap == null)
                return default(int);

            RegisterBitmap(bitmap);

            return bitmap.GetHashCode();
        }
    }
}
