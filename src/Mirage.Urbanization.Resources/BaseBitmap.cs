using System;
using System.Drawing;
using System.Threading;

namespace Mirage.Urbanization.Tilesets
{
    public abstract class BaseBitmap
    {
        public Bitmap Bitmap { get; }
        private static int _idCounter = default(int);

        public int Id { get; }

        protected BaseBitmap(Bitmap bitmap)
        {
            if (bitmap == null) throw new ArgumentNullException(nameof(bitmap));
            Bitmap = bitmap;
            Id = Interlocked.Increment(ref _idCounter);
        }
    }
}