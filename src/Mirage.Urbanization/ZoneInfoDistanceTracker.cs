using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization
{
    class ZoneInfoDistanceTracker
    {
        private readonly Dictionary<Func<IZoneInfo, bool>, int?> _findFuncs;
        private readonly Dictionary<IZoneInfo, DistanceAndConsumption> _nodesAndDistances = new Dictionary<IZoneInfo, DistanceAndConsumption>();

        public ZoneInfoDistanceTracker(params Func<IZoneInfo, bool>[] findFuncs)
        {
            _findFuncs = findFuncs.ToDictionary(x => x, x => default(int?));
        }

        private class DistanceAndConsumption
        {
            public DistanceAndConsumption(int distance)
            {
                Distance = distance;
            }

            public int Distance { get; private set; }

            public void UpdateDistance(int distance)
            {
                Distance = distance;
            }
        }

        private const int MaximumDistance = 150;

        public bool DoesNotExceedMaximumDistanceForAllCriteria(IZoneInfo zoneInfo, int distance)
        {
            if (distance >= MaximumDistance)
                return false;
            foreach (var kvp in _findFuncs.ToDictionary(x => x.Key, x => x.Value))
            {
                if (kvp.Key(zoneInfo))
                {
                    if (kvp.Value.HasValue)
                    {
                        var currentDistance = kvp.Value;
                        if (currentDistance > distance)
                        {
                            _findFuncs[kvp.Key] = distance;
                        }
                    }
                    else
                    {
                        _findFuncs[kvp.Key] = distance;
                    }
                }
            }

            if (_findFuncs.All(x => x.Value.HasValue))
            {
                var maxDistance = _findFuncs.Max(x => x.Value.Value);

                return maxDistance >= distance;
            }
            return true;
        }

        public bool IsPreviouslyNotSeenOrSeenAtLargerDistance(IZoneInfo node, int distance, IZoneInfoPathNode previousPathNode)
        {
            var currentConsumption = node.ConsumptionState.GetZoneConsumption();
            var previousConsumption = previousPathNode?.ZoneInfo.ConsumptionState.GetZoneConsumption();

            if (currentConsumption is IntersectingZoneConsumption)
            {
                var currentConsumptionAsIntersection = currentConsumption as IntersectingZoneConsumption;
                if (!_intersectionsAndSeenTypes.ContainsKey(currentConsumptionAsIntersection))
                {
                    _intersectionsAndSeenTypes.Add(currentConsumptionAsIntersection, new HashSet<Type>());
                }
                if (previousConsumption != null)
                    return _intersectionsAndSeenTypes[currentConsumptionAsIntersection].Add(previousConsumption.GetType());
                else
                    throw new InvalidOperationException();
            }

            if (_nodesAndDistances.ContainsKey(node))
            {
                if (_nodesAndDistances[node].Distance > distance)
                {
                    _nodesAndDistances[node].UpdateDistance(distance);
                    return true;
                }
                return false;
            }
            else
            {
                _nodesAndDistances.Add(node, new DistanceAndConsumption(distance));
                return true;
            }
        }

        private readonly Dictionary<IntersectingZoneConsumption, HashSet<Type>> _intersectionsAndSeenTypes = new Dictionary<IntersectingZoneConsumption, HashSet<Type>>();
    }
}