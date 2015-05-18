using System;
using System.Drawing;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneConsumption.Base.Behaviours;

namespace Mirage.Urbanization.ZoneConsumption
{
    public class PoliceStationZoneClusterConsumption : StaticZoneClusterConsumption
    {
        public override string Name
        {
            get { return "Police station"; }
        }

        public override char KeyChar { get { return 'u'; } }

        public override int Value { get { return 500; } }

        public PoliceStationZoneClusterConsumption(Func<ZoneInfoFinder> createZoneInfoFinderFunc) : base(
            createZoneInfoFinderFunc: createZoneInfoFinderFunc,
            electricityBehaviour: new ElectricityConsumerBehaviour(10), 
            pollutionInUnits: 0,  
            color: Color.Blue, 
            widthInZones: 3, 
            heightInZones: 3)
        {
            _crimeBehaviour = new DynamicCrimeBehaviour(() => HasPower ? -500 : -50);
        }
        private readonly ICrimeBehaviour _crimeBehaviour;
        public override ICrimeBehaviour CrimeBehaviour { get { return _crimeBehaviour; } }
    }
}