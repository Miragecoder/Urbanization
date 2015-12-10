using Mirage.Urbanization.ZoneConsumption;

namespace Mirage.Urbanization.Tilesets
{
    class ParkZoneTileAccessor : BaseNetworkZoneTileAccessor<ParkZoneConsumption>
    {
        protected override string NetworkName => "Park";
    }
}