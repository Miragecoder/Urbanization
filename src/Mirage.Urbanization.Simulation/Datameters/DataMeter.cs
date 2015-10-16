using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Mirage.Urbanization.Simulation.Persistence;

namespace Mirage.Urbanization.Simulation.Datameters
{
    public class DataMeter
    {
        private readonly Func<PersistedCityStatisticsWithFinancialData, int> _getValue;

        private readonly IReadOnlyCollection<Threshold> _thresholds;

        private readonly Lazy<int> _measureUnitSumLazy;

        private static int AmountOfWebIds = 0;

        public int WebId { get; } = Interlocked.Add(ref AmountOfWebIds, 1);

        public string Name { get; }

        public bool RepresentsIssue { get; }

        public DataMeter(int measureUnit, string name, Func<PersistedCityStatisticsWithFinancialData, int> getValue, bool representsIssue)
        {
            Name = name;
            _getValue = getValue;
            RepresentsIssue = representsIssue;

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
            return new DataMeterResult(this, amount, GetPercentageScore(amount), GetScoreCategory(amount));
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
            public DataMeterValueCategory Category { get; }

            public int MeasureUnitThreshold { get; }

            public Threshold(int measureUnitTreshold, DataMeterValueCategory category)
            {
                MeasureUnitThreshold = measureUnitTreshold;
                Category = category;
            }
        }
    }
}