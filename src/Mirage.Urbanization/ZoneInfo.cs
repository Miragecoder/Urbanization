using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mirage.Urbanization.GrowthPathFinding;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneConsumption.Base.Behaviours;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization
{
    [DebuggerDisplay("Zone info: (X: {Point.X}, Y: {Point.Y})")]
    public class ZoneInfo : IZoneInfo
    {
        private readonly GetRelativeZoneInfoDelegate _getRelativeZoneInfo;
        private readonly ILandValueCalculator _landValueCalculator;
        private readonly GrowthAlgorithmHighlightState _highlightState = new GrowthAlgorithmHighlightState();

        public IGrowthAlgorithmHighlightState GrowthAlgorithmHighlightState => _highlightState;

        public IZoneConsumptionState ConsumptionState { get; } = new ZoneConsumptionState();

        public ZonePoint Point { get; }

        public ZoneInfo(
            ZonePoint zonePoint,
            GetRelativeZoneInfoDelegate getRelativeZoneInfo,
            ILandValueCalculator landValueCalculator
            )
        {
            Point = zonePoint;
            _getRelativeZoneInfo = getRelativeZoneInfo ?? throw new ArgumentNullException(nameof(getRelativeZoneInfo));
            _landValueCalculator = landValueCalculator ?? throw new ArgumentNullException(nameof(landValueCalculator));
        }

        public QueryResult<IZoneInfo, RelativeZoneInfoQuery> GetRelativeZoneInfo(RelativeZoneInfoQuery relativeZoneInfoQuery)
        {
            return _getRelativeZoneInfo(relativeZoneInfoQuery);
        }

        public int GetDistanceScoreBasedOnConsumption()
        {
            return ConsumptionState.GetIsRailroadNetworkMember() || ConsumptionState.GetIsPowerGridMember()
                ? 1
                : 3;

        }

        private QueryResult<IZoneInfo, RelativeZoneInfoQuery> _north;
        public QueryResult<IZoneInfo, RelativeZoneInfoQuery> GetNorth()
        {
            return _north ?? (_north = _getRelativeZoneInfo(
                new RelativeZoneInfoQuery(relativeX: 0, relativeY: -1)
                ));
        }

        private QueryResult<IZoneInfo, RelativeZoneInfoQuery> _south;
        public QueryResult<IZoneInfo, RelativeZoneInfoQuery> GetSouth()
        {
            return _south ?? (_south = _getRelativeZoneInfo(
                new RelativeZoneInfoQuery(relativeX: 0, relativeY: 1)
                ));
        }

        private QueryResult<IZoneInfo, RelativeZoneInfoQuery> _east;
        public QueryResult<IZoneInfo, RelativeZoneInfoQuery> GetEast()
        {
            return _east ?? (_east = _getRelativeZoneInfo(
                new RelativeZoneInfoQuery(relativeX: 1, relativeY: 0)
                ));
        }

        private QueryResult<IZoneInfo, RelativeZoneInfoQuery> _west;
        public QueryResult<IZoneInfo, RelativeZoneInfoQuery> GetWest()
        {
            return _west ?? (_west = _getRelativeZoneInfo(
                new RelativeZoneInfoQuery(relativeX: -1, relativeY: 0)
                ));
        }

        public IEnumerable<QueryResult<IZoneInfo, RelativeZoneInfoQuery>> GetNorthEastSouthWest()
        {
            yield return GetNorth();
            yield return GetEast();
            yield return GetSouth();
            yield return GetWest();
        }

        public IEnumerable<IZoneInfo> CrawlAllDirections(Func<IZoneInfo, bool> predicate)
        {
            return CrawlAllDirections(predicate, new HashSet<IZoneInfo>());
        }

        private IEnumerable<IZoneInfo> CrawlAllDirections(Func<IZoneInfo, bool> predicate, HashSet<IZoneInfo> samples)
        {
            foreach (var match in GetNorthEastSouthWest()
                .Where(x => x.HasMatch && predicate(x.MatchingObject))
                .Where(x => samples.Add(x.MatchingObject)))
            {
                foreach (var subEntry in (match.MatchingObject as ZoneInfo).CrawlAllDirections(predicate, samples))
                    samples.Add(subEntry);
            }
            return samples;
        }

        private readonly IDictionary<int, ISet<QueryResult<IZoneInfo, RelativeZoneInfoQuery>>> _diamondCache =
            new Dictionary<int, ISet<QueryResult<IZoneInfo, RelativeZoneInfoQuery>>>();

        private readonly object _diamondCacheLock = new object();

        public IEnumerable<QueryResult<IZoneInfo, RelativeZoneInfoQuery>> GetSurroundingZoneInfosDiamond(int size)
        {
            lock (_diamondCacheLock)
            {
                if (!_diamondCache.ContainsKey(size))
                {
                    _diamondCache.Add(size, new HashSet<QueryResult<IZoneInfo, RelativeZoneInfoQuery>>());
                    for (int y = -size; y <= size; y++)
                    {
                        for (int x = -size; x <= size; x++)
                        {
                            if (Math.Abs(x) + Math.Abs(y) <= size)
                            {
                                _diamondCache[size].Add(_getRelativeZoneInfo(new RelativeZoneInfoQuery(x, y)));
                            }
                        }
                    }
                }
                return _diamondCache[size];
            }
        }

        IReadOnlyZoneConsumptionState IReadOnlyZoneInfo.ZoneConsumptionState => ConsumptionState;

        public QueryResult<IQueryPollutionResult> QueryPollution()
        {
            int pollution = (
                from match in GetSurroundingZoneInfosDiamond(4)
                    .Where(x => x.HasMatch)
                let pollutionBehaviourResult = match
                    .MatchingObject
                    .GetPollutionBehaviour()
                where pollutionBehaviourResult.HasMatch
                select pollutionBehaviourResult
                    .MatchingObject
                    .GetPollutionInUnits(match.QueryObject)
                ).Sum();

            _lastQueryPollutionResult = QueryResult<IQueryPollutionResult>.Create(pollution > 0 ? new QueryPollutionResult(pollution) : null);
            return _lastQueryPollutionResult;
        }

        private QueryResult<IQueryPollutionResult> _lastQueryPollutionResult;
        public QueryResult<IQueryPollutionResult> GetLastQueryPollutionResult()
        {
            return _lastQueryPollutionResult ?? QueryPollution();
        }

        public QueryResult<IPollutionBehaviour> GetPollutionBehaviour()
        {
            var consumptionState = ConsumptionState.GetZoneConsumption();
            IPollutionBehaviour pollutionBehaviour = null;

            if (consumptionState is ZoneClusterMemberConsumption)
            {
                pollutionBehaviour = (consumptionState as ZoneClusterMemberConsumption).ParentBaseZoneClusterConsumption
                    .PollutionBehaviour;
            }
            else if (consumptionState is ISingleZoneConsumptionWithPollutionBehaviour)
            {
                pollutionBehaviour = (consumptionState as ISingleZoneConsumptionWithPollutionBehaviour).PollutionBehaviour;
            }
            else
            {
                ConsumptionState.WithNetworkMember<RoadZoneConsumption>(roadZoneConsumption => pollutionBehaviour = roadZoneConsumption.PollutionBehaviour);
            }

            return QueryResult<IPollutionBehaviour>.Create(pollutionBehaviour);
        }

        public QueryResult<ICrimeBehaviour> GetCrimeBehaviour()
        {
            var consumptionState = ConsumptionState.GetZoneConsumption();
            ICrimeBehaviour crimeBehaviour = null;

            if (consumptionState is ZoneClusterMemberConsumption)
            {
                crimeBehaviour =
                    (consumptionState as ZoneClusterMemberConsumption).ParentBaseZoneClusterConsumption.CrimeBehaviour;
            }
            return QueryResult<ICrimeBehaviour>.Create(crimeBehaviour);
        }

        private QueryResult<IQueryCrimeResult> _lastQueryCrimeResult;
        public QueryResult<IQueryCrimeResult> QueryCrime()
        {
            if (!this.ConsumptionState.GetIsZoneClusterMember())
            {
                return _lastQueryCrimeResult = QueryResult<IQueryCrimeResult>.Create(null);
            }

            int crimeInUnits = (from match in GetSurroundingZoneInfosDiamond(20)
                .Where(x => x.HasMatch)
                let crimeBehaviourResult = match
                    .MatchingObject
                    .GetCrimeBehaviour()
                where crimeBehaviourResult.HasMatch
                select crimeBehaviourResult
                    .MatchingObject
                    .GetCrimeInUnits(match.QueryObject)
                ).Sum();

            return _lastQueryCrimeResult = QueryResult<IQueryCrimeResult>.Create(new QueryCrimeResult(crimeInUnits));
        }

        public QueryResult<IQueryCrimeResult> GetLastQueryCrimeResult()
        {
            return _lastQueryCrimeResult ?? QueryCrime();
        }

        public QueryResult<IFireHazardBehaviour> GetFireHazardBehaviour()
        {
            var consumptionState = ConsumptionState.GetZoneConsumption();
            IFireHazardBehaviour FireHazardBehaviour = null;

            if (consumptionState is ZoneClusterMemberConsumption)
            {
                FireHazardBehaviour =
                    (consumptionState as ZoneClusterMemberConsumption).ParentBaseZoneClusterConsumption.FireHazardBehaviour;
            }
            return QueryResult<IFireHazardBehaviour>.Create(FireHazardBehaviour);
        }

        private QueryResult<IQueryFireHazardResult> _lastQueryFireHazardResult;
        public QueryResult<IQueryFireHazardResult> QueryFireHazard()
        {
            if (!this.ConsumptionState.GetIsZoneClusterMember())
            {
                return _lastQueryFireHazardResult = QueryResult<IQueryFireHazardResult>.Create();
            }

            int FireHazardInUnits = (from match in GetSurroundingZoneInfosDiamond(20)
                .Where(x => x.HasMatch)
                let FireHazardBehaviourResult = match
                    .MatchingObject
                    .GetFireHazardBehaviour()
                where FireHazardBehaviourResult.HasMatch
                select FireHazardBehaviourResult
                    .MatchingObject
                    .GetFireHazardInUnits(match.QueryObject)
                ).Sum();

            return _lastQueryFireHazardResult = QueryResult<IQueryFireHazardResult>.Create(new QueryFireHazardResult(FireHazardInUnits));
        }

        public QueryResult<IQueryFireHazardResult> GetLastQueryFireHazardResult()
        {
            return _lastQueryFireHazardResult ?? QueryFireHazard();
        }

        public void WithZoneConsumptionIf<T>(Action<T> action) where T : BaseZoneConsumption
        {
            if (this.ConsumptionState.GetZoneConsumption() is T consumption)
                action(consumption);
        }

        public QueryResult<TNetworkZoneConsumption> GetNetworkZoneConsumption<TNetworkZoneConsumption>()
            where TNetworkZoneConsumption : BaseNetworkZoneConsumption
        {
            var consumption = ConsumptionState.GetZoneConsumption();

            if (consumption is TNetworkZoneConsumption)
                return QueryResult<TNetworkZoneConsumption>.Create(consumption as TNetworkZoneConsumption);

            if (consumption is IntersectingZoneConsumption)
            {
                return QueryResult<TNetworkZoneConsumption>.Create((consumption as IntersectingZoneConsumption)
                    .GetIntersectingZoneConsumptions()
                    .OfType<TNetworkZoneConsumption>()
                    .SingleOrDefault()
                    );
            }
            return QueryResult<TNetworkZoneConsumption>.Create();
        }

        public void WithNetworkConsumptionIf<TNetworkZoneConsumption>(Action<TNetworkZoneConsumption> action)
            where TNetworkZoneConsumption : BaseNetworkZoneConsumption
        {
            GetNetworkZoneConsumption<TNetworkZoneConsumption>()
                .WithResultIfHasMatch(action);
        }

        public bool IsGrowthZoneClusterOfType<T>() where T : BaseGrowthZoneClusterConsumption
        {
            var match = false;
            WithZoneClusterIf<T>(t => match = t.CanGrowAndHasPower);
            return match;
        }

        public QueryResult<T> GetAsZoneCluster<T>() where T : BaseZoneClusterConsumption
        {
            T value = null;

            WithZoneClusterIf<T>(x => value = x);
            return QueryResult<T>.Create(value);
        }

        public void WithZoneClusterIf<T>(Action<T> action) where T : BaseZoneClusterConsumption
        {
            WithZoneConsumptionIf<ZoneClusterMemberConsumption>(x =>
            {
                if (x.ParentBaseZoneClusterConsumption is T parent)
                    action(parent);
            });
        }

        public int? GetLastAverageTravelDistance()
        {
            var returnValue = default(int?);
            WithZoneClusterIf<BaseGrowthZoneClusterConsumption>(cluster =>
            {
                returnValue = cluster.AverageTravelDistance;
            });
            return returnValue;
        }


        public int GetPopulationDensity()
        {
            var returnValue = default(int);
            WithZoneClusterIf<BaseGrowthZoneClusterConsumption>(cluster =>
            {
                returnValue = cluster.PopulationDensity;
            });
            return returnValue;
        }

        public ZoneInfoSnapshot TakeSnapshot()
        {
            return ConsumptionState
                .GetZoneConsumption()
                .Pipe(consumption =>
                {
                    return new ZoneInfoSnapshot(
                        point: Point,
                        areaZoneConsumption: consumption,
                        getNorthEastSouthWestFunc:  GetNorthEastSouthWest,
                        trafficDensity: (consumption as RoadZoneConsumption)
                            .ToQueryResult()
                            .WithResultIfHasMatch(x => x.GetTrafficDensity(),
                                (consumption as IntersectingZoneConsumption)
                                    .ToQueryResult()
                                    .WithResultIfHasMatch(x => x.GetTrafficDensity()
                            )
                        )
                    );
                });
        }

        public QueryResult<IQueryLandValueResult> QueryLandValue()
        {
            return _lastQueryLandValueResult = _landValueCalculator.GetFor(this);
        }

        private QueryResult<IQueryLandValueResult> _lastQueryLandValueResult;

        public QueryResult<IQueryLandValueResult> GetLastLandValueResult()
        {
            return _lastQueryLandValueResult ?? QueryLandValue();
        }

        public IEnumerable<Func<IZoneInfo, QueryResult<IZoneInfo, RelativeZoneInfoQuery>>> GetSteerDirections(Func<IZoneInfo, QueryResult<IZoneInfo, RelativeZoneInfoQuery>> expression)
        {
            var result = expression(this);

            if (result == GetNorth() || result == GetSouth())
            {
                yield return x => x.GetEast();
                yield return x => x.GetWest();
            }
            else if (result == GetEast() || result == GetWest())
            {
                yield return x => x.GetNorth();
                yield return x => x.GetSouth();
            }
            else
                throw new InvalidOperationException();
        }
    }
}