using System;
using System.Collections.Concurrent;

namespace Mirage.Urbanization.WinForms.Rendering.SharpDx
{
    public class ConverterAndCacher<TIn, TOut>
    {
        private readonly ConcurrentDictionary<TIn, TOut> _cache = new ConcurrentDictionary<TIn, TOut>();

        private readonly Func<TIn, TOut> _converter;

        public ConverterAndCacher(Func<TIn, TOut> converter)
        {
            _converter = converter;
        }

        public TOut Convert(TIn input)
        {
            if (_cache.TryGetValue(input, out TOut result)) return result;
            result = _converter(input);
            _cache.TryAdd(input, _converter(input));
            return result;
        }
    }
}