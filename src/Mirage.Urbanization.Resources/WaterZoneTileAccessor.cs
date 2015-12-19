using System.Collections.Generic;
using System.Linq;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Tilesets
{
    class WaterZoneTileAccessor : BaseNetworkZoneTileAccessor<WaterZoneConsumption>
    {
        protected override string NetworkName => "Water";
        protected override bool IsConnected(IAreaZoneConsumption consumption) => TryIsConnected(consumption).Any(x => x);

        private static IEnumerable<bool> TryIsConnected(IAreaZoneConsumption consumption)
        {
            yield return (consumption as ZoneClusterMemberConsumption)
                .ToQueryResult()
                .WithResultIfHasMatch(x => x.ParentBaseZoneClusterConsumption is SeaPortZoneClusterConsumption);
            yield return (consumption as IntersectingZoneConsumption)
                .ToQueryResult()
                .WithResultIfHasMatch(
                    x =>
                        x.NorthSouthZoneConsumption is WaterZoneConsumption ||
                        x.EastWestZoneConsumption is WaterZoneConsumption);
        }

        protected override bool ConnectToEdgeOfMap => true;
    }
}