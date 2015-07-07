using System;

namespace Mirage.Urbanization
{
    public class AreaConsumptionResultEventArgs : EventArgs
    {
        private readonly IAreaConsumptionResult _areaConsumptionResult;
        public IAreaConsumptionResult AreaConsumptionResult { get { return _areaConsumptionResult; } }
        public AreaConsumptionResultEventArgs(IAreaConsumptionResult areaConsumptionResult)
        {
            if (areaConsumptionResult == null) throw new ArgumentNullException("areaConsumptionResult");
            _areaConsumptionResult = areaConsumptionResult;
        }
    }
}