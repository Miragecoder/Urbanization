using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mirage.Urbanization.ZoneConsumption;

namespace Mirage.Urbanization.Vehicles
{
    internal class ShipController : BaseStructureVehicleController<IShip, SeaPortZoneClusterConsumption>
    {
        private readonly TimeSpan _shipInsertionInterval;
        private readonly int _shipSpeedInMilliSeconds;
        private readonly int _maxDistance;
        internal const int AmountOfShipsPerHarbour = 2;

        public ShipController(
            IReadOnlyArea area,
            Func<ISet<IZoneInfo>> getZoneInfosFunc)
            :this(getZoneInfosFunc, TimeSpan.FromSeconds(5), 500)
        {
            _maxDistance = area.AmountOfZonesX + area.AmountOfZonesY;
        }
        internal ShipController(Func<ISet<IZoneInfo>> getZoneInfosFunc, TimeSpan shipInsertionInterval, int shipSpeedInMilliSeconds)
            : base(getZoneInfosFunc)
        {
            _shipInsertionInterval = shipInsertionInterval;
            _shipSpeedInMilliSeconds = shipSpeedInMilliSeconds;
            _candidatesCache = new SimpleCache<ISet<IZoneInfo>>(() => new HashSet<IZoneInfo>(GetZoneInfosFunc()
                .Where(IsSuitableForShip)), new TimeSpan(0, 1, 0));
        }

        private readonly SimpleCache<ISet<IZoneInfo>> _candidatesCache;

        private DateTime _lastShipInsertion = DateTime.Now;

        private bool NewShipCanBeInserted => DateTime.Now.Add(-_shipInsertionInterval) > _lastShipInsertion;

        protected override void PrepareVehiclesWithStructures(SeaPortZoneClusterConsumption[] structures)
        {
            int desiredAmountOfShips = structures.Count() * AmountOfShipsPerHarbour;

            var candidates = _candidatesCache.GetValue();

            if (candidates == null)
                return;

            while (NewShipCanBeInserted
                && Vehicles.Count() < desiredAmountOfShips
                && Vehicles.All(x => x.IsReadyAndMoving))
            {
                var candidate = GetZoneInfosFunc()
                    .Where(IsSuitableForShip)
                    .OrderBy(x => Random.Next())
                    .FirstOrDefault();

                if (candidate != null)
                {
                    AddVehicle(new Ship(GetZoneInfosFunc, candidate, _shipSpeedInMilliSeconds, _maxDistance));
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
            protected override int SpeedInMilliseconds { get; }

            public Ship(Func<ISet<IZoneInfo>> getZoneInfosFunc, IZoneInfo currentPosition, int speedInMilliseconds, int maxDistance)
                : base(getZoneInfosFunc, currentPosition)
            {
                SpeedInMilliseconds = speedInMilliseconds;
                _pathEnumeratorTask = new Task<IEnumerator<ShipPathNode>>(() =>
                {
                    var pathNode = new ShipPathNode(CurrentPosition, maxDistance);

                    var enumerator = pathNode
                        .EnumerateAllChildPathNodes()
                        .Where(x => x.Distance < maxDistance)
                        .GroupBy(x => CalculateDistance(CurrentPosition.Point, x.CurrentZoneInfo.Point))
                        .OrderByDescending(x => x.Key)
                        .FirstOrDefault()
                        ?.OrderBy(x => x.Distance)
                        ?.FirstOrDefault()
                        ?.EnumeratePathBackwards()
                        ?.GetEnumerator() ?? new List<ShipPathNode>() { pathNode }.GetEnumerator();

                    enumerator.MoveNext();

                    return enumerator;
                });
                _pathEnumeratorTask.Start();
            }

            private readonly Task<IEnumerator<ShipPathNode>> _pathEnumeratorTask;

            public bool IsReadyAndMoving => _pathEnumeratorTask.IsCompleted;

            public void Move()
            {
                if (!_pathEnumeratorTask.IsCompleted)
                    return;
                IfMustBeMoved(() =>
                {
                    var current = _pathEnumeratorTask.Result.Current;
                    Move(current?.CurrentZoneInfo);

                    if (!_pathEnumeratorTask.Result.MoveNext())
                        Move(null);
                });
            }

            private static double CalculateDistance(ZonePoint x, ZonePoint y)
            {
                int x1 = x.X, x2 = y.X, y1 = x.Y, y2 = y.Y;

                return ((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
            }

            private class ShipPathNode
            {
                private readonly IZoneInfo _rootZoneInfo;
                private readonly ShipPathNode _preceedingShipPathNode;
                private readonly int _maxDistance;

                private readonly Lazy<IEnumerable<ShipPathNode>> _childrenLazy;

                public ShipPathNode(IZoneInfo zoneInfo, int maxDistance)
                    : this(
                        rootZoneInfo: zoneInfo,
                        currentZoneInfo: zoneInfo,
                        preceedingShipPathNode: null,
                        seenPaths: new HashSet<IZoneInfo>(),
                        distance: 0,
                        maxDistance: maxDistance
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

                public IZoneInfo CurrentZoneInfo { get; }

                public IEnumerable<ShipPathNode> EnumeratePathBackwards()
                {
                    yield return this;
                    if (_preceedingShipPathNode != null)
                        foreach (var x in _preceedingShipPathNode.EnumeratePathBackwards())
                            yield return x;
                }

                public int Distance { get; }

                private ShipPathNode(
                    IZoneInfo rootZoneInfo,
                    IZoneInfo currentZoneInfo,
                    ShipPathNode preceedingShipPathNode,
                    ISet<IZoneInfo> seenPaths,
                    int distance,
                    int maxDistance
                    )
                {
                    _maxDistance = maxDistance;
                    _rootZoneInfo = rootZoneInfo ?? throw new ArgumentNullException(nameof(rootZoneInfo));
                    CurrentZoneInfo = currentZoneInfo ?? throw new ArgumentNullException(nameof(currentZoneInfo));

                    if (!seenPaths.Add(currentZoneInfo))
                        throw new ArgumentException("'currentZoneInfo' was already added to this path.", nameof(currentZoneInfo));

                    _preceedingShipPathNode = preceedingShipPathNode;

                    Distance = distance;
                    _childrenLazy = new Lazy<IEnumerable<ShipPathNode>>(() => CurrentZoneInfo
                        .GetNorthEastSouthWest()
                        .Where(x => Distance + 1 < maxDistance)
                        .Where(x => x.HasMatch)
                        .Where(x => _preceedingShipPathNode != null ? x.MatchingObject != _preceedingShipPathNode.CurrentZoneInfo : true)
                        .Where(x => !seenPaths.Contains(x.MatchingObject))
                        .Where(x => IsSuitableForShip(x.MatchingObject))
                        .OrderByDescending(x => CalculateDistance(x.MatchingObject.Point, _rootZoneInfo.Point))
                        .Where(x => !seenPaths.Contains(x.MatchingObject))
                        .Select(x =>
                            new ShipPathNode(
                                rootZoneInfo: rootZoneInfo,
                                currentZoneInfo: x.MatchingObject,
                                preceedingShipPathNode: this,
                                seenPaths: seenPaths,
                                distance: Distance + 1,
                                maxDistance: _maxDistance
                                )
                        )
                        );
                }
            }
        }
    }
}