using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirage.Urbanization
{
    public class SimpleCache<T>
        where T : class
    {
        private readonly Func<T> _acquisitionFunc;
        private readonly TimeSpan _cacheDuration;

        public SimpleCache(Func<T> acquisitionFunc, TimeSpan cacheDuration)
        {
            _acquisitionFunc = acquisitionFunc;
            _cacheDuration = cacheDuration;
        }

        private T _lastRetrievedValue;
        private DateTime _lastRetrievedDateTime = DateTime.MinValue;

        private void Acquire()
        {
            _lastRetrievedDateTime = DateTime.Now;
            _lastRetrievedValue = _acquisitionFunc();
        }

        private readonly object _locker = new object();

        public T GetValue()
        {
            lock (_locker)
            {
                if (_lastRetrievedValue == null)
                {
                    Acquire();
                }

                if (_lastRetrievedDateTime.Add(_cacheDuration) < DateTime.Now)
                {
                    Task.Run(() => Acquire());
                }
                return _lastRetrievedValue;
            }
        }
    }
}
