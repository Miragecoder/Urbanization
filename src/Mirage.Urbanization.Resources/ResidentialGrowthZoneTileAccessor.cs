using Mirage.Urbanization.ZoneConsumption;

namespace Mirage.Urbanization.Tilesets
{
    class ResidentialGrowthZoneTileAccessor : BaseGrowthZoneTileAccessor<ResidentialZoneClusterConsumption>
    {
        public override string Namespace => "Residential";
    }
}