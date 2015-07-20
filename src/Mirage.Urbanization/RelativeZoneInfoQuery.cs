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

        public int Distance => Math.Abs(RelativeX) + Math.Abs(RelativeY);

        public int RelativeY => _relativeY;
        public int RelativeX => _relativeX;
    }
}