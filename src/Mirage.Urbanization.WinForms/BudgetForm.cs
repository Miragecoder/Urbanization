using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Mirage.Urbanization.Simulation;
using Mirage.Urbanization.Simulation.Persistence;

namespace Mirage.Urbanization.WinForms
{
    public partial class BudgetForm : FormWithCityStatisticsEvent
    {
        private readonly BudgetComponentDefinitionGridViewController<TaxDefinition> _taxDefinitionGridViewController;
        private readonly BudgetComponentDefinitionGridViewController<CityServiceDefinition> _cityServiceDefinitionGridViewController;

        public BudgetForm(SimulationRenderHelper helper)
            : base(helper)
        {
            InitializeComponent();

            _taxDefinitionGridViewController = new BudgetComponentDefinitionGridViewController<TaxDefinition>(
                targetGridView: dataGridView2,
                definitions: TaxDefinition.TaxDefinitions,
                budget: helper.SimulationSession.Budget,
                costsLabel: "Projected income",
                getCostsFunc: (definition, statistics) => definition.GetProjectedIncome(statistics)
            );

            _cityServiceDefinitionGridViewController = new BudgetComponentDefinitionGridViewController<CityServiceDefinition>(
                targetGridView: dataGridView1,
                definitions: CityServiceDefinition.CityServiceDefinitions,
                budget: helper.SimulationSession.Budget,
                costsLabel: "Projected expenses",
                getCostsFunc: (definition, statistics) => definition.GetProjectedExpenses(statistics)
            );
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
                _cityServiceDefinitionGridViewController.UpdateWith(yearMates);
            }));
        }

        private class BudgetComponentDefinitionGridViewController<TBudgetComponentDefinition>
                where TBudgetComponentDefinition : BudgetComponentDefinition
        {
            private const string
                Sector = "Sector",
                Rate = "Rate";

            public BudgetComponentDefinitionGridViewController(
                DataGridView targetGridView,
                IEnumerable<TBudgetComponentDefinition> definitions,
                IBudget budget,
                string costsLabel,
                Func<TBudgetComponentDefinition, ISet<PersistedCityStatisticsWithFinancialData>, decimal> getCostsFunc)
            {
                foreach (var name in new[] { Sector, costsLabel, Rate })
                    targetGridView.Columns.Add(new DataGridViewTextBoxColumn() { Name = name, HeaderText = name, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });

                _taxDefinitionControlSets = definitions
                    .Select(x => new BudgetComponentDefinitionControlSet(targetGridView, x, budget, costsLabel, getCostsFunc))
                    .ToList();
            }

            public void UpdateWith(ISet<PersistedCityStatisticsWithFinancialData> cityStatisticsWithFinancialDatas)
            {
                foreach (var taxDefinitionControl in _taxDefinitionControlSets)
                    taxDefinitionControl.UpdateWith(cityStatisticsWithFinancialDatas);
            }

            private readonly IList<BudgetComponentDefinitionControlSet> _taxDefinitionControlSets;

            private class BudgetComponentDefinitionControlSet
            {
                private readonly TBudgetComponentDefinition _budgetComponentDefinition;
                private readonly Func<TBudgetComponentDefinition, ISet<PersistedCityStatisticsWithFinancialData>, decimal> _getCostsFunc;
                private readonly DataGridViewCell _projectedCostsCell;

                public BudgetComponentDefinitionControlSet(
                    DataGridView dataGridView,
                    TBudgetComponentDefinition budgetComponentDefinition,
                    IBudget budget,
                    string costsLabel,
                    Func<TBudgetComponentDefinition, ISet<PersistedCityStatisticsWithFinancialData>, decimal> getCostsFunc)
                {
                    _budgetComponentDefinition = budgetComponentDefinition;
                    _getCostsFunc = getCostsFunc;
                    var dataGridViewRow = new DataGridViewRow();
                    var index = dataGridView.Rows.Add(dataGridViewRow);
                    dataGridView[Sector, index].Value = budgetComponentDefinition.Name;
                    dataGridView[Sector, index].ReadOnly = true;

                    var combobox = new DataGridViewComboBoxCell();
                    _projectedCostsCell = dataGridView[costsLabel, index];
                    dataGridView[Rate, index] = combobox;

                    combobox.DataSource = _budgetComponentDefinition
                        .GetSelectableRatePercentages()
                        .Select(x => new
                        {
                            Value = x,
                            Label = x.ToString("P")
                        })
                        .OrderByDescending(x => x.Value)
                        .ToList();
                    combobox.DisplayMember = "Label";
                    combobox.ValueMember = "Value";

                    combobox.Value = budgetComponentDefinition.CurrentRate(budget);

                    dataGridView.CellValueChanged += (s, e) =>
                    {
                        if (combobox == dataGridView[e.ColumnIndex, e.RowIndex])
                        {
                            budgetComponentDefinition.SetCurrentRate(budget, Convert.ToDecimal(combobox.Value));
                        }
                    };
                }

                public void UpdateWith(ISet<PersistedCityStatisticsWithFinancialData> cityStatisticsWithFinancialDatas)
                {
                    if (_projectedCostsCell.DataGridView.IsHandleCreated)
                        _projectedCostsCell.Value = _getCostsFunc(_budgetComponentDefinition, cityStatisticsWithFinancialDatas).ToString("C");
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
