using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using Mirage.Urbanization.Simulation.Persistence;
using ZedGraph;
using Image = System.Drawing.Image;

namespace Mirage.Urbanization.Charts
{
    internal class ZedGraphChartDrawer : BaseChartDrawer
    {
        public override Image Draw(
            GraphDefinition graphDefinition,
            IEnumerable<PersistedCityStatisticsWithFinancialData> statistics,
            Font font,
            Size size)
        {
            using (var chartMemoryStream = new MemoryStream())
            {
                var zg = new ZedGraphControl
                {
                    Size = size,
                    Font = font
                };

                var chart = zg.GraphPane;

                foreach (var axis in new[] { chart.XAxis, chart.YAxis as Axis })
                {
                    axis.Scale.IsUseTenPower = false;
                    axis.Scale.Format = "F0";
                }

                chart.XAxis.Title.Text = "Time";
                chart.XAxis.Type = AxisType.LinearAsOrdinal;
                chart.YAxis.Type = AxisType.Linear;
                chart.YAxis.Title.Text = graphDefinition.Title;

                foreach (var z in graphDefinition.GraphSeriesSet)
                {
                    var pointPairList = new PointPairList();

                    foreach (var statistic in statistics)
                    {
                        pointPairList.Add(statistic.PersistedCityStatistics.TimeCode, z.GetValue(statistic));
                    }

                    chart.AddCurve(z.Label, pointPairList, z.Color, SymbolType.None);
                }

                chart.Title.Text = graphDefinition.Title;

                zg.AxisChange();

                return zg.GetImage();
            }
        }
    }
}