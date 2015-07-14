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
    public partial class BudgetForm : FormWithCityStatisticsEvent
    {
        public BudgetForm(SimulationRenderHelper helper)
            : base(helper)
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public override void Update(IReadOnlyCollection<PersistedCityStatisticsWithFinancialData> statistics, PersistedCityStatisticsWithFinancialData current)
        {
            if (!IsHandleCreated) return;

            string budgetDescription = current.PersistedCityStatistics.GetYearAndMonth().CurrentYear.ToString();



            textBox1.BeginInvoke(new MethodInvoker(() =>
            {
                textBox1.Text = budgetDescription;
            }));
        }
    }
}
