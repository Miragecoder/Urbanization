using System.Drawing;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization.Tilesets
{
    public interface INetworkZoneTileset
    {
        BitmapInfo GetBitmapInfoFor(BaseNetworkZoneConsumption intersection);
    }

    public interface IRoadNetworkZoneTileset : INetworkZoneTileset
    {
        BitmapInfo GetBitmapInfoFor(IIntersectingZoneConsumption intersection);
    }
}