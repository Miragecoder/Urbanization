using System;
using System.Drawing;
using System.IO;
using System.Threading;
using SixLabors.ImageSharp;

namespace Mirage.Urbanization.Tilesets
{
    public abstract class BaseBitmap
    {
        public Image Bitmap { get; }

        public byte[] PngBytes => _getPngBytesLazy.Value;

        private readonly Lazy<byte[]> _getPngBytesLazy; 

        protected BaseBitmap(Image bitmap)
        {
            if (bitmap == null) throw new ArgumentNullException(nameof(bitmap));
            Bitmap = bitmap;

            _getPngBytesLazy = new Lazy<byte[]>(() =>
            {
                using var ms = new MemoryStream();
                Bitmap.SaveAsPng(ms);
                return ms.ToArray();
            });
            var x = _getPngBytesLazy.Value;
        }
    }
}