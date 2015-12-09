using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Tilesets
{
    class WaterZoneTileAccessor : BaseNetworkZoneTileAccessor<WaterZoneConsumption>
    {
        protected override string NetworkName => "Water";
        protected override bool IsConnected(IAreaZoneConsumption consumption)
        {
            return (consumption as IntersectingZoneConsumption)
                .ToQueryResult()
                .WithResultIfHasMatch(
                    x =>
                        x.NorthSouthZoneConsumption is WaterZoneConsumption ||
                        x.EastWestZoneConsumption is WaterZoneConsumption);
        }
    }
}