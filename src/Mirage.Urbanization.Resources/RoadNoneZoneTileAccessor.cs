using Mirage.Urbanization.ZoneConsumption;

namespace Mirage.Urbanization.Tilesets
{
    class RoadNoneZoneTileAccessor : BaseNetworkZoneTileAccessor<RoadZoneConsumption>
    {
        protected override string NetworkName => "RoadNone";
        protected override bool Filter(ZoneInfoSnapshot snapshot) => snapshot.TrafficDensity == TrafficDensity.None;
    }
}