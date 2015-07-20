using System;
using System.Drawing;
using Mirage.Urbanization.ZoneConsumption.Base.Behaviours;

namespace Mirage.Urbanization.ZoneConsumption.Base
{
    public abstract class StaticZoneClusterConsumption : BaseImplementedZoneClusterConsumption
    {
        public override IPollutionBehaviour PollutionBehaviour { get; }

        protected StaticZoneClusterConsumption(
            Func<ZoneInfoFinder> createZoneInfoFinderFunc,
            IElectricityBehaviour electricityBehaviour,
            int pollutionInUnits,
            Color color,
            int widthInZones,
            int heightInZones)
            : base(createZoneInfoFinderFunc, electricityBehaviour, color, widthInZones, heightInZones)
        {
            PollutionBehaviour = new DynamicPollutionBehaviour(() => pollutionInUnits);
        }
    }
}