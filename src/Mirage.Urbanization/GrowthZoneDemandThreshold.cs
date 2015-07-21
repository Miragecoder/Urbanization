using System.Collections.Generic;
using System.Linq;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization
{
    internal class GrowthZoneDemandThreshold<TDemandingConsumption, TDemandedConsumption> : IGrowthZoneDemandThreshold
        where TDemandingConsumption : BaseZoneClusterConsumption
        where TDemandedConsumption : BaseZoneClusterConsumption
    {
        private int _availableConsumptions;

        public GrowthZoneDemandThreshold(IEnumerable<TDemandedConsumption> currentlyOffered, string onExceededMessage, int growthFactor)
        {
            OnExceededMessage = onExceededMessage;
            _availableConsumptions = growthFactor + (new HashSet<TDemandedConsumption>(currentlyOffered.Where(x => x.HasPower)).Count * growthFactor);
        }

        public bool DecrementAvailableConsumption(BaseZoneClusterConsumption baseZoneClusterConsumption)
        {
            if (baseZoneClusterConsumption is TDemandingConsumption)
            {
                _availableConsumptions--;
                return true;
            }
            return false;
        }

        public string OnExceededMessage { get; }

        public bool AvailableConsumptionsExceeded => _availableConsumptions <= 0;
    }
}