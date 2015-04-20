using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq;
using System.Drawing;
using Mirage.Urbanization.ZoneConsumption.Base.Behaviours;

namespace Mirage.Urbanization.ZoneConsumption.Base
{
    public interface IBaseGrowthZoneClusterConsumptionInLoadContext : IAreaConsumption
    {
        void Set(int id, int population);
    }

    public abstract class BaseGrowthZoneClusterConsumption : BaseImplementedZoneClusterConsumption, IAreaObjectWithSeed, IBaseGrowthZoneClusterConsumptionInLoadContext
    {
        private int _id = Random.Next(0, Int32.MaxValue);

        public int Id { get { return _id; } }

        private static readonly Random Random = new Random();

        private readonly IPollutionBehaviour _pollutionBehaviour;

        public override IPollutionBehaviour PollutionBehaviour
        {
            get { return _pollutionBehaviour; }
        }

        public override ICrimeBehaviour CrimeBehaviour
        {
            get { return _crimeBehaviour; }
        }

        private readonly ICrimeBehaviour _crimeBehaviour;

        protected BaseGrowthZoneClusterConsumption(Func<ZoneInfoFinder> createZoneInfoFinderFunc, Color color)
            : base(createZoneInfoFinderFunc, new ElectricityConsumerBehaviour(15), color, 3, 3)
        {
            _pollutionBehaviour = new DynamicPollutionBehaviour(() => Convert.ToInt32(_populationDensity * PopulationPollutionFactor));
            _crimeBehaviour = new DynamicCrimeBehaviour(() => Convert.ToInt32(_populationDensity * PopulationCrimeFactor));
        }

        protected abstract decimal PopulationPollutionFactor { get; }
        protected abstract decimal PopulationCrimeFactor { get; }

        private int _populationDensity;

        public int PopulationDensity { get { return _populationDensity; } }

        public bool CanGrow { get { return _populationDensity < 50; } }

        public bool CanGrowAndHasPower { get { return CanGrow && HasPower; } }

        public bool IsPopulated { get { return _populationDensity > 0; } }

        public void IncreasePopulation()
        {
            if (CanGrow) _populationDensity++;
        }

        public void IncreaseOrDecreaseByPoweredState()
        {
            if (this.HasPower)
                IncreasePopulation();
            else
                DecreasePopulation();
        }

        public void DecreasePopulation()
        {
            if (_populationDensity > 0) _populationDensity--;
        }

        private int _averageTravelDistance;

        public int AverageTravelDistance { get { return _averageTravelDistance; } }

        public void SetAverageTravelDistance(int distance)
        {
            _averageTravelDistance = distance;
        }

        void IBaseGrowthZoneClusterConsumptionInLoadContext.Set(int id, int populationDensity)
        {
            _id = id;
            _populationDensity = populationDensity;
        }
    }
}