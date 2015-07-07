namespace Mirage.Urbanization.ZoneStatisticsQuerying
{
    public class QueryLandValueResult : QueryCellValueResult, IQueryLandValueResult
    {
        public QueryLandValueResult(int valueInUnits) : base(valueInUnits) { }
    }
}