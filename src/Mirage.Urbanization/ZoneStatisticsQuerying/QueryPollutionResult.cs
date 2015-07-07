namespace Mirage.Urbanization.ZoneStatisticsQuerying
{
    internal class QueryPollutionResult : QueryCellValueResult, IQueryPollutionResult
    {
        public QueryPollutionResult(int valueInUnits) : base(valueInUnits) { }
    }
}