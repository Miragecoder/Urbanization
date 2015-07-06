using System;
using System.Drawing;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneConsumption.Base.Behaviours;

namespace Mirage.Urbanization.ZoneConsumption
{
    public class AirportZoneClusterConsumption : StaticZoneClusterConsumption
    {
        public AirportZoneClusterConsumption(Func<ZoneInfoFinder> createZoneInfoFinderFunc)
            : base(
            createZoneInfoFinderFunc: createZoneInfoFinderFunc,
                electricityBehaviour: new ElectricityConsumerBehaviour(30),
                pollutionInUnits: 70,
                color: Color.Tomato,
                widthInZones: 5,
                heightInZones: 6
                )
        {
            _crimeBehaviour = new DynamicCrimeBehaviour(() => 50);
        }

        public override char KeyChar { get { return 'a'; } }

        public override string Name
        {
            get { return "Airport"; }
        }
        private readonly ICrimeBehaviour _crimeBehaviour;
        public override ICrimeBehaviour CrimeBehaviour { get { return _crimeBehaviour; } }
        public override int Cost { get { return 10000; } }

        private readonly IFireHazardBehaviour _fireHazardBehaviour = new DynamicFireHazardBehaviour(() => 20);
        public override IFireHazardBehaviour FireHazardBehaviour { get { return _fireHazardBehaviour; } }
    }
}