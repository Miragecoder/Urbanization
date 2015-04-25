using System.Windows.Forms;

namespace Mirage.Urbanization.WinForms
{
    public static class ControlExtensions
    {
        public static T AddControlTo<T>(this T control, Control targetControl)
            where T : Control
        {
            targetControl.Controls.Add(control);
            return control;
        }
    }
}