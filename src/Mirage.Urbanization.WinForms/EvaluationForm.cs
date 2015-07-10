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

            AddLabelValue("Population", cityStatistics.Population.ToString("N0"));
            AddLabelValue("Assessed value", cityStatistics.AssessedValue.ToString("C"));
            AddLabelValue("Category", cityStatistics.CityCategory);
            AddLabelValue("Current amount of funds", cityStatistics.CurrentAmountOfFunds.ToString("C"));
            AddLabelValue("Projected income", cityStatistics.CurrentProjectedAmountOfFunds.ToString("C"));

            listBox1.DataSource = cityStatistics
                .DataMeterResults
                .Where(x => x.ValueCategory > DataMeterValueCategory.None)
                .OrderByDescending(x => x.PercentageScore)
                .Select(x => string.Format("{0} - {1} ({2}%)", x.Name, x.ValueCategory, x.PercentageScoreString))
                .ToList();

            var negativeOpinion = cityStatistics
                .DataMeterResults
                .Average(x => x.PercentageScore);

            var positiveOpinion = 1 - negativeOpinion;

            listBox2.DataSource =
                new[]
                {
                    new
                    {
                        Percentage = negativeOpinion.ToString("P"),
                        Name = "Negative"
                    },
                    new
                    {
                        Percentage = positiveOpinion.ToString("P"),
                        Name = "Positive"
                    }
                }
                .OrderByDescending(x => x.Percentage)
                .Select(x => x.Name + ' ' + x.Percentage)
                .ToList();
        }

        private void AddLabelValue(string label, string value)
        {
            foreach (var text in new[] { label, value })
                new Label { Text = text }.AddControlTo(overallFlowLayoutPanel);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
