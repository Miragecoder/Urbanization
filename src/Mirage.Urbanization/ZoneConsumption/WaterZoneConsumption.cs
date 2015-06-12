using System;
using System.Drawing;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization.ZoneConsumption
{
    public class WaterZoneConsumption : BaseInfrastructureNetworkZoneConsumption
    {
        public WaterZoneConsumption(ZoneInfoFinder neighborNavigator)
            : base(neighborNavigator)
        {

        }

        public override char KeyChar { get { return 'w'; } }

        public override string Name
        {
            get { return "Water"; }
        }

        public override Color Color
        {
            get { return System.Drawing.Color.DarkBlue; }
        }

        protected override bool GetIsOrientatableNeighbor(QueryResult<IZoneInfo, RelativeZoneInfoQuery> consumptionQueryResult)
        {
            return base.GetIsOrientatableNeighbor(consumptionQueryResult) ||
                consumptionQueryResult.HasMatch && consumptionQueryResult.MatchingObject.GetAsZoneCluster<SeaPortZoneClusterConsumption>().HasMatch;
        }

        public override IGetCanOverrideWithResult GetCanOverrideWith(IAreaZoneConsumption consumption)
        {
            if (consumption is RoadZoneConsumption)
                return GenerateBridgingOverrideInfoResult(consumption as RoadZoneConsumption);
            else if (consumption is RailRoadZoneConsumption)
                return GenerateBridgingOverrideInfoResult(consumption as RailRoadZoneConsumption);
            else if (consumption is PowerLineConsumption)
                return GenerateBridgingOverrideInfoResult(consumption as PowerLineConsumption);

            return new AreaZoneConsumptionOverrideInfoResult(this, consumption);
        }

        private AreaZoneConsumptionOverrideInfoResult GenerateBridgingOverrideInfoResult<TBaseInfrastructureNetworkZoneConsumption>
            (TBaseInfrastructureNetworkZoneConsumption consumption)
            where TBaseInfrastructureNetworkZoneConsumption : BaseInfrastructureNetworkZoneConsumption
        {
            var zoneInfo = NeighborNavigator.GetZoneInfoFor(this).MatchingObject;

            var north = zoneInfo.GetNorth();
            var south = zoneInfo.GetSouth();
            var east = zoneInfo.GetEast();
            var west = zoneInfo.GetWest();

            if (BridgesOrRiverbedsWith<TBaseInfrastructureNetworkZoneConsumption>(north, x => x.NorthSouthZoneConsumption)
                || BridgesOrRiverbedsWith<TBaseInfrastructureNetworkZoneConsumption>(south, x => x.NorthSouthZoneConsumption))
            {
                var newConsumption = new IntersectingZoneConsumption(NeighborNavigator, this, consumption);
                return new AreaZoneConsumptionOverrideInfoResult(newConsumption, newConsumption);
            }
            else if (BridgesOrRiverbedsWith<TBaseInfrastructureNetworkZoneConsumption>(east, x => x.EastWestZoneConsumption)
                || BridgesOrRiverbedsWith<TBaseInfrastructureNetworkZoneConsumption>(west, x => x.EastWestZoneConsumption))
            {
                var newConsumption = new IntersectingZoneConsumption(NeighborNavigator, consumption, this);
                return new AreaZoneConsumptionOverrideInfoResult(newConsumption, newConsumption);
            }

            return new AreaZoneConsumptionOverrideInfoResult(this, consumption);
        }

        private static bool BridgesOrRiverbedsWith<TBaseInfrastructureNetworkZoneConsumption>(QueryResult<IZoneInfo, RelativeZoneInfoQuery> queryResult,
            Func<IntersectingZoneConsumption, BaseNetworkZoneConsumption> comparison)
                        where TBaseInfrastructureNetworkZoneConsumption : BaseInfrastructureNetworkZoneConsumption
        {
            return BridgesWith<TBaseInfrastructureNetworkZoneConsumption>(queryResult, comparison) || RiverbedsWith<TBaseInfrastructureNetworkZoneConsumption>(queryResult);
        }

        private static bool RiverbedsWith<TBaseInfrastructureNetworkZoneConsumption>(QueryResult<IZoneInfo, RelativeZoneInfoQuery> queryResult)
                        where TBaseInfrastructureNetworkZoneConsumption : BaseInfrastructureNetworkZoneConsumption
        {
            return queryResult.HasMatch &&
                   queryResult.MatchingObject.ConsumptionState.GetZoneConsumption() is TBaseInfrastructureNetworkZoneConsumption;
        }

        private static bool BridgesWith<TBaseInfrastructureNetworkZoneConsumption>(QueryResult<IZoneInfo, RelativeZoneInfoQuery> queryResult, Func<IntersectingZoneConsumption, BaseNetworkZoneConsumption> comparison)
            where TBaseInfrastructureNetworkZoneConsumption : BaseInfrastructureNetworkZoneConsumption
        {
            var intersectingZoneConsumption = queryResult.MatchingObject.ConsumptionState.GetZoneConsumption() as IntersectingZoneConsumption;
            return (intersectingZoneConsumption != null
                && (queryResult.HasMatch
                && queryResult.MatchingObject.ConsumptionState.GetZoneConsumption() is IntersectingZoneConsumption
                && comparison(intersectingZoneConsumption).GetType() == typeof(TBaseInfrastructureNetworkZoneConsumption)));
        }

        public override bool CanBeOverriddenByZoneClusters
        {
            get { return false; }
        }
    }
}