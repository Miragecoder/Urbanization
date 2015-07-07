namespace Mirage.Urbanization
{
    public class QueryLandValueResult : QueryCellValueResult, IQueryLandValueResult
    {
        public QueryLandValueResult(int valueInUnits) : base(valueInUnits) { }
    }
}