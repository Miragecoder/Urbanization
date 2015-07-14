using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
        private readonly TaxDefinitionGridViewController _taxDefinitionGridViewController;

        public BudgetForm(SimulationRenderHelper helper)
            : base(helper)
        {
            InitializeComponent();

            _taxDefinitionGridViewController = new TaxDefinitionGridViewController(dataGridView2, helper.SimulationSession.Budget);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public override void Update(IReadOnlyCollection<PersistedCityStatisticsWithFinancialData> statistics, PersistedCityStatisticsWithFinancialData current)
        {
            if (!IsHandleCreated) return;

            var yearMates = new HashSet<PersistedCityStatisticsWithFinancialData>(current
                .CombineWithYearMates(statistics));

            var summary = new BudgetSummary(yearMates);

            textBox1.BeginInvoke(new MethodInvoker(() =>
            {
                _taxDefinitionGridViewController.UpdateWith(yearMates);
            }));
        }

        private class TaxDefinitionGridViewController
        {
            private const string
                Sector = "Sector",
                ProjectedIncome = "Projected Income",
                Rate = "Rate";

            public TaxDefinitionGridViewController(DataGridView targetGridView, IBudget budget)
            {
                foreach (var name in new[] { Sector, ProjectedIncome, Rate })
                    targetGridView.Columns.Add(new DataGridViewTextBoxColumn() { Name = name, HeaderText = name, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });

                _taxDefinitionControlSets = TaxDefinition
                    .TaxDefinitions
                    .Select(x => new TaxDefinitionControlSet(targetGridView, x, budget))
                    .ToList();
            }

            public void UpdateWith(ISet<PersistedCityStatisticsWithFinancialData> cityStatisticsWithFinancialDatas)
            {
                foreach (var taxDefinitionControl in _taxDefinitionControlSets)
                    taxDefinitionControl.UpdateWith(cityStatisticsWithFinancialDatas);
            }

            private readonly IList<TaxDefinitionControlSet> _taxDefinitionControlSets;

            private class TaxDefinitionControlSet
            {
                private readonly TaxDefinition _taxDefinition;
                private readonly IBudget _budget;
                private readonly DataGridViewCell _projectedIncomeCell, _currentRateCell;

                public TaxDefinitionControlSet(DataGridView dataGridView, TaxDefinition taxDefinition, IBudget budget)
                {
                    _taxDefinition = taxDefinition;
                    _budget = budget;
                    var dataGridViewRow = new DataGridViewRow();
                    var index = dataGridView.Rows.Add(dataGridViewRow);
                    dataGridView[Sector, index].Value = taxDefinition.Name;

                    var combobox = new DataGridViewComboBoxCell();
                    _projectedIncomeCell = dataGridView[ProjectedIncome, index];
                    dataGridView[Rate, index] = combobox;

                    combobox.DataSource = Enumerable
                        .Range(0, 20)
                        .Select(x => (decimal) x)
                        .Select(x => x / 100)
                        .Select(x => new
                        {
                            Value = x,
                            Label = x.ToString("P")
                        })
                        .ToList();
                    combobox.DisplayMember = "Label";
                    combobox.ValueMember = "Value";

                    combobox.Value = taxDefinition.CurrentRate(budget);

                    dataGridView.CellValueChanged += (s, e) =>
                    {
                        if (combobox == dataGridView[e.ColumnIndex, e.RowIndex])
                        {
                            taxDefinition.SetCurrentRate(budget, Convert.ToDecimal(combobox.Value));
                        }
                    };
                }

                public void UpdateWith(ISet<PersistedCityStatisticsWithFinancialData> cityStatisticsWithFinancialDatas)
                {
                    if (_projectedIncomeCell.DataGridView.IsHandleCreated)
                        _projectedIncomeCell.Value = _taxDefinition.GetProjectedIncome(cityStatisticsWithFinancialDatas).ToString("C");
                }
            }
        }

        private class TaxDefinition
        {
            private readonly string _name;
            private readonly Func<IBudget, decimal> _getCurrentRate;
            private readonly Action<IBudget, decimal> _setCurrentRate;
            private readonly Func<ISet<PersistedCityStatisticsWithFinancialData>, int> _getProjectedIncome;

            private TaxDefinition(
                string name,
                Expression<Func<IBudget, decimal>> currentRate,
                Func<ISet<PersistedCityStatisticsWithFinancialData>, int> getProjectedIncome)
            {
                _name = name;
                _getCurrentRate = currentRate.Compile();
                _setCurrentRate = (budget, rate) => ((PropertyInfo)((MemberExpression)currentRate.Body).Member).SetValue(budget, rate);
                _getProjectedIncome = getProjectedIncome;
            }

            public string Name { get { return _name; } }
            public decimal CurrentRate(IBudget budget) { return _getCurrentRate(budget); }
            public void SetCurrentRate(IBudget budget, decimal rate) { _setCurrentRate(budget, rate); }
            public int GetProjectedIncome(ISet<PersistedCityStatisticsWithFinancialData> cityStatistics)
            {
                return _getProjectedIncome(cityStatistics);
            }

            private static readonly TaxDefinition ResidentialTaxDefinition = new TaxDefinition("Residential", x => x.ResidentialTaxRate, x => x.Sum(y => y.ResidentialTaxIncome));
            private static readonly TaxDefinition CommercialTaxDefinition = new TaxDefinition("Commercial", x => x.CommercialTaxRate, x => x.Sum(y => y.CommercialTaxIncome));
            private static readonly TaxDefinition IndustrialTaxDefinition = new TaxDefinition("Industrial", x => x.IndustrialTaxRate, x => x.Sum(y => y.IndustrialTaxIncome));

            public static IEnumerable<TaxDefinition> TaxDefinitions
            {
                get
                {
                    yield return ResidentialTaxDefinition;
                    yield return CommercialTaxDefinition;
                    yield return IndustrialTaxDefinition;
                }
            }
        }

        private class BudgetSummary
        {
            private readonly ISet<PersistedCityStatisticsWithFinancialData> _statistics;

            public int Year { get { return _statistics.First().PersistedCityStatistics.GetYearAndMonth().CurrentYear; } }

            public BudgetSummary(ISet<PersistedCityStatisticsWithFinancialData> statistics)
            {
                _statistics = statistics;

                var statisticsGroupedByYear = _statistics.GroupBy(x => x.PersistedCityStatistics.GetYearAndMonth().CurrentYear);
                if (statisticsGroupedByYear.Count() != 1)
                    throw new ArgumentException(
                        message: string.Format(
                            format: "'{0}'-instances encountered for multiple years ({1}); only one is supported.",
                            arg0: typeof(PersistedCityStatisticsWithFinancialData).Name,
                            arg1: string.Join(", ", _statistics.Select(x => x.PersistedCityStatistics.GetYearAndMonth().CurrentYear))
                        )
                    );
            }
        }
    }
}
