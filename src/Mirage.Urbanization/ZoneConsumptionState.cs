using System;
using System.Linq;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization
{
    public interface IReadOnlyZoneConsumptionState
    {
        IAreaZoneConsumption GetZoneConsumption();
        DateTime LastUpdateDateTime { get; }
        bool GetIsRailroadNetworkMember();
    }

    public interface IZoneConsumptionState : IReadOnlyZoneConsumptionState
    {
        bool GetIsPowerGridMember();
        bool GetIsRoadNetworkMember();
        bool GetIsZoneClusterMember();
        QueryResult<ZoneClusterMemberConsumption> QueryAsZoneClusterMember();

        void WithNetworkMember<TBaseNetworkZoneConsumption>(Action<TBaseNetworkZoneConsumption> action)
            where TBaseNetworkZoneConsumption : BaseInfrastructureNetworkZoneConsumption;

        IConsumeAreaOperation TryConsumeWith(IAreaZoneConsumption consumption);
    }

    public class ZoneConsumptionState : IZoneConsumptionState
    {
        private IAreaZoneConsumption _zoneConsumption = new EmptyZoneConsumption();
        private DateTime _lastUpdateDateTime = DateTime.Now;
        public DateTime LastUpdateDateTime { get { return _lastUpdateDateTime; } }

        public IAreaZoneConsumption GetZoneConsumption()
        {
            return _zoneConsumption;
        }

        public IConsumeAreaOperation TryConsumeWith(IAreaZoneConsumption consumption)
        {
            if (consumption == null) throw new ArgumentNullException("consumption");
            return new ConsumeAreaOperation(_zoneConsumption.GetCanOverrideWith(consumption), (toBeDeployedConsumption) =>
            {
                _zoneConsumption = toBeDeployedConsumption;
                _lastUpdateDateTime = DateTime.Now;
            });
        }

        public bool GetIsPowerGridMember()
        {
            return GetIsNetworkMember<PowerLineConsumption>() ||
                   GetZoneConsumption() is ZoneClusterMemberConsumption;
        }

        private bool GetIsNetworkMember<TBaseNetworkZoneConsumption>()
            where TBaseNetworkZoneConsumption : BaseInfrastructureNetworkZoneConsumption
        {
            var consumptionState = GetZoneConsumption();
            var consumptionAsPowerLine = consumptionState as TBaseNetworkZoneConsumption;
            var consumptionAsIntersection = consumptionState as IntersectingZoneConsumption;

            if (consumptionAsPowerLine != null)
                return true;

            return consumptionAsIntersection != null && consumptionAsIntersection
                .GetIntersectingTypes()
                .Contains(typeof(TBaseNetworkZoneConsumption));  
        }

        public bool GetIsRoadNetworkMember()
        {
            return GetIsNetworkMember<RoadZoneConsumption>();
        }

        public bool GetIsRailroadNetworkMember()
        {
            return GetIsNetworkMember<RailRoadZoneConsumption>();
        }

        public bool GetIsZoneClusterMember()
        {
            return GetZoneConsumption() is ZoneClusterMemberConsumption;
        }

        public QueryResult<ZoneClusterMemberConsumption> QueryAsZoneClusterMember()
        {
            var consumption = GetZoneConsumption() as ZoneClusterMemberConsumption;
            return new QueryResult<ZoneClusterMemberConsumption>(consumption);
        }

        public void WithNetworkMember<TBaseNetworkZoneConsumption>(Action<TBaseNetworkZoneConsumption> action)
            where TBaseNetworkZoneConsumption : BaseInfrastructureNetworkZoneConsumption
        {
            var consumptionState = GetZoneConsumption();
            var consumptionAsPowerLine = consumptionState as TBaseNetworkZoneConsumption;
            var consumptionAsIntersection = consumptionState as IntersectingZoneConsumption;

            if (consumptionAsPowerLine != null)
                action(consumptionAsPowerLine);
            else if (consumptionAsIntersection != null)
            {
                var match =
                    consumptionAsIntersection.GetIntersectingZoneConsumptions()
                        .OfType<TBaseNetworkZoneConsumption>()
                        .FirstOrDefault();
                if (match != null)
                    action(match);
            }
        }
    }
}