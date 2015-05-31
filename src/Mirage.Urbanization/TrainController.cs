using System;
using System.Collections.Generic;
using System.Linq;

namespace Mirage.Urbanization
{
    internal class TrainController : ITrainController
    {
        private readonly Func<IDictionary<IReadOnlyZoneInfo, ZoneInfo>> _getZoneInfosFunc;

        public TrainController(Func<IDictionary<IReadOnlyZoneInfo, ZoneInfo>> getZoneInfosFunc)
        {
            _getZoneInfosFunc = getZoneInfosFunc;

            _cachedNetworks = new SimpleCache<ISet<ISet<ZoneInfo>>>(GetRailwayNetworks, new TimeSpan(0, 0, 1));
        }

        public void ForEachActiveTrain(Action<ITrain> trainAction)
        {
            PerformTrainMovementCycle();

            foreach (var train in _trains)
            {
                trainAction(train);
            }
        }

        private void PerformTrainMovementCycle()
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
                    trainsInNetwork = _trains
                        .Where(x => network.Contains(x.CurrentPosition))
                        .ToList();

                    int desiredAdditionaTrains = desiredAmountOfTrains - trainsInNetwork.Count;

                    if (desiredAdditionaTrains > 0)
                    {
                        foreach (var iteration in Enumerable.Range(0, desiredAmountOfTrains - trainsInNetwork.Count))
                        {
                            _trains.Add(new Train(_getZoneInfosFunc, network
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

            foreach (var orphanTrain in _trains.Where(x => x.CanBeRemoved).ToArray())
                _trains.Remove(orphanTrain);

        }

        private readonly HashSet<ITrain> _trains = new HashSet<ITrain>();

        private static readonly Random Random = new Random();

        private class Train : ITrain
        {
            private ZoneInfo _currentPosition;
            private ZoneInfo _previousPosition;
            private ZoneInfo _previousPreviousPosition;
            private ZoneInfo _previousPreviousPreviousPosition;
            private ZoneInfo _previousPreviousPreviousPreviousPosition;

            public ZoneInfo CurrentPosition { get { return _currentPosition; } }

            public bool CanBeRemoved
            {
                get
                {
                    return _currentPosition == null || _lastChange < DateTime.Now.AddSeconds(-3);
                }
            }

            public ZoneInfo PreviousPreviousPreviousPreviousPosition { get { return _previousPreviousPreviousPreviousPosition; } }
            public ZoneInfo PreviousPreviousPreviousPosition { get { return _previousPreviousPreviousPosition; } }
            public ZoneInfo PreviousPreviousPosition { get { return _previousPreviousPosition; } }
            public ZoneInfo PreviousPosition { get { return _previousPosition; } }

            private readonly Func<IDictionary<IReadOnlyZoneInfo, ZoneInfo>> _getZoneInfosFunc;

            public Train(Func<IDictionary<IReadOnlyZoneInfo, ZoneInfo>> getZoneInfosFunc, ZoneInfo currentPosition)
            {
                _getZoneInfosFunc = getZoneInfosFunc;
                _currentPosition = currentPosition;
            }

            private DateTime _lastChange = DateTime.Now;

            public void CrawlNetwork(ISet<ZoneInfo> trainNetwork)
            {
                if (_lastChange > DateTime.Now.AddMilliseconds(-300))
                {
                    return;
                }
                _lastChange = DateTime.Now;
                if (!trainNetwork.Contains(_currentPosition))
                {
                    _currentPosition = trainNetwork.First();
                }
                else
                {
                    var queryNext = _currentPosition
                        .GetNorthEastSouthWest()
                        .OrderBy(x => Random.Next())
                        .Where(x => x.HasMatch)
                        .Select(x => x.MatchingObject)
                        .Where(trainNetwork.Contains)
                        .Select(x => _getZoneInfosFunc()[x])
                        .AsQueryable();

                    var next = queryNext
                        .FirstOrDefault(x => x != _previousPosition && x != _currentPosition)
                               ?? queryNext.FirstOrDefault();

                    _previousPreviousPreviousPreviousPosition = _previousPreviousPreviousPosition;
                    _previousPreviousPreviousPosition = _previousPreviousPosition;
                    _previousPreviousPosition = _previousPosition;

                    _previousPosition = _currentPosition;

                    _currentPosition = next;
                }
            }
        }

        private readonly SimpleCache<ISet<ISet<ZoneInfo>>> _cachedNetworks;

        private ISet<ISet<ZoneInfo>> GetRailwayNetworks()
        {
            var railwayNetworks = new HashSet<ISet<ZoneInfo>>();
            foreach (var railroadZoneInfo in _getZoneInfosFunc()
                .Where(x => x.Key.ZoneConsumptionState.GetIsRailroadNetworkMember())
                .Where(x => !railwayNetworks.SelectMany(y => y).Contains(x.Value)))
            {
                var railwayNetwork = new HashSet<ZoneInfo> { railroadZoneInfo.Value };

                foreach (var member in railroadZoneInfo
                    .Key
                    .CrawlAllDirections(x => x.ConsumptionState.GetIsRailroadNetworkMember())
                    )
                {
                    railwayNetwork.Add(_getZoneInfosFunc().First(x => x.Key == member).Value);
                }

                railwayNetworks.Add(railwayNetwork);
            }
            return railwayNetworks;
        }
    }

    public interface ITrain
    {
        bool CanBeRemoved { get; }
        ZoneInfo CurrentPosition { get; }
        void CrawlNetwork(ISet<ZoneInfo> network);
        ZoneInfo PreviousPreviousPreviousPreviousPosition { get; }
        ZoneInfo PreviousPreviousPreviousPosition { get; }
        ZoneInfo PreviousPreviousPosition { get; }
        ZoneInfo PreviousPosition { get; }
    }

    public interface ITrainController
    {
        void ForEachActiveTrain(Action<ITrain> trainAction);
    }
}