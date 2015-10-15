using System;

namespace Mirage.Urbanization
{
    public class ZoneInfoEventArgs : EventArgs
    {
        public IReadOnlyZoneInfo ZoneInfo { get; }

        public ZoneInfoEventArgs(IReadOnlyZoneInfo zoneInfo)
        {
            ZoneInfo = zoneInfo;
        }
    }
}