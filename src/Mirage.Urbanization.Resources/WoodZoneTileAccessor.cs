using Mirage.Urbanization.ZoneConsumption;

namespace Mirage.Urbanization.Tilesets
{
    class WoodZoneTileAccessor : BaseNetworkZoneTileAccessor<WoodlandZoneConsumption>
    {
        protected override string NetworkName => "Wood";
    }
}