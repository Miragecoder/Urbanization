using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Mirage.Urbanization.WinForms.Rendering.SharpDx;

namespace Mirage.Urbanization.WinForms.Rendering.BufferedGraphics
{
    public class BufferedGraphicsManagerWrapper : BaseGraphicsManagerWrapper
    {
        private readonly IGraphicsWrapper _wrapper;
        private readonly System.Drawing.BufferedGraphics _bufferedGraphics;

        public BufferedGraphicsManagerWrapper(Panel panel, Action renderAction)
            : base(renderAction)
        {
            _bufferedGraphics = BufferedGraphicsManager.Current.Allocate(panel.CreateGraphics(), panel.DisplayRectangle);
            _wrapper = new BufferedGraphicsWrapper(_bufferedGraphics);
        }

        public override IGraphicsWrapper GetGraphicsWrapper()
        {
            return _wrapper;
        }

        public override void Dispose()
        {
            base.Dispose();
            _bufferedGraphics.Dispose();
        }

        private class BufferedGraphicsWrapper : IGraphicsWrapper
        {
            private readonly System.Drawing.BufferedGraphics _bufferedGraphics;

            public BufferedGraphicsWrapper(System.Drawing.BufferedGraphics bufferedGraphics)
            {
                _bufferedGraphics = bufferedGraphics;
            }

            public void DrawImage(Bitmap bitmap, Rectangle rectangle)
            {
                _bufferedGraphics.Graphics.DrawImage(bitmap, rectangle);
            }

            public void FillRectangle(SolidBrush brush, Rectangle rectangle)
            {
                _bufferedGraphics.Graphics.FillRectangle(brush, rectangle);
            }

            public void DrawString(string s, Font font, SolidBrush brush, RectangleF layoutRectangle)
            {
                _bufferedGraphics.Graphics.DrawString(s, font, brush, layoutRectangle);
            }

            public void DrawRectangle(Pen pen, Rectangle rectangle)
            {
                _bufferedGraphics.Graphics.DrawRectangle(pen, rectangle);
            }
        }

        public override void InitializeRenderAction()
        {

        }

        public override void EndRenderAction()
        {
            _bufferedGraphics.Render();
        }
    }
}