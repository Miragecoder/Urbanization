using System.Collections.Generic;
using System.Drawing;
using Mirage.Urbanization.Simulation.Persistence;

namespace Mirage.Urbanization.Charts
{
    public interface IChartDrawer
    {
        Image Draw(
            GraphDefinition graphDefinition,
            IReadOnlyCollection<PersistedCityStatisticsWithFinancialData> statistics,
            Font font,
            Size size);
    }
}