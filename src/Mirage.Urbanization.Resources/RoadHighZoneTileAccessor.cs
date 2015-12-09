using Mirage.Urbanization.ZoneConsumption;

namespace Mirage.Urbanization.Tilesets
{
    class RoadHighZoneTileAccessor : BaseNetworkZoneTileAccessor<RoadZoneConsumption>
    {
        protected override FramerateDelay Delay => FramerateDelay.TrafficFramerate;
        protected override string NetworkName => "RoadHigh";
        protected override bool Filter(ZoneInfoSnapshot snapshot) => snapshot.TrafficDensity == TrafficDensity.High;
    }
}