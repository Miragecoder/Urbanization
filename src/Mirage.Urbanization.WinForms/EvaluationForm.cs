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
using Mirage.Urbanization.Simulation.Persistence;
using Mirage.Urbanization.Statistics;

namespace Mirage.Urbanization.WinForms
{
    public partial class EvaluationForm : FormWithCityStatisticsEvent
    {
        public EvaluationForm(SimulationRenderHelper helper) :base(helper)
        {
            InitializeComponent();
        }

        public override void Update(IReadOnlyCollection<PersistedCityStatisticsWithFinancialData> statistics, PersistedCityStatisticsWithFinancialData current)
        {
            var cityStatistics = new CityStatisticsView(current);

            groupBox1.BeginInvoke(new MethodInvoker(() =>
            {

            AddLabelValue("Population", cityStatistics.Population.ToString("N0"), dataGridView1);
            AddLabelValue("Assessed value", cityStatistics.AssessedValue.ToString("C"), dataGridView1);
            AddLabelValue("Category", cityStatistics.CityCategory, dataGridView1);
            AddLabelValue("Current funds", cityStatistics.CurrentAmountOfFunds.ToString("C"), dataGridView2);
            AddLabelValue("Projected income", cityStatistics.CurrentProjectedAmountOfFunds.ToString("C"), dataGridView2);

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

            }));
        }

        private static void AddLabelValue(string label, string value, DataGridView gridView)
        {
            gridView.BeginInvoke(new MethodInvoker(() =>
            {
                var match = gridView.Rows.Cast<DataGridViewRow>().FirstOrDefault(x => x.Cells[0].Value.ToString() == label);
                if (match != null)
                {
                    match.Cells[1].Value = value;
                    gridView.UpdateCellValue(1, gridView.Rows.IndexOf(match));
                }
                else
                {
                    gridView.Rows.Add(label, value);
                }
            }));
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
