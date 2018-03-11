using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Mirage.Urbanization.Charts;
using Mirage.Urbanization.Persistence;
using Mirage.Urbanization.Simulation;
using Mirage.Urbanization.Simulation.Persistence;
using Mirage.Urbanization.Web;
using Mirage.Urbanization.WinForms.Overlay;

namespace Mirage.Urbanization.WinForms
{
    public partial class MainForm : Form
    {
        private Panel _gamePanel;
        private SimulationRenderHelper _areaRenderHelper;
        private readonly GraphicsManagerSelection _graphicsManagerSelection;
        private readonly OverlaySelection _overlaySelection;

        private readonly CitySaveStateController _citySaveStateController;

        private void WithAreaRenderHelper(Action<SimulationRenderHelper> action)
        {
            var helper = _areaRenderHelper;
            if (helper != null) action(helper);
        }

        private void ReloadGamePanel()
        {
            if (_gamePanel != null && Controls.Contains(_gamePanel))
            {
                Controls.Remove(_gamePanel);
            }

            _gamePanel = new Panel();

            Controls.Add(_gamePanel);
            _gamePanel.BringToFront();
            _gamePanel.Dock = DockStyle.Fill;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            {
                var keyCharHandled = false;
                var keyDataAsString = keyData.ToString().ToLowerInvariant();
                if (keyDataAsString.Length == 1)
                {
                    WithAreaRenderHelper(
                        helper =>
                            keyCharHandled = helper.HandleKeyChar(keyDataAsString[0]));
                }
                if (keyCharHandled)
                    return true;
            }

            switch (keyData)
            {
                case Keys.Oemplus:
                    WithAreaRenderHelper(helper => helper.ToggleZoom(ZoomMode.Full));
                    break;
                case Keys.OemMinus:
                    WithAreaRenderHelper(helper => helper.ToggleZoom(ZoomMode.Half));
                    break;
                case Keys.Left:
                    WithAreaRenderHelper(helper => helper.MoveLeft());
                    break;
                case Keys.Right:
                    WithAreaRenderHelper(helper => helper.MoveRight());
                    break;
                case Keys.Up:
                    WithAreaRenderHelper(helper => helper.MoveUp());
                    break;
                case Keys.Down:
                    WithAreaRenderHelper(helper => helper.MoveDown());
                    break;
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
            return true;
        }

        public MainForm()
        {
            InitializeComponent();
            ToggleFormText(String.Empty);

            ReloadGamePanel();

            _citySaveStateController = new CitySaveStateController(saveFunctionalityAvailable =>
                saveCityToolStripMenuItem.Enabled
                = saveCityAsToolStripMenuItem.Enabled
                = saveFunctionalityAvailable
            );

            _graphicsManagerSelection = new GraphicsManagerSelection(rendererToolStripMenuItem);
            _graphicsManagerSelection.OnSelectionChanged += (sender, e) => WithAreaRenderHelper(helper => helper.ChangeRenderer(e.ToolstripMenuOption.Factory));

            _overlaySelection = new OverlaySelection(overlayMenuItem, () => toggleOverlayNumbers.Checked);

            Size = new Size(Screen.PrimaryScreen.WorkingArea.Width * 75 / 100, Screen.PrimaryScreen.WorkingArea.Height * 75 / 100);
            CenterToScreen();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            WithAreaRenderHelper(helper => helper.Stop());
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            WithAreaRenderHelper(helper => helper.ToggleZoom(ZoomMode.Full));
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            WithAreaRenderHelper(helper => helper.ToggleZoom(ZoomMode.Half));
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void newCityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var newCityForm = new NewCityForm();

            var dialogResult = newCityForm.ShowDialog();

            switch (dialogResult)
            {
                case DialogResult.OK:
                    WithAreaRenderHelper(helper => helper.Stop());

                    RegisterAreaRenderHelper(terraformingOptions: newCityForm.GetTerraformingOptions());
                    ToggleFormText("New city");
                    break;
                default:

                    break;
            }
        }

        private void RegisterAreaRenderHelper(TerraformingOptions terraformingOptions = null, PersistedSimulation persistedSimulation = null)
        {
            _gamePanel.Controls.Clear();

            var areaOptions = terraformingOptions != null
                ? new SimulationOptions(terraformingOptions,
                    new ProcessOptions(() => showGrowthPathFinding.Checked, () => enableMoneyCheatToolStripMenuItem.Checked))
                : new SimulationOptions(persistedSimulation,
                    new ProcessOptions(() => showGrowthPathFinding.Checked, () => enableMoneyCheatToolStripMenuItem.Checked));

            if (_areaRenderHelper != null)
            {
                _areaRenderHelper.Stop();
                ReloadGamePanel();
            }

            _areaRenderHelper = new SimulationRenderHelper(
                           gamePanel: _gamePanel,
                           renderZoneOptions: new RenderZoneOptions(
                               showDebugGrowthPathFinding: () => showGrowthPathFinding.Checked,
                               selectedGraphicsManagerFunc: () => _graphicsManagerSelection.GetCurrentOption(),
                               getCurrentOverlayOptionFunc: () => _overlaySelection.GetCurrentOption()
                           ),
                           options: areaOptions
                       );

            _areaRenderHelper.SimulationSession.OnAreaMessage +=
                (_sender, _e) =>
                    statusStrip1.BeginInvoke(new MethodInvoker(() => { toolStripStatusLabel1.Text = _e.Message; }));

            _areaRenderHelper.SimulationSession.OnAreaHotMessage +=
                (_sender, _e) =>
                {
                    if (_webserverForm == null)
                        MessageBox.Show(_e.Message, _e.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        statusStrip1.BeginInvoke(new MethodInvoker(() => { toolStripStatusLabel1.Text = _e.Message; }));
                };

            _areaRenderHelper.SimulationSession.OnCityBudgetValueChanged +=
                (_sender, _e) =>
                    statusStrip1.BeginInvoke(new MethodInvoker(() =>
                    {
                        foreach (var x in new[]
                        {
                            new
                            {
                                LabelControl = cityBudgetLabel,
                                Text = _e.EventData.CurrentAmountDescription,
                                Amount = _e.EventData.CurrentAmount
                            },
                            new
                            {
                                LabelControl = projectedIncomeLabel,
                                Text = _e.EventData.ProjectedIncomeDescription,
                                Amount = _e.EventData.ProjectedIncome
                            }
                        })
                        {
                            x.LabelControl.Text = x.Text;
                            x.LabelControl.ForeColor = x.Amount >= 0
                                ? SystemColors.ControlText
                                : Color.Red;
                        }
                    }));

            _areaRenderHelper.SimulationSession.OnYearAndOrMonthChanged +=
                (_sender, _e) =>
                {
                    if (statusStrip1.IsHandleCreated)
                    {
                        statusStrip1.BeginInvoke(new MethodInvoker(
                            () => { monthAndYearLabel.Text = _e.EventData.GetCurrentDescription(); }));
                    }
                };

            _areaRenderHelper.Start();
            _citySaveStateController.ToggleSaveFunctionalityAvailable();
        }

        private void saveCityAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WithAreaRenderHelper(helper =>
            {
                switch (saveCityDialog.ShowDialog())
                {
                    case DialogResult.OK:
                        _citySaveStateController.SaveFile(saveCityDialog.FileName, helper.SimulationSession.GeneratePersistedArea());
                        ToggleFormText(saveCityDialog.FileName);
                        break;
                }
            });
        }

        private void saveCityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_citySaveStateController.CurrentFilenameIsKnown)
                WithAreaRenderHelper(helper => _citySaveStateController.Save(helper.SimulationSession.GeneratePersistedArea()));
            else
            {
                saveCityAsToolStripMenuItem_Click(sender, e);
            }
        }

        private void openCityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (openCityDialog.ShowDialog())
            {
                case DialogResult.OK:
                    RegisterAreaRenderHelper(persistedSimulation: _citySaveStateController.LoadFile(openCityDialog.FileName));
                    ToggleFormText(openCityDialog.FileName);
                    break;
            }
        }

        private void ToggleFormText(string cityName)
        {
            Text = "Urbanization";
            if (!String.IsNullOrWhiteSpace(cityName))
            {
                Text += " - " + cityName;
            }
            cityBudgetLabel.Text = monthAndYearLabel.Text = toolStripStatusLabel1.Text = string.Empty;
        }

        private readonly SimulationRenderHelperFormManager<EvaluationForm> _evaluationFormManager = new SimulationRenderHelperFormManager<EvaluationForm>(helper => new EvaluationForm(helper));

        private void evaluationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WithAreaRenderHelper(helper =>
            {
                _evaluationFormManager.Show(this, helper);
            });
        }

        private readonly SimulationRenderHelperFormManager<BudgetForm> _budgetFormManager = new SimulationRenderHelperFormManager<BudgetForm>(helper => new BudgetForm(helper));

        private void cityBudgetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WithAreaRenderHelper(helper =>
            {
                _budgetFormManager.Show(this, helper);
            });
        }

        private readonly SimulationRenderHelperFormManager<StatisticsForm> _statisticsFormManager = new SimulationRenderHelperFormManager<StatisticsForm>(helper => new StatisticsForm(helper, ChartDrawerFactory.Create));

        private void statisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WithAreaRenderHelper(helper =>
            {
                _statisticsFormManager.Show(this, helper);
            });
        }

        private readonly FormManager<LogWindow> _logWindowFormManager = new FormManager<LogWindow>(() => new LogWindow());

        private void debugWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _logWindowFormManager.Show(this);
        }

        private WebServerForm _webserverForm;

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (_webserverForm == null)
                WithAreaRenderHelper(helper =>
                {
                    _webserverForm = new WebServerForm(helper.SimulationSession);
                    _webserverForm.Show(this);
                    _webserverForm.Closed += (s, x) =>
                    {
                        _webserverForm.Dispose();
                        _webserverForm = null;
                    };
                });
        }

        private void forceZedGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChartDrawerFactory.ForceZedGraph = forceZedGraphToolStripMenuItem.Checked;
        }
    }
}
