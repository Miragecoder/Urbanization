using System;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using Mirage.Urbanization.Simulation.Persistence;

namespace Mirage.Urbanization.Charts
{
    public class GraphSeries
    {
        private readonly Func<PersistedCityStatisticsWithFinancialData, int> _valueGetterFunc;

        public GraphSeries(Func<PersistedCityStatisticsWithFinancialData, int> valueGetterFunc, string label, Color color)
        {
            _valueGetterFunc = valueGetterFunc;
            Label = label;
            Color = color;
        }

        public int GetValue(PersistedCityStatisticsWithFinancialData citytStatistics) => _valueGetterFunc(citytStatistics);

        public string Label { get; }

        public Color Color { get; }
    }
}
