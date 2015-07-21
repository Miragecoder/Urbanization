using System;
using System.Drawing;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneConsumption.Base.Behaviours;

namespace Mirage.Urbanization.ZoneConsumption
{
    public class SeaPortZoneClusterConsumption : StaticZoneClusterConsumption
    {
        public SeaPortZoneClusterConsumption(Func<ZoneInfoFinder> createZoneInfoFinderFunc)
            : base(
            createZoneInfoFinderFunc: createZoneInfoFinderFunc,
            electricityBehaviour: new ElectricityConsumerBehaviour(60),
            pollutionInUnits: 90,
            color: Color.Navy,
            widthInZones: 4,
            heightInZones: 4
        )
        {
            
        }

        public override char KeyChar => 'y';
        public override ICrimeBehaviour CrimeBehaviour { get; } = new DynamicCrimeBehaviour(() => 20);

        public override IFireHazardBehaviour FireHazardBehaviour { get; } = new DynamicFireHazardBehaviour(() => 20);

        public override int Cost => 5000;
        public override string Name => "Sea port";
    }
}