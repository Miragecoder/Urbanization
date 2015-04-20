using System;
using System.Windows.Forms;
using Mirage.Urbanization.WinForms.Rendering;

namespace Mirage.Urbanization.WinForms
{
    public class RenderZoneOptions
    {
        private readonly Func<bool> _renderPollutionValues, _renderCrimeValues;
        private readonly Func<bool> _renderLandValueValues;
        private readonly Func<bool> _showTrafficStatistics, _showGrowthPathFinding, _showPopulationDensity;
        private readonly Func<bool> _showAverageTravelDistances;
        private readonly Func<GraphicsManagerWrapperOption> _selectedGraphicsManagerFunc;

        public bool ShowTrafficStatistics { get { return _showTrafficStatistics(); } }
        public bool RenderLandValueValues { get { return _renderLandValueValues(); } }
        public bool RenderCrimeValues { get { return _renderCrimeValues(); } }
        public bool RenderPollutionValues { get { return _renderPollutionValues(); } }
        public bool ShowGrowthPathFinding { get { return _showGrowthPathFinding(); } }
        public bool ShowAverageTravelDistances { get { return _showAverageTravelDistances(); } }
        public bool ShowPopulationDensity { get { return _showPopulationDensity(); } }
        public GraphicsManagerWrapperOption SelectedGraphicsManager { get { return _selectedGraphicsManagerFunc(); } }

        public RenderZoneOptions(
            Func<bool> renderPollutionValues, 
            Func<bool> renderCrimeValues, 
            Func<bool> renderLandValueValues,
            Func<bool> showTrafficStatistics, 
            Func<bool> showGrowthPathFinding,
            Func<bool> showAverageTravelDistances,
            Func<bool> showPopulationDensity,
            Func<GraphicsManagerWrapperOption> selectedGraphicsManagerFunc
            )
        {
            _renderPollutionValues = renderPollutionValues;
            _renderCrimeValues = renderCrimeValues;
            _renderLandValueValues = renderLandValueValues;
            _showTrafficStatistics = showTrafficStatistics;
            _selectedGraphicsManagerFunc = selectedGraphicsManagerFunc;
            _showPopulationDensity = showPopulationDensity;
            _showGrowthPathFinding = showGrowthPathFinding;
            _showAverageTravelDistances = showAverageTravelDistances;
        }
    }
}