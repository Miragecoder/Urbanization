using System;
using System.Drawing;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization.ZoneConsumption
{
    public class CommercialZoneClusterConsumption : BaseGrowthZoneClusterConsumption
    {
        public CommercialZoneClusterConsumption(Func<ZoneInfoFinder> createZoneInfoFinderFunc) : base(createZoneInfoFinderFunc, Color.Blue) { }

        protected override decimal PopulationPollutionFactor
        {
            get { return 0.5M; }
        }

        protected override decimal PopulationCrimeFactor
        {
            get { return 0.7M; }
        }

        public override string Name { get { return "Commercial zone"; } }
    }
}