using System;
using Mirage.Urbanization.Persistence;

namespace Mirage.Urbanization
{
    public class AreaOptions
    {
        private readonly TerraformingOptions _terraformingOptions;
        private readonly ProcessOptions _processOptions;
        private readonly ILandValueCalculator _landValueCalculator;

        private readonly PersistedArea _persistedArea;

        public AreaOptions(ILandValueCalculator landValueCalculator, TerraformingOptions terraformingOptions, ProcessOptions processOptions)
            : this(landValueCalculator, processOptions)
        {
            if (terraformingOptions == null) throw new ArgumentNullException("terraformingOptions");
            _terraformingOptions = terraformingOptions;
        }

        private AreaOptions(ILandValueCalculator landValueCalculator, ProcessOptions processOptions)
        {
            if (landValueCalculator == null) throw new ArgumentNullException("landValueCalculator");
            if (processOptions == null) throw new ArgumentNullException("processOptions");
            _processOptions = processOptions;
            _landValueCalculator = landValueCalculator;
        }

        public AreaOptions(ILandValueCalculator landValueCalculator, PersistedArea persistedArea, ProcessOptions processOptions)
            : this(landValueCalculator, processOptions)
        {
            if (persistedArea == null) throw new ArgumentNullException("persistedArea");
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

        public ProcessOptions ProcessOptions
        {
            get { return _processOptions; }
        }

        public ILandValueCalculator LandValueCalculator { get { return _landValueCalculator; } }

        public int GetZoneWidthAndHeight()
        {
            return _terraformingOptions != null
                ? _terraformingOptions.ZoneWidthAndHeight
                : _persistedArea.GetZoneWidthAndHeight();
        }
    }
}