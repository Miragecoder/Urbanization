using System.Collections.Generic;

namespace Mirage.Urbanization.ZoneConsumption.Base
{
    public interface IAreaZoneClusterConsumption : IAreaConsumption
    {
        IReadOnlyCollection<ZoneClusterMemberConsumption> ZoneClusterMembers { get; }
    }
}