namespace Mirage.Urbanization.ZoneStatisticsQuerying
{
    public abstract class QueryCellValueResult : IQueryCellValueResult
    {
        protected QueryCellValueResult(int valueInUnits)
        {
            ValueInUnits = valueInUnits > 0 ? valueInUnits : 0;
        }
        public int ValueInUnits { get; }
    }
}