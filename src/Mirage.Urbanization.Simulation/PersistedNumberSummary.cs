using Mirage.Urbanization.Statistics;

namespace Mirage.Urbanization.Simulation
{
    public class PersistedNumberSummary
    {
        public PersistedNumberSummary()
        {

        }

        public PersistedNumberSummary(INumberSummary summary)
        {
            Count = summary.Count;
            Sum = summary.Sum;
            Max = summary.Highest;
            Min = summary.Lowest;
            Average = summary.Average;
        }

        public int Count { get; set; }
        public int Sum { get; set; }
        public int Max { get; set; }
        public int Min { get; set; }
        public int Average { get; set; }
    }
}