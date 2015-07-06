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
        IBaseGrowthZoneClusterConsumptionInLoadContext
    {
        private static readonly Random Random = new Random();
        private readonly ICrimeBehaviour _crimeBehaviour;
        private readonly IPollutionBehaviour _pollutionBehaviour;
        private readonly IFireHazardBehaviour _fireHazardBehaviour;
        public override IFireHazardBehaviour FireHazardBehaviour { get { return _fireHazardBehaviour; } }
        private int _id = Random.Next(0, int.MaxValue);

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
            _pollutionBehaviour =
                new DynamicPollutionBehaviour(() => Convert.ToInt32(PopulationDensity * PopulationPollutionFactor));
            _crimeBehaviour = new DynamicCrimeBehaviour(() => Convert.ToInt32(PopulationDensity * PopulationCrimeFactor));
            _fireHazardBehaviour = new DynamicFireHazardBehaviour(() => Convert.ToInt32(PopulationDensity * PopulationCrimeFactor));
        }
        public override int Cost { get { return 100; } }

        public override IPollutionBehaviour PollutionBehaviour
        {
            get { return _pollutionBehaviour; }
        }

        public override ICrimeBehaviour CrimeBehaviour
        {
            get { return _crimeBehaviour; }
        }

        protected abstract decimal PopulationPollutionFactor { get; }
        protected abstract decimal PopulationCrimeFactor { get; }
        protected abstract decimal PopulationFireHazardFactor { get; }
        public int PopulationDensity { get; private set; }

        public int PopulationStatistics
        {
            get { return PopulationDensity * 3; }
        }

        private const int MaximumPopulation = 50;

        public bool CanGrow
        {
            get { return PopulationDensity < MaximumPopulation; }
        }

        public bool CanGrowAndHasPower
        {
            get { return CanGrow && HasPower; }
        }

        public bool IsPopulated
        {
            get { return PopulationDensity > 0; }
        }

        public int AverageTravelDistance { get; private set; }

        public int Id
        {
            get { return _id; }
        }

        void IBaseGrowthZoneClusterConsumptionInLoadContext.Set(int id, int populationDensity)
        {
            _id = id;
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