using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using Mirage.Urbanization.Simulation.Persistence;

namespace Mirage.Urbanization.Charts
{
    internal abstract class BaseChartDrawer:  IChartDrawer
    {
        protected DataTable ConvertIntoDataTable(
            GraphDefinition graphDefinition,
            IEnumerable<PersistedCityStatisticsWithFinancialData> statistics)
        {
            var dataTable = new DataTable();

            dataTable.Columns.Add("Type", typeof(string));
            dataTable.Columns.Add(graphDefinition.Title, typeof(int));
            dataTable.Columns.Add("TimeCode", typeof(string));

            foreach (var statistic in statistics)
            {
                foreach (var col in graphDefinition.GraphSeriesSet)
                {
                    dataTable.Rows.Add(col.Label, col.GetValue(statistic),
                        statistic.PersistedCityStatistics.TimeCode.ToString(CultureInfo.InvariantCulture));
                }
            }
            return dataTable;
        }

        public abstract Image Draw(
            GraphDefinition graphDefinition,
            IEnumerable<PersistedCityStatisticsWithFinancialData> statistics, 
            Font font, 
            Size size);
    }
}