using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirage.Urbanization.Statistics
{
    public interface IMiscCityStatistics
    {
        INumberSummary CrimeNumbers { get; }
        INumberSummary LandValueNumbers { get; }
        INumberSummary PollutionNumbers { get; }
    }

    public class MiscCityStatistics : IMiscCityStatistics
    {
        private readonly INumberSummary _crimeNumbers;
        private readonly INumberSummary _landValueNumbers;
        private readonly INumberSummary _pollutionNumbers;

        public MiscCityStatistics(
            IEnumerable<IQueryCrimeResult> queryCrimeResults,
            IEnumerable<IQueryLandValueResult> queryLandValueResults,
            IEnumerable<IQueryPollutionResult> queryPollutionResults
        )
        {
            if (queryCrimeResults == null) throw new ArgumentNullException("queryCrimeResults");
            if (queryLandValueResults == null) throw new ArgumentNullException("queryLandValueResults");
            if (queryPollutionResults == null) throw new ArgumentNullException("queryPollutionResults");

            _crimeNumbers = new NumberSummary(queryCrimeResults.Select(x => x.CrimeInUnits));
            _landValueNumbers = new NumberSummary(queryLandValueResults.Select(x => x.LandValueInUnits));
            _pollutionNumbers = new NumberSummary(queryPollutionResults.Select(x => x.PollutionInUnits));
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
