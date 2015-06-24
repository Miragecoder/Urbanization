using System;
using System.Windows.Forms;
using Mirage.Urbanization.WinForms.Rendering;

namespace Mirage.Urbanization.WinForms
{
    public class RenderZoneOptions
    {
        private readonly Func<bool> _renderDebugPollutionValues, _renderDebugCrimeValues;
        private readonly Func<bool> _renderDebugLandValueValues;
        private readonly Func<bool> _showDebugTrafficStatistics, _showDebugGrowthPathFinding, _showDebugPopulationDensity;
        private readonly Func<bool> _showDebugAverageTravelDistances;
        private readonly Func<GraphicsManagerWrapperOption> _selectedGraphicsManagerFunc;
        private readonly Func<OverlayOption> _getCurrentOverlayOptionFunc;

        public OverlayOption CurrentOverlayOption { get { return _getCurrentOverlayOptionFunc(); } }
        public bool ShowDebugTrafficStatistics { get { return _showDebugTrafficStatistics(); } }
        public bool RenderDebugLandValueValues { get { return _renderDebugLandValueValues(); } }
        public bool RenderDebugCrimeValues { get { return _renderDebugCrimeValues(); } }
        public bool RenderDebugPollutionValues { get { return _renderDebugPollutionValues(); } }
        public bool ShowDebugGrowthPathFinding { get { return _showDebugGrowthPathFinding(); } }
        public bool ShowDebugAverageTravelDistances { get { return _showDebugAverageTravelDistances(); } }
        public bool ShowDebugPopulationDensity { get { return _showDebugPopulationDensity(); } }
        public GraphicsManagerWrapperOption SelectedGraphicsManager { get { return _selectedGraphicsManagerFunc(); } }

        public RenderZoneOptions(
            Func<bool> renderDebugPollutionValues, 
            Func<bool> renderDebugCrimeValues, 
            Func<bool> renderDebugLandValueValues,
            Func<bool> showDebugTrafficStatistics, 
            Func<bool> showDebugGrowthPathFinding,
            Func<bool> showDebugAverageTravelDistances,
            Func<bool> showDebugPopulationDensity,
            Func<GraphicsManagerWrapperOption> selectedGraphicsManagerFunc,
            Func<OverlayOption> getCurrentOverlayOptionFunc 
            )
        {
            _renderDebugPollutionValues = renderDebugPollutionValues;
            _renderDebugCrimeValues = renderDebugCrimeValues;
            _renderDebugLandValueValues = renderDebugLandValueValues;
            _showDebugTrafficStatistics = showDebugTrafficStatistics;
            _selectedGraphicsManagerFunc = selectedGraphicsManagerFunc;
            _getCurrentOverlayOptionFunc = getCurrentOverlayOptionFunc;
            _showDebugPopulationDensity = showDebugPopulationDensity;
            _showDebugGrowthPathFinding = showDebugGrowthPathFinding;
            _showDebugAverageTravelDistances = showDebugAverageTravelDistances;
        }
    }
}