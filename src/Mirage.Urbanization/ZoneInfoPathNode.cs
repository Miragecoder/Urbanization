using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization
{
    class ZoneInfoPathNode : IZoneInfoPathNode
    {
        private readonly IZoneInfo _zoneInfo;
        private readonly IZoneInfoPathNode _previousPathNode;
        private readonly IZoneInfoPathNode _originParentPathNode;
        private readonly Lazy<List<IZoneInfoPathNode>> _childPathsLazy;
        private readonly int _distance;

        private readonly int? _destinationHashCode;

        public IZoneInfoPathNode PreviousPathNode { get { return _previousPathNode; } }

        public ZoneInfoPathNode(IZoneInfo zoneInfo, Func<IZoneInfoPathNode, QueryResult<IZoneInfo, RelativeZoneInfoQuery>, bool> canBeJoinedFunc, Func<IZoneInfo, int?> getDestinationHashCode, ZoneInfoDistanceTracker distanceTracker)
            : this(
            zoneInfo: zoneInfo,
            canBeJoinedFunc: canBeJoinedFunc,
            getDestinationHashCode: getDestinationHashCode,
            previousPathNode: null,
            originParentPathNode: null,
            distance: 0,
            distanceTracker: distanceTracker)
        {

        }

        private ZoneInfoPathNode(
            IZoneInfo zoneInfo,
            Func<IZoneInfoPathNode, QueryResult<IZoneInfo, RelativeZoneInfoQuery>, bool> canBeJoinedFunc,
            Func<IZoneInfo, int?> getDestinationHashCode,
            IZoneInfoPathNode previousPathNode,
            IZoneInfoPathNode originParentPathNode,
            int distance,
            ZoneInfoDistanceTracker distanceTracker)
        {
            _zoneInfo = zoneInfo;
            _previousPathNode = previousPathNode;

            _originParentPathNode = originParentPathNode ?? this;

            _destinationHashCode = getDestinationHashCode(zoneInfo);

            _distance = distance;

            _childPathsLazy = new Lazy<List<IZoneInfoPathNode>>(() => 
                !distanceTracker.IsPreviouslyNotSeenOrSeenAtLargerDistance(_zoneInfo, distance, previousPathNode) 
                ? Enumerable.Empty<IZoneInfoPathNode>().ToList()
                : ZoneInfo
                    .GetNorthEastSouthWest()
                    .Where(x => x.HasMatch)
                    .Where(x => x.MatchingObject != (_previousPathNode != null ? _previousPathNode.ZoneInfo : null))
                    .Where(x => distanceTracker.DoesNotExceedMaximumDistanceForAllCriteria(_zoneInfo, distance))
                    .Where(x => canBeJoinedFunc(this, x))
                    .Select(x => new ZoneInfoPathNode(x.MatchingObject, canBeJoinedFunc, getDestinationHashCode, this, _originParentPathNode, _distance + x.MatchingObject.GetDistanceScoreBasedOnConsumption(), distanceTracker))
                    .ToList<IZoneInfoPathNode>());
        }

        public bool IsDestination { get { return _destinationHashCode.HasValue; } }

        public int? DestinationHashCode { get { return _destinationHashCode; } }

        public IZoneInfo ZoneInfo
        {
            get { return _zoneInfo; }
        }

        public bool GetIsPartOfParentCluster()
        {
            var thisAsClusterMember = _zoneInfo.ConsumptionState.GetZoneConsumption() as ZoneClusterMemberConsumption;
            if (thisAsClusterMember == null) return false;

            var thatAsClusterMember = GetOrigin().ConsumptionState.GetZoneConsumption() as ZoneClusterMemberConsumption;

            if (thatAsClusterMember == null) throw new InvalidOperationException();

            return thisAsClusterMember.ParentBaseZoneClusterConsumption ==
                   thatAsClusterMember.ParentBaseZoneClusterConsumption;
        }

        public IEnumerable<IZoneInfoPathNode> EnumerateAllChildPathNodes()
        {
            foreach (var x in _childPathsLazy.Value)
            {
                yield return x;
                foreach (var t in x.EnumerateAllChildPathNodes())
                    yield return t;
            }
        }

        public int Distance
        {
            get { return _distance; }
        }

        private IZoneInfo GetOrigin()
        {
            return _originParentPathNode.ZoneInfo;
        }

        public IEnumerable<IZoneInfoPathNode> EnumeratePathBackwards()
        {
            yield return this;
            if (_previousPathNode != null)
                foreach (var x in _previousPathNode.EnumeratePathBackwards())
                    yield return x;
        }

        public IZoneInfoPathNode WithPathMembers(Action<IZoneInfoPathNode> func)
        {
            foreach (var pathMember in EnumeratePathBackwards())
                func(pathMember);
            return this;
        }

        public IZoneInfoPathNode WithDestination(Action<IZoneInfo> func)
        {
            if (IsDestination)
                func(ZoneInfo);
            return this;
        }
    }
}
