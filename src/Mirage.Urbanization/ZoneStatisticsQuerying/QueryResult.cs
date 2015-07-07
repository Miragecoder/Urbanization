using System;

namespace Mirage.Urbanization.ZoneStatisticsQuerying
{
    public class QueryResult<T>
        where T : class
    {
        private readonly T _matchingObject;
        public T MatchingObject { get { return _matchingObject; } }
        public bool HasMatch { get { return _matchingObject != null; } }
        public bool HasNoMatch { get { return !HasMatch; } }
        public QueryResult(T @object = null) { _matchingObject = @object; }

        public void WithResultIfHasMatch(Action<T> action)
        {
            if (HasMatch) action(MatchingObject);
        }

        public R WithResultIfHasMatch<R>(Func<T, R> actionIfHasMatch, R @default = default(R))
        {
            return HasMatch ? actionIfHasMatch(_matchingObject) : @default;
        }

        public static readonly QueryResult<T> Empty = new QueryResult<T>();
    }

    public class QueryResult<TMatchingObject, TQuery> : QueryResult<TMatchingObject>
        where TMatchingObject : class
        where TQuery : class
    {
        private readonly TQuery _queryObject;

        public TQuery QueryObject { get { return _queryObject; } }

        public QueryResult(TQuery queryObject, TMatchingObject matchingObject = null)
            : base(matchingObject)
        {
            if (queryObject == null) throw new ArgumentNullException("queryObject");
            _queryObject = queryObject;
        }
    }
}
