using System;
using System.Drawing;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneConsumption.Base.Behaviours;

namespace Mirage.Urbanization.ZoneConsumption
{
    public class StadiumZoneClusterConsumption : StaticZoneClusterConsumption
    {
        public StadiumZoneClusterConsumption(Func<ZoneInfoFinder> createZoneInfoFinderFunc)
            : base(
                createZoneInfoFinderFunc: createZoneInfoFinderFunc,
                electricityBehaviour: new ElectricityConsumerBehaviour(60),
                pollutionInUnits: 90,
                color: Color.GreenYellow,
                widthInZones: 4,
                heightInZones: 4
                )
        {

        }

        public override char KeyChar { get { return 'v'; } }
        private readonly ICrimeBehaviour _crimeBehaviour = new DynamicCrimeBehaviour(() => 20);
        public override ICrimeBehaviour CrimeBehaviour { get { return _crimeBehaviour; } }

        private readonly IFireHazardBehaviour _fireHazardBehaviour = new DynamicFireHazardBehaviour(() => 20);
        public override IFireHazardBehaviour FireHazardBehaviour { get { return _fireHazardBehaviour; } }

        public override int Cost { get { return 5000; } }
        public override string Name { get { return "Stadium"; } }
    }
}