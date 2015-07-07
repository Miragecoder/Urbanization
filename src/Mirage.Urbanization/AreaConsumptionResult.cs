using System.Collections;
using System.Collections.Generic;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization
{
    internal class AreaConsumptionResult : IAreaConsumptionResult
    {
        private readonly IAreaConsumption _areaConsumption;
        private readonly string _message;
        private readonly bool _success;

        public bool Success { get { return _success; } }
        public string Message { get { return _message; } }
        public IAreaConsumption AreaConsumption { get { return _areaConsumption; } }

        public AreaConsumptionResult(IAreaConsumption areaConsumption, bool success, string message)
        {
            _areaConsumption = areaConsumption;
            _message = message;
            _success = success;
        }
    }
}