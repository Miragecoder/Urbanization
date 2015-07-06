using System;
using System.Drawing;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneConsumption.Base.Behaviours;

namespace Mirage.Urbanization.ZoneConsumption
{
    public class FireStationZoneclusterConsumption : StaticZoneClusterConsumption
    {
        public override string Name
        {
            get { return "Fire station"; }
        }

        public override char KeyChar { get { return 'f'; } }

        public override int Cost { get { return 500; } }

        public FireStationZoneclusterConsumption(Func<ZoneInfoFinder> createZoneInfoFinderFunc)
            : base(
                createZoneInfoFinderFunc: createZoneInfoFinderFunc,
                electricityBehaviour: new ElectricityConsumerBehaviour(10),
                pollutionInUnits: 0,
                color: Color.Blue,
                widthInZones: 3,
                heightInZones: 3)
        {
            _fireHazardBehaviour = new DynamicFireHazardBehaviour(() => HasPower ? -900 : -90);
        }

        private readonly ICrimeBehaviour _crimeBehaviour = new DynamicCrimeBehaviour(() => 0);
        public override ICrimeBehaviour CrimeBehaviour { get { return _crimeBehaviour; } }

        private readonly IFireHazardBehaviour _fireHazardBehaviour;
        public override IFireHazardBehaviour FireHazardBehaviour { get { return _fireHazardBehaviour; } }
    }
}