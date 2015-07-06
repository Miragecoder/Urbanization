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


        public override char KeyChar { get { return 'x'; } }
        public override string Name { get { return "Nuclear power plant"; } }
        private readonly ICrimeBehaviour _crimeBehaviour = new DynamicCrimeBehaviour(() => 0);
        public override ICrimeBehaviour CrimeBehaviour { get { return _crimeBehaviour; } }
        public override int Cost { get { return 30000; } }

        private readonly IFireHazardBehaviour _fireHazardBehaviour = new DynamicFireHazardBehaviour(() => 20);
        public override IFireHazardBehaviour FireHazardBehaviour { get { return _fireHazardBehaviour; } }
    }
}