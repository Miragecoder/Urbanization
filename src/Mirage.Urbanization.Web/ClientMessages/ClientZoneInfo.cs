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
                point = new ClientZonePoint
                {
                    x = zoneInfo.Point.X,
                    y = zoneInfo.Point.Y
                },
                color = zoneInfo.ZoneConsumptionState.GetZoneConsumption().ColorName,
            };

        public string key { get; set; }
        public int bitmapLayerOne { get; set; }
        public int bitmapLayerTwo { get; set; }
        public ClientZonePoint point { get; set; }
        public string color { get; set; }
        public string GetIdentityString() => $"{key}_{bitmapLayerOne}_{bitmapLayerTwo}_{point.GetIdentityString()}_{color}";
    }
}