using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;
using Mirage.Urbanization.Simulation.Persistence;
using Image = System.Drawing.Image;

namespace Mirage.Urbanization.Charts
{
    internal class DataVisualizationChartDrawer : BaseChartDrawer
    {
        public override Image Draw(
            GraphDefinition graphDefinition,
            IReadOnlyCollection<PersistedCityStatisticsWithFinancialData> statistics,
            Font font,
            Size size)
        {
            using (var chartMemoryStream = new MemoryStream())
            {
                var chart = new Chart
                {
                    Width = new Unit(size.Width),
                    Height = new Unit(size.Height),
                    Palette = ChartColorPalette.Berry,
                    BorderColor = Color.DodgerBlue
                };

                chart.ChartAreas.Add("Test");

                var dataTable = ConvertIntoDataTable(graphDefinition, statistics);

                chart.DataBindCrossTable(dataTable.Rows, "Type", "TimeCode", graphDefinition.Title, String.Empty);

                chart.Titles.Add(graphDefinition.Title);

                chart.Font.Name = font.Name;
                chart.Font.Size = new FontUnit(font.SizeInPoints);

                chart.ChartAreas[0].BackGradientStyle = GradientStyle.TopBottom;
                chart.ChartAreas[0].BackSecondaryColor = Color.LightSkyBlue;
                chart.ChartAreas[0].BackColor = Color.LightBlue;

                chart.Legends.Add("Legend");

                foreach (var series in chart.Series)
                {
                    series.ChartType = SeriesChartType.Line;
                    series.BorderWidth = series.BorderWidth * 4;

                    series.Color = graphDefinition.GraphSeriesSet.Single(x => x.Label == series.Name).Color;
                }

                chart.SaveImage(chartMemoryStream);

                return Bitmap.FromStream(chartMemoryStream);
            }
        }
    }
}
