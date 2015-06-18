using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization.Tilesets
{
    internal class InfiniteEnumeratorCycler
    {
        private DateTime _lastCycle = DateTime.Now;

        private readonly List<IEnumerator> _enumerators = new List<IEnumerator>();

        public void LoadEnumerator(IEnumerator enumerator)
        {
            _enumerators.Add(enumerator);
        }

        public void CheckCycle()
        {
            if (DateTime.Now.AddMilliseconds(-100) > _lastCycle)
            {
                _lastCycle = DateTime.Now;
                foreach (var enumerator in _enumerators)
                    enumerator.MoveNext();
            }
        }
    }

    internal class AnimatedRoadNetworkZoneTileset : IRoadNetworkZoneTileset
    {
        private readonly InfiniteEnumeratorCycler _cycler;
        private readonly IEnumerator<NetworkZoneTileset> _networkTilesetEnumerator;
        private readonly IEnumerator<Bitmap> _powerNorthSouthRoadEastWestFrameEnumerator;
        private readonly IEnumerator<Bitmap> _railNorthSouthRoadEastWestFrameEnumerator;
        private readonly IEnumerator<Bitmap> _waterNorthSouthRoadEastWestFrameEnumerator;

        private readonly IEnumerator<Bitmap> _roadNorthSouthPowerEastWestFrameEnumerator;
        private readonly IEnumerator<Bitmap> _roadNorthSouthRailEastWestFrameEnumerator;
        private readonly IEnumerator<Bitmap> _roadNorthSouthWaterEastWestFrameEnumerator;

        public AnimatedRoadNetworkZoneTileset(
            IEnumerable<NetworkZoneTileset> frameTileSets, 
            ICollection<Bitmap> powerNorthSouthRoadEastWestFrames,
            ICollection<Bitmap> railNorthSouthRoadEastWestFrames,
            ICollection<Bitmap> waterNorthSouthRoadEastWestFrames,
            InfiniteEnumeratorCycler cycler
        )
        {
            _cycler = cycler;
            _powerNorthSouthRoadEastWestFrameEnumerator = powerNorthSouthRoadEastWestFrames
                .GetInfiniteEnumerator();
            _railNorthSouthRoadEastWestFrameEnumerator = railNorthSouthRoadEastWestFrames
                .GetInfiniteEnumerator();
            _waterNorthSouthRoadEastWestFrameEnumerator = waterNorthSouthRoadEastWestFrames
                .GetInfiniteEnumerator();

            _roadNorthSouthPowerEastWestFrameEnumerator =
                powerNorthSouthRoadEastWestFrames
                .Select(x => x.Get90DegreesRotatedClone())
                .GetInfiniteEnumerator();

            _roadNorthSouthRailEastWestFrameEnumerator =
                railNorthSouthRoadEastWestFrames
                .Select(x => x.Get90DegreesRotatedClone())
                .GetInfiniteEnumerator();

            _roadNorthSouthWaterEastWestFrameEnumerator =
                waterNorthSouthRoadEastWestFrames
                .Select(x => x.Get90DegreesRotatedClone())
                .GetInfiniteEnumerator();

            _networkTilesetEnumerator = frameTileSets.GetInfiniteEnumerator();

            foreach (var x in GetEnumerators())
            {
                cycler.LoadEnumerator(x);
                x.MoveNext();
            }

            _networkTilesetEnumerator.MoveNext();
        }

        private DateTime _lastCycle = DateTime.Now;

        private IEnumerable<IEnumerator> GetEnumerators()
        {
            yield return _networkTilesetEnumerator;
            yield return _powerNorthSouthRoadEastWestFrameEnumerator;
            yield return _railNorthSouthRoadEastWestFrameEnumerator;
            yield return _roadNorthSouthPowerEastWestFrameEnumerator;
            yield return _roadNorthSouthRailEastWestFrameEnumerator;
            yield return _waterNorthSouthRoadEastWestFrameEnumerator;
            yield return _roadNorthSouthWaterEastWestFrameEnumerator;
        }

        public Bitmap GetBitmapFor(IIntersectingZoneConsumption intersection)
        {
            if (intersection.NorthSouthZoneConsumption is RoadZoneConsumption)
            {
                if (intersection.EastWestZoneConsumption is RailRoadZoneConsumption)
                    return _roadNorthSouthRailEastWestFrameEnumerator.Current;
                else if (intersection.EastWestZoneConsumption is PowerLineConsumption)
                    return _roadNorthSouthPowerEastWestFrameEnumerator.Current;
                else if (intersection.EastWestZoneConsumption is WaterZoneConsumption)
                    return _roadNorthSouthWaterEastWestFrameEnumerator.Current;
                else
                    throw new ArgumentException("Could not map the specified intersection to a frame.", "intersection");
            }
            else if (intersection.EastWestZoneConsumption is RoadZoneConsumption)
            {
                if (intersection.NorthSouthZoneConsumption is RailRoadZoneConsumption)
                    return _railNorthSouthRoadEastWestFrameEnumerator.Current;
                else if (intersection.NorthSouthZoneConsumption is PowerLineConsumption)
                    return _powerNorthSouthRoadEastWestFrameEnumerator.Current;
                else if (intersection.NorthSouthZoneConsumption is WaterZoneConsumption)
                    return _waterNorthSouthRoadEastWestFrameEnumerator.Current;
                else
                    throw new ArgumentException("Could not map the specified intersection to a frame.", "intersection");
            }
            else
                throw new ArgumentException("Could not map the specified intersection to a frame.", "intersection");
        }

        public Bitmap GetBitmapFor(BaseNetworkZoneConsumption baseNetworkZoneConsumption)
        {
            _cycler.CheckCycle();

            return _networkTilesetEnumerator.Current.GetBitmapFor(baseNetworkZoneConsumption);
        }
    }

    public static class EnumerableExtensions
    {
        public static IEnumerator<T> GetInfiniteEnumerator<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.GetInfiniteEnumerable().GetEnumerator();
        }

        private static IEnumerable<T> GetInfiniteEnumerable<T>(this IEnumerable<T> enumerable)
        {
            var enumerableArray = enumerable.ToArray();
            while (true)
            {
                foreach (var x in enumerableArray)
                    yield return x;
            }
        }
    }
}