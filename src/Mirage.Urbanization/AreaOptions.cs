using System;
using Mirage.Urbanization.Persistence;

namespace Mirage.Urbanization
{
    public class AreaOptions
    {
        private readonly TerraformingOptions _terraformingOptions;
        private readonly ProcessOptions _processOptions;

        private readonly PersistedArea _persistedArea;

        public AreaOptions(TerraformingOptions terraformingOptions, ProcessOptions processOptions)
            : this(processOptions)
        {
            if (terraformingOptions == null) throw new ArgumentNullException("terraformingOptions");
            _terraformingOptions = terraformingOptions;
        }

        private AreaOptions(ProcessOptions processOptions)
        {
            if (processOptions == null) throw new ArgumentNullException("processOptions");
            _processOptions = processOptions;
        }

        public AreaOptions(PersistedArea persistedArea, ProcessOptions processOptions)
            : this(processOptions)
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

        public int GetZoneWidthAndHeight()
        {
            return _terraformingOptions != null
                ? _terraformingOptions.ZoneWidthAndHeight
                : _persistedArea.GetZoneWidthAndHeight();
        }
    }
}