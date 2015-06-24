using System;
using System.Windows.Forms;
using Mirage.Urbanization.WinForms.Overlay;
using Mirage.Urbanization.WinForms.Rendering;

namespace Mirage.Urbanization.WinForms
{
    public class RenderZoneOptions
    {
        private readonly Func<bool> _renderDebugPollutionValues;
        private readonly Func<bool> _renderDebugLandValueValues;
        private readonly Func<bool> _showDebugGrowthPathFinding;
        private readonly Func<bool> _showDebugAverageTravelDistances;
        private readonly Func<GraphicsManagerWrapperOption> _selectedGraphicsManagerFunc;
        private readonly Func<OverlayOption> _getCurrentOverlayOptionFunc;

        public OverlayOption CurrentOverlayOption { get { return _getCurrentOverlayOptionFunc(); } }
        public bool RenderDebugLandValueValues { get { return _renderDebugLandValueValues(); } }
        public bool RenderDebugPollutionValues { get { return _renderDebugPollutionValues(); } }
        public bool ShowDebugGrowthPathFinding { get { return _showDebugGrowthPathFinding(); } }
        public bool ShowDebugAverageTravelDistances { get { return _showDebugAverageTravelDistances(); } }
        public GraphicsManagerWrapperOption SelectedGraphicsManager { get { return _selectedGraphicsManagerFunc(); } }

        public RenderZoneOptions(
            Func<bool> renderDebugPollutionValues, 
            Func<bool> renderDebugLandValueValues,
            Func<bool> showDebugGrowthPathFinding,
            Func<bool> showDebugAverageTravelDistances,
            Func<GraphicsManagerWrapperOption> selectedGraphicsManagerFunc,
            Func<OverlayOption> getCurrentOverlayOptionFunc 
            )
        {
            _renderDebugPollutionValues = renderDebugPollutionValues;
            _renderDebugLandValueValues = renderDebugLandValueValues;
            _selectedGraphicsManagerFunc = selectedGraphicsManagerFunc;
            _getCurrentOverlayOptionFunc = getCurrentOverlayOptionFunc;
            _showDebugGrowthPathFinding = showDebugGrowthPathFinding;
            _showDebugAverageTravelDistances = showDebugAverageTravelDistances;
        }
    }
}