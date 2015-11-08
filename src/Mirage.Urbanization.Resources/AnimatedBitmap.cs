using System;
using System.Collections.Generic;
using System.Drawing;

namespace Mirage.Urbanization.Tilesets
{


    public class AnimatedBitmapFrame
    {
        public AnimatedBitmapFrame(AnimatedBitmap parent, Bitmap frame)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (frame == null) throw new ArgumentNullException(nameof(frame));
            Parent = parent;
            Frame = frame;
        }

        public AnimatedBitmap Parent { get; }
        public Bitmap Frame { get; }
    }

    public class AnimatedBitmap
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

        private Bitmap GetCurrentBitmapFramePrivate()
        {
            if (DateTime.Now - _frameLifeSpan > _lastFrameSkip)
                CycleNextFrame();
            return _currentBitmap;
        }

        public AnimatedBitmapFrame GetCurrentBitmapFrame()
            => new AnimatedBitmapFrame(this, GetCurrentBitmapFramePrivate());

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