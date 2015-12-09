using System.Drawing;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Tilesets
{
    public class GrowthZoneTileCellInfo
    {
        public GrowthZoneTileCellInfo(GrowthZoneTileInfo growthZoneTileInfo, Point point)
        {
            GrowthZoneTileInfo = growthZoneTileInfo;
            Point = point;
        }

        public GrowthZoneTileInfo GrowthZoneTileInfo { get; }
        public Point Point { get; }

        public override int GetHashCode()
        {
            return (GrowthZoneTileInfo.GroupId + Point.X + Point.Y).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return (obj as GrowthZoneTileCellInfo)
                .ToQueryResult()
                .WithResultIfHasMatch(x => x.GetHashCode() == GetHashCode());
        }
    }
}