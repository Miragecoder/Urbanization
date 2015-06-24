using System;
using System.Windows.Forms;

namespace Mirage.Urbanization.WinForms.Overlay
{
    public class OverlaySelection : ToolstripMenuInitializer<OverlayOption>
    {
        public OverlaySelection(ToolStripMenuItem targetToolstripMenuItem, Func<bool> toggleShowNumbersFunc)
            : base(targetToolstripMenuItem, OverlayOption.CreateOverlayOptionInstances(toggleShowNumbersFunc))
        {

        }
    }
}