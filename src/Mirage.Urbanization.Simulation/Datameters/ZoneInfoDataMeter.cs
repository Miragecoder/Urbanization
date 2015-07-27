using System;
using System.Linq;
using Mirage.Urbanization.Simulation.Persistence;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization.Simulation.Datameters
{
    public class ZoneInfoDataMeter : DataMeter
    {
        private readonly Func<IReadOnlyZoneInfo, int> _getValueCategoryFunc;
        private readonly Type[] _undesireableForTypes;

        public ZoneInfoDataMeter(
            int measureUnit,
            string name,
            Func<PersistedCityStatisticsWithFinancialData, int> getValue,
            Func<IReadOnlyZoneInfo, int> getValueCategoryFunc,
            bool representsIssue,
            Type[] undesireableForTypes
            )
            : base(measureUnit, name, getValue, representsIssue)
        {
            _getValueCategoryFunc = getValueCategoryFunc;
            _undesireableForTypes = undesireableForTypes;
        }

        public DataMeterResult GetDataMeterResult(IReadOnlyZoneInfo zoneInfo)
        {
            return GetDataMeterResult(_getValueCategoryFunc(zoneInfo));
        }

        public bool RepresentsUndesirabilityFor(BaseGrowthZoneClusterConsumption baseGrowthZoneClusterConsumption)
        {
            return _undesireableForTypes.Contains(baseGrowthZoneClusterConsumption.GetType());
        }
    }
}