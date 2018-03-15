using System;
using System.Drawing;
using System.Reflection;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneConsumption.Base.Behaviours;

namespace Mirage.Urbanization.ZoneConsumption
{
    public class TrainStationZoneClusterConsumption : StaticZoneClusterConsumption
    {
        public override string Name => "Train station";

        public override char KeyChar => 's';

        public override int Cost => 250;

        public TrainStationZoneClusterConsumption(Func<ZoneInfoFinder> createZoneInfoFinderFunc)
            : base(
                createZoneInfoFinderFunc: createZoneInfoFinderFunc,
                electricityBehaviour: new ElectricityConsumerBehaviour(10),
                pollutionInUnits: 0,
                color: Color.Yellow,
                widthInZones: 2,
                heightInZones: 2)
        {
            CrimeBehaviour = new DynamicCrimeBehaviour(() => 10);
        }

        public override ICrimeBehaviour CrimeBehaviour { get; }
        public override Image Tile => new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Mirage.Urbanization.Tiles.trainstation.png"));

        public override IFireHazardBehaviour FireHazardBehaviour { get; } = new DynamicFireHazardBehaviour(() => 20);
    }
}