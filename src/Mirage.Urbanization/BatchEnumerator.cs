using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirage.Urbanization
{
    public class BatchEnumerator<T>
    {
        private readonly IEnumerator<T> _enumerator;
        public BatchEnumerator(ICollection<T> enumerable)
        {
            _enumerator = enumerable.GetEnumerator();
        }

        public IEnumerable<T> GetBatch()
        {
            foreach (var iteration in Enumerable.Range(0, 27))
            {
                if (_enumerator.MoveNext())
                    yield return _enumerator.Current;
                else
                {
                    _enumerator.Reset();
                    _enumerator.MoveNext();
                    yield return _enumerator.Current;
                }
            }
        }
    }
}
