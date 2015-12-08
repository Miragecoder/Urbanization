using System.Linq;
using Mirage.Urbanization.Simulation.Datameters;
using Mirage.Urbanization.Tilesets;
using Mirage.Urbanization.Vehicles;

namespace Mirage.Urbanization.Web
{

    public struct ClientZoneInfo
    {
        public static ClientZoneInfo Create(IReadOnlyZoneInfo zoneInfo)
            => new ClientZoneInfo
            {
                bitmapLayerOne = TilesetProvider
                    .GetTilePathFor(zoneInfo, x => x.LayerOne),
                bitmapLayerTwo = TilesetProvider
                    .GetTilePathFor(zoneInfo, x => x.LayerTwo.WithResultIfHasMatch(y => y)),
                x = zoneInfo.Point.X,
                y = zoneInfo.Point.Y
            };
        
        public int bitmapLayerOne { get; set; }
        public int bitmapLayerTwo { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public string GetIdentityString() => $"{x}_{y}_{bitmapLayerOne}_{bitmapLayerTwo}";
    }

    public struct ClientVehiclePositionInfo
    {
        public ClientZonePoint pointOne { get; set; }
        public ClientZonePoint pointTwo { get; set; }
        public int bitmapId { get; set; }
        public bool isShip;

        public static ClientVehiclePositionInfo Create(VehicleBitmapAndPoint point) =>
            new ClientVehiclePositionInfo()
            {
                pointOne = ClientZonePoint.Create(point.Second.Point),
                pointTwo = ClientZonePoint.Create(point.Third.Point),
                bitmapId = point.Bitmap.GetHashCode(),
                isShip = point.Vehicle is IShip
            };
    }
}