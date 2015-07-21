using System;

namespace Mirage.Urbanization
{
    public class RelativeZoneInfoQuery
    {
        public RelativeZoneInfoQuery(int relativeX, int relativeY)
        {
            RelativeX = relativeX;
            RelativeY = relativeY;
        }

        public int Distance => Math.Abs(RelativeX) + Math.Abs(RelativeY);

        public int RelativeY { get; }

        public int RelativeX { get; }
    }
}