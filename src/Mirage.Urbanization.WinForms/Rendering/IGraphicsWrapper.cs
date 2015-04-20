using System.Drawing;

namespace Mirage.Urbanization.WinForms.Rendering
{
    public interface IGraphicsWrapper
    {
        void DrawImage(Bitmap bitmap, Rectangle rectangle);
        void FillRectangle(SolidBrush brush, Rectangle rectangle);

        void DrawString(string s, Font font, SolidBrush brush, RectangleF layoutRectangle);
        void DrawRectangle(Pen pen, Rectangle rectangle);
    }
}