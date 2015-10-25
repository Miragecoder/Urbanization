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
using Mirage.Urbanization.Charts;
using Mirage.Urbanization.Statistics;
using Mirage.Urbanization.Simulation;
using Mirage.Urbanization.Simulation.Persistence;
using Image = System.Drawing.Image;

namespace Mirage.Urbanization.WinForms
{
    public partial class StatisticsForm : FormWithCityStatisticsEvent
    {
        private readonly Lazy<IChartDrawer> _chartDrawer;

        public StatisticsForm(SimulationRenderHelper helper, Func<IChartDrawer> createChartDrawerFunc)
            : base(helper)
        {
            _chartDrawer = new Lazy<IChartDrawer>(createChartDrawerFunc);
            InitializeComponent();

            foreach (var tabPage in _graphControlDefinitions)
                tabControl1.TabPages.Add(tabPage.TabPage);
        }

        public override void Update(
            IReadOnlyCollection<PersistedCityStatisticsWithFinancialData> statistics, 
            PersistedCityStatisticsWithFinancialData current)
        {
            var bitmapsAndControls = _graphControlDefinitions
                .Select(graph => new
                {
                    GraphControl = graph,
                    Bitmap = _chartDrawer.Value.Draw(graph.GraphDefinition, statistics, graph.TabPage.Font, graph.TabPage.Size)
                })
                .ToList();

            if (!IsHandleCreated) return;
            BeginInvoke(new MethodInvoker(() =>
            {
                foreach (var bitmapAndGraphControl in bitmapsAndControls)
                {
                    bitmapAndGraphControl.GraphControl.DrawImage(bitmapAndGraphControl.Bitmap);
                }
            }));
        }

        private readonly IList<GraphControlDefinition> _graphControlDefinitions = GraphDefinitions
            .Instances
            .Select(x => new GraphControlDefinition(x))
            .ToList();

        private class GraphControlDefinition
        {
            public readonly GraphDefinition GraphDefinition;
            private readonly PictureBox _pictureBox;

            public GraphControlDefinition(GraphDefinition graphDefinition)
            {
                GraphDefinition = graphDefinition;
                {
                    TabPage = new TabPage() { Text = graphDefinition.Title, Dock = DockStyle.Fill };
                    _pictureBox = new PictureBox() { Dock = DockStyle.Fill };

                    _pictureBox.SendToBack();

                    TabPage.Controls.Add(_pictureBox);
                }
            }

            public TabPage TabPage { get; }

            public void DrawImage(Image image)
            {
                var current = _pictureBox.Image;
                _pictureBox.Image = image;
                if (current != null)
                    current.Dispose();
            }
        }
    }
}
