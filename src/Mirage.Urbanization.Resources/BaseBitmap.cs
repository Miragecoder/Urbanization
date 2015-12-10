using System;
using System.Drawing;
using System.IO;
using System.Threading;

namespace Mirage.Urbanization.Tilesets
{
    public abstract class BaseBitmap
    {
        public Bitmap Bitmap { get; }
        private static int _idCounter = default(int);

        public byte[] PngBytes => _getPngBytesLazy.Value;

        private readonly Lazy<byte[]> _getPngBytesLazy; 

        public int Id { get; }

        protected BaseBitmap(Bitmap bitmap)
        {
            if (bitmap == null) throw new ArgumentNullException(nameof(bitmap));
            Bitmap = bitmap;
            Id = Interlocked.Increment(ref _idCounter);

            _getPngBytesLazy = new Lazy<byte[]>(() =>
            {
                using (var ms = new MemoryStream())
                {
                    Bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return ms.ToArray();
                }
            });
        }
    }
}