using System;
using System.Collections.Generic;
using System.Linq;

namespace Mirage.Urbanization.Tilesets
{
    public class SynchronizedAnimationFramePicker
    {
        public CellBitmap GetFrame(AnimatedCellBitmapSet bitmapSet) => GetFrame(bitmapSet.Bitmaps, bitmapSet.Delay);
        private T GetFrame<T>(T[] frames, int delay)
        {
            if (!_set.Any(x => x.Delay == delay && x.FrameCount == frames.Length))
            {
                _set.Add(new AnimationFrame(frames.Length, delay));
            }
            return _set
                .Single(x => x.Delay == delay && x.FrameCount == frames.Length)
                .Current
                .Pipe(x => frames[x]);
        }

        private readonly IList<AnimationFrame> _set = new List<AnimationFrame>(); 

        private class AnimationFrame
        {
            public AnimationFrame(int frameCount, int delay)
            {
                FrameCount = frameCount;
                Delay = delay;
            }
            public int FrameCount { get; }
            public int MaxIndex => FrameCount - 1;
            public int Delay { get; }
            private DateTime _lastFrameSkip;
            private int _current;
            public int Current
            {
                get
                {
                    if (DateTime.Now.AddMilliseconds(-Delay) > _lastFrameSkip)
                    {
                        if (MaxIndex <= _current)
                            _current = 0;
                        else
                            _current++;
                        _lastFrameSkip = DateTime.Now;
                    }
                    return _current;
                }
            }
        }
    }
}