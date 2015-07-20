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

            FireHazardNumbers = new NumberSummary(queryFireHazardResults.Select(x => x.ValueInUnits));
            CrimeNumbers = new NumberSummary(queryCrimeResults.Select(x => x.ValueInUnits));
            LandValueNumbers = new NumberSummary(queryLandValueResults.Select(x => x.ValueInUnits));
            PollutionNumbers = new NumberSummary(queryPollutionResults.Select(x => x.ValueInUnits));
            TravelDistanceNumbers = new NumberSummary(queryTravelDistanceResults);
        }

        public INumberSummary CrimeNumbers { get; }

        public INumberSummary LandValueNumbers { get; }

        public INumberSummary PollutionNumbers { get; }

        public INumberSummary TravelDistanceNumbers { get; }

        public INumberSummary FireHazardNumbers { get; }
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
        public NumberSummary(IEnumerable<int> numbers)
        {
            var capturedNumbers = numbers.ToList();
            capturedNumbers = capturedNumbers.Any() ? capturedNumbers : new List<int>() { 0 };

            Lowest = capturedNumbers.Min();
            Average = Convert.ToInt32(capturedNumbers.Average());
            Highest = capturedNumbers.Max();
            Sum = capturedNumbers.Sum();
            Count = capturedNumbers.Count();
        }

        public int Lowest { get; }

        public int Average { get; }

        public int Highest { get; }

        public int Sum { get; }

        public int Count { get; }
    }
}
