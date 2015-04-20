using System;
using System.Collections.Generic;
using System.Linq;

namespace Mirage.Urbanization.Networks
{
    internal abstract class BaseZoneNetworkInfo
    {
        protected readonly ISet<IZoneInfo> MemberZoneInfos;

        protected BaseZoneNetworkInfo(IEnumerable<IZoneInfo> memberZoneInfos)
        {
            if (memberZoneInfos == null) throw new ArgumentNullException("memberZoneInfos");

            MemberZoneInfos = new HashSet<IZoneInfo>(memberZoneInfos);
        }

        protected static IList<ISet<IZoneInfo>> CollectZoneInfoNetworkSets(IReadOnlyDictionary<ZonePoint, ZoneInfo> zoneInfos, Func<IZoneInfo, bool> isNetworkMember)
        {
            var networkSets = new List<ISet<IZoneInfo>>();
            foreach (var networkZoneInfo in zoneInfos
                .Values
                .Where(isNetworkMember)
                .Where(networkZoneInfo => !networkSets
                    .Any(x => x.Contains(networkZoneInfo))
                )
                )
            {
                networkSets.Add(new HashSet<IZoneInfo>(networkZoneInfo.CrawlAllDirections(x => isNetworkMember(x))));
            }

            return networkSets;
        }
    }
}