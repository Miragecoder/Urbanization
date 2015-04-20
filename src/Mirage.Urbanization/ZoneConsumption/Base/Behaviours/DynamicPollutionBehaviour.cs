using System;

namespace Mirage.Urbanization.ZoneConsumption.Base.Behaviours
{
    internal class DynamicPollutionBehaviour : BaseDynamicBehaviour, IPollutionBehaviour
    {
        public DynamicPollutionBehaviour(Func<int> getPollutionInUnits)
            : base(getPollutionInUnits)
        {

        }

        public int GetPollutionInUnits(RelativeZoneInfoQuery relativeZoneInfoQuery)
        {
            return GetAmountInUnits(relativeZoneInfoQuery);
        }
    }
}