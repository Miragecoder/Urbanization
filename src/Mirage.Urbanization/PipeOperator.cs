using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirage.Urbanization
{
    public static class PipeOperator
    {
        public static void Pipe<T>(this T input, Action<T> action) => action(input);
        public static TR Pipe<T, TR>(this T input, Func<T, TR> action) => action(input);
    }
}
