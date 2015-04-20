using System;
using System.Collections.Generic;
using System.Drawing;

namespace Mirage.Urbanization.Tilesets
{
    internal class AnimatedBitmap
    {
        private Bitmap _currentBitmap;
        private DateTime _lastFrameSkip = DateTime.Now;
        private readonly TimeSpan _frameLifeSpan;
        private readonly IEnumerator<Bitmap> _bitmapEnumerator;

        internal AnimatedBitmap(int milliseconds, params Bitmap[] frames)
        {
            _frameLifeSpan = new TimeSpan(0, 0, 0, 0, milliseconds);
            _bitmapEnumerator = EnumerateBitmapsInfinity(frames).GetEnumerator();
            CycleNextFrame();
        }

        public Bitmap GetCurrentBitmapFrame()
        {
            if (DateTime.Now - _frameLifeSpan > _lastFrameSkip)
                CycleNextFrame();
            return _currentBitmap;
        }

        private void CycleNextFrame()
        {
            _bitmapEnumerator.MoveNext();
            _currentBitmap = _bitmapEnumerator.Current;
            _lastFrameSkip = DateTime.Now;
        }

        private static IEnumerable<Bitmap> EnumerateBitmapsInfinity(Bitmap[] frames)
        {
            while (true)
                foreach (var bitmap in frames)
                    yield return bitmap;
        }
    }
}