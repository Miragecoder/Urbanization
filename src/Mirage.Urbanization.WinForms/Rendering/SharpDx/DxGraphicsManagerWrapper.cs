using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.WIC;
using SharpDX.Windows;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Bitmap = SharpDX.WIC.Bitmap;
using BitmapInterpolationMode = SharpDX.Direct2D1.BitmapInterpolationMode;
using Factory = SharpDX.Direct2D1.Factory;
using Font = System.Drawing.Font;
using PixelFormat = SharpDX.Direct2D1.PixelFormat;
using Rectangle = System.Drawing.Rectangle;
using RectangleF = SharpDX.RectangleF;

namespace Mirage.Urbanization.WinForms.Rendering.SharpDx
{
    public class SharpDxGraphicsManagerWrapper : BaseGraphicsManagerWrapper
    {
        private readonly DxGraphicsWrapper _wrapper;
        private readonly WindowRenderTarget _renderTarget;

        public SharpDxGraphicsManagerWrapper(Panel targetPanel, Action renderAction)
            : base(renderAction)
        {
            var Factory2D = new SharpDX.Direct2D1.Factory();
            var FactoryDWrite = new SharpDX.DirectWrite.Factory();

            var properties = new HwndRenderTargetProperties();
            properties.Hwnd = targetPanel.Handle;
            properties.PixelSize = new SharpDX.Size2(targetPanel.Width, targetPanel.Height);
            properties.PresentOptions = PresentOptions.None;

            _renderTarget = new WindowRenderTarget(Factory2D, new RenderTargetProperties(new PixelFormat(Format.Unknown, AlphaMode.Premultiplied)), properties);

            _renderTarget.AntialiasMode = AntialiasMode.PerPrimitive;

            _wrapper = new DxGraphicsWrapper(_renderTarget, FactoryDWrite, new ImagingFactory());
        }

        public override IGraphicsWrapper GetGraphicsWrapper()
        {
            return _wrapper;
        }

        public override void EndRenderAction()
        {
            _renderTarget.EndDraw();
        }

        public override void InitializeRenderAction()
        {
            _renderTarget.BeginDraw();
        }

        public override void Dispose()
        {
            base.Dispose();
            _renderTarget.Dispose();
        }

        private class DxGraphicsWrapper : IGraphicsWrapper
        {
            private readonly WindowRenderTarget _renderTarget2D;
            private readonly SharpDX.DirectWrite.Factory _factory2D;
            private readonly ImagingFactory _imagingFactory;
            private readonly ConverterAndCacher<System.Drawing.SolidBrush, SolidColorBrush> _solidColorBrushConverter;
            private readonly ConverterAndCacher<System.Drawing.Font, TextFormat> _textFormatConverterAndCacher;

            public DxGraphicsWrapper(WindowRenderTarget renderTarget2D, SharpDX.DirectWrite.Factory factory2D, ImagingFactory imagingFactory)
            {
                _renderTarget2D = renderTarget2D;
                _factory2D = factory2D;
                _imagingFactory = imagingFactory;

                _solidColorBrushConverter = new ConverterAndCacher<SolidBrush, SolidColorBrush>(
                    brush => new SolidColorBrush(_renderTarget2D, new Color4(
                    red: ((float)brush.Color.R) / byte.MaxValue,
                    green: ((float)brush.Color.G) / byte.MaxValue,
                    blue: ((float)brush.Color.B) / byte.MaxValue,
                    alpha: 1
                    )
                ));

                _textFormatConverterAndCacher = new ConverterAndCacher<Font, TextFormat>(
                    f => new TextFormat(_factory2D, f.FontFamily.Name, f.Size));
            }

            public void DrawImage(System.Drawing.Bitmap bitmap, System.Drawing.Rectangle rectangle)
            {
                _renderTarget2D.DrawBitmap(
                    bitmap: _bitmapHelper.LoadFromFile(_renderTarget2D, bitmap),
                    destinationRectangle: _rectangleConverter.Convert(rectangle),
                    opacity: 1.0f,
                    interpolationMode: BitmapInterpolationMode.Linear);
            }

            public void FillRectangle(System.Drawing.SolidBrush brush, System.Drawing.Rectangle rectangle)
            {
                _renderTarget2D.FillRectangle(
                    _rectangleConverter.Convert(rectangle),
                    _solidColorBrushConverter.Convert(brush)
                );
            }

            public void DrawString(string s, System.Drawing.Font font, System.Drawing.SolidBrush brush, System.Drawing.RectangleF layoutRectangle)
            {
                _renderTarget2D.DrawText(s, _textFormatConverterAndCacher.Convert(font),
                    _rectangleFConverter.Convert(layoutRectangle), _solidColorBrushConverter.Convert(brush));
            }

            public void DrawRectangle(System.Drawing.Pen pen, System.Drawing.Rectangle rectangle)
            {
                _renderTarget2D.DrawRectangle(
                    _rectangleConverter.Convert(rectangle),
                    new SolidColorBrush(_renderTarget2D,
                        new Color4(red: pen.Color.R, blue: pen.Color.B, green: pen.Color.G, alpha: pen.Color.A)
                    )
                );
            }

            private readonly ConverterAndCacher<System.Drawing.RectangleF, RectangleF> _rectangleFConverter
                = new ConverterAndCacher<System.Drawing.RectangleF, RectangleF>(rectangle => new RectangleF(
                    rectangle.X,
                    rectangle.Y,
                    rectangle.Width,
                    rectangle.Height
                )
            );

            private readonly ConverterAndCacher<System.Drawing.Rectangle, RectangleF> _rectangleConverter
                = new ConverterAndCacher<Rectangle, RectangleF>(rectangle => new RectangleF(
                    rectangle.X,
                    rectangle.Y,
                    rectangle.Width,
                    rectangle.Height
                )
            );

            private readonly BitmapHelper _bitmapHelper = new BitmapHelper();
        }

        private class BitmapHelper
        {
            private readonly ConcurrentDictionary<System.Drawing.Bitmap, SharpDX.Direct2D1.Bitmap> _cache = new ConcurrentDictionary<System.Drawing.Bitmap, SharpDX.Direct2D1.Bitmap>();

            public SharpDX.Direct2D1.Bitmap LoadFromFile(RenderTarget renderTarget, System.Drawing.Bitmap bitmap)
            {
                SharpDX.Direct2D1.Bitmap match;

                if (_cache.TryGetValue(bitmap, out match))
                {
                    return match;
                }

                var sourceArea = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
                var bitmapProperties = new BitmapProperties(new PixelFormat(Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied));
                var size = new Size2(bitmap.Width, bitmap.Height);

                // Transform pixels from BGRA to RGBA
                int stride = bitmap.Width * sizeof(int);
                using (var tempStream = new DataStream(bitmap.Height * stride, true, true))
                {
                    // Lock System.Drawing.Bitmap
                    var bitmapData = bitmap.LockBits(sourceArea, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                    // Convert all pixels 
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        int offset = bitmapData.Stride * y;
                        for (int x = 0; x < bitmap.Width; x++)
                        {
                            // Not optimized 
                            byte B = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte G = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte R = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            byte A = Marshal.ReadByte(bitmapData.Scan0, offset++);
                            int rgba = R | (G << 8) | (B << 16) | (A << 24);
                            tempStream.Write(rgba);
                        }

                    }
                    bitmap.UnlockBits(bitmapData);
                    tempStream.Position = 0;

                    match = new SharpDX.Direct2D1.Bitmap(renderTarget, size, tempStream, stride, bitmapProperties);

                    _cache.TryAdd(bitmap, match);
                }
                return match;
            }
        }
    }
}
