using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Mirage.Urbanization.Persistence
{
    public class PersistedArea
    {
        public PersistedSingleZoneConsumption[] PersistedSingleZoneConsumptions { get; set; }
        public PersistedIntersectingZoneConsumption[] PersistedIntersectingZoneConsumptions { get; set; }
        public PersistedStaticZoneClusterCentralMemberConsumption[] PersistedStaticZoneClusterCentralMemberConsumptions { get; set; }
        public PersistedGrowthZoneClusterCentralMemberConsumption[] PersistedGrowthZoneClusterCentralMemberConsumptions { get; set; }
        public PersistedTrafficState[] PersistedTrafficStates { get; set; }

        private static IEnumerable<int> TryGetIfNotEmpty<TValue>(IEnumerable<TValue> value, Func<IEnumerable<TValue>, int> lengthFunc)
        {
            if (value != null && value.Any())
                yield return lengthFunc(value);
            yield return default(int);
        }

        internal int GetZoneWidthAndHeight()
        {
            var entries = TryGetIfNotEmpty(PersistedSingleZoneConsumptions, x => x.Max(y => y.X))
                .Concat(TryGetIfNotEmpty(PersistedSingleZoneConsumptions, x => x.Max(y => y.Y)))
                .Concat(TryGetIfNotEmpty(PersistedIntersectingZoneConsumptions, x => x.Max(y => y.X)))
                .Concat(TryGetIfNotEmpty(PersistedIntersectingZoneConsumptions, x => x.Max(y => y.Y)))
                .Concat(TryGetIfNotEmpty(PersistedStaticZoneClusterCentralMemberConsumptions, x => x.Max(y => y.X)))
                .Concat(TryGetIfNotEmpty(PersistedStaticZoneClusterCentralMemberConsumptions, x => x.Max(y => y.Y)))
                .Concat(TryGetIfNotEmpty(PersistedGrowthZoneClusterCentralMemberConsumptions, x => x.Max(y => y.X)))
                .Concat(TryGetIfNotEmpty(PersistedGrowthZoneClusterCentralMemberConsumptions, x => x.Max(y => y.Y)));

            return entries.Max() + 1;
        }
    }
}