using System;
using System.Drawing;
using Mirage.Urbanization.ZoneConsumption.Base.Behaviours;

namespace Mirage.Urbanization.ZoneConsumption.Base
{
    public abstract class StaticZoneClusterConsumption : BaseImplementedZoneClusterConsumption
    {
        private readonly IPollutionBehaviour _pollutionBehaviour;


        public override IPollutionBehaviour PollutionBehaviour
        {
            get { return _pollutionBehaviour; }
        }

        protected StaticZoneClusterConsumption(
            Func<ZoneInfoFinder> createZoneInfoFinderFunc,
            IElectricityBehaviour electricityBehaviour,
            int pollutionInUnits,
            Color color,
            int widthInZones,
            int heightInZones)
            : base(createZoneInfoFinderFunc, electricityBehaviour, color, widthInZones, heightInZones)
        {
            _pollutionBehaviour = new DynamicPollutionBehaviour(() => pollutionInUnits);
        }
    }
}