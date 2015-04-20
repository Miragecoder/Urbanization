using System;

namespace Mirage.Urbanization.ZoneConsumption.Base.Behaviours
{
    internal abstract class BaseDynamicBehaviour : IBehaviour
    {
        private readonly Func<int> _getAmountInUnits;

        protected BaseDynamicBehaviour(Func<int> getAmountInUnits)
        {
            if (getAmountInUnits == null) throw new ArgumentNullException("getAmountInUnits");
            _getAmountInUnits = getAmountInUnits;
        }

        public int GetAmountInUnits(RelativeZoneInfoQuery relativeZoneInfoQuery)
        {
            int pollution = _getAmountInUnits();
            if (relativeZoneInfoQuery.Distance > 0 && pollution != 0)
            {
                return pollution / relativeZoneInfoQuery.Distance;
            }
            return pollution;
        }
    }
}