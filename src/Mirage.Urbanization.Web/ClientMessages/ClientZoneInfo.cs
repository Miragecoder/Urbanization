using System.Linq;
using Mirage.Urbanization.Simulation.Datameters;
using Mirage.Urbanization.Tilesets;
using Mirage.Urbanization.Vehicles;

namespace Mirage.Urbanization.Web
{
    public struct ClientDataMeterResult
    {
        public int webId { get; set; }
        public string colour { get; set; }

        public string GetIdentityString() => $"{webId}_{colour}";
    }

    public struct ClientZoneInfo
    {
        public static ClientZoneInfo Create(IReadOnlyZoneInfo zoneInfo)
            => new ClientZoneInfo
            {
                bitmapLayerOne = TilesetProvider
                    .GetTilePathFor(zoneInfo.ZoneConsumptionState.GetZoneConsumption(), x => x.LayerOne),
                bitmapLayerTwo = TilesetProvider
                    .GetTilePathFor(zoneInfo.ZoneConsumptionState.GetZoneConsumption(), x => x.LayerTwo),
                x = zoneInfo.Point.X,
                y = zoneInfo.Point.Y,
                dataMeterResults = DataMeterInstances
                    .DataMeters
                    .Select(x => x.GetDataMeterResult(zoneInfo))
                    .Select(x => new ClientDataMeterResult()
                    {
                        webId = x.WebId,
                        colour = BrushManager
                            .Instance
                            .GetBrushFor(x.ValueCategory)
                            .WithResultIfHasMatch(brush => System.Drawing.ColorTranslator.ToHtml(brush.Color), string.Empty)
                    })
                    .ToArray()
            };
        
        public int bitmapLayerOne { get; set; }
        public int bitmapLayerTwo { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public string GetIdentityString() => $"{x}_{y}_{bitmapLayerOne}_{bitmapLayerTwo}_({string.Join("|", dataMeterResults.Select(meter => meter.GetIdentityString()))})";

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