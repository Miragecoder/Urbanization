using System;
using System.Linq;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization
{
    public static class ZonePointExtensions
    {
        public static QueryResult<ZoneInfo> GetZoneInfoOn(this ZonePoint zonePoint, Area area)
        {
            return new QueryResult<ZoneInfo>(area.EnumerateZoneInfos().Single(x => x.Point == zonePoint));
        }

        public static Orientation OrientationTo(this ZonePoint x, ZonePoint y)
        {
            Orientation? orientation = null;

            if (x.X < y.X)
            {
                orientation = Orientation.East;
            }
            if (x.X > y.X)
            {
                orientation = orientation ^ Orientation.West ?? Orientation.West;
            }
            if (x.Y > y.Y)
            {
                orientation = orientation ^ Orientation.North ?? Orientation.North;
            }
            if (x.Y < y.Y)
            {
                orientation = orientation ^ Orientation.South ?? Orientation.South;
            }

            if (orientation.HasValue)
                return orientation.Value;

            throw new InvalidOperationException();
        }
    }
}