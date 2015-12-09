using Mirage.Urbanization.ZoneConsumption;

namespace Mirage.Urbanization.Tilesets
{
    class RoadLowZoneTileAccessor : BaseNetworkZoneTileAccessor<RoadZoneConsumption>
    {
        protected override FramerateDelay Delay => FramerateDelay.TrafficFramerate;
        protected override string NetworkName => "RoadLow";
        protected override bool Filter(ZoneInfoSnapshot snapshot) => snapshot.TrafficDensity == TrafficDensity.Low;
    }
}