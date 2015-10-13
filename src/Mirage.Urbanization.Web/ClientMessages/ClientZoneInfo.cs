using System.Linq;
using Mirage.Urbanization.Simulation.Datameters;
using Mirage.Urbanization.Tilesets;
using Mirage.Urbanization.Vehicles;

namespace Mirage.Urbanization.Web
{
    public struct ClientDataMeterResult
    {
        public string name { get; set; }
        public string level { get; set; }
        public string colour { get; set; }

        public string GetIdentityString() => $"{name}_{level}";
    }

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
                dataMeterResults = DataMeterInstances
                    .DataMeters
                    .Select(x => x.GetDataMeterResult(zoneInfo))
                    .Select(x => new ClientDataMeterResult()
                    {
                        name = x.Name,
                        level = x.ValueCategory.ToString(),
                        colour = BrushManager
                            .Instance
                            .GetBrushFor(x.ValueCategory)
                            .WithResultIfHasMatch(brush => System.Drawing.ColorTranslator.ToHtml(brush.Color), string.Empty)
                    })
                    .ToArray()
            };

        public string key { get; set; }
        public int bitmapLayerOne { get; set; }
        public int bitmapLayerTwo { get; set; }
        public ClientZonePoint point { get; set; }
        public string color { get; set; }
        public string GetIdentityString() => $"{key}_{bitmapLayerOne}_{bitmapLayerTwo}_{point.GetIdentityString()}_{color}_({string.Join("|", dataMeterResults.Select(x => x.GetIdentityString()))})";

        public ClientDataMeterResult[] dataMeterResults { get; set; }
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