using System;
using Mirage.Urbanization.Simulation.Persistence;

namespace Mirage.Urbanization.Simulation.Datameters
{
    public class ZoneInfoDataMeter : DataMeter
    {
        private readonly Func<IReadOnlyZoneInfo, int> _getValueCategoryFunc;
        public ZoneInfoDataMeter(
            int measureUnit,
            string name,
            Func<PersistedCityStatisticsWithFinancialData, int> getValue,
            Func<IReadOnlyZoneInfo, int> getValueCategoryFunc,
            bool representsIssue
            )
            : base(measureUnit, name, getValue, representsIssue)
        {
            _getValueCategoryFunc = getValueCategoryFunc;
        }

        public DataMeterResult GetDataMeterResult(IReadOnlyZoneInfo zoneInfo)
        {
            return GetDataMeterResult(_getValueCategoryFunc(zoneInfo));
        }
    }
}