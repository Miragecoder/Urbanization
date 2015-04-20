using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mirage.Urbanization.Statistics;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization
{
    class GrowthZoneConnector
    {
        private readonly CancellationToken _cancellationToken;

        class TrafficDensity
        {
            private readonly int _oldValue;

            public int HalfOfOldDensity
            {
                get
                {
                    return _oldValue > 0
                        ? _oldValue / 2
                        : _oldValue;
                }
            }

            public int OldDensity { get { return _oldValue; } }

            public TrafficDensity(int oldValue)
            {
                _oldValue = oldValue;
            }

            private int? _newDensity;

            public int? NewDensity { get { return _newDensity; } }

            public void AddToNewValue(int density)
            {
                if (!NewDensity.HasValue) _newDensity = default(int);
                _newDensity += density;
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
            else throw new ArgumentException("Origin consumption is not of a valid type: " + originConsumption.GetType(), "growthZoneInfoPathNode");
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
                        cluster => cluster.IncreaseOrDecreaseByPoweredState()
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
            return new QueryResult<Action>(result);
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

        public void DecreasePopulation(HashSet<BaseGrowthZoneClusterConsumption> growthZones)
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
                growthZone.DecreasePopulation();
            }
        }
    }

    public interface IRoadInfrastructureStatistics
    {
        int NumberOfRoadZones { get; }
        INumberSummary TrafficNumbers { get; }
    }

    public interface IRailroadInfrastructureStatistics
    {
        int NumberOfTrainStations { get; }
        int NumberOfRailRoadZones { get; }
    }

    public class RailroadInfrastructureStatistics : IRailroadInfrastructureStatistics
    {
        private readonly int _numberOfTrainStations;
        private readonly int _numberOfRailRoadZones;

        public RailroadInfrastructureStatistics(int numberOfTrainStations, int numberOfRailRoadZones)
        {
            _numberOfTrainStations = numberOfTrainStations;
            _numberOfRailRoadZones = numberOfRailRoadZones;
        }

        public int NumberOfTrainStations
        {
            get { return _numberOfTrainStations; }
        }

        public int NumberOfRailRoadZones
        {
            get { return _numberOfRailRoadZones; }
        }
    }

    public class RoadInfrastructureStatistics : IRoadInfrastructureStatistics
    {
        private readonly int _numberOfRoadZones;
        private readonly INumberSummary _trafficNumbers;

        public RoadInfrastructureStatistics(IEnumerable<RoadZoneConsumption> roadZoneConsumptions)
        {
            var capturedRoadZoneConsumptions = roadZoneConsumptions.ToList();

            _numberOfRoadZones = capturedRoadZoneConsumptions.Count();
            _trafficNumbers = new NumberSummary(capturedRoadZoneConsumptions.Select(x => x.GetTrafficDensityAsInt()));
        }

        public int NumberOfRoadZones
        {
            get { return _numberOfRoadZones; }
        }

        public INumberSummary TrafficNumbers
        {
            get { return _trafficNumbers; }
        }
    }
}
