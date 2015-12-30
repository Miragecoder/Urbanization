using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        public static readonly TextureAtlas TextureAtlas = new TextureAtlas();
        
        public static IEnumerable<VehicleBitmapAndPoint> GetBitmapsAndPointsFor(IMoveableVehicle vehicle)
        {
            foreach (VehicleBitmapAndPoint x in TextureAtlas.TilesetAccessor.GetBitmapsAndPointsFor(vehicle))
            {
                yield return x;
            }
        }

        public static ClientAnimatedCellBitmapSet GetAnimatedCellBitmapSetIdFor(
            IReadOnlyZoneInfo zoneInfo,
            Func<AnimatedCellBitmapSetLayers, AnimatedCellBitmapSet> bitmapSelector)
        {
            var result = TextureAtlas.TilesetAccessor.TryGetBitmapFor(snapShot: zoneInfo.TakeSnapshot(), includeNoElectricity: true);

            if (result.HasNoMatch)
                return default(ClientAnimatedCellBitmapSet);

            var animatedCellBitmapSet = bitmapSelector(result.MatchingObject);
            if (animatedCellBitmapSet == null)
                return default(ClientAnimatedCellBitmapSet);

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
