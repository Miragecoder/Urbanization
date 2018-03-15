using System;
using System.Drawing;
using System.Reflection;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization.ZoneConsumption
{
    public class CommercialZoneClusterConsumption : BaseGrowthZoneClusterConsumption
    {
        public CommercialZoneClusterConsumption(Func<ZoneInfoFinder> createZoneInfoFinderFunc) : base(
            createZoneInfoFinderFunc, Color.Blue)
        {
        }

        protected override decimal PopulationPollutionFactor => 0.9M;

        protected override decimal PopulationCrimeFactor => 0.7M;

        protected override decimal PopulationFireHazardFactor => 0.5M;

        public override int RequiredNeighborsToExceedLowDensity => 1;
        public override int RequiredNeighborsToExceedMediumDensity => 2;
        public override char KeyChar => 'c';

        public override Image Tile =>
            new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Mirage.Urbanization.Tiles.commercial.png"));

        public override string Name => "Commercial zone";
    }
}