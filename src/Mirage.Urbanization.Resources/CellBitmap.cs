using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

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

    public class VehicleBitmap : BaseBitmap
    {
        public VehicleBitmap(Bitmap bitmap) : base(bitmap) { }
    }

    public class DirectionalCellBitmap
    {
        public AnimatedCellBitmapSetLayers North { get; }
        public AnimatedCellBitmapSetLayers South { get; }
        public AnimatedCellBitmapSetLayers East { get; }
        public AnimatedCellBitmapSetLayers West { get; }

        public DirectionalCellBitmap(AnimatedCellBitmapSetLayers cellBitmapSet)
        {
            if (cellBitmapSet == null) throw new ArgumentNullException(nameof(cellBitmapSet));
            North = cellBitmapSet;
            East = North.Generate90DegreesRotatedClone();
            South = East.Generate90DegreesRotatedClone();
            West = South.Generate90DegreesRotatedClone();
        }
    }

    public class CellBitmap : BaseBitmap
    {
        public CellBitmap(Bitmap bitmap) : base(bitmap)
        {

        }
    }

    public class AnimatedCellBitmapSet
    {
        public int Delay { get; }
        public CellBitmap[] Bitmaps { get; }

        public Bitmap Current
        {
            get
            {
                if (DateTime.Now.AddMilliseconds(-Delay) > _lastFrameSkip)
                {
                    _bitmapEnumerator.MoveNext();
                    _lastFrameSkip = DateTime.Now;
                }
                return _bitmapEnumerator.Current.Bitmap;
            }
        }

        private readonly Lazy<AnimatedCellBitmapSet> _rotatedCloneLazy; 

        public AnimatedCellBitmapSet(int delay, params CellBitmap[] bitmaps)
        {
            Delay = delay;
            Bitmaps = bitmaps;
            _rotatedCloneLazy = new Lazy<AnimatedCellBitmapSet>(() => new AnimatedCellBitmapSet(
                Delay,
                Bitmaps.Select(x => x.Bitmap.Get90DegreesRotatedClone()).Select(x => new CellBitmap(x)).ToArray()
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

    public class AnimatedCellBitmapSetLayers
    {
        public AnimatedCellBitmapSet LayerOne { get; }
        public QueryResult<AnimatedCellBitmapSet> LayerTwo { get; }

        private readonly Lazy<AnimatedCellBitmapSetLayers> _rotatedCloneLazy;
        
        public AnimatedCellBitmapSetLayers(
            AnimatedCellBitmapSet layerOne, 
            AnimatedCellBitmapSet layerTwo
        )
        {
            LayerOne = layerOne;
            LayerTwo = layerTwo.ToQueryResult();

            _rotatedCloneLazy = new Lazy<AnimatedCellBitmapSetLayers>(() =>
                new AnimatedCellBitmapSetLayers(LayerOne.Generate90DegreesRotatedClone(), 
                LayerTwo.WithResultIfHasMatch(x => x.Generate90DegreesRotatedClone())
            ));
        }

        public AnimatedCellBitmapSetLayers Generate90DegreesRotatedClone()
        {
            return _rotatedCloneLazy.Value;
        }
    }

    public class CellBitmapCluster
    {
        public IDictionary<Point, AnimatedCellBitmapSetLayers> Bitmaps { get; }

        public CellBitmapCluster(IDictionary<Point, AnimatedCellBitmapSetLayers> bitmaps)
        {
            Bitmaps = bitmaps;
        }
    }


    public class CellBitmapNetwork
    {
        public DirectionalCellBitmap Center { get; set; }
        public DirectionalCellBitmap East { get; set; }
        public DirectionalCellBitmap EastWest { get; set; }
        public DirectionalCellBitmap NorthWest { get; set; }
        public DirectionalCellBitmap NorthWestEast { get; set; }
        public DirectionalCellBitmap NorthWestEastSouth { get; set; }

        public CellBitmapNetwork(
            DirectionalCellBitmap center,
            DirectionalCellBitmap east,
            DirectionalCellBitmap eastWest,
            DirectionalCellBitmap northWest,
            DirectionalCellBitmap northWestEast,
            DirectionalCellBitmap northWestEastSouth)
        {
            Center = center;
            East = east;
            EastWest = eastWest;
            NorthWest = northWest;
            NorthWestEast = northWestEast;
            NorthWestEastSouth = northWestEastSouth;
        }
    }
}
