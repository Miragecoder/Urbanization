using System;
using System.Drawing;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneConsumption.Base.Behaviours;

namespace Mirage.Urbanization.ZoneConsumption
{
    public class PoliceStationZoneClusterConsumption : StaticZoneClusterConsumption
    {
        public override string Name => "Police station";

        public override char KeyChar => 'u';

        public override int Cost => 500;

        public PoliceStationZoneClusterConsumption(Func<ZoneInfoFinder> createZoneInfoFinderFunc)
            : base(
                createZoneInfoFinderFunc: createZoneInfoFinderFunc,
                electricityBehaviour: new ElectricityConsumerBehaviour(10),
                pollutionInUnits: 0,
                color: Color.Blue,
                widthInZones: 3,
                heightInZones: 3)
        {
            CrimeBehaviour = new DynamicCrimeBehaviour(() => HasPower ? -500 : -50);
        }

        public override ICrimeBehaviour CrimeBehaviour { get; }

        public override IFireHazardBehaviour FireHazardBehaviour { get; } = new DynamicFireHazardBehaviour(() => 20);
    }
}