using System;
using System.Linq;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Tilesets
{
    public class GrowthZoneHouseTileInfo
    {
        public GrowthZoneHouseTileInfo(string resourceName)
        {
            var fileName = resourceName.Split('.').Reverse().Skip(1).Take(1).First();
            //q1n1a1.png
            var segments = fileName.Split('_');
            
            Quality = Convert.ToInt32(segments[0].Substring(1));
            Number = Convert.ToInt32(segments[1].Substring(1));
        }

        public int Quality { get; }
        public int Number { get; }

        public string Id => string.Join("_", Quality, Number);

        public override bool Equals(object obj)
        {
            return (obj as GrowthZoneHouseTileInfo).ToQueryResult()
                .WithResultIfHasMatch(x => Id == x.Id, false);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}