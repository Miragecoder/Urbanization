using System;
using System.Linq;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Tilesets
{
    public class GrowthZoneTileInfo
    {
        public GrowthZoneTileInfo(string resourceName)
        {
            var fileName = resourceName.Split('.').Reverse().Skip(1).Take(1).First();
            //d0_q0_n1_a1.png
            var segments = fileName.Split('_');

            Density = Convert.ToInt32(segments[0].Substring(1));
            Quality = Convert.ToInt32(segments[1].Substring(1));
            Id = Convert.ToInt32(segments[2].Substring(1));
        }

        public int Density { get; }
        public int Quality { get; }
        public int Id { get; }
        public string GroupId => string.Join("_", Density, Quality, Id);

        public override bool Equals(object obj)
        {
            return (obj as GrowthZoneTileInfo).ToQueryResult()
                .WithResultIfHasMatch(x => GroupId == x.GroupId, false);
        }

        public override int GetHashCode()
        {
            return GroupId.GetHashCode();
        }
    }
}