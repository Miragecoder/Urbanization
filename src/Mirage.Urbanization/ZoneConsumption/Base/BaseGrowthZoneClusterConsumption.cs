using System;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using System.Linq;
using Mirage.Urbanization.ZoneConsumption.Base.Behaviours;
using Mirage.Urbanization.ZoneStatisticsQuerying;

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

        public const int MaximumPopulation = 50;

        public const int LowDensityThreshold = 8;
        public const int MediumDensityThreshold = 25;

        public virtual int RequiredNeighborsToExceedLowDensity => 2;
        public virtual int RequiredNeighborsToExceedMediumDensity => 5;

        public bool CanGrow
        {
            get
            {
                var neighborCount = _zoneClusterMembers
                    .Single(x => x.IsCentralClusterMember)
                    .GetZoneInfo()
                    .WithResultIfHasMatch(centerZoneInfo =>
                        centerZoneInfo
                            .GetSurroundingZoneInfosDiamond(20)
                            .Count(x => x.WithResultIfHasMatch(zoneInfo => (zoneInfo
                                .ZoneConsumptionState
                                .GetZoneConsumption() as ZoneClusterMemberConsumption)
                                .ToQueryResult()
                                .WithResultIfHasMatch(
                                    zoneClusterMember => zoneClusterMember.IsCentralClusterMember &&
                                    (zoneClusterMember.ParentBaseZoneClusterConsumption as BaseGrowthZoneClusterConsumption)
                                    .ToQueryResult()
                                    .WithResultIfHasMatch(neighbor => neighbor.PopulationDensity >= PopulationDensity && neighbor.GetType() == GetType())))));

                if (neighborCount <= RequiredNeighborsToExceedLowDensity)
                    return PopulationDensity < LowDensityThreshold;
                else if (neighborCount <= RequiredNeighborsToExceedMediumDensity)
                    return PopulationDensity < MediumDensityThreshold;
                return PopulationDensity < MaximumPopulation;
            }
        }

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
        private readonly Stack<ZoneClusterMemberConsumption> _houseMembers = new Stack<ZoneClusterMemberConsumption>();

        public bool RenderAsHouse(ZoneClusterMemberConsumption zoneClusterMember)
        {
            if (!_zoneClusterMembers.Contains(zoneClusterMember))
                throw new InvalidOperationException();
            int localDensity = PopulationDensity;
            if (localDensity > _zoneClusterMembers.Count(x => !x.IsCentralClusterMember))
                return false;

            while (_houseMembers.Count > localDensity)
            {
                _houseMembers.Pop();
            };

            while (_houseMembers.Count < localDensity)
            {
                _houseMembers
                    .Push(_zoneClusterMembers
                        .Where(x => !x.IsCentralClusterMember)
                        .OrderBy(x => Guid.NewGuid()
                    )
                    .Except(_houseMembers)
                    .First()
                );
            };

            return _houseMembers.Contains(zoneClusterMember);
        }
    }
}