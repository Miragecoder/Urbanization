using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Statistics
{
    public interface IMiscCityStatistics
    {
        INumberSummary CrimeNumbers { get; }
        INumberSummary FireHazardNumbers { get; }
        INumberSummary LandValueNumbers { get; }
        INumberSummary PollutionNumbers { get; }
        INumberSummary TravelDistanceNumbers { get; }
    }

    public class MiscCityStatistics : IMiscCityStatistics
    {
        private readonly INumberSummary _fireHazardNumbers;
        private readonly INumberSummary _crimeNumbers;
        private readonly INumberSummary _landValueNumbers;
        private readonly INumberSummary _pollutionNumbers;
        private readonly INumberSummary _travelDistanceNumbers;

        public MiscCityStatistics(
            IEnumerable<IQueryCrimeResult> queryCrimeResults,
            IEnumerable<IQueryFireHazardResult> queryFireHazardResults,
            IEnumerable<IQueryLandValueResult> queryLandValueResults,
            IEnumerable<IQueryPollutionResult> queryPollutionResults,
            IEnumerable<int> queryTravelDistanceResults 
        )
        {
            if (queryCrimeResults == null) throw new ArgumentNullException(nameof(queryCrimeResults));
            if (queryFireHazardResults == null) throw new ArgumentNullException(nameof(queryFireHazardResults));
            if (queryLandValueResults == null) throw new ArgumentNullException(nameof(queryLandValueResults));
            if (queryPollutionResults == null) throw new ArgumentNullException(nameof(queryPollutionResults));
            if (queryTravelDistanceResults == null) throw new ArgumentNullException(nameof(queryTravelDistanceResults));

            _fireHazardNumbers = new NumberSummary(queryFireHazardResults.Select(x => x.ValueInUnits));
            _crimeNumbers = new NumberSummary(queryCrimeResults.Select(x => x.ValueInUnits));
            _landValueNumbers = new NumberSummary(queryLandValueResults.Select(x => x.ValueInUnits));
            _pollutionNumbers = new NumberSummary(queryPollutionResults.Select(x => x.ValueInUnits));
            _travelDistanceNumbers = new NumberSummary(queryTravelDistanceResults);
        }

        public INumberSummary CrimeNumbers
        {
            get { return _crimeNumbers; }
        }

        public INumberSummary LandValueNumbers
        {
            get { return _landValueNumbers; }
        }

        public INumberSummary PollutionNumbers
        {
            get { return _pollutionNumbers; }
        }

        public INumberSummary TravelDistanceNumbers
        {
            get { return _travelDistanceNumbers; }
        }

        public INumberSummary FireHazardNumbers
        {
            get { return _fireHazardNumbers; }
        }
    }

    public interface INumberSummary
    {
        int Lowest { get; }
        int Average { get; }
        int Highest { get; }
        int Count { get; }
        int Sum { get; }
    }

    internal class NumberSummary : INumberSummary
    {
        private readonly int _lowest;
        private readonly int _average;
        private readonly int _highest;
        private readonly int _sum;
        private readonly int _count;

        public NumberSummary(IEnumerable<int> numbers)
        {
            var capturedNumbers = numbers.ToList();
            capturedNumbers = capturedNumbers.Any() ? capturedNumbers : new List<int>() { 0 };

            _lowest = capturedNumbers.Min();
            _average = Convert.ToInt32(capturedNumbers.Average());
            _highest = capturedNumbers.Max();
            _sum = capturedNumbers.Sum();
            _count = capturedNumbers.Count();
        }

        public int Lowest
        {
            get { return _lowest; }
        }

        public int Average
        {
            get { return _average; }
        }

        public int Highest
        {
            get { return _highest; }
        }

        public int Sum { get { return _sum; } }

        public int Count { get { return _count; } }
    }
}
