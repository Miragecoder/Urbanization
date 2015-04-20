using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization.WinForms
{
    public class ZoneSelectionPanelCreator
    {
        private readonly Panel _targetPanel;
        private readonly IReadOnlyArea _area;

        private readonly IDictionary<Button, Func<IAreaConsumption>> _buttonsAndFactories;
        private Func<IAreaConsumption> _currentFactory;

        public ZoneSelectionPanelCreator(IReadOnlyArea area, Panel targetPanel)
        {
            if (targetPanel == null) throw new ArgumentNullException("targetPanel");
            if (area == null) throw new ArgumentNullException("area");

            _targetPanel = targetPanel;

            _area = area;

            EventHandler currentClickHandler = null;

            _buttonsAndFactories = _area.GetSupportedZoneConsumptionFactories().Reverse()
                .Select(factory =>
                {
                    var sample = factory();

                    var button = new Button
                    {
                        Text = sample.Name,
                        Dock = DockStyle.Top,
                        BackColor = SystemColors.Control
                    };
                    button.Parent = _targetPanel;

                    currentClickHandler = (sender, e) =>
                    {
                        foreach (var btn in _buttonsAndFactories.Keys.Where(btn => btn != button))
                            btn.Enabled = true;
                        button.Enabled = false;
                        _currentFactory = factory;
                        _currentZoneConsumptionSample = _currentFactory();
                    };

                    button.Click += currentClickHandler;


                    return new KeyValuePair<Button, Func<IAreaConsumption>>(button, factory);
                })
                .ToDictionary(x => x.Key, x => x.Value);

            currentClickHandler(this, new EventArgs());

            if (_currentFactory == null) throw new InvalidOperationException();
        }

        private IAreaConsumption _currentZoneConsumptionSample;

        public IAreaConsumption CurrentZoneConsumptionSample { get { return _currentZoneConsumptionSample; } }

        public bool IsCurrentlyNetworkZoning
        {
            get { return _currentZoneConsumptionSample is BaseInfrastructureNetworkZoneConsumption; }
        }

        public IAreaConsumption CreateNewCurrentZoneConsumption()
        {
            return _currentFactory();
        }
    }
}