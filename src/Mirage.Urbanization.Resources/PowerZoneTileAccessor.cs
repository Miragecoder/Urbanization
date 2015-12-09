using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization.Tilesets
{
    class PowerZoneTileAccessor : BaseNetworkZoneTileAccessor<PowerLineConsumption>
    {
        protected override string NetworkName => "Power";

        protected override bool IsConnected(IAreaZoneConsumption consumption) => consumption is ZoneClusterMemberConsumption;
    }
}