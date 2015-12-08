using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Mirage.Urbanization
{
    public static class EnumerableExtensions
    {
        private static IEnumerable<T> ToInfinity<T>(this IEnumerable<T> enumerable)
        {
            var array = enumerable.ToArray();
            while (true)
            {
                foreach (var x in array)
                    yield return x;
            }
        }

        public static IEnumerator<T> GetInifiniteEnumerator<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.ToInfinity().GetEnumerator();
        } 

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enumerable) => new HashSet<T>(enumerable);

        public static IEnumerable<IEnumerable<T>> GetBatched<T>(this IEnumerable<T> enumerable, int batchSize)
        {
            var enumerator = enumerable.GetEnumerator();
            while (true)
            {
                var currentBatch = new List<T>();
                foreach (var iteration in Enumerable.Range(0, 50))
                {
                    if (enumerator.MoveNext())
                        currentBatch.Add(enumerator.Current);
                    else
                    {
                        yield return currentBatch;
                        yield break;
                    }
                }
                yield return currentBatch;
            }
        } 
    }
}