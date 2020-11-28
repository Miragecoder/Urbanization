using System;
using SixLabors.ImageSharp;
using System.Reflection;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneConsumption.Base.Behaviours;

namespace Mirage.Urbanization.ZoneConsumption
{
    public class FireStationZoneclusterConsumption : StaticZoneClusterConsumption
    {
        public override string Name => "Fire station";

        public override char KeyChar => 'f';

        public override int Cost => 500;

        public FireStationZoneclusterConsumption(
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
            FireHazardBehaviour = new DynamicFireHazardBehaviour(() => Convert.ToInt32((HasPower ? -900 : -90) * getCityServiceStrengthLevels().FireSquadStrength));
        }

        public override ICrimeBehaviour CrimeBehaviour { get; } = new DynamicCrimeBehaviour(() => 0);
        public override Image Tile => Image.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("Mirage.Urbanization.Tiles.firestation.png"));
        public override IFireHazardBehaviour FireHazardBehaviour { get; }
    }
}