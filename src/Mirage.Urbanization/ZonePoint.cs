using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Mirage.Urbanization
{
    [DebuggerDisplay("ZonePoint (X: {X}, Y: {Y})")]
    public struct ZonePoint : IEquatable<ZonePoint>
    {
        public int X { get; set; }
        public int Y { get; set; }

        public override bool Equals(object obj)
        {
            return obj is ZonePoint && this == (ZonePoint) obj;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public static bool operator ==(ZonePoint x, ZonePoint y)
        {
            return x.X == y.X && x.Y == y.Y;
        }

        public static bool operator !=(ZonePoint x, ZonePoint y)
        {
            return x.X != y.X || x.Y != y.Y;
        }

        public bool Equals(ZonePoint other)
        {
            return X == other.X && Y == other.Y;
        }
    }
}
