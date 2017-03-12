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
            _getStepByStepGrowthCyclingToggledFunc = getStepByStepGrowthCyclingToggledFunc ?? throw new ArgumentNullException(nameof(getStepByStepGrowthCyclingToggledFunc));
            _getIsMoneyCheatEnabledFunc = getIsMoneyCheatEnabledFunc ?? throw new ArgumentNullException(nameof(getIsMoneyCheatEnabledFunc));
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