using System;
using Mirage.Urbanization.Persistence;

namespace Mirage.Urbanization
{
    public class AreaOptions
    {
        private readonly TerraformingOptions _terraformingOptions;

        private readonly PersistedArea _persistedArea;

        private readonly Func<ICityServiceStrengthLevels> _getCityServiceStrengthLevels;

        private readonly Func<ILandValueCalculator> _getLandValueCalculator;
        public ICityServiceStrengthLevels GetCityServiceStrengthLevels()
        {
            return _getCityServiceStrengthLevels();
        }

        public AreaOptions(Func<ILandValueCalculator> getLandValueCalculator, 
            TerraformingOptions terraformingOptions, 
            ProcessOptions processOptions,
            Func<ICityServiceStrengthLevels> getCityServiceStrengthLevels)
            : this(getLandValueCalculator, processOptions, getCityServiceStrengthLevels)
        {
            if (terraformingOptions == null) throw new ArgumentNullException(nameof(terraformingOptions));
            _terraformingOptions = terraformingOptions;
        }

        private AreaOptions(
            Func<ILandValueCalculator> getLandValueCalculator, 
            ProcessOptions processOptions,
            Func<ICityServiceStrengthLevels> getCityServiceStrengthLevels)
        {
            if (getLandValueCalculator == null) throw new ArgumentNullException(nameof(getLandValueCalculator));
            if (processOptions == null) throw new ArgumentNullException(nameof(processOptions));
            if (getCityServiceStrengthLevels == null) throw new ArgumentNullException(nameof(getCityServiceStrengthLevels));
            ProcessOptions = processOptions;
            _getLandValueCalculator = getLandValueCalculator;
            _getCityServiceStrengthLevels = getCityServiceStrengthLevels;
        }

        public AreaOptions(Func<ILandValueCalculator> getLandValueCalculator, 
            PersistedArea persistedArea, 
            ProcessOptions processOptions,
            Func<ICityServiceStrengthLevels> getCityServiceStrengthLevels)
            : this(getLandValueCalculator, processOptions, getCityServiceStrengthLevels)
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
        public ILandValueCalculator GetLandValueCalculator() => _getLandValueCalculator();

        public int GetZoneWidthAndHeight()
        {
            return _terraformingOptions?.ZoneWidthAndHeight ?? _persistedArea.GetZoneWidthAndHeight();
        }
    }
}