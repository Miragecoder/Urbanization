namespace Mirage.Urbanization.ZoneStatisticsQuerying
{
    public abstract class QueryCellValueResult : IQueryCellValueResult
    {
        private readonly int _valueInUnits;
        protected QueryCellValueResult(int valueInUnits)
        {
            _valueInUnits = valueInUnits > 0 ? valueInUnits : 0;
        }
        public int ValueInUnits { get { return _valueInUnits; } }
    }
}