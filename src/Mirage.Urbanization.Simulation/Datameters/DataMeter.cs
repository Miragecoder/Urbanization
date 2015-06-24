using System;
using System.Collections.Generic;
using System.Linq;
using Mirage.Urbanization.ZoneConsumption;

namespace Mirage.Urbanization.Simulation.Datameters
{
    public class ZoneInfoDataMeter : DataMeter
    {
        private readonly Func<IReadOnlyZoneInfo, int> _getValueCategoryFunc;
        public ZoneInfoDataMeter(
            int measureUnit,
            string name,
            Func<PersistedCityStatistics, int> getValue,
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

    public static class DataMeterInstances
    {
        private static readonly ZoneInfoDataMeter
            CrimeDataMeter = new ZoneInfoDataMeter(200,
                "Crime",
                x => x.CrimeNumbers.Average,
                x => x.GetLastQueryCrimeResult().WithResultIfHasMatch(y => y.CrimeInUnits), 
                true
                ),
            PollutionDataMeter = new ZoneInfoDataMeter(300,
                "Pollution",
                x => x.PollutionNumbers.Average,
                x => x.GetLastQueryPollutionResult().WithResultIfHasMatch(y => y.PollutionInUnits), 
                true
                ),
            TrafficDataMeter = new ZoneInfoDataMeter(
                350,
                "Traffic",
                x => x.TrafficNumbers.Average,
                x => new QueryResult<IZoneConsumptionWithTraffic>(x.ZoneConsumptionState.GetZoneConsumption() as IZoneConsumptionWithTraffic)
                    .WithResultIfHasMatch(y => y.GetTrafficDensityAsInt()), 
                true
                ),
            PopulationDataMeter = new ZoneInfoDataMeter(
                10, "Population",
                x => x.GlobalZonePopulationStatistics.Average,
                x => x.GetPopulationDensity(),
                false
            );

        public static readonly IReadOnlyCollection<ZoneInfoDataMeter> DataMeters = new[]
        {
            CrimeDataMeter,
            PollutionDataMeter,
            TrafficDataMeter,
            PopulationDataMeter
        };

        public static IEnumerable<DataMeterResult> GetDataMeterResults(PersistedCityStatistics statistics, Func<DataMeter, bool> predicate)
        {
            return DataMeters.Where(predicate).Select(meter => meter.GetDataMeterResult(statistics));
        }
    }

    public class DataMeter
    {

        private readonly string _name;
        private readonly Func<PersistedCityStatistics, int> _getValue;
        private readonly bool _representsIssue;

        private readonly IReadOnlyCollection<Threshold> _thresholds;

        private readonly Lazy<int> _measureUnitSumLazy;

        public string Name { get { return _name; } }
        public bool RepresentsIssue { get { return _representsIssue; } }

        public DataMeter(int measureUnit, string name, Func<PersistedCityStatistics, int> getValue, bool representsIssue)
        {
            _name = name;
            _getValue = getValue;
            _representsIssue = representsIssue;

            _thresholds = Enumerable.Range(0, 5)
                .Select(i => new Threshold(i * measureUnit, (DataMeterValueCategory)i))
                .ToList();

            if (Enum
                .GetValues(typeof(DataMeterValueCategory))
                .Cast<DataMeterValueCategory>()
                .Any(x => _thresholds.Count(y => y.Category.Equals(x)) != 1))
            {
                throw new InvalidOperationException();
            }

            _measureUnitSumLazy = new Lazy<int>(() => _thresholds.Max(x => x.MeasureUnitThreshold));
        }

        public DataMeterResult GetDataMeterResult(PersistedCityStatistics statistics)
        {
            return GetDataMeterResult(_getValue(statistics));
        }

        public DataMeterResult GetDataMeterResult(int amount)
        {
            return new DataMeterResult(_name, amount, GetPercentageScore(amount), GetScoreCategory(amount));
        }

        private DataMeterValueCategory GetScoreCategory(int amount)
        {
            return _thresholds
                .Where(x => amount >= x.MeasureUnitThreshold)
                .OrderByDescending(x => x.MeasureUnitThreshold)
                .First()
                .Category;
        }

        private decimal GetPercentageScore(int amount)
        {
            return Math.Round(((decimal)amount / _measureUnitSumLazy.Value), 2) * 100;
        }

        private class Threshold
        {
            private readonly int _measureUnitTreshold;
            private readonly DataMeterValueCategory _category;

            public DataMeterValueCategory Category { get { return _category; } }
            public int MeasureUnitThreshold { get { return _measureUnitTreshold; } }

            public Threshold(int measureUnitTreshold, DataMeterValueCategory category)
            {
                _measureUnitTreshold = measureUnitTreshold;
                _category = category;
            }
        }
    }
}