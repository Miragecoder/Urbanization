using System;
using System.Collections.Generic;

namespace Mirage.Urbanization
{
    public interface IZoneInfoPathNode
    {
        IZoneInfoPathNode PreviousPathNode { get; }
        bool IsDestination { get; }
        int? DestinationHashCode { get; }
        IZoneInfo ZoneInfo { get; }
        int Distance { get; }
        bool GetIsPartOfParentCluster();
        IEnumerable<IZoneInfoPathNode> EnumerateAllChildPathNodes();
        IEnumerable<IZoneInfoPathNode> EnumeratePathBackwards();
        IZoneInfoPathNode WithPathMembers(Action<IZoneInfoPathNode> func);
        IZoneInfoPathNode WithDestination(Action<IZoneInfo> func);
    }
}