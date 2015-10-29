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

        public IReadOnlyCollection<IThreshold> Thresholds { get; }

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

            Thresholds = Enumerable.Range(0, 5)
                .Select(i => new Threshold(i * measureUnit, (i * measureUnit) + measureUnit, (DataMeterValueCategory)i))
                .ToList();

            if (Enum
                .GetValues(typeof(DataMeterValueCategory))
                .Cast<DataMeterValueCategory>()
                .Any(x => Thresholds.Count(y => y.Category.Equals(x)) != 1))
            {
                throw new InvalidOperationException();
            }

            _measureUnitSumLazy = new Lazy<int>(() => Thresholds.Max(x => x.MaxMeasureUnitThreshold));
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
            return Thresholds
                .Where(x => amount >= x.MinMeasureUnitThreshold)
                .OrderByDescending(x => x.MinMeasureUnitThreshold)
                .First()
                .Category;
        }

        private decimal GetPercentageScore(int amount)
        {
            return Math.Round(((decimal)amount / _measureUnitSumLazy.Value), 2);
        }

        public interface IThreshold
        {
            DataMeterValueCategory Category { get; }

            int MinMeasureUnitThreshold { get; }
            int MaxMeasureUnitThreshold { get; }
        }

        private class Threshold : IThreshold
        {
            public DataMeterValueCategory Category { get; }

            public int MinMeasureUnitThreshold { get; }
            public int MaxMeasureUnitThreshold { get; }

            public Threshold(int minMeasureUnitTreshold, int maxMeasureUnitThreshold, DataMeterValueCategory category)
            {
                MinMeasureUnitThreshold = minMeasureUnitTreshold;
                MaxMeasureUnitThreshold = maxMeasureUnitThreshold;
                Category = category;
            }
        }
    }
}