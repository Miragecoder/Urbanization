using Mirage.Urbanization.Tilesets;
using Mirage.Urbanization.Vehicles;

namespace Mirage.Urbanization.Web
{
    public struct ClientZoneInfo
    {
        public static ClientZoneInfo Create(IReadOnlyZoneInfo zoneInfo)
            => new ClientZoneInfo
            {
                key = $"{zoneInfo.Point.X}_{zoneInfo.Point.Y}",
                bitmapLayerOne = TilesetProvider
                    .GetTilePathFor(zoneInfo.ZoneConsumptionState.GetZoneConsumption(), x => x.LayerOne),
                bitmapLayerTwo = TilesetProvider
                    .GetTilePathFor(zoneInfo.ZoneConsumptionState.GetZoneConsumption(), x => x.LayerTwo),
                point = ClientZonePoint.Create(zoneInfo.Point),
                color = zoneInfo.ZoneConsumptionState.GetZoneConsumption().ColorName,
            };

        public string key { get; set; }
        public int bitmapLayerOne { get; set; }
        public int bitmapLayerTwo { get; set; }
        public ClientZonePoint point { get; set; }
        public string color { get; set; }
        public string GetIdentityString() => $"{key}_{bitmapLayerOne}_{bitmapLayerTwo}_{point.GetIdentityString()}_{color}";
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