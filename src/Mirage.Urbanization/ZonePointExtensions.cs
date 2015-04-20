using System.Linq;

namespace Mirage.Urbanization
{
    public static class ZonePointExtensions
    {
        public static QueryResult<ZoneInfo> GetZoneInfoOn(this ZonePoint zonePoint, Area area)
        {
            return new QueryResult<ZoneInfo>(area.EnumerateZoneInfos().Single(x => x.Point == zonePoint));
        }
    }
}