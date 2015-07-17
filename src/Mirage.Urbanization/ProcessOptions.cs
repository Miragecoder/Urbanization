using System;

namespace Mirage.Urbanization
{
    public class ProcessOptions
    {
        private readonly Func<bool> _getStepByStepGrowthCyclingToggledFunc;
        private readonly Func<bool> _getIsMoneyCheatEnabledFunc;

        public ProcessOptions(
            Func<bool> getStepByStepGrowthCyclingToggledFunc,
            Func<bool> getIsMoneyCheatEnabledFunc)
        {
            if (getStepByStepGrowthCyclingToggledFunc == null) throw new ArgumentNullException("getStepByStepGrowthCyclingToggledFunc");
            if (getIsMoneyCheatEnabledFunc == null) throw new ArgumentNullException("getIsMoneyCheatEnabledFunc");
            _getStepByStepGrowthCyclingToggledFunc = getStepByStepGrowthCyclingToggledFunc;
            _getIsMoneyCheatEnabledFunc = getIsMoneyCheatEnabledFunc;
        }

        public bool GetStepByStepGrowthCyclingToggled()
        {
            return _getStepByStepGrowthCyclingToggledFunc();
        }

        public bool GetIsMoneyCheatEnabled()
        {
            return _getIsMoneyCheatEnabledFunc();
        }
    }
}