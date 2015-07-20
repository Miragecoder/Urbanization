using System;
using System.Drawing;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneConsumption.Base.Behaviours;

namespace Mirage.Urbanization.ZoneConsumption
{
    public class FireStationZoneclusterConsumption : StaticZoneClusterConsumption
    {
        public override string Name => "Fire station";

        public override char KeyChar => 'f';

        public override int Cost => 500;

        public FireStationZoneclusterConsumption(Func<ZoneInfoFinder> createZoneInfoFinderFunc)
            : base(
                createZoneInfoFinderFunc: createZoneInfoFinderFunc,
                electricityBehaviour: new ElectricityConsumerBehaviour(10),
                pollutionInUnits: 0,
                color: Color.Blue,
                widthInZones: 3,
                heightInZones: 3)
        {
            FireHazardBehaviour = new DynamicFireHazardBehaviour(() => HasPower ? -900 : -90);
        }

        public override ICrimeBehaviour CrimeBehaviour { get; } = new DynamicCrimeBehaviour(() => 0);

        public override IFireHazardBehaviour FireHazardBehaviour { get; }
    }
}