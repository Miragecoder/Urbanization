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
    public partial class BudgetForm : Form
    {
        private Simulation.Persistence.PersistedCityStatisticsWithFinancialData _statistics;

        public BudgetForm(Simulation.Persistence.PersistedCityStatisticsWithFinancialData statistics)
        {
            InitializeComponent();
            _statistics = statistics;

            textBox1.Text = "Annual city budget for year: " +
                statistics.PersistedCityStatistics.TimeCode.ToString().Substring(0, 4);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
