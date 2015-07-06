using System;
using System.Drawing;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization.ZoneConsumption
{
    public class IndustrialZoneClusterConsumption : BaseGrowthZoneClusterConsumption
    {
        public IndustrialZoneClusterConsumption(Func<ZoneInfoFinder> createZoneInfoFinderFunc) : base(createZoneInfoFinderFunc, Color.Yellow) { }

        protected override decimal PopulationPollutionFactor
        {
            get { return 1.4M; }
        }

        public override char KeyChar { get { return 'i'; } }

        protected override decimal PopulationCrimeFactor
        {
            get { return 0.7M; }
        }

        protected override decimal PopulationFireHazardFactor
        {
            get { return 0.5M; }
        }

        public override string Name { get { return "Industrial zone"; } }
    }
}