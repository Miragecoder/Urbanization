using Mirage.Urbanization.ZoneConsumption;

namespace Mirage.Urbanization.Tilesets
{
    class RailZoneTileAccessor : BaseNetworkZoneTileAccessor<RailRoadZoneConsumption>
    {
        protected override string NetworkName => "Rail";
    }
}