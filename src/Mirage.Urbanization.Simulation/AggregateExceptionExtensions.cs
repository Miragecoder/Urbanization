using System;
using System.Linq;
using System.Threading.Tasks;

namespace Mirage.Urbanization.Simulation
{
    public static class AggregateExceptionExtensions
    {
        public static bool IsCancelled(this AggregateException aggregateException)
        {
            if (aggregateException.InnerExceptions.All(x => x.GetType() == typeof(TaskCanceledException)) ||
                aggregateException.InnerExceptions.All(x =>
                {
                    var t = x as AggregateException;
                    return t != null &&
                           t.InnerExceptions.All(y => y.GetType() == typeof(TaskCanceledException));
                }))
            {
                return true;
            }
            return false;
        }
    }
}