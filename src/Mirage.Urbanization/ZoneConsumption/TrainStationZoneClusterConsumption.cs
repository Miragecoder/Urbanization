using System;
using System.Drawing;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneConsumption.Base.Behaviours;

namespace Mirage.Urbanization.ZoneConsumption
{
    public class TrainStationZoneClusterConsumption : StaticZoneClusterConsumption
    {
        public override string Name
        {
            get { return "Train station"; }
        }

        public TrainStationZoneClusterConsumption(Func<ZoneInfoFinder> createZoneInfoFinderFunc)
            : base(
                createZoneInfoFinderFunc: createZoneInfoFinderFunc,
                electricityBehaviour: new ElectricityConsumerBehaviour(10), 
                pollutionInUnits: 0,  
                color: Color.Yellow, 
                widthInZones: 2, 
                heightInZones: 2)
        {
            _crimeBehaviour = new DynamicCrimeBehaviour(() => 10);
        }
        private readonly ICrimeBehaviour _crimeBehaviour;
        public override ICrimeBehaviour CrimeBehaviour { get { return _crimeBehaviour; } }
    }
}