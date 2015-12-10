using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace Mirage.Urbanization.Tilesets
{
    public class FramerateDelay
    {
        public static FramerateDelay TrafficFramerate = new FramerateDelay(60);
        public static FramerateDelay None = new FramerateDelay(60);

        public static FramerateDelay Structure = new FramerateDelay(200);

        private FramerateDelay(int delay)
        {
            Delay = delay;
        }

        public int Delay { get; }
    }

    public class AnimatedCellBitmapSet
    {
        public int Delay { get; }
        public CellBitmap[] Bitmaps { get; }

        private readonly Lazy<AnimatedCellBitmapSet> _rotatedCloneLazy;
        private static int _idCounter = 0;
        public int Id { get; }
        public AnimatedCellBitmapSet(FramerateDelay delay, params CellBitmap[] bitmaps)
        {
            Id = Interlocked.Increment(ref _idCounter);
            Delay = delay.Delay;
            Bitmaps = bitmaps;
            _rotatedCloneLazy = new Lazy<AnimatedCellBitmapSet>(() => new AnimatedCellBitmapSet(
                delay,
                Bitmaps
                .Select(x => BitmapExtensions.Get90DegreesRotatedClone(x.Bitmap))
                .Select(x => new CellBitmap(x)).ToArray()
                ));
            _bitmapEnumerator = Bitmaps.GetInifiniteEnumerator();
            _bitmapEnumerator.MoveNext();
        }

        private DateTime _lastFrameSkip = DateTime.Now;

        private readonly IEnumerator<CellBitmap> _bitmapEnumerator;

        public AnimatedCellBitmapSet Generate90DegreesRotatedClone()
        {
            return _rotatedCloneLazy.Value;
        }
    }
}