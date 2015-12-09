using System.Drawing;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Tilesets
{
    public class GrowthZoneTileCellInfo
    {
        public GrowthZoneTileCellInfo(GrowthZoneClusterTileInfo growthZoneClusterTileInfo, Point point)
        {
            GrowthZoneClusterTileInfo = growthZoneClusterTileInfo;
            Point = point;
        }

        public GrowthZoneClusterTileInfo GrowthZoneClusterTileInfo { get; }
        public Point Point { get; }

        public override int GetHashCode()
        {
            return (GrowthZoneClusterTileInfo.GroupId + Point.X + Point.Y).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return (obj as GrowthZoneTileCellInfo)
                .ToQueryResult()
                .WithResultIfHasMatch(x => x.GetHashCode() == GetHashCode());
        }
    }
}