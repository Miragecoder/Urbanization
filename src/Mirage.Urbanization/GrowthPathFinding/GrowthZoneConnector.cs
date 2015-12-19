using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Mirage.Urbanization.Statistics;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.GrowthPathFinding
{
    class GrowthZoneConnector
    {
        private readonly CancellationToken _cancellationToken;

        class TrafficDensity
        {
            public int HalfOfOldDensity => OldDensity > 0
                ? OldDensity / 2
                : OldDensity;

            public int OldDensity { get; }

            public TrafficDensity(int oldValue)
            {
                OldDensity = oldValue;
            }

            public int? NewDensity { get; private set; }

            public void AddToNewValue(int density)
            {
                if (!NewDensity.HasValue) NewDensity = default(int);
                NewDensity += density;
            }
        }

        private readonly Dictionary<RoadZoneConsumption, TrafficDensity> _roadZonesAndTraffic;
        private readonly Dictionary<BaseGrowthZoneClusterConsumption, List<int>> _growthZonesAndTravelDistances = new Dictionary<BaseGrowthZoneClusterConsumption, List<int>>();

        public GrowthZoneConnector(ZoneInfoGrid zoneInfoGrid, CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _roadZonesAndTraffic = zoneInfoGrid
                .ZoneInfos
                .Values
                .Select(x => x.GetNetworkZoneConsumption<RoadZoneConsumption>())
                .Where(x => x.HasMatch)
                .Select(x => x.MatchingObject)
                .ToDictionary(x => x, x => new TrafficDensity(x.GetTrafficDensityAsInt()));
        }

        public void Process(GrowthZoneInfoPathNode growthZoneInfoPathNode)
        {
            var originConsumption = growthZoneInfoPathNode.OriginBaseZoneClusterConsumption as BaseGrowthZoneClusterConsumption;

            if (originConsumption == null || !originConsumption.HasPower)
                return;

            var originAsIndustry = originConsumption as IndustrialZoneClusterConsumption;
            var originAsCommercial = originConsumption as CommercialZoneClusterConsumption;
            var originAsResidential = originConsumption as ResidentialZoneClusterConsumption;

            {
                var validation =
                    new List<object> { originAsIndustry, originAsCommercial, originAsResidential }
                    .Single(x => x != null);
            }

            var destinations = growthZoneInfoPathNode
                .EnumerateAllChildPathNodes()
                .Where(x => x.DestinationHashCode.HasValue)
                .GroupBy(x => x.DestinationHashCode.Value)
                .ToList();

            _cancellationToken.ThrowIfCancellationRequested();

            if (originAsResidential != null)
            {
                ObtainMatchAction<IndustrialZoneClusterConsumption>(originAsResidential, destinations)
                    .WithResultIfHasMatch(matchActionOne =>
                        ObtainMatchAction<CommercialZoneClusterConsumption>(originAsResidential, destinations)
                            .WithResultIfHasMatch(
                                matchActionTwo =>
                                {
                                    matchActionOne();
                                    matchActionTwo();
                                }
                            )
                        );
            }
            else if (originAsCommercial != null)
            {
                ObtainMatchAction<IndustrialZoneClusterConsumption>(originAsCommercial, destinations)
                    .WithResultIfHasMatch(matchActionOne =>
                        ObtainMatchAction<ResidentialZoneClusterConsumption>(originAsCommercial, destinations)
                            .WithResultIfHasMatch(
                                matchActionTwo =>
                                {
                                    matchActionOne();
                                    matchActionTwo();
                                }
                            )
                        );
            }
            else if (originAsIndustry != null)
            {
                ObtainMatchAction<ResidentialZoneClusterConsumption>(originAsIndustry, destinations)
                    .WithResultIfHasMatch(x => x());
            }
            else throw new ArgumentException("Origin consumption is not of a valid type: " + originConsumption.GetType(), nameof(growthZoneInfoPathNode));
        }

        private void AddTrafficTo(RoadZoneConsumption zoneInfo, params BaseGrowthZoneClusterConsumption[] involvedGrowthZones)
        {
            if (_roadZonesAndTraffic.ContainsKey(zoneInfo))
                _roadZonesAndTraffic[zoneInfo].AddToNewValue(involvedGrowthZones.Sum(x => x.PopulationDensity));
        }

        private bool AllowsForTraffic(IZoneInfoPathNode node)
        {
            return node.EnumeratePathBackwards().All(member =>
            {
                _cancellationToken.ThrowIfCancellationRequested();
                var roadZoneConsumption = member.ZoneInfo
                    .GetNetworkZoneConsumption<RoadZoneConsumption>();

                if (roadZoneConsumption.HasMatch)
                {
                    if (_roadZonesAndTraffic.ContainsKey(roadZoneConsumption.MatchingObject))
                    {
                        return _roadZonesAndTraffic[roadZoneConsumption.MatchingObject].OldDensity < 600;
                    }
                }
                return true;
            });
        }

        private QueryResult<Action> ObtainMatchAction<TBaseGrowthZoneClusterConsumption>(
            BaseGrowthZoneClusterConsumption origin,
            IEnumerable<IGrouping<int, IZoneInfoPathNode>> destinations
        )
            where TBaseGrowthZoneClusterConsumption : BaseGrowthZoneClusterConsumption
        {
            _cancellationToken.ThrowIfCancellationRequested();
            Action result = null;
            foreach (var zoneInfoPathNode in destinations.SelectMany(x => x.OrderBy(y => y.Distance)))
            {
                var foundMatch = false;

                zoneInfoPathNode.WithDestination(z => z.WithZoneClusterIf<TBaseGrowthZoneClusterConsumption>(
                    cluster =>
                    {
                        if (cluster.CanGrowAndHasPower)
                            foundMatch = true;
                    })
                );

                if (!foundMatch || !AllowsForTraffic(zoneInfoPathNode))
                    continue;

                var localZoneInfoPathNode = zoneInfoPathNode;
                result = () => localZoneInfoPathNode.WithDestination(z =>
                {
                    if (!_growthZonesAndTravelDistances.ContainsKey(origin))
                        _growthZonesAndTravelDistances.Add(origin, new List<int>());

                    z.WithZoneClusterIf<TBaseGrowthZoneClusterConsumption>(
                        cluster =>
                        {
                            cluster.IncreaseOrDecreaseByPoweredState();
                            _toBeDecreased.Remove(cluster);
                        }
                    );

                    var pathLength = 0;
                    zoneInfoPathNode.WithPathMembers(zz =>
                    {
                        zz.ZoneInfo.WithNetworkConsumptionIf<RoadZoneConsumption>(x => AddTrafficTo(x, origin));
                        zz.ZoneInfo.GrowthAlgorithmHighlightState.SetState(HighlightState.UsedAsPath);
                        pathLength += zz.ZoneInfo.GetDistanceScoreBasedOnConsumption();
                    });
                    _growthZonesAndTravelDistances[origin].Add(pathLength);
                });
            }
            return QueryResult<Action>.Create(result);
        }

        public void ApplyAverageTravelDistances()
        {
            foreach (var x in _growthZonesAndTravelDistances)
            {
                x.Key.SetAverageTravelDistance(Convert.ToInt32(x.Value.Average()));
            }
        }

        public IRoadInfrastructureStatistics ApplyTraffic()
        {
            foreach (var zone in _roadZonesAndTraffic)
            {
                zone.Key.SetTrafficDensity(
                    zone.Value.NewDensity.HasValue
                    ? zone.Value.NewDensity.Value
                    : zone.Value.HalfOfOldDensity
                );
            }
            return new RoadInfrastructureStatistics(_roadZonesAndTraffic.Keys);
        }

        private readonly ISet<BaseGrowthZoneClusterConsumption> _toBeDecreased = new HashSet<BaseGrowthZoneClusterConsumption>();

        public void DecreasePopulation()
        {
            foreach (var x in _toBeDecreased) x.DecreasePopulation();
        }

        public void MarkForPopulationDecrease(ISet<BaseGrowthZoneClusterConsumption> growthZones)
        {
            foreach (var growthZone in
                from growthZone in growthZones
                from iteration in Enumerable.Range(
                    start: 0,
                    count: (growthZone.HasPower ? 1 : 3)
                )
                select growthZone
            )
            {
                _toBeDecreased.Add(growthZone);
                //growthZone.DecreasePopulation();
            }
        }
    }
}
