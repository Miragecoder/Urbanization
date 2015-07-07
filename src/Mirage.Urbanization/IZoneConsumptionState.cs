using System;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization
{
    public interface IZoneConsumptionState : IReadOnlyZoneConsumptionState
    {
        bool GetIsPowerGridMember();
        bool GetIsRoadNetworkMember();
        bool GetIsZoneClusterMember();
        bool GetIsNetworkMember<TBaseNetworkZoneConsumption>()
            where TBaseNetworkZoneConsumption : BaseInfrastructureNetworkZoneConsumption;

        bool GetIsWater();
        QueryResult<ZoneClusterMemberConsumption> QueryAsZoneClusterMember();

        void WithNetworkMember<TBaseNetworkZoneConsumption>(Action<TBaseNetworkZoneConsumption> action)
            where TBaseNetworkZoneConsumption : BaseInfrastructureNetworkZoneConsumption;

        IConsumeAreaOperation TryConsumeWith(IAreaZoneConsumption consumption);
    }
}