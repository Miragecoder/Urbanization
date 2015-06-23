using System;
using System.Collections.Generic;
using System.Linq;

namespace Mirage.Urbanization.Simulation.Datameters
{
    public class DataMeter
    {
        public static readonly DataMeter CrimeDataMeter = new DataMeter(200, "Crime", x => x.CrimeNumbers.Average);

        private static readonly IReadOnlyCollection<DataMeter> DataMeters = new[]
        {
            CrimeDataMeter,
            new DataMeter(300, "Pollution", x => x.PollutionNumbers.Average),
            new DataMeter(350, "Traffic", x => x.TrafficNumbers.Average)
        };

        public static IEnumerable<DataMeterResult> GetDataMeterResults(PersistedCityStatistics statistics)
        {
            return DataMeters.Select(meter => meter.GetDataMeterResult(statistics));
        }

        private readonly string _name;
        private readonly Func<PersistedCityStatistics, int> _getValue;

        private readonly IReadOnlyCollection<Threshold> _thresholds;

        private readonly Lazy<int> _measureUnitSumLazy;

        private DataMeter(int measureUnit, string name, Func<PersistedCityStatistics, int> getValue)
        {
            _name = name;
            _getValue = getValue;

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
            return new DataMeterResult(_name, GetPercentageScore(amount), GetScoreCategory(amount));
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