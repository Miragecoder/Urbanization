using System;
using System.Collections.Generic;

namespace Mirage.Urbanization.ZoneConsumption.Base
{
    public interface IIntersectingZoneConsumption : IAreaZoneConsumption
    {
        IEnumerable<Type> GetIntersectingTypes();

        BaseInfrastructureNetworkZoneConsumption EastWestZoneConsumption { get; }
        BaseInfrastructureNetworkZoneConsumption NorthSouthZoneConsumption { get; }
    }
}