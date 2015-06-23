using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mirage.Urbanization.Simulation;
using Mirage.Urbanization.Simulation.Datameters;
using Mirage.Urbanization.Statistics;

namespace Mirage.Urbanization.WinForms
{
    public partial class EvaluationForm : Form
    {
        public EvaluationForm(CityStatisticsView cityStatistics)
        {
            InitializeComponent();

            AddLabelValue("Population", cityStatistics.Population.ToString());
            AddLabelValue("Assessed value", cityStatistics.AssessdValue.ToString("C0"));
            AddLabelValue("Category", cityStatistics.CityCategory);

            listBox1.DataSource = cityStatistics
                .DataMeterResults
                .Where(x => x.ValueCategory > DataMeterValueCategory.None)
                .OrderByDescending(x => x.PercentageScore)
                .Select(x => string.Format("{0} - {1} ({2}%)", x.Name, x.ValueCategory, x.PercentageScore))
                .ToList();
        }

        private void AddLabelValue(string label, string value)
        {
            foreach (var text in new[] { label, value })
                new Label { Text = text }.AddControlTo(overallFlowLayoutPanel);
        }
    }
}
