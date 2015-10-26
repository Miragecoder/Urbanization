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
        private readonly TotalsGridViewController _totalsGridViewController;

        public BudgetForm(SimulationRenderHelper helper)
            : base(helper)
        {
            InitializeComponent();

            _taxDefinitionGridViewController = new BudgetComponentDefinitionGridViewController<TaxDefinition>(
                targetGridView: taxesDataGridView,
                definitions: TaxDefinition.TaxDefinitions,
                cityBudgetConfiguration: helper.SimulationSession.CityBudgetConfiguration,
                costsLabel: "Projected income",
                getCostsFunc: (definition, statistics) => definition.GetProjectedIncome(statistics)
            );

            _cityServiceDefinitionGridViewController = new BudgetComponentDefinitionGridViewController<CityServiceDefinition>(
                targetGridView: serviceExpensesDataGridView,
                definitions: CityServiceDefinition.CityServiceDefinitions,
                cityBudgetConfiguration: helper.SimulationSession.CityBudgetConfiguration,
                costsLabel: "Projected expenses",
                getCostsFunc: (definition, statistics) => definition.GetProjectedExpenses(statistics)
            );

            _totalsGridViewController = new TotalsGridViewController(totalsDataGridView);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public override void Update(IEnumerable<PersistedCityStatisticsWithFinancialData> statistics, PersistedCityStatisticsWithFinancialData current)
        {
            if (!IsHandleCreated) return;

            var yearMates = new HashSet<PersistedCityStatisticsWithFinancialData>(current
                .CombineWithYearMates(statistics));

            textBox1.BeginInvoke(new MethodInvoker(() =>
            {
                _taxDefinitionGridViewController.UpdateWith(yearMates);
                _cityServiceDefinitionGridViewController.UpdateWith(yearMates);
                _totalsGridViewController.UpdateWith(yearMates);
            }));
        }

        private class BudgetComponentDefinitionGridViewController<TBudgetComponentDefinition>
                where TBudgetComponentDefinition : BudgetComponentDefinition
        {
            private const string
                Sector = "Sector",
                Rate = "Rate";

            private readonly DataGridViewRow _totalDataGridViewRow = new DataGridViewRow();
            private readonly DataGridView _dataGridView;

            private readonly int _totalRowIndex;

            public BudgetComponentDefinitionGridViewController(
                DataGridView targetGridView,
                IEnumerable<TBudgetComponentDefinition> definitions,
                ICityBudgetConfiguration cityBudgetConfiguration,
                string costsLabel,
                Func<TBudgetComponentDefinition, ISet<PersistedCityStatisticsWithFinancialData>, decimal> getCostsFunc)
            {
                foreach (var name in new[] { Sector, costsLabel, Rate })
                    targetGridView.Columns.Add(new DataGridViewTextBoxColumn() { Name = name, HeaderText = name, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });

                _taxDefinitionControlSets = definitions
                    .Select(x => new BudgetComponentDefinitionControlSet(targetGridView, x, cityBudgetConfiguration, costsLabel, getCostsFunc))
                    .ToList();

                _dataGridView = targetGridView;

                _totalRowIndex = targetGridView.Rows.Add(_totalDataGridViewRow);

                _totalDataGridViewRow.Cells[costsLabel] = new DataGridViewTextBoxCell();
            }

            public void UpdateWith(ISet<PersistedCityStatisticsWithFinancialData> cityStatisticsWithFinancialDatas)
            {
                foreach (var taxDefinitionControl in _taxDefinitionControlSets)
                    taxDefinitionControl.UpdateWith(cityStatisticsWithFinancialDatas);

                var value = _taxDefinitionControlSets.Sum(x => x.GetValueFrom(cityStatisticsWithFinancialDatas));

                _dataGridView[0, _totalRowIndex].Value = "Total";
                _dataGridView[1, _totalRowIndex].Value = value.ToString("C");
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
                    ICityBudgetConfiguration cityBudgetConfiguration,
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

                    combobox.Value = budgetComponentDefinition.CurrentRate(cityBudgetConfiguration);

                    dataGridView.CellValueChanged += (s, e) =>
                    {
                        if (combobox == dataGridView[e.ColumnIndex, e.RowIndex])
                        {
                            budgetComponentDefinition.SetCurrentRate(cityBudgetConfiguration, Convert.ToDecimal(combobox.Value));
                        }
                    };
                }

                public void UpdateWith(ISet<PersistedCityStatisticsWithFinancialData> cityStatisticsWithFinancialDatas)
                {
                    if (_projectedCostsCell.DataGridView.IsHandleCreated)
                        _projectedCostsCell.Value = _getCostsFunc(_budgetComponentDefinition, cityStatisticsWithFinancialDatas).ToString("C");
                }

                public decimal GetValueFrom(ISet<PersistedCityStatisticsWithFinancialData> cityStatisticsWithFinancialDatas)
                {
                    return _getCostsFunc(_budgetComponentDefinition, cityStatisticsWithFinancialDatas);
                }
            }
        }

        private class TotalsGridViewController
        {
            private readonly DataGridView _targetGridView;

            private const string
                ColumnCategory = "Category",
                ColumnValue = "Value";

            private const string
                RowTaxIncome = "Tax income",
                RowCityServiceExpenses = "City service expenses",
                RowProjectedIncome = "Projected income";

            private readonly Dictionary<string, int> _rowNamesAndIds = new Dictionary<string, int>();

            public TotalsGridViewController(DataGridView targetGridView)
            {
                _targetGridView = targetGridView;
                foreach (var name in new[] { ColumnCategory, ColumnValue })
                    targetGridView.Columns.Add(new DataGridViewTextBoxColumn() { Name = name, HeaderText = name, AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });

                foreach (var rowName in new[] { RowTaxIncome, RowCityServiceExpenses, RowProjectedIncome })
                {
                    _rowNamesAndIds.Add(rowName, targetGridView.Rows.Add(new DataGridViewRow()));
                    targetGridView[ColumnCategory, _rowNamesAndIds[rowName]].Value = rowName;
                }
            }

            public void UpdateWith(HashSet<PersistedCityStatisticsWithFinancialData> yearMates)
            {
                _targetGridView[ColumnValue, _rowNamesAndIds[RowTaxIncome]].Value = yearMates.Sum(x => x.GetIncome()).ToString("C");
                _targetGridView[ColumnValue, _rowNamesAndIds[RowCityServiceExpenses]].Value = yearMates.Sum(x => x.GetExpenses()).ToString("C");
                _targetGridView[ColumnValue, _rowNamesAndIds[RowProjectedIncome]].Value = yearMates.Sum(x => x.GetTotal()).ToString("C");
            }
        }
    }
}
