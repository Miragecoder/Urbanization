using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneConsumption.Base.Behaviours;

namespace Mirage.Urbanization.ZoneConsumption
{
    public interface ISingleZoneConsumptionWithPollutionBehaviour : IAreaZoneConsumption
    {
        IPollutionBehaviour PollutionBehaviour { get; }
    }
}