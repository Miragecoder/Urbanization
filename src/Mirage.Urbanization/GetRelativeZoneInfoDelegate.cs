using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization
{
    public delegate QueryResult<IZoneInfo, RelativeZoneInfoQuery> GetRelativeZoneInfoDelegate(RelativeZoneInfoQuery relativeZoneInfoQuery);
}