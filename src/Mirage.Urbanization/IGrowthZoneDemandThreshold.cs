using System;
using System.Text;
using System.Threading.Tasks;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization
{
    internal interface IGrowthZoneDemandThreshold
    {
        bool DecrementAvailableConsumption(BaseZoneClusterConsumption baseZoneClusterConsumption);
        bool AvailableConsumptionsExceeded { get; }
        string OnExceededMessage { get; }
    }
}
