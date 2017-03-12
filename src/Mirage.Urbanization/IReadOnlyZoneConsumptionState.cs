using System;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization
{
    public interface IReadOnlyZoneConsumptionState
    {
        IAreaZoneConsumption GetZoneConsumption();
        DateTime LastUpdateDateTime { get; }
        bool GetIsRailroadNetworkMember();

        QueryResult<ZoneClusterMemberAndParent<T>> GetIfZoneClusterAndParent<T>()
            where T : BaseZoneClusterConsumption;
    }

    public class ZoneClusterMemberAndParent<T>
        where T : BaseZoneClusterConsumption
    {
        public ZoneClusterMemberAndParent(ZoneClusterMemberConsumption zoneClusterMemberConsumption, T baseZoneClusterConsumption)
        {
            ZoneClusterMemberConsumption = zoneClusterMemberConsumption ?? throw new ArgumentNullException(nameof(zoneClusterMemberConsumption));
            BaseZoneClusterConsumption = baseZoneClusterConsumption ?? throw new ArgumentNullException(nameof(baseZoneClusterConsumption));
        }

        public ZoneClusterMemberConsumption ZoneClusterMemberConsumption { get; }
        public T BaseZoneClusterConsumption { get; }
    }
}