using System;
using System.Collections.Generic;
using System.Drawing;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization.Tilesets
{


    public class AnimatedBitmapFrame
    {
        public AnimatedBitmapFrame(AnimatedBitmap parent, SegmentableBitmap frame)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (frame == null) throw new ArgumentNullException(nameof(frame));
            ParentAnimatedBitmap = parent;
            ParentSegmentableBitmap = frame;
        }

        public AnimatedBitmap ParentAnimatedBitmap { get; }
        public SegmentableBitmap ParentSegmentableBitmap { get; }
    }

    public class AnimatedBitmap
    {
        private SegmentableBitmap _currentBitmap;
        private DateTime _lastFrameSkip = DateTime.Now;
        private readonly TimeSpan _frameLifeSpan;
        private readonly IEnumerator<SegmentableBitmap> _bitmapEnumerator;

        internal AnimatedBitmap(int milliseconds, params SegmentableBitmap[] frames)
        {
            _frameLifeSpan = new TimeSpan(0, 0, 0, 0, milliseconds);
            MilliSeconds = milliseconds;
            Frames = frames;
            _bitmapEnumerator = EnumerateBitmapsInfinity(frames).GetEnumerator();
            CycleNextFrame();
        }

        public int MilliSeconds { get; }
        public IEnumerable<SegmentableBitmap> Frames { get; }

        private SegmentableBitmap GetCurrentBitmapFramePrivate()
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

        private static IEnumerable<SegmentableBitmap> EnumerateBitmapsInfinity(SegmentableBitmap[] frames)
        {
            while (true)
                foreach (var bitmap in frames)
                    yield return bitmap;
        }

        public BitmapLayer GetBitmapLayer(ZoneClusterMemberConsumption member) => new BitmapLayer(GetBitmapInfoFor(member));

        public BitmapInfo GetBitmapInfoFor(ZoneClusterMemberConsumption member)
        {
            var current = this.GetCurrentBitmapFrame();
            
            return new BitmapInfo(
                bitmapSegment: current.ParentSegmentableBitmap.GetBitmapLayerFor(member).LayerOne.BitmapSegment, 
                parent: current.ParentSegmentableBitmap, 
                parentAnimatedBitmap: current.ParentAnimatedBitmap);
        }
    }
}