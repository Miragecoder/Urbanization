using System;

namespace Mirage.Urbanization.ZoneConsumption.Base.Behaviours
{
    internal abstract class BaseDynamicBehaviour : IBehaviour
    {
        private readonly Func<int> _getAmountInUnits;

        protected BaseDynamicBehaviour(Func<int> getAmountInUnits)
        {
            _getAmountInUnits = getAmountInUnits ?? throw new ArgumentNullException(nameof(getAmountInUnits));
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