using System;
using System.Drawing;
using Mirage.Urbanization.ZoneConsumption.Base.Behaviours;

namespace Mirage.Urbanization.ZoneConsumption.Base
{
    public interface IBaseGrowthZoneClusterConsumptionInLoadContext : IAreaConsumption
    {
        void Set(int id, int population);
    }

    public abstract class BaseGrowthZoneClusterConsumption : BaseImplementedZoneClusterConsumption, IAreaObjectWithSeed,
        IBaseGrowthZoneClusterConsumptionInLoadContext, IAreaObjectWithPopulationDensity
    {
        private static readonly Random Random = new Random();
        public override IFireHazardBehaviour FireHazardBehaviour { get; }


        protected BaseGrowthZoneClusterConsumption(
            Func<ZoneInfoFinder> createZoneInfoFinderFunc, 
            Color color)
            : base(
            createZoneInfoFinderFunc: createZoneInfoFinderFunc, 
            electricityBehaviour: new ElectricityConsumerBehaviour(15), 
            color: color, 
            widthInZones: 3, 
            heightInZones: 3
        )
        {
            PollutionBehaviour =
                new DynamicPollutionBehaviour(() => Convert.ToInt32(PopulationDensity * PopulationPollutionFactor));
            CrimeBehaviour = new DynamicCrimeBehaviour(() => Convert.ToInt32(PopulationDensity * PopulationCrimeFactor));
            FireHazardBehaviour = new DynamicFireHazardBehaviour(() => Convert.ToInt32(PopulationDensity * PopulationCrimeFactor));
        }
        public override int Cost => 100;

        public override IPollutionBehaviour PollutionBehaviour { get; }

        public override ICrimeBehaviour CrimeBehaviour { get; }

        protected abstract decimal PopulationPollutionFactor { get; }
        protected abstract decimal PopulationCrimeFactor { get; }
        protected abstract decimal PopulationFireHazardFactor { get; }
        public int PopulationDensity { get; private set; }

        public int PopulationStatistics => PopulationDensity * 3;

        private const int MaximumPopulation = 50;

        public bool CanGrow => PopulationDensity < MaximumPopulation;

        public bool CanGrowAndHasPower => CanGrow && HasPower;

        public bool IsPopulated => PopulationDensity > 0;

        public int AverageTravelDistance { get; private set; }

        public int Id { get; private set; } = Random.Next(0, int.MaxValue);

        void IBaseGrowthZoneClusterConsumptionInLoadContext.Set(int id, int populationDensity)
        {
            Id = id;
            PopulationDensity = populationDensity;
        }

        public void IncreasePopulation()
        {
            if (CanGrow) PopulationDensity++;
        }

        public void IncreaseOrDecreaseByPoweredState()
        {
            if (HasPower)
                IncreasePopulation();
            else
                DecreasePopulation();
        }

        public void DecreasePopulation()
        {
            if (PopulationDensity > 0) PopulationDensity--;
        }

        public void SetAverageTravelDistance(int distance)
        {
            AverageTravelDistance = distance;
        }
    }
}