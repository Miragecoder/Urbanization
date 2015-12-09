using System;
using System.Linq;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Tilesets
{
    class WoodZoneTileAccessor : BaseNetworkZoneTileAccessor<WoodlandZoneConsumption>
    {
        protected override string NetworkName => "Wood";
    }
    class WaterZoneTileAccessor : BaseNetworkZoneTileAccessor<WaterZoneConsumption>
    {
        protected override string NetworkName => "Water";
        protected override bool IsConnected(IAreaZoneConsumption consumption)
        {
            return (consumption as IntersectingZoneConsumption)
                .ToQueryResult()
                .WithResultIfHasMatch(
                    x =>
                        x.NorthSouthZoneConsumption is WaterZoneConsumption ||
                        x.EastWestZoneConsumption is WaterZoneConsumption);
        }
    }
    class RoadNoneZoneTileAccessor : BaseNetworkZoneTileAccessor<RoadZoneConsumption>
    {
        protected override string NetworkName => "RoadNone";
        protected override bool Filter(ZoneInfoSnapshot snapshot) => snapshot.TrafficDensity == TrafficDensity.None;
    }
    class RoadLowZoneTileAccessor : BaseNetworkZoneTileAccessor<RoadZoneConsumption>
    {
        protected override string NetworkName => "RoadLow";
        protected override bool Filter(ZoneInfoSnapshot snapshot) => snapshot.TrafficDensity == TrafficDensity.Low;
    }
    class RoadHighZoneTileAccessor : BaseNetworkZoneTileAccessor<RoadZoneConsumption>
    {
        protected override string NetworkName => "RoadLow";
        protected override bool Filter(ZoneInfoSnapshot snapshot) => snapshot.TrafficDensity == TrafficDensity.High;
    }

    class RailZoneTileAccessor : BaseNetworkZoneTileAccessor<RailRoadZoneConsumption>
    {
        protected override string NetworkName => "Rail";
    }
    class PowerZoneTileAccessor : BaseNetworkZoneTileAccessor<PowerLineConsumption>
    {
        protected override string NetworkName => "Power";

        protected override bool IsConnected(IAreaZoneConsumption consumption) => consumption is ZoneClusterMemberConsumption;
    }

    class NetworkZoneTileAccessor
    {
        private readonly IBaseNetworkZoneTileAccessor[] _accessors = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(
                domainAssembly => domainAssembly.GetTypes(), 
                (domainAssembly, assemblyType) => new { domainAssembly, assemblyType }
            )
            .Where(@t => typeof(IBaseNetworkZoneTileAccessor).IsAssignableFrom(@t.assemblyType))
            .Where(@t => !@t.assemblyType.IsAbstract)
            .Where(@t => @t.assemblyType.IsClass)
            .Select(@t => @t.assemblyType)
            .Select(Activator.CreateInstance).Cast<IBaseNetworkZoneTileAccessor>()
            .ToArray();

        public QueryResult<AnimatedCellBitmapSetLayers> GetFor(ZoneInfoSnapshot snapshot)
        {
            return _accessors
                .Select(x => x.GetFor(snapshot).MatchingObject)
                .SingleOrDefault(x => x != null)
                .ToQueryResult();
        }
    }
}