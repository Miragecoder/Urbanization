using System.Collections;
using System.Collections.Generic;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization
{
    internal class AreaConsumptionResult : IAreaConsumptionResult
    {
        public bool Success { get; }
        public string Message { get; }
        public IAreaConsumption AreaConsumption { get; }

        public AreaConsumptionResult(IAreaConsumption areaConsumption, bool success, string message)
        {
            AreaConsumption = areaConsumption;
            Message = message;
            Success = success;
        }
    }
}