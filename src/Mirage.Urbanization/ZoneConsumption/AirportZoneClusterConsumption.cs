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
            CrimeBehaviour = new DynamicCrimeBehaviour(() => 50);
        }

        public override char KeyChar => 'a';

        public override string Name => "Airport";

        public override ICrimeBehaviour CrimeBehaviour { get; }
        public override int Cost => 10000;

        public override IFireHazardBehaviour FireHazardBehaviour { get; } = new DynamicFireHazardBehaviour(() => 20);
    }
}