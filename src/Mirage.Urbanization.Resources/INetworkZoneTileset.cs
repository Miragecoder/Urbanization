using System.Drawing;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization.Tilesets
{
    public interface INetworkZoneTileset
    {
        Bitmap GetBitmapFor(BaseNetworkZoneConsumption intersection);
    }

    public interface IRoadNetworkZoneTileset : INetworkZoneTileset
    {
        Bitmap GetBitmapFor(IIntersectingZoneConsumption naseNetworkZoneConsumption);
    }
}