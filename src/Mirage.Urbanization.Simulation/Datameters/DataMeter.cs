using System;
using System.Collections.Generic;
using System.Linq;
using Mirage.Urbanization.Simulation.Persistence;

namespace Mirage.Urbanization.Simulation.Datameters
{
    public class DataMeter
    {
        private readonly string _name;
        private readonly Func<PersistedCityStatisticsWithFinancialData, int> _getValue;
        private readonly bool _representsIssue;

        private readonly IReadOnlyCollection<Threshold> _thresholds;

        private readonly Lazy<int> _measureUnitSumLazy;

        public string Name => _name;
        public bool RepresentsIssue => _representsIssue;

        public DataMeter(int measureUnit, string name, Func<PersistedCityStatisticsWithFinancialData, int> getValue, bool representsIssue)
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

        public DataMeterResult GetDataMeterResult(PersistedCityStatisticsWithFinancialData statistics)
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
            return Math.Round(((decimal)amount / _measureUnitSumLazy.Value), 2);
        }

        private class Threshold
        {
            private readonly int _measureUnitTreshold;
            private readonly DataMeterValueCategory _category;

            public DataMeterValueCategory Category => _category;
            public int MeasureUnitThreshold => _measureUnitTreshold;

            public Threshold(int measureUnitTreshold, DataMeterValueCategory category)
            {
                _measureUnitTreshold = measureUnitTreshold;
                _category = category;
            }
        }
    }
}