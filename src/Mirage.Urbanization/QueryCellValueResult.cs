using System.Drawing.Text;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Collections;

namespace Mirage.Urbanization
{
    public abstract class QueryCellValueResult : IQueryCellValueResult
    {
        private readonly int _valueInUnits;
        protected QueryCellValueResult(int valueInUnits)
        {
            _valueInUnits = valueInUnits > 0 ? valueInUnits : 0;
        }
        public int ValueInUnits { get { return _valueInUnits; } }
    }
}