using System;

namespace Mirage.Urbanization.ZoneConsumption.Base.Behaviours
{
    internal class DynamicCrimeBehaviour : BaseDynamicBehaviour, ICrimeBehaviour
    {
        public DynamicCrimeBehaviour(Func<int> getCrimeInUnits)
            : base(getCrimeInUnits)
        {

        }

        public int GetCrimeInUnits(RelativeZoneInfoQuery relativeZoneInfoQuery)
        {
            return GetAmountInUnits(relativeZoneInfoQuery);
        }
    }
}