using Mirage.Urbanization.ZoneConsumption;

namespace Mirage.Urbanization.Tilesets
{
    class CommercialGrowthZoneTileAccessor : BaseGrowthZoneTileAccessor<CommercialZoneClusterConsumption>
    {
        public override string Namespace => "Commercial";
    }
}