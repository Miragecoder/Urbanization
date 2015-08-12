using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.ZoneConsumption.Base
{
    public class IntersectingZoneConsumption : IIntersectingZoneConsumption, IZoneConsumptionWithTraffic
    {
        private readonly ZoneInfoFinder _zoneInfoFinder;

        public char KeyChar { get { throw new NotImplementedException(); } }

        public IGetCanOverrideWithResult GetCanOverrideWith(IAreaZoneConsumption consumption)
        {
            if (consumption is EmptyZoneConsumption)
            {
                if (this.GetIntersectingTypes().Contains(typeof(WaterZoneConsumption)))
                {
                    var waterZone = new WaterZoneConsumption(_zoneInfoFinder);
                    return new AreaZoneConsumptionOverrideInfoResult(waterZone, waterZone);
                }
                return new AreaZoneConsumptionOverrideInfoResult(consumption, consumption);
            }
            else
                return new AreaZoneConsumptionOverrideInfoResult(this, consumption);
        }

        public BaseInfrastructureNetworkZoneConsumption EastWestZoneConsumption { get; }
        public BaseInfrastructureNetworkZoneConsumption NorthSouthZoneConsumption { get; }

        public int Cost => EastWestZoneConsumption.Cost + NorthSouthZoneConsumption.Cost;

        public IEnumerable<BaseInfrastructureNetworkZoneConsumption> GetIntersectingZoneConsumptions()
        {
            yield return EastWestZoneConsumption;
            yield return NorthSouthZoneConsumption;
        }

        public IntersectingZoneConsumption(ZoneInfoFinder zoneInfoFinder, BaseInfrastructureNetworkZoneConsumption eastWestZoneConsumption, BaseInfrastructureNetworkZoneConsumption northSouthZoneConsumption)
        {
            if (zoneInfoFinder == null) throw new ArgumentNullException(nameof(zoneInfoFinder));
            if (eastWestZoneConsumption == null) throw new ArgumentNullException(nameof(eastWestZoneConsumption));
            if (northSouthZoneConsumption == null) throw new ArgumentNullException(nameof(northSouthZoneConsumption));

            if (eastWestZoneConsumption.GetType() == northSouthZoneConsumption.GetType())
                throw new InvalidOperationException("An intersecting zone consumption must consist of two differing types.");

            _zoneInfoFinder = zoneInfoFinder;
            EastWestZoneConsumption = eastWestZoneConsumption;
            NorthSouthZoneConsumption = northSouthZoneConsumption;
        }

        public string Name => EastWestZoneConsumption.Name + " and " + NorthSouthZoneConsumption.Name;

        public IEnumerable<Type> GetIntersectingTypes()
        {
            yield return EastWestZoneConsumption.GetType();
            yield return NorthSouthZoneConsumption.GetType();
        }

        public Color Color => Color.Firebrick;
        public string ColorName => Color.Name;

        public TrafficDensity GetTrafficDensity()
        {
            var match = TryGetRoad();
            var density = TrafficDensity.None;

            match.WithResultIfHasMatch(road => density = road.GetTrafficDensity());

            return density;
        }

        private QueryResult<RoadZoneConsumption> TryGetRoad()
        {

            var road = (NorthSouthZoneConsumption as RoadZoneConsumption) ??
                       (EastWestZoneConsumption as RoadZoneConsumption);

            return QueryResult<RoadZoneConsumption>.Create(road);
        }

        public int GetTrafficDensityAsInt()
        {
            var match = TryGetRoad();
            var density = default(int);

            match.WithResultIfHasMatch(road => density = road.GetTrafficDensityAsInt());

            return density;
        }


        public BaseInfrastructureNetworkZoneConsumption GetZoneConsumptionOfType<TBaseInfrastructureNetworkZoneConsumption>()
            where TBaseInfrastructureNetworkZoneConsumption : BaseInfrastructureNetworkZoneConsumption
        {
            if (NorthSouthZoneConsumption is TBaseInfrastructureNetworkZoneConsumption)
                return NorthSouthZoneConsumption;
            else if (EastWestZoneConsumption is TBaseInfrastructureNetworkZoneConsumption)
                return EastWestZoneConsumption;
            else
                throw new InvalidOperationException();
        }
    }
}