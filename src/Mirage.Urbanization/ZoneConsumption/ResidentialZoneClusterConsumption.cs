using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization.ZoneConsumption
{
    public class ResidentialZoneClusterConsumption : BaseGrowthZoneClusterConsumption
    {
        public ResidentialZoneClusterConsumption(Func<ZoneInfoFinder> createZoneInfoFinderFunc)
            : base(createZoneInfoFinderFunc, Color.Green)
        {

        }

        public override char KeyChar => 'g';

        protected override decimal PopulationPollutionFactor => 0.5M;

        protected override decimal PopulationCrimeFactor => 0.5M;

        protected override decimal PopulationFireHazardFactor => 0.5M;
        public override Image Tile => new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Mirage.Urbanization.Tiles.residential.png"));

        public override string Name => "Residential zone";
    }
}