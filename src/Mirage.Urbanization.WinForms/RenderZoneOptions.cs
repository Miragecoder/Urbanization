using System;
using System.Windows.Forms;
using Mirage.Urbanization.WinForms.Overlay;
using Mirage.Urbanization.WinForms.Rendering;

namespace Mirage.Urbanization.WinForms
{
    public class RenderZoneOptions
    {
        private readonly Func<bool> _showDebugGrowthPathFinding;
        private readonly Func<GraphicsManagerWrapperOption> _selectedGraphicsManagerFunc;
        private readonly Func<OverlayOption> _getCurrentOverlayOptionFunc;

        public OverlayOption CurrentOverlayOption => _getCurrentOverlayOptionFunc();
        public bool ShowDebugGrowthPathFinding => _showDebugGrowthPathFinding();
        public GraphicsManagerWrapperOption SelectedGraphicsManager => _selectedGraphicsManagerFunc();

        public RenderZoneOptions(
            Func<bool> showDebugGrowthPathFinding,
            Func<GraphicsManagerWrapperOption> selectedGraphicsManagerFunc,
            Func<OverlayOption> getCurrentOverlayOptionFunc 
            )
        {
            _selectedGraphicsManagerFunc = selectedGraphicsManagerFunc;
            _getCurrentOverlayOptionFunc = getCurrentOverlayOptionFunc;
            _showDebugGrowthPathFinding = showDebugGrowthPathFinding;
        }
    }
}