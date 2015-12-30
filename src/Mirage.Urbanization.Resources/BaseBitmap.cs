using System;
using System.Drawing;
using System.IO;
using System.Threading;

namespace Mirage.Urbanization.Tilesets
{
    public abstract class BaseBitmap
    {
        public Bitmap Bitmap { get; }

        public byte[] PngBytes => _getPngBytesLazy.Value;

        private readonly Lazy<byte[]> _getPngBytesLazy; 

        protected BaseBitmap(Bitmap bitmap)
        {
            if (bitmap == null) throw new ArgumentNullException(nameof(bitmap));
            Bitmap = bitmap;

            _getPngBytesLazy = new Lazy<byte[]>(() =>
            {
                using (var ms = new MemoryStream())
                {
                    Bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return ms.ToArray();
                }
            });
            var x = _getPngBytesLazy.Value;
        }
    }
}