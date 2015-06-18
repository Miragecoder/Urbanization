using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization
{
    internal class ShipController : BaseStructureVehicleController<IShip, SeaPortZoneClusterConsumption>
    {
        public ShipController(Func<ISet<IZoneInfo>> getZoneInfosFunc)
            : base(getZoneInfosFunc)
        {
            _candidatesCache = new SimpleCache<ISet<IZoneInfo>>(() => new HashSet<IZoneInfo>(GetZoneInfosFunc()
                    .Where(IsSuitableForShip)), new TimeSpan(0, 1, 0));
        }

        private readonly SimpleCache<ISet<IZoneInfo>> _candidatesCache;

        private DateTime _lastShipInsertion = DateTime.Now;

        private bool NewShipCanBeInserted { get { return DateTime.Now.AddSeconds(-5) > _lastShipInsertion; } }

        protected override void PrepareVehiclesWithStructures(SeaPortZoneClusterConsumption[] structures)
        {
            int desiredAmountOfShips = structures.Count() * 2;

            var candidates = _candidatesCache.GetValue();

            if (candidates == null)
                return;

            while (NewShipCanBeInserted && Vehicles.Count < desiredAmountOfShips)
            {
                var candidate = GetZoneInfosFunc()
                    .Where(IsSuitableForShip)
                    .OrderBy(x => Random.Next())
                    .FirstOrDefault();

                if (candidate != null)
                {
                    Vehicles.Add(new Ship(GetZoneInfosFunc, candidate));
                    _lastShipInsertion = DateTime.Now;
                }
                else
                {
                    return;
                }
            }
        }

        private static bool IsSuitableForShip(IReadOnlyZoneInfo zoneInfo)
        {
            return zoneInfo
                .GetSurroundingZoneInfosDiamond(2)
                .All(x => x.HasNoMatch || x.MatchingObject.ConsumptionState.GetIsWater());
        }

        private class Ship : BaseVehicle, IShip
        {
            public Ship(Func<ISet<IZoneInfo>> getZoneInfosFunc, IZoneInfo currentPosition)
                : base(getZoneInfosFunc, currentPosition)
            {
                _pathEnumeratorTask = new Task<IEnumerator<ShipPathNode>>(() =>
                {
                    var pathNode = new ShipPathNode(CurrentPosition);

                    var enumerator = pathNode
                        .EnumerateAllChildPathNodes()
                        .GroupBy(x => CalculateDistance(CurrentPosition.Point, x.CurrentZoneInfo.Point))
                        .OrderByDescending(x => x.Key)
                        .First()
                        .OrderBy(x => x.Distance)
                        .First()
                        .EnumeratePathBackwards()
                        .GetEnumerator();

                    enumerator.MoveNext();

                    return enumerator;
                });
                _pathEnumeratorTask.Start();
            }

            private readonly Task<IEnumerator<ShipPathNode>> _pathEnumeratorTask;

            public void Move()
            {
                if (!_pathEnumeratorTask.IsCompleted)
                    return;
                IfMustBeMoved(() =>
                {
                    var current = _pathEnumeratorTask.Result.Current;
                    Move(current != null ? current.CurrentZoneInfo : null);

                    if (!_pathEnumeratorTask.Result.MoveNext())
                        Move(null);
                });
            }

            private static double CalculateDistance(ZonePoint x, ZonePoint y)
            {
                int x1 = x.X, x2 = y.X, y1 = x.Y, y2 = y.Y;

                return ((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
            }

            protected override int SpeedInMilliseconds
            {
                get { return 500; }
            }

            private class ShipPathNode
            {
                private readonly IZoneInfo _rootZoneInfo;
                private readonly IZoneInfo _currentZoneInfo;
                private readonly ShipPathNode _preceedingShipPathNode;

                private readonly Lazy<IEnumerable<ShipPathNode>> _childrenLazy;

                public ShipPathNode(IZoneInfo zoneInfo)
                    : this(
                    rootZoneInfo: zoneInfo,
                    currentZoneInfo: zoneInfo,
                    preceedingShipPathNode: null,
                    seenPaths: new HashSet<IZoneInfo>(),
                    distance: 0
                )
                {

                }

                public IEnumerable<ShipPathNode> EnumerateAllChildPathNodes()
                {
                    foreach (var x in _childrenLazy.Value)
                    {
                        yield return x;
                        foreach (var t in x.EnumerateAllChildPathNodes())
                            yield return t;
                    }
                }

                public IZoneInfo CurrentZoneInfo { get { return _currentZoneInfo; } }

                public IEnumerable<ShipPathNode> EnumeratePathBackwards()
                {
                    yield return this;
                    if (_preceedingShipPathNode != null)
                        foreach (var x in _preceedingShipPathNode.EnumeratePathBackwards())
                            yield return x;
                }

                public int Distance { get { return _distance; } }

                private readonly int _distance;

                private ShipPathNode(
                    IZoneInfo rootZoneInfo,
                    IZoneInfo currentZoneInfo,
                    ShipPathNode preceedingShipPathNode,
                    ISet<IZoneInfo> seenPaths,
                    int distance
                )
                {
                    if (rootZoneInfo == null) throw new ArgumentNullException("rootZoneInfo");
                    _rootZoneInfo = rootZoneInfo;
                    if (currentZoneInfo == null) throw new ArgumentNullException("currentZoneInfo");
                    _currentZoneInfo = currentZoneInfo;

                    if (!seenPaths.Add(currentZoneInfo))
                        throw new ArgumentException("'currentZoneInfo' was already added to this path.", "currentZoneInfo");

                    _preceedingShipPathNode = preceedingShipPathNode;

                    _distance = distance;
                    _childrenLazy = new Lazy<IEnumerable<ShipPathNode>>(() => _currentZoneInfo
                        .GetNorthEastSouthWest()
                        .Where(x => x.HasMatch && IsSuitableForShip(x.MatchingObject))
                        .OrderByDescending(x => CalculateDistance(x.MatchingObject.Point, _rootZoneInfo.Point))
                        .Where(x => !seenPaths.Contains(x.MatchingObject))
                        .Select(x =>
                            new ShipPathNode(
                                rootZoneInfo: rootZoneInfo,
                                currentZoneInfo: x.MatchingObject,
                                preceedingShipPathNode: this,
                                seenPaths: seenPaths,
                                distance: _distance + 1
                            )
                        )
                    );
                }
            }
        }
    }

    internal abstract class BaseStructureVehicleController<TVehicle, TStructure> : BaseVehicleController<TVehicle>
        where TVehicle : IMoveableVehicle
        where TStructure : BaseZoneClusterConsumption
    {
        protected BaseStructureVehicleController(Func<ISet<IZoneInfo>> getZoneInfosFunc)
            : base(getZoneInfosFunc)
        {

        }

        protected override sealed void PerformMoveCycle()
        {
            PrepareVehiclesWithStructures(GetStructures());


            foreach (var plane in Vehicles)
            {
                plane.Move();
            }

            RemoveOrphanVehicles();
        }

        protected abstract void PrepareVehiclesWithStructures(TStructure[] structures);

        private TStructure[] GetStructures()
        {
            return GetZoneInfosFunc()
                .Select(x => x.GetAsZoneCluster<TStructure>())
                .Where(x => x.HasMatch)
                .Select(x => x.MatchingObject)
                .Distinct()
                .ToArray();
        }
    }

    internal class AirplaneController : BaseStructureVehicleController<IAirplane, AirportZoneClusterConsumption>
    {
        public AirplaneController(Func<ISet<IZoneInfo>> getZoneInfosFunc)
            : base(getZoneInfosFunc)
        {

        }

        private static readonly Func<IZoneInfo, QueryResult<IZoneInfo, RelativeZoneInfoQuery>>[] Directions =
        {
            x => x.GetNorth(),
            x => x.GetSouth(),
            x => x.GetEast(),
            x => x.GetWest()
        };

        protected override void PrepareVehiclesWithStructures(AirportZoneClusterConsumption[] structures)
        {
            int desiredAmountOfPlanes = structures.Count() * 2;

            while (Vehicles.Count < desiredAmountOfPlanes)
            {
                var spawnPoint = structures.OrderBy(x => Random.Next()).First();
                var centralCell = spawnPoint.ZoneClusterMembers.Single(y => y.IsCentralClusterMember);

                centralCell.GetZoneInfo().WithResultIfHasMatch(zoneInfo =>
                {
                    var steerDirection = Directions.OrderBy(d => Random.Next()).First();
                    var alternateDirection = zoneInfo.GetSteerDirections(steerDirection).OrderBy(x => Random.Next()).First();

                    Vehicles.Add(new Airplane(GetZoneInfosFunc, zoneInfo, steerDirection, alternateDirection, Random.Next(5, 10)));
                });
            }
        }

        private class Airplane : BaseVehicle, IAirplane
        {
            private readonly Func<IZoneInfo, QueryResult<IZoneInfo, RelativeZoneInfoQuery>> _directionQuery;
            private readonly Func<IZoneInfo, QueryResult<IZoneInfo, RelativeZoneInfoQuery>> _alternateDirectionQuery;
            private readonly int _alternateRate;
            private readonly IEnumerator<Func<IZoneInfo, QueryResult<IZoneInfo, RelativeZoneInfoQuery>>> _directionEnumerator;

            public Airplane(Func<ISet<IZoneInfo>> getZoneInfosFunc, IZoneInfo currentPosition,
                Func<IZoneInfo, QueryResult<IZoneInfo, RelativeZoneInfoQuery>> directionQuery,
                Func<IZoneInfo, QueryResult<IZoneInfo, RelativeZoneInfoQuery>> alternateDirectionQuery,
                int alternateRate
            )
                : base(getZoneInfosFunc, currentPosition)
            {
                _directionQuery = directionQuery;
                _alternateDirectionQuery = alternateDirectionQuery;
                _alternateRate = alternateRate;
                _directionEnumerator = DirectionQuery().GetEnumerator();
            }

            private IEnumerable<Func<IZoneInfo, QueryResult<IZoneInfo, RelativeZoneInfoQuery>>> DirectionQuery()
            {
                while (true)
                {
                    foreach (var iteration in Enumerable.Range(0, _alternateRate))
                        yield return _directionQuery;
                    yield return _alternateDirectionQuery;
                }
            }

            protected override int SpeedInMilliseconds { get { return 200; } }

            public void Move()
            {
                IfMustBeMoved(() =>
                {
                    _directionEnumerator.MoveNext();
                    var result = _directionEnumerator.Current(CurrentPosition);
                    Move(result.MatchingObject);
                });
            }
        }
    }

    internal abstract class BaseVehicleController
    {
        protected static readonly Random Random = new Random();
    }

    internal abstract class BaseVehicleController<TVehicle> : BaseVehicleController, IVehicleController<TVehicle> where TVehicle : IVehicle
    {
        protected readonly Func<ISet<IZoneInfo>> GetZoneInfosFunc;

        protected BaseVehicleController(Func<ISet<IZoneInfo>> getZoneInfosFunc)
        {
            GetZoneInfosFunc = getZoneInfosFunc;
        }

        protected readonly HashSet<TVehicle> Vehicles = new HashSet<TVehicle>();

        protected abstract void PerformMoveCycle();

        public void ForEachActiveVehicle(Action<TVehicle> vehicleAction)
        {
            PerformMoveCycle();

            foreach (var vehicle in Vehicles)
            {
                vehicleAction(vehicle);
            }
        }

        protected void RemoveOrphanVehicles()
        {
            foreach (var orphanVehicle in Vehicles.Where(x => x.CanBeRemoved).ToArray())
                Vehicles.Remove(orphanVehicle);
        }
    }

    internal class TrainController : BaseVehicleController<ITrain>
    {
        public TrainController(Func<ISet<IZoneInfo>> getZoneInfosFunc)
            : base(getZoneInfosFunc)
        {
            _cachedNetworks = new SimpleCache<ISet<ISet<IZoneInfo>>>(GetRailwayNetworks, new TimeSpan(0, 0, 1));
        }

        protected override void PerformMoveCycle()
        {
            var cachedNetworksEntry = _cachedNetworks.GetValue();

            if (!cachedNetworksEntry.SelectMany(x => x).Any())
                return;

            foreach (var network in cachedNetworksEntry.Where(x => x.Count() > 20))
            {
                var desiredAmountOfTrains = Math.Abs(network.Count() / 50) + 1;

                List<ITrain> trainsInNetwork = null;

                while (trainsInNetwork == null || trainsInNetwork.Count() < desiredAmountOfTrains)
                {
                    trainsInNetwork = Vehicles
                        .Where(x => network.Contains(x.CurrentPosition))
                        .ToList();

                    int desiredAdditionaTrains = desiredAmountOfTrains - trainsInNetwork.Count;

                    if (desiredAdditionaTrains > 0)
                    {
                        foreach (var iteration in Enumerable.Range(0, desiredAmountOfTrains - trainsInNetwork.Count))
                        {
                            Vehicles.Add(new Train(GetZoneInfosFunc, network
                                .OrderBy(x => Random.Next())
                                .First()
                                ));
                        }
                    }
                }

                foreach (var train in trainsInNetwork)
                {
                    train.SetTrainNetwork(network);
                    train.Move();
                }
            }

            RemoveOrphanVehicles();
        }

        private class Train : BaseVehicle, ITrain
        {
            public Train(Func<ISet<IZoneInfo>> getZoneInfosFunc, IZoneInfo currentPosition)
                : base(getZoneInfosFunc, currentPosition)
            {
            }

            protected override int SpeedInMilliseconds { get { return 300; } }

            public void SetTrainNetwork(ISet<IZoneInfo> trainNetwork)
            {
                _currentTrainNetwork = trainNetwork;
            }

            private ISet<IZoneInfo> _currentTrainNetwork;

            public void Move()
            {
                IfMustBeMoved(() =>
                {
                    if (!_currentTrainNetwork.Contains(CurrentPosition))
                    {
                        CurrentPosition = _currentTrainNetwork.First();
                    }
                    else
                    {
                        var queryNext = CurrentPosition
                            .GetNorthEastSouthWest()
                            .OrderBy(x => Random.Next())
                            .Where(x => x.HasMatch)
                            .Select(x => x.MatchingObject)
                            .Where(_currentTrainNetwork.Contains)
                            .AsQueryable();

                        var next = queryNext
                            .FirstOrDefault(x => x != PreviousPosition && x != CurrentPosition)
                                   ?? queryNext.FirstOrDefault();

                        Move(next);
                    }
                });
            }
        }

        private readonly SimpleCache<ISet<ISet<IZoneInfo>>> _cachedNetworks;

        private ISet<ISet<IZoneInfo>> GetRailwayNetworks()
        {
            var railwayNetworks = new HashSet<ISet<IZoneInfo>>();
            foreach (var railroadZoneInfo in GetZoneInfosFunc()
                .Where(x => x.ZoneConsumptionState.GetIsRailroadNetworkMember())
                .Where(x => !railwayNetworks.SelectMany(y => y).Contains(x)))
            {
                var railwayNetwork = new HashSet<IZoneInfo> { railroadZoneInfo };

                foreach (var member in railroadZoneInfo
                    .CrawlAllDirections(x => x.ConsumptionState.GetIsRailroadNetworkMember())
                    )
                {
                    railwayNetwork.Add(GetZoneInfosFunc().First(x => x == member));
                }

                railwayNetworks.Add(railwayNetwork);
            }
            return railwayNetworks;
        }
    }

    public interface IVehicle
    {
        bool CanBeRemoved { get; }
        IZoneInfo CurrentPosition { get; }
        IZoneInfo PreviousPreviousPreviousPreviousPosition { get; }
        IZoneInfo PreviousPreviousPreviousPosition { get; }
        IZoneInfo PreviousPreviousPosition { get; }
        IZoneInfo PreviousPosition { get; }
    }

    internal abstract class BaseVehicle : IVehicle
    {
        protected readonly Func<ISet<IZoneInfo>> GetZoneInfosFunc;

        protected BaseVehicle(Func<ISet<IZoneInfo>> getZoneInfosFunc, IZoneInfo currentPosition)
        {
            GetZoneInfosFunc = getZoneInfosFunc;
            CurrentPosition = currentPosition;
        }

        protected void Move(IZoneInfo next)
        {
            PreviousPreviousPreviousPreviousPosition = PreviousPreviousPreviousPosition;
            PreviousPreviousPreviousPosition = PreviousPreviousPosition;
            PreviousPreviousPosition = PreviousPosition;

            PreviousPosition = CurrentPosition;

            CurrentPosition = next;
        }

        public IZoneInfo PreviousPreviousPreviousPreviousPosition { get; protected set; }
        public IZoneInfo PreviousPreviousPreviousPosition { get; protected set; }
        public IZoneInfo PreviousPreviousPosition { get; protected set; }
        public IZoneInfo PreviousPosition { get; protected set; }
        public IZoneInfo CurrentPosition { get; protected set; }

        private DateTime _lastChange = DateTime.Now;

        public bool CanBeRemoved
        {
            get
            {
                return CurrentPosition == null || _lastChange < DateTime.Now.AddSeconds(-3);
            }
        }

        protected abstract int SpeedInMilliseconds { get; }

        protected void IfMustBeMoved(Action action)
        {
            if (_lastChange > DateTime.Now.AddMilliseconds(-SpeedInMilliseconds))
            {
                return;
            }
            _lastChange = DateTime.Now;
            action();
        }
    }

    public interface ITrain : IMoveableVehicle
    {
        void SetTrainNetwork(ISet<IZoneInfo> trainNetwork);
    }

    public interface IMoveableVehicle : IVehicle
    {
        void Move();
    }

    public interface IAirplane : IMoveableVehicle
    {
    }

    public interface IShip : IMoveableVehicle
    {
    }

    public interface IVehicleController<out TVehicle> where TVehicle : IVehicle
    {
        void ForEachActiveVehicle(Action<TVehicle> vehicleAction);
    }
}