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

            this.tabControl1.TabIndexChanged += (sender, e) => UpdateGrid(helper.SimulationSession.GetAllCityStatistics());
            this.SizeChanged += (sender, e) => UpdateGrid(helper.SimulationSession.GetAllCityStatistics());

            helper.SimulationSession.CityStatisticsUpdated += (x, y) => UpdateGrid(helper.SimulationSession.GetAllCityStatistics());
        }

        public void UpdateGrid(IReadOnlyCollection<PersistedCityStatistics> statistics)
        {
            if (!IsHandleCreated) return;
            this.BeginInvoke(new MethodInvoker(() =>
            {
                foreach (var x in _graphControlDefinitions)
                    x.ProduceRenderAction(statistics)();
            }));
        }

        private static IEnumerable<GraphDefinition> GenerateGraphDefinitions()
        {
            yield return new GraphDefinition("Amount of zones",
                new GraphSeries(
                    x => x.ResidentialZonePopulationStatistics.Count,
                    "Residential",
                    Color.Green
                ), new GraphSeries(
                    x => x.CommercialZonePopulationStatistics.Count,
                    "Commercial",
                    Color.Blue
                ), new GraphSeries(
                    x => x.IndustrialZonePopulationStatistics.Count,
                    "Industrial",
                    Color.Goldenrod
                ), new GraphSeries(
                    x => x.GlobalZonePopulationStatistics.Count,
                    "Global",
                    Color.DarkRed
                )
            );

            yield return new GraphDefinition("Population",
                new GraphSeries(
                    x => x.ResidentialZonePopulationStatistics.Sum,
                    "Residential",
                    Color.Green
                ), new GraphSeries(
                    x => x.CommercialZonePopulationStatistics.Sum,
                    "Commercial",
                    Color.Blue
                ), new GraphSeries(
                    x => x.IndustrialZonePopulationStatistics.Sum,
                    "Industrial",
                    Color.Goldenrod
                ), new GraphSeries(
                    x => x.GlobalZonePopulationStatistics.Sum,
                    "Global",
                    Color.DarkRed
                )
            );

            foreach (var x in
                GetNumbarSummaryGraphs(x => x.CrimeNumbers, "Crime", Color.Red, Color.DarkRed)
                .Concat(GetNumbarSummaryGraphs(x => x.PollutionNumbers, "Pollution", Color.Green, Color.DarkOliveGreen))
                .Concat(GetNumbarSummaryGraphs(x => x.TrafficNumbers, "Traffic", Color.Blue, Color.DarkBlue))
                .Concat(GetNumbarSummaryGraphs(x => x.LandValueNumbers, "Land value", Color.Yellow, Color.GreenYellow))
                )
                yield return x;

            yield return new GraphDefinition("Power grid",
                new GraphSeries(
                    x => x.PowerSupplyInUnits,
                    "Power supply (Total)",
                    Color.Green
                ), new GraphSeries(
                    x => x.PowerConsumptionInUnits,
                    "Consumption",
                    Color.Yellow
                ), new GraphSeries(
                    x => x.PowerSupplyInUnits - x.PowerConsumptionInUnits,
                    "Power supply (Remaining)",
                    Color.Red
                )
            );

            yield return new GraphDefinition("Infastructure size",
                new GraphSeries(
                    x => x.NumberOfRoadZones,
                    "Total amount of road zones",
                    Color.Blue
                ), new GraphSeries(
                    x => x.NumberOfRailRoadZones,
                    "Total amount of railroad zones",
                    Color.Goldenrod
                )
            );
        }

        private static IEnumerable<GraphDefinition> GetNumbarSummaryGraphs(Func<PersistedCityStatistics,PersistedNumberSummary> getNumberSummary, string title, Color primaryColor, Color secondaryColor)
        {
            yield return new GraphDefinition("Total " + title,
                new GraphSeries(
                    x => getNumberSummary(x).Sum,
                    "Total",
                    primaryColor
                )
            );

            yield return new GraphDefinition("Average " + title,
                new GraphSeries(
                    x => getNumberSummary(x).Average,
                    "Average", 
                    secondaryColor
                ),
                new GraphSeries(
                    x => getNumberSummary(x).Max,
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

            public Action ProduceRenderAction(IReadOnlyCollection<PersistedCityStatistics> statistics)
            {
                return () =>
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

                        dataTable.Columns.Add("Type", typeof(string));
                        dataTable.Columns.Add(chartDef.Title, typeof(int));
                        dataTable.Columns.Add("TimeCode", typeof(string));

                        foreach (var statistic in statistics)
                        {
                            foreach (var col in chartDef.GraphSeriesSet)
                            {
                                dataTable.Rows.Add(col.Label, col.GetValue(statistic),
                                    statistic.TimeCode.ToString(CultureInfo.InvariantCulture));
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
                            series.BorderWidth = series.BorderWidth * 4;

                            series.Color = chartDef.GraphSeriesSet.Single(x => x.Label == series.Name).Color;
                        }

                        chart.SaveImage(chartMemoryStream);

                        Image chartImage = Bitmap.FromStream(chartMemoryStream);

                        _pictureBox.Image = chartImage;
                    }
                };
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
            private readonly Func<PersistedCityStatistics, int> valueGetterFunc;
            private readonly string _label;
            private readonly Color _color;

            public GraphSeries(Func<PersistedCityStatistics, int> valueGetterFunc, string label, Color color)
            {
                this.valueGetterFunc = valueGetterFunc;
                _label = label;
                _color = color;
            }

            public int GetValue(PersistedCityStatistics citytStatistics)
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
