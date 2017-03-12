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
            _terraformingOptions = terraformingOptions ?? throw new ArgumentNullException(nameof(terraformingOptions));
        }

        private AreaOptions(
            Func<ILandValueCalculator> getLandValueCalculator, 
            ProcessOptions processOptions,
            Func<ICityServiceStrengthLevels> getCityServiceStrengthLevels)
        {
            ProcessOptions = processOptions ?? throw new ArgumentNullException(nameof(processOptions));
            _getLandValueCalculator = getLandValueCalculator ?? throw new ArgumentNullException(nameof(getLandValueCalculator));
            _getCityServiceStrengthLevels = getCityServiceStrengthLevels ?? throw new ArgumentNullException(nameof(getCityServiceStrengthLevels));
        }

        public AreaOptions(Func<ILandValueCalculator> getLandValueCalculator, 
            PersistedArea persistedArea, 
            ProcessOptions processOptions,
            Func<ICityServiceStrengthLevels> getCityServiceStrengthLevels)
            : this(getLandValueCalculator, processOptions, getCityServiceStrengthLevels)
        {
            _persistedArea = persistedArea ?? throw new ArgumentNullException(nameof(persistedArea));
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