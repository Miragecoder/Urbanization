using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using Mirage.Urbanization.Statistics;
using Mirage.Urbanization.Simulation;
using Mirage.Urbanization.Simulation.Persistence;
using Image = System.Drawing.Image;

namespace Mirage.Urbanization.WinForms
{
    public partial class StatisticsForm : Form
    {
        public StatisticsForm(SimulationRenderHelper helper)
        {
            InitializeComponent();

            foreach (var tabPage in _graphControlDefinitions)
                tabControl1.TabPages.Add(tabPage.TabPage);

            this.SizeChanged += (sender, e) => UpdateCharts(helper.SimulationSession.GetAllCityStatistics());

            helper.SimulationSession.CityStatisticsUpdated += (x, y) => UpdateCharts(helper.SimulationSession.GetAllCityStatistics());
        }

        public void UpdateCharts(IReadOnlyCollection<PersistedCityStatisticsWithFinancialData> statistics)
        {
            if (!IsHandleCreated) return;

            var bitmapsAndControls = _graphControlDefinitions
                .Select(graph => new
                {
                    GraphControl = graph,
                    Bitmap = graph.ProduceBitmapFor(statistics)
                })
                .ToList();

            this.BeginInvoke(new MethodInvoker(() =>
            {
                foreach (var bitmapAndGraphControl in bitmapsAndControls)
                {
                    bitmapAndGraphControl.GraphControl.DrawImage(bitmapAndGraphControl.Bitmap);
                }
            }));
        }

        private static IEnumerable<GraphDefinition> GenerateGraphDefinitions()
        {
            yield return new GraphDefinition("Amount of zones",
                new GraphSeries(
                    x => x.PersistedCityStatistics.ResidentialZonePopulationStatistics.Count,
                    "Residential",
                    Color.Green
                ), new GraphSeries(
                    x => x.PersistedCityStatistics.CommercialZonePopulationStatistics.Count,
                    "Commercial",
                    Color.Blue
                ), new GraphSeries(
                    x => x.PersistedCityStatistics.IndustrialZonePopulationStatistics.Count,
                    "Industrial",
                    Color.Goldenrod
                ), new GraphSeries(
                    x => x.PersistedCityStatistics.GlobalZonePopulationStatistics.Count,
                    "Global",
                    Color.DarkRed
                )
            );

            yield return new GraphDefinition("Amount of funds",
                new GraphSeries(x => x.CurrentAmountOfFunds, "Current amount of funds", Color.Red),
                new GraphSeries(x => x.CurrentProjectedAmountOfFunds, "Projected income", Color.Gray));

            yield return new GraphDefinition("Tax income",
                new GraphSeries(x => x.ResidentialTaxIncome, "Residential zones", Color.Green),
                new GraphSeries(x => x.CommercialTaxIncome, "Commercial zones", Color.Blue),
                new GraphSeries(x => x.IndustrialTaxIncome, "Industrial zones", Color.Goldenrod));

            yield return new GraphDefinition("Public sector expenses",
                new GraphSeries(x => x.PoliceServiceExpenses, "Police force", Color.Blue),
                new GraphSeries(x => x.FireServiceExpenses, "Fire fighters", Color.Red),
                new GraphSeries(x => x.RoadInfrastructureExpenses, "Infrastructure (Road)", Color.Gray),
                new GraphSeries(x => x.RailroadInfrastructureExpenses, "Infrastructure (Railroad)", Color.Yellow));

            yield return new GraphDefinition("Population",
                new GraphSeries(
                    x => x.PersistedCityStatistics.ResidentialZonePopulationStatistics.Sum,
                    "Residential",
                    Color.Green
                ), new GraphSeries(
                    x => x.PersistedCityStatistics.CommercialZonePopulationStatistics.Sum,
                    "Commercial",
                    Color.Blue
                ), new GraphSeries(
                    x => x.PersistedCityStatistics.IndustrialZonePopulationStatistics.Sum,
                    "Industrial",
                    Color.Goldenrod
                ), new GraphSeries(
                    x => x.PersistedCityStatistics.GlobalZonePopulationStatistics.Sum,
                    "Global",
                    Color.DarkRed
                )
            );

            foreach (var x in
                GetNumbarSummaryGraphs(x => x.PersistedCityStatistics.CrimeNumbers, "Crime", Color.Red, Color.DarkRed)
                .Concat(GetNumbarSummaryGraphs(x => x.PersistedCityStatistics.FireHazardNumbers, "Fire hazard", Color.Red, Color.DarkRed))
                .Concat(GetNumbarSummaryGraphs(x => x.PersistedCityStatistics.PollutionNumbers, "Pollution", Color.Green, Color.DarkOliveGreen))
                .Concat(GetNumbarSummaryGraphs(x => x.PersistedCityStatistics.TrafficNumbers, "Traffic", Color.Blue, Color.DarkBlue))
                .Concat(GetNumbarSummaryGraphs(x => x.PersistedCityStatistics.LandValueNumbers, "Land value", Color.Yellow, Color.GreenYellow))
                .Concat(GetNumbarSummaryGraphs(x => x.PersistedCityStatistics.AverageTravelDistanceStatistics, "Travel distances", Color.Blue, Color.DarkBlue))
                )
                yield return x;

            yield return new GraphDefinition("Power grid",
                new GraphSeries(
                    x => x.PersistedCityStatistics.PowerSupplyInUnits,
                    "Power supply (Total)",
                    Color.Green
                ), new GraphSeries(
                    x => x.PersistedCityStatistics.PowerConsumptionInUnits,
                    "Consumption",
                    Color.Yellow
                ), new GraphSeries(
                    x => x.PersistedCityStatistics.PowerSupplyInUnits - x.PersistedCityStatistics.PowerConsumptionInUnits,
                    "Power supply (Remaining)",
                    Color.Red
                )
            );

            yield return new GraphDefinition("Infastructure size",
                new GraphSeries(
                    x => x.PersistedCityStatistics.NumberOfRoadZones,
                    "Total amount of road zones",
                    Color.Blue
                ), new GraphSeries(
                    x => x.PersistedCityStatistics.NumberOfRailRoadZones,
                    "Total amount of railroad zones",
                    Color.Goldenrod
                )
            );
        }

        private static IEnumerable<GraphDefinition> GetNumbarSummaryGraphs(
            Func<PersistedCityStatisticsWithFinancialData,PersistedNumberSummary> getNumberSummary, 
            string title, 
            Color primaryColor, 
            Color secondaryColor)
        {
            Func<PersistedCityStatisticsWithFinancialData, PersistedNumberSummary> getNumberSummarySafeFunc =
                x => getNumberSummary(x) ?? PersistedNumberSummary.EmptyInstance;
            
            yield return new GraphDefinition("Total " + title,
                new GraphSeries(
                    x => getNumberSummarySafeFunc(x).Sum,
                    "Total",
                    primaryColor
                )
            );

            yield return new GraphDefinition("Average " + title,
                new GraphSeries(
                    x => getNumberSummarySafeFunc(x).Average,
                    "Average", 
                    secondaryColor
                ),
                new GraphSeries(
                    x => getNumberSummarySafeFunc(x).Max,
                    "Highest",
                    primaryColor
                )
            );
        }

        private readonly IList<GraphControlDefinition> _graphControlDefinitions = GenerateGraphDefinitions().Select(x => new GraphControlDefinition(x)).ToList();

        private class GraphControlDefinition
        {
            private readonly GraphDefinition _graphDefinition;
            private readonly TabPage _tabPage;
            private readonly PictureBox _pictureBox;

            public GraphControlDefinition(GraphDefinition graphDefinition)
            {
                _graphDefinition = graphDefinition;
                {
                    _tabPage = new TabPage() { Text = graphDefinition.Title, Dock = DockStyle.Fill };
                    _pictureBox = new PictureBox() { Dock = DockStyle.Fill };

                    _pictureBox.SendToBack();

                    _tabPage.Controls.Add(_pictureBox);
                }
            }

            public TabPage TabPage
            {
                get { return _tabPage; }
            }

            public Image ProduceBitmapFor(IReadOnlyCollection<PersistedCityStatisticsWithFinancialData> statistics)
            {
                using (var chartMemoryStream = new MemoryStream())
                {
                    var chart = new Chart
                    {
                        Width = _pictureBox.Width,
                        Height = _pictureBox.Height,
                        Palette = ChartColorPalette.Berry,
                        BorderColor = Color.DodgerBlue
                    };

                    var chartDef = _graphDefinition;

                    chart.ChartAreas.Add("Test");

                    var dataTable = new DataTable();

                    dataTable.Columns.Add("Type", typeof (string));
                    dataTable.Columns.Add(chartDef.Title, typeof (int));
                    dataTable.Columns.Add("TimeCode", typeof (string));

                    foreach (var statistic in statistics)
                    {
                        foreach (var col in chartDef.GraphSeriesSet)
                        {
                            dataTable.Rows.Add(col.Label, col.GetValue(statistic),
                                statistic.PersistedCityStatistics.TimeCode.ToString(CultureInfo.InvariantCulture));
                        }
                    }


                    chart.DataBindCrossTable(dataTable.Rows, "Type", "TimeCode", chartDef.Title, String.Empty);

                    chart.Titles.Add(chartDef.Title);

                    chart.Font.Name = _tabPage.Font.Name;
                    chart.Font.Size = new FontUnit(_tabPage.Font.SizeInPoints);

                    chart.ChartAreas[0].BackGradientStyle = GradientStyle.TopBottom;
                    chart.ChartAreas[0].BackSecondaryColor = Color.LightSkyBlue;
                    chart.ChartAreas[0].BackColor = Color.LightBlue;

                    chart.Legends.Add("Legend");

                    foreach (var series in chart.Series)
                    {
                        series.ChartType = SeriesChartType.Line;
                        series.BorderWidth = series.BorderWidth*4;

                        series.Color = chartDef.GraphSeriesSet.Single(x => x.Label == series.Name).Color;
                    }

                    chart.SaveImage(chartMemoryStream);

                    return Bitmap.FromStream(chartMemoryStream);
                }
            }

            public void DrawImage(Image image)
            {
                var current = _pictureBox.Image;
                _pictureBox.Image = image;
                if (current != null)    
                    current.Dispose();
            }
        }

        private class GraphDefinition
        {
            private readonly IReadOnlyCollection<GraphSeries> _graphSeriesSet;
            private readonly string _title;

            public GraphDefinition(string title, params GraphSeries[] graphSeriesSet)
            {
                _title = title;
                _graphSeriesSet = graphSeriesSet;
            }

            public string Title
            {
                get { return _title; }
            }

            public IReadOnlyCollection<GraphSeries> GraphSeriesSet
            {
                get { return _graphSeriesSet; }
            }
        }

        private class GraphSeries
        {
            private readonly Func<PersistedCityStatisticsWithFinancialData, int> valueGetterFunc;
            private readonly string _label;
            private readonly Color _color;

            public GraphSeries(Func<PersistedCityStatisticsWithFinancialData, int> valueGetterFunc, string label, Color color)
            {
                this.valueGetterFunc = valueGetterFunc;
                _label = label;
                _color = color;
            }

            public int GetValue(PersistedCityStatisticsWithFinancialData citytStatistics)
            {
                return valueGetterFunc(citytStatistics);
            }

            public string Label
            {
                get { return _label; }
            }

            public Color Color
            {
                get { return _color; }
            }
        }
    }
}
