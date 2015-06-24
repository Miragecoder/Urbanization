using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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

        private readonly BaseInfrastructureNetworkZoneConsumption _eastWestZoneConsumption;
        private readonly BaseInfrastructureNetworkZoneConsumption _northSouthZoneConsumption;

        public BaseInfrastructureNetworkZoneConsumption EastWestZoneConsumption { get { return _eastWestZoneConsumption; } }
        public BaseInfrastructureNetworkZoneConsumption NorthSouthZoneConsumption { get { return _northSouthZoneConsumption; } }

        public int Cost { get { return EastWestZoneConsumption.Cost + NorthSouthZoneConsumption.Cost; } }

        public IEnumerable<BaseInfrastructureNetworkZoneConsumption> GetIntersectingZoneConsumptions()
        {
            yield return EastWestZoneConsumption;
            yield return NorthSouthZoneConsumption;
        }

        public IntersectingZoneConsumption(ZoneInfoFinder zoneInfoFinder, BaseInfrastructureNetworkZoneConsumption eastWestZoneConsumption, BaseInfrastructureNetworkZoneConsumption northSouthZoneConsumption)
        {
            if (zoneInfoFinder == null) throw new ArgumentNullException("zoneInfoFinder");
            if (eastWestZoneConsumption == null) throw new ArgumentNullException("eastWestZoneConsumption");
            if (northSouthZoneConsumption == null) throw new ArgumentNullException("northSouthZoneConsumption");

            if (eastWestZoneConsumption.GetType() == northSouthZoneConsumption.GetType())
                throw new InvalidOperationException("An intersecting zone consumption must consist of two differing types.");

            _zoneInfoFinder = zoneInfoFinder;
            _eastWestZoneConsumption = eastWestZoneConsumption;
            _northSouthZoneConsumption = northSouthZoneConsumption;
        }

        public string Name
        {
            get { return _eastWestZoneConsumption.Name + " and " + _northSouthZoneConsumption.Name; }
        }

        public IEnumerable<Type> GetIntersectingTypes()
        {
            yield return _eastWestZoneConsumption.GetType();
            yield return _northSouthZoneConsumption.GetType();
        }

        public Color Color
        {
            get { return Color.Firebrick; }
        }

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

            return new QueryResult<RoadZoneConsumption>(road);
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