using System;
using SixLabors.ImageSharp;
using System.Reflection;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneConsumption.Base.Behaviours;

namespace Mirage.Urbanization.ZoneConsumption
{
    public class PoliceStationZoneClusterConsumption : StaticZoneClusterConsumption
    {
        public override string Name => "Police station";

        public override char KeyChar => 'u';

        public override int Cost => 500;

        public PoliceStationZoneClusterConsumption(
            Func<ZoneInfoFinder> createZoneInfoFinderFunc,
            Func<ICityServiceStrengthLevels> getCityServiceStrengthLevels)
            : base(
                createZoneInfoFinderFunc: createZoneInfoFinderFunc,
                electricityBehaviour: new ElectricityConsumerBehaviour(10),
                pollutionInUnits: 0,
                color: Color.Blue,
                widthInZones: 3,
                heightInZones: 3)
        {
            CrimeBehaviour = new DynamicCrimeBehaviour(() => Convert.ToInt32((HasPower ? -500 : -50) * getCityServiceStrengthLevels().PoliceStrength));
        }

        public override ICrimeBehaviour CrimeBehaviour { get; }
        public override Image Tile => Image.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("Mirage.Urbanization.Tiles.police.png"));

        public override IFireHazardBehaviour FireHazardBehaviour { get; } = new DynamicFireHazardBehaviour(() => 20);
    }
}