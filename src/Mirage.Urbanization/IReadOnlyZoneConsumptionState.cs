using System;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization
{
    public interface IReadOnlyZoneConsumptionState
    {
        IAreaZoneConsumption GetZoneConsumption();
        DateTime LastUpdateDateTime { get; }
        bool GetIsRailroadNetworkMember();
    }
}