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
        public AnimatedCellBitmapSetLayers Up { get; }
        public AnimatedCellBitmapSetLayers Down { get; }
        public AnimatedCellBitmapSetLayers Right { get; }
        public AnimatedCellBitmapSetLayers Left { get; }

        public DirectionalCellBitmap(AnimatedCellBitmapSetLayers cellBitmapSet)
        {
            if (cellBitmapSet == null) throw new ArgumentNullException(nameof(cellBitmapSet));
            Up = cellBitmapSet;
            Right = Up.Generate90DegreesRotatedClone();
            Down = Right.Generate90DegreesRotatedClone();
            Left = Down.Generate90DegreesRotatedClone();
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

        public AnimatedCellBitmapSetLayers GetForDirections(bool north, bool east, bool south, bool west)
        {
            var specificationCount = new[] { north, east, south, west }.Count(x => x);

            switch (specificationCount)
            {
                case 0:
                    return Center.Right;
                case 1:
                    if (east)
                        return East.Up;
                    else if (south)
                        return East.Right;
                    else if (west)
                        return East.Down;
                    else if (north)
                        return East.Left;
                    else
                        throw new InvalidOperationException();
                case 2:
                    if (east && west)
                        return EastWest.Up;
                    else if (north && south)
                        return EastWest.Right;
                    else if (west && north)
                        return NorthWest.Up;
                    else if (north && east)
                        return NorthWest.Right;
                    else if (east && south)
                        return NorthWest.Down;
                    else if (south && west)
                        return NorthWest.Left;
                    else
                        throw new InvalidOperationException();
                case 3:
                    if (west && north && east)
                        return NorthWestEast.Up;
                    else if (north && east && south)
                        return NorthWestEast.Right;
                    else if (east && south && west)
                        return NorthWestEast.Down;
                    else if (south && west && north)
                        return NorthWestEast.Left;
                    else
                        throw new InvalidOperationException();
                case 4:
                    return NorthWestEastSouth.Up;
                default:
                    throw new InvalidOperationException();
            }
        }

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
