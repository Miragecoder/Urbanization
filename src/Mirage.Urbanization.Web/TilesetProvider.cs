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

        private static readonly Dictionary<int, BaseBitmap> BitmapsById = new Dictionary<int, BaseBitmap>();

        public static BaseBitmap GetBitmapForId(int id)
        {
            return BitmapsById[id];
        }

        public static IEnumerable<VehicleBitmapAndPoint> GetBitmapsAndPointsFor(IMoveableVehicle vehicle)
        {
            foreach (VehicleBitmapAndPoint x in TilesetAccessor.GetBitmapsAndPointsFor(vehicle))
            {
                RegisterBitmap(x.Bitmap);

                yield return x;
            }
        }

        private static void RegisterBitmap(BaseBitmap bitmap)
        {
            if (!BitmapsById.ContainsKey(bitmap.Id))
                BitmapsById.Add(bitmap.Id, bitmap);
        }

        public static ClientAnimatedCellBitmapSet GetAnimatedCellBitmapSetIdFor(
            IReadOnlyZoneInfo zoneInfo,
            Func<AnimatedCellBitmapSetLayers, AnimatedCellBitmapSet> bitmapSelector)
        {
            var result = TilesetAccessor.TryGetBitmapFor(snapShot: zoneInfo.TakeSnapshot(), includeNoElectricity: true);

            if (result.HasNoMatch)
                return default(ClientAnimatedCellBitmapSet);

            var animatedCellBitmapSet = bitmapSelector(result.MatchingObject);
            if (animatedCellBitmapSet == null)
                return default(ClientAnimatedCellBitmapSet);

            foreach (var x in animatedCellBitmapSet.Bitmaps)
                RegisterBitmap(x);

            return new ClientAnimatedCellBitmapSet(
                animatedCellBitmapSet.Id,
                animatedCellBitmapSet.Delay,
                animatedCellBitmapSet.Bitmaps.Select(x => x.Id).ToArray());
        }
    }

    public class ClientAnimatedCellBitmapSet
    {
        public ClientAnimatedCellBitmapSet(int id, int delay, int[] bitmapIds)
        {
            this.id = id;
            this.bitmapIds = bitmapIds;
            this.delay = delay;
        }

        public int id { get; }
        public int[] bitmapIds { get; }
        public int delay { get; }

        public string GetIdentityString() => $"{id}_{string.Join("|", bitmapIds)}";
    }
}
