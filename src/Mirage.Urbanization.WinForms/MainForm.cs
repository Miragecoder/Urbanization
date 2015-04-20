using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Mirage.Urbanization.Persistence;
using Mirage.Urbanization.Simulation;
using Mirage.Urbanization.Simulation.Persistence;

namespace Mirage.Urbanization.WinForms
{
    public partial class MainForm : Form
    {
        private Panel _gamePanel;
        private SimulationRenderHelper _areaRenderHelper;
        private readonly GraphicsManagerSelection _graphicsManagerSelection;

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
            _graphicsManagerSelection.OnSelectionChanged += (sender, e) => WithAreaRenderHelper(helper => helper.ChangeRenderer(e.GraphicsManagerWrapperOption.Factory));
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            WithAreaRenderHelper(helper => helper.Stop());
        }

        private void statisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WithAreaRenderHelper(helper =>
            {
                var statistics = helper.SimulationSession.GetAllCityStatistics();

                _currentStatisticsForm = new StatisticsForm(helper);
                _currentStatisticsForm.UpdateGrid(statistics);
                _currentStatisticsForm.Show();
            });
        }

        private StatisticsForm _currentStatisticsForm;

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
                    new ProcessOptions(() => showGrowthPathfindingToolStripMenuItem.Checked))
                : new SimulationOptions(persistedSimulation,
                    new ProcessOptions(() => showGrowthPathfindingToolStripMenuItem.Checked));

            if (_areaRenderHelper != null)
            {
                _areaRenderHelper.Stop();
                ReloadGamePanel();
            }

            _areaRenderHelper = new SimulationRenderHelper(
                           gamePanel: _gamePanel,
                           renderZoneOptions: new RenderZoneOptions(
                               renderPollutionValues: () => showPollution.Checked,
                               renderCrimeValues: () => showCrime.Checked,
                               showTrafficStatistics: () => showTrafficStats.Checked,
                               showGrowthPathFinding: () => showGrowthPathfindingToolStripMenuItem.Checked,
                               showAverageTravelDistances: () => showTravelDistancesToolStripMenuItem.Checked,
                               showPopulationDensity: () => showPopulationDensityToolStripMenuItem.Checked,
                               selectedGraphicsManagerFunc: () => _graphicsManagerSelection.GetCurrentOption(),
                               renderLandValueValues: () => showLandValueToolStripMenuItem.Checked
                           ),
                           options: areaOptions
                       );


            _areaRenderHelper.SimulationSession.Area.OnAreaMessage +=
                (_sender, _e) =>
                    statusStrip1.BeginInvoke(new MethodInvoker(() => { toolStripStatusLabel1.Text = _e.Message.Message; }));

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
        }

        private class CitySaveStateController
        {
            private readonly Action<bool> _onSaveFunctionalityAvailableAction;

            public CitySaveStateController(Action<bool> onSaveFunctionalityAvailableAction)
            {
                _onSaveFunctionalityAvailableAction = onSaveFunctionalityAvailableAction;
            }

            private string _lastSaveFileName;

            private readonly XmlSerializer _xmlSerializer = new XmlSerializer(typeof(PersistedSimulation));

            public void ToggleSaveFunctionalityAvailable()
            {
                _onSaveFunctionalityAvailableAction(true);
            }

            public PersistedSimulation LoadFile(string fileName)
            {
                _onSaveFunctionalityAvailableAction(true);
                _lastSaveFileName = fileName;
                using (var file = File.OpenRead(fileName))
                    return _xmlSerializer.Deserialize(file) as PersistedSimulation;
            }

            public bool CurrentFilenameIsKnown { get { return _lastSaveFileName != null; } }

            public void Save(PersistedSimulation simulation)
            {
                if (!CurrentFilenameIsKnown) throw new InvalidOperationException();
                SaveFile(_lastSaveFileName, simulation);
            }

            public void SaveFile(string fileName, PersistedSimulation simulation)
            {
                using (var file = File.Open(fileName, FileMode.Create, FileAccess.Write))
                    _xmlSerializer.Serialize(file, simulation);
                _lastSaveFileName = fileName;
            }
        }
    }
}
