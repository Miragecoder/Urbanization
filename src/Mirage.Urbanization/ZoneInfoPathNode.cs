using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization
{
    class ZoneInfoPathNode : IZoneInfoPathNode
    {
        private readonly IZoneInfoPathNode _originParentPathNode;
        private readonly Lazy<List<IZoneInfoPathNode>> _childPathsLazy;

        public IZoneInfoPathNode PreviousPathNode { get; }

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
            ZoneInfo = zoneInfo;
            PreviousPathNode = previousPathNode;

            _originParentPathNode = originParentPathNode ?? this;

            DestinationHashCode = getDestinationHashCode(zoneInfo);

            Distance = distance;

            _childPathsLazy = new Lazy<List<IZoneInfoPathNode>>(() => 
                !distanceTracker.IsPreviouslyNotSeenOrSeenAtLargerDistance(ZoneInfo, distance, previousPathNode) 
                ? Enumerable.Empty<IZoneInfoPathNode>().ToList()
                : ZoneInfo
                    .GetNorthEastSouthWest()
                    .Where(x => x.HasMatch)
                    .Where(x => x.MatchingObject != (PreviousPathNode != null ? PreviousPathNode.ZoneInfo : null))
                    .Where(x => distanceTracker.DoesNotExceedMaximumDistanceForAllCriteria(ZoneInfo, distance))
                    .Where(x => canBeJoinedFunc(this, x))
                    .Select(x => new ZoneInfoPathNode(x.MatchingObject, canBeJoinedFunc, getDestinationHashCode, this, _originParentPathNode, Distance + x.MatchingObject.GetDistanceScoreBasedOnConsumption(), distanceTracker))
                    .ToList<IZoneInfoPathNode>());
        }

        public bool IsDestination => DestinationHashCode.HasValue;

        public int? DestinationHashCode { get; }

        public IZoneInfo ZoneInfo { get; }

        public bool GetIsPartOfParentCluster()
        {
            var thisAsClusterMember = ZoneInfo.ConsumptionState.GetZoneConsumption() as ZoneClusterMemberConsumption;
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

        public int Distance { get; }

        private IZoneInfo GetOrigin()
        {
            return _originParentPathNode.ZoneInfo;
        }

        public IEnumerable<IZoneInfoPathNode> EnumeratePathBackwards()
        {
            yield return this;
            if (PreviousPathNode != null)
                foreach (var x in PreviousPathNode.EnumeratePathBackwards())
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
