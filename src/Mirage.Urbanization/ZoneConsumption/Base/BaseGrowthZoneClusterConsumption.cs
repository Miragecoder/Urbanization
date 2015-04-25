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
        private int _id = Random.Next(0, int.MaxValue);

        protected BaseGrowthZoneClusterConsumption(Func<ZoneInfoFinder> createZoneInfoFinderFunc, Color color)
            : base(createZoneInfoFinderFunc, new ElectricityConsumerBehaviour(15), color, 3, 3)
        {
            _pollutionBehaviour =
                new DynamicPollutionBehaviour(() => Convert.ToInt32(PopulationDensity*PopulationPollutionFactor));
            _crimeBehaviour = new DynamicCrimeBehaviour(() => Convert.ToInt32(PopulationDensity*PopulationCrimeFactor));
        }

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
        public int PopulationDensity { get; private set; }

        public int PopulationStatistics
        {
            get { return PopulationDensity*3; }
        }

        public bool CanGrow
        {
            get { return PopulationDensity < 50; }
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