using System;
using System.Collections.Generic;
using System.Linq;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.ZoneConsumption.Base
{
    [Flags]
    public enum Orientation
    {
        North = 1,
        West = 2,
        East = 4,
        South = 8,
        NorthSouth = North ^ South,
        EastWest = East ^ West,
        NorthWest = North ^ West,
        NorthEast = North ^ East,
        SouthWest = South ^ West,
        SouthEast = South ^ East,
        NorthWestEast = NorthWest ^ East,
        NorthEastSouth = NorthEast ^ South,
        EastSouthWest = SouthEast ^ West,
        SouthWestNorth = SouthWest ^ North,
        AllDirections = SouthWestNorth ^ East
    }

    public abstract class BaseNetworkZoneConsumption : BaseSingleZoneConsumption
    {
        protected readonly ZoneInfoFinder NeighborNavigator;
        public override BuildStyle BuildStyle => BuildStyle.ClickAndDrag;

        protected BaseNetworkZoneConsumption(ZoneInfoFinder neighborNavigator)
        {
            if (neighborNavigator == null) throw new ArgumentNullException(nameof(neighborNavigator));

            NeighborNavigator = neighborNavigator;
        }

        public bool GetIsOrientedNorth() { return GetIsOriented(x => x.GetNorth()); }
        public bool GetIsOrientedSouth() { return GetIsOriented(x => x.GetSouth()); }
        public bool GetIsOrientedEast() { return GetIsOriented(x => x.GetEast()); }
        public bool GetIsOrientedWest() { return GetIsOriented(x => x.GetWest()); }

        private bool GetIsOriented(Func<IZoneInfo, QueryResult<IZoneInfo, RelativeZoneInfoQuery>> getOrientation)
        {
            var match = NeighborNavigator.GetZoneInfoFor(this);
            return match.HasMatch && GetIsOrientatableNeighbor(getOrientation(match.MatchingObject));
        }

        public void WithOrientation(Action<Orientation> action)
        {
            var x = GetOrientation();
            if (x.HasValue)
                action(x.Value);
        }

        public Orientation? GetOrientation()
        {
            Orientation? orientation = default(Orientation?);

            if (GetIsOrientedNorth())
                orientation = Orientation.North;
            if (GetIsOrientedEast())
            {
                if (orientation.HasValue)
                    orientation = orientation.Value ^ Orientation.East;
                else
                    orientation = Orientation.East;
            }
            if (GetIsOrientedWest())
            {
                if (orientation.HasValue)
                    orientation = orientation.Value ^ Orientation.West;
                else
                    orientation = Orientation.West;
            }
            if (GetIsOrientedSouth())
            {
                if (orientation.HasValue)
                    orientation = orientation.Value ^ Orientation.South;
                else
                    orientation = Orientation.South;
            }
            return orientation;
        }

        protected virtual bool GetIsOrientatableNeighbor(QueryResult<IZoneInfo, RelativeZoneInfoQuery> consumptionQueryResult)
        {
            if (consumptionQueryResult.HasNoMatch) return false;

            var consumption = consumptionQueryResult.MatchingObject.ConsumptionState.GetZoneConsumption();

            if (consumption is IIntersectingZoneConsumption)
            {
                if ((consumption as IIntersectingZoneConsumption).GetIntersectingTypes().Any(x => x == GetType()))
                {
                    if (GetType() == typeof (WaterZoneConsumption))
                        return true;

                    var intersection = (consumption as IIntersectingZoneConsumption);

                    if (consumptionQueryResult.QueryObject.RelativeX == 0 &&
                        consumptionQueryResult.QueryObject.RelativeY != 0)
                        return intersection.NorthSouthZoneConsumption.GetType() == GetType();

                    if (consumptionQueryResult.QueryObject.RelativeX != 0 &&
                        consumptionQueryResult.QueryObject.RelativeY == 0)
                        return intersection.EastWestZoneConsumption.GetType() == GetType();

                    throw new InvalidOperationException("Unsupported query issued.");
                }
            }
            return (consumption.GetType() == GetType());
        }
    }

    public abstract class BaseInfrastructureNetworkZoneConsumption : BaseNetworkZoneConsumption
    {

        protected BaseInfrastructureNetworkZoneConsumption(ZoneInfoFinder neighborNavigator)
            : base(neighborNavigator)
        {

        }

        public bool GetIsCornered()
        {
            return (GetIsOrientedNorth() && (GetIsOrientedEast() || GetIsOrientedWest()))
                   || (GetIsOrientedSouth() && (GetIsOrientedEast() || GetIsOrientedWest()));
        }

        public override IGetCanOverrideWithResult GetCanOverrideWith(IAreaZoneConsumption consumption)
        {
            if (consumption is EmptyZoneConsumption)
                return new AreaZoneConsumptionOverrideInfoResult(
                    resultingAreaConsumption: consumption,
                    toBeDeployedAreaConsumption: consumption
                );

            if (GetIsCornered())
                return new AreaZoneConsumptionOverrideInfoResult(this, consumption);

            if (consumption is BaseInfrastructureNetworkZoneConsumption && GetType() != consumption.GetType())
            {
                IntersectingZoneConsumption intersectingZoneConsumption = null;
                switch (GetOrientation())
                {
                    case Orientation.NorthSouth:
                        intersectingZoneConsumption = new IntersectingZoneConsumption(
                            zoneInfoFinder: NeighborNavigator,
                            eastWestZoneConsumption: consumption as BaseInfrastructureNetworkZoneConsumption,
                            northSouthZoneConsumption: this
                        );
                        break;
                    case Orientation.EastWest:
                        intersectingZoneConsumption = new IntersectingZoneConsumption(
                            zoneInfoFinder: NeighborNavigator,
                            eastWestZoneConsumption: this,
                            northSouthZoneConsumption: consumption as BaseInfrastructureNetworkZoneConsumption
                        );
                        break;
                }
                if (intersectingZoneConsumption != null)
                    return new AreaZoneConsumptionOverrideInfoResult(
                        intersectingZoneConsumption,
                        intersectingZoneConsumption);
            }

            if (CanBeOverriddenByZoneClusters && consumption is ZoneClusterMemberConsumption)
            {
                return new AreaZoneConsumptionOverrideInfoResult(
                    resultingAreaConsumption: consumption,
                    toBeDeployedAreaConsumption: consumption
                );
            }

            return new AreaZoneConsumptionOverrideInfoResult(
                resultingAreaConsumption: this,
                toBeDeployedAreaConsumption: consumption
            );
        }

        public abstract bool CanBeOverriddenByZoneClusters { get; }
    }
}