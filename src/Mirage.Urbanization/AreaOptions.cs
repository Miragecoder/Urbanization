using System;
using Mirage.Urbanization.Persistence;

namespace Mirage.Urbanization
{
    public class AreaOptions
    {
        private readonly TerraformingOptions _terraformingOptions;

        private readonly PersistedArea _persistedArea;

        private readonly Func<ICityServiceStrengthLevels> _getCityServiceStrengthLevels;
        public ICityServiceStrengthLevels GetCityServiceStrengthLevels()
        {
            return _getCityServiceStrengthLevels();
        }

        public AreaOptions(ILandValueCalculator landValueCalculator, 
            TerraformingOptions terraformingOptions, 
            ProcessOptions processOptions,
            Func<ICityServiceStrengthLevels> getCityServiceStrengthLevels)
            : this(landValueCalculator, processOptions, getCityServiceStrengthLevels)
        {
            if (terraformingOptions == null) throw new ArgumentNullException(nameof(terraformingOptions));
            _terraformingOptions = terraformingOptions;
        }

        private AreaOptions(
            ILandValueCalculator landValueCalculator, 
            ProcessOptions processOptions,
            Func<ICityServiceStrengthLevels> getCityServiceStrengthLevels)
        {
            if (landValueCalculator == null) throw new ArgumentNullException(nameof(landValueCalculator));
            if (processOptions == null) throw new ArgumentNullException(nameof(processOptions));
            if (getCityServiceStrengthLevels == null) throw new ArgumentNullException(nameof(getCityServiceStrengthLevels));
            ProcessOptions = processOptions;
            LandValueCalculator = landValueCalculator;
            _getCityServiceStrengthLevels = getCityServiceStrengthLevels;
        }

        public AreaOptions(ILandValueCalculator landValueCalculator, 
            PersistedArea persistedArea, 
            ProcessOptions processOptions,
            Func<ICityServiceStrengthLevels> getCityServiceStrengthLevels)
            : this(landValueCalculator, processOptions, getCityServiceStrengthLevels)
        {
            if (persistedArea == null) throw new ArgumentNullException(nameof(persistedArea));
            _persistedArea = persistedArea;
        }

        public void WithTerraformingOptions(Action<TerraformingOptions> action)
        {
            if (_terraformingOptions != null) action(_terraformingOptions);
        }

        public void WithPersistedArea(Action<PersistedArea> persistedAreaAction)
        {
            if (_persistedArea != null) persistedAreaAction(_persistedArea);
        }

        public ProcessOptions ProcessOptions { get; }
        public ILandValueCalculator LandValueCalculator { get; }

        public int GetZoneWidthAndHeight()
        {
            return _terraformingOptions?.ZoneWidthAndHeight ?? _persistedArea.GetZoneWidthAndHeight();
        }
    }
}