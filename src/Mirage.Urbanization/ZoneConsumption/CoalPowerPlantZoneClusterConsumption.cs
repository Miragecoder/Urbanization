using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneConsumption.Base.Behaviours;

namespace Mirage.Urbanization.ZoneConsumption
{
    public class CoalPowerPlantZoneClusterConsumption : StaticZoneClusterConsumption
    {
        public CoalPowerPlantZoneClusterConsumption(Func<ZoneInfoFinder> createZoneInfoFinderFunc)
            : base(
            createZoneInfoFinderFunc: createZoneInfoFinderFunc,
                electricityBehaviour: new ElectricitySupplierBehaviour(960),
                pollutionInUnits: 90,
                color: Color.Indigo,
                widthInZones: 4,
                heightInZones: 4
                ) { }


        public override char KeyChar { get { return 'p'; } }
        public override string Name { get { return "Coal power plant"; } }
        private readonly ICrimeBehaviour _crimeBehaviour = new DynamicCrimeBehaviour(() => 0);
        public override ICrimeBehaviour CrimeBehaviour { get { return _crimeBehaviour; } }
        public override int Cost { get { return 15000; } }

        private readonly IFireHazardBehaviour _fireHazardBehaviour = new DynamicFireHazardBehaviour(() => 20);
        public override IFireHazardBehaviour FireHazardBehaviour { get { return _fireHazardBehaviour; } }
    }
}
