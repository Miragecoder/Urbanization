using System;

namespace Mirage.Urbanization.ZoneStatisticsQuerying
{
    public class QueryResult<T>
        where T : class
    {
        public T MatchingObject { get; }
        public bool HasMatch => MatchingObject != null;
        public bool HasNoMatch => !HasMatch;
        protected QueryResult(T @object = null) { MatchingObject = @object; }

        public static QueryResult<T> Create(T @object = null)
        {
            return @object != null
                ? new QueryResult<T>(@object)
                : Empty;
        }  

        public void WithResultIfHasMatch(Action<T> action)
        {
            if (HasMatch) action(MatchingObject);
        }

        public R WithResultIfHasMatch<R>(Func<T, R> actionIfHasMatch, R @default = default(R))
        {
            return HasMatch ? actionIfHasMatch(MatchingObject) : @default;
        }

        public static readonly QueryResult<T> Empty = new QueryResult<T>();
    }

    public class QueryResult<TMatchingObject, TQuery> : QueryResult<TMatchingObject> where TMatchingObject : class
        where TQuery : class
    {
        public TQuery QueryObject { get; }

        public QueryResult(TQuery queryObject, TMatchingObject matchingObject = null)
            : base(matchingObject)
        {
            if (queryObject == null) throw new ArgumentNullException(nameof(queryObject));
            QueryObject = queryObject;
        }
    }
}
