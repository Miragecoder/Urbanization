using System;
using System.Drawing;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneConsumption.Base.Behaviours;

namespace Mirage.Urbanization.ZoneConsumption
{
    public class NuclearPowerPlantZoneClusterConsumption : StaticZoneClusterConsumption
    {
        public NuclearPowerPlantZoneClusterConsumption(Func<ZoneInfoFinder> createZoneInfoFinderFunc)
            : base(
                createZoneInfoFinderFunc: createZoneInfoFinderFunc,
                electricityBehaviour: new ElectricitySupplierBehaviour(2688),
                pollutionInUnits: 50,
                color: Color.Indigo,
                widthInZones: 4,
                heightInZones: 4
                ) { }


        public override char KeyChar => 'x';
        public override string Name => "Nuclear power plant";
        public override ICrimeBehaviour CrimeBehaviour { get; } = new DynamicCrimeBehaviour(() => 0);
        public override int Cost => 30000;

        public override IFireHazardBehaviour FireHazardBehaviour { get; } = new DynamicFireHazardBehaviour(() => 20);
    }
}