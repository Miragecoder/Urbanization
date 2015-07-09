using Mirage.Urbanization.Statistics;

namespace Mirage.Urbanization.Simulation.Persistence
{
    public class PersistedNumberSummary
    {
        public static readonly PersistedNumberSummary EmptyInstance = new PersistedNumberSummary();

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