using System;
using System.Collections.Generic;
using System.Linq;

namespace Mirage.Urbanization.Vehicles
{
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
}