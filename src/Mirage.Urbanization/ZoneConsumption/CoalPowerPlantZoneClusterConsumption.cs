using System;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using System.Linq;
using System.Reflection;
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


        public override char KeyChar => 'p';
        public override string Name => "Coal power plant";
        public override ICrimeBehaviour CrimeBehaviour { get; } = new DynamicCrimeBehaviour(() => 0);
        public override int Cost => 15000;
        public override Image Tile => Image.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("Mirage.Urbanization.Tiles.coalpowerplant.png"));

        public override IFireHazardBehaviour FireHazardBehaviour { get; } = new DynamicFireHazardBehaviour(() => 20);
    }
}
