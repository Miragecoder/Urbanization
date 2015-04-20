using System;

namespace Mirage.Urbanization
{
    public class ProcessOptions
    {
        private readonly Func<bool> _getStepByStepGrowthCyclingToggledFunc; 

        public ProcessOptions(Func<bool> getStepByStepGrowthCyclingToggledFunc)
        {
            if (getStepByStepGrowthCyclingToggledFunc == null) throw new ArgumentNullException("getStepByStepGrowthCyclingToggledFunc");
            _getStepByStepGrowthCyclingToggledFunc = getStepByStepGrowthCyclingToggledFunc;
        }

        public bool GetStepByStepGrowthCyclingToggled()
        {
            return _getStepByStepGrowthCyclingToggledFunc();
        }
    }
}