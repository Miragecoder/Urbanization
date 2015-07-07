using System;

namespace Mirage.Urbanization
{
    public class RelativeZoneInfoQuery
    {
        private readonly int _relativeX, _relativeY;

        public RelativeZoneInfoQuery(int relativeX, int relativeY)
        {
            _relativeX = relativeX;
            _relativeY = relativeY;
        }

        public int Distance
        {
            get { return Math.Abs(RelativeX) + Math.Abs(RelativeY); }
        }

        public int RelativeY { get { return _relativeY; } }
        public int RelativeX { get { return _relativeX; } }
    }
}