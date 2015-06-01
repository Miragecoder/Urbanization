using System;
using System.Collections.Generic;
using System.Linq;
using Mirage.Urbanization.ZoneConsumption;

namespace Mirage.Urbanization
{
    internal class AirplaneController : BaseVehicleController<IAirplane>
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

        protected override void PerformMoveCycle()
        {
            var airports = GetZoneInfosFunc()
                .Select(x => x.GetAsZoneCluster<AirportZoneClusterConsumption>())
                .Where(x => x.HasMatch)
                .Select(x => x.MatchingObject)
                .Distinct()
                .ToArray();

            if (!airports.Any())
                return;

            int desiredAmountOfPlanes = airports.Count() * 2;

            while (Vehicles.Count < desiredAmountOfPlanes)
            {
                var spawnPoint = airports.OrderBy(x => Random.Next()).First();
                var centralCell = spawnPoint.ZoneClusterMembers.Single(y => y.IsCentralClusterMember);

                centralCell.GetZoneInfo().WithResultIfHasMatch(z =>
                {
                    var steerDirection = Directions.OrderBy(d => Random.Next()).First();
                    centralCell.GetZoneInfo().WithResultIfHasMatch(zoneInfo =>
                    {
                        var alternateDirection =
                            zoneInfo.GetSteerDirections(steerDirection).OrderBy(x => Random.Next()).First();

                        Vehicles.Add(new Airplane(GetZoneInfosFunc, z, steerDirection, alternateDirection, Random.Next(5, 10)));
                    });
                });

            }

            foreach (var plane in Vehicles)
            {
                plane.Move();
            }

            RemoveOrphanVehicles();
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
                    train.CrawlNetwork(network);
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

            public void CrawlNetwork(ISet<IZoneInfo> trainNetwork)
            {
                IfMustBeMoved(() =>
                {
                    if (!trainNetwork.Contains(CurrentPosition))
                    {
                        CurrentPosition = trainNetwork.First();
                    }
                    else
                    {
                        var queryNext = CurrentPosition
                            .GetNorthEastSouthWest()
                            .OrderBy(x => Random.Next())
                            .Where(x => x.HasMatch)
                            .Select(x => x.MatchingObject)
                            .Where(trainNetwork.Contains)
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

    public interface ITrain : IVehicle
    {
        void CrawlNetwork(ISet<IZoneInfo> network);
    }

    public interface IAirplane : IVehicle
    {
        void Move();
    }

    public interface IVehicleController<out TVehicle> where TVehicle : IVehicle
    {
        void ForEachActiveVehicle(Action<TVehicle> vehicleAction);
    }
}