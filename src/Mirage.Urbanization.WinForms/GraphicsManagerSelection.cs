using System.Windows.Forms;
using Mirage.Urbanization.WinForms.Rendering;

namespace Mirage.Urbanization.WinForms
{
    public class GraphicsManagerSelection : ToolstripMenuInitializer<GraphicsManagerWrapperOption>
    {
        public GraphicsManagerSelection(ToolStripMenuItem targetToopToolStripMenuItem)
            : base(targetToopToolStripMenuItem, GraphicsManagerWrapperOption.GetOptions())
        {

        }
    }
}