using System;

namespace Mirage.Urbanization
{
    public class AreaConsumptionResultEventArgs : EventArgs
    {
        public IAreaConsumptionResult AreaConsumptionResult { get; }

        public AreaConsumptionResultEventArgs(IAreaConsumptionResult areaConsumptionResult)
        {
            if (areaConsumptionResult == null) throw new ArgumentNullException(nameof(areaConsumptionResult));
            AreaConsumptionResult = areaConsumptionResult;
        }
    }
}