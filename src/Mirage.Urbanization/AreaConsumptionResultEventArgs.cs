using System;

namespace Mirage.Urbanization
{
    public class AreaConsumptionResultEventArgs : EventArgs
    {
        public IAreaConsumptionResult AreaConsumptionResult { get; }

        public AreaConsumptionResultEventArgs(IAreaConsumptionResult areaConsumptionResult)
        {
            AreaConsumptionResult = areaConsumptionResult ?? throw new ArgumentNullException(nameof(areaConsumptionResult));
        }
    }
}