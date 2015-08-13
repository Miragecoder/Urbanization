namespace Mirage.Urbanization.Web
{
    public static class ReadOnlyZoneInfoExtensions
    {
        public static ClientZoneInfo ToClientZoneInfo(this IReadOnlyZoneInfo zoneInfo)
        {
            return new ClientZoneInfo
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
        }
    }
}