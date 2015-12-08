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
            if (zoneClusterMemberConsumption == null) throw new ArgumentNullException(nameof(zoneClusterMemberConsumption));
            if (baseZoneClusterConsumption == null) throw new ArgumentNullException(nameof(baseZoneClusterConsumption));

            ZoneClusterMemberConsumption = zoneClusterMemberConsumption;
            BaseZoneClusterConsumption = baseZoneClusterConsumption;
        }

        public ZoneClusterMemberConsumption ZoneClusterMemberConsumption { get; }
        public T BaseZoneClusterConsumption { get; }
    }
}