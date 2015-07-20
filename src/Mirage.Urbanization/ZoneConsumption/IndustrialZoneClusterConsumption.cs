using System;
using System.Drawing;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization.ZoneConsumption
{
    public class IndustrialZoneClusterConsumption : BaseGrowthZoneClusterConsumption
    {
        public IndustrialZoneClusterConsumption(Func<ZoneInfoFinder> createZoneInfoFinderFunc) : base(createZoneInfoFinderFunc, Color.Yellow) { }

        protected override decimal PopulationPollutionFactor => 1.4M;

        public override char KeyChar => 'i';

        protected override decimal PopulationCrimeFactor => 0.7M;

        protected override decimal PopulationFireHazardFactor => 0.5M;

        public override string Name => "Industrial zone";
    }
}