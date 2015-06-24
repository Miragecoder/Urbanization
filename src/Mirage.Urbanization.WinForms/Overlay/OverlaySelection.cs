using System.Windows.Forms;

namespace Mirage.Urbanization.WinForms.Overlay
{
    public class OverlaySelection : ToolstripMenuInitializer<OverlayOption>
    {
        public OverlaySelection(ToolStripMenuItem targetToolstripMenuItem)
            : base(targetToolstripMenuItem, OverlayOption.OverlayOptions)
        {

        }
    }
}