namespace Mirage.Urbanization
{
    internal class QueryPollutionResult : QueryCellValueResult, IQueryPollutionResult
    {
        public QueryPollutionResult(int valueInUnits) : base(valueInUnits) { }
    }
}