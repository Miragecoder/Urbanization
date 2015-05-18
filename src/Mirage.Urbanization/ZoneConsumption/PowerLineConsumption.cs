using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization.ZoneConsumption
{
    public class PowerLineConsumption : BaseInfrastructureNetworkZoneConsumption
    {
        public PowerLineConsumption(ZoneInfoFinder neighborNavigator) : base(neighborNavigator) { }

        public override string Name
        {
            get { return "Power line"; }
        }

        public override char KeyChar { get { return 'l'; } }

        public override Color Color
        {
            get { return Color.Teal; }
        }

        public override bool CanBeOverriddenByZoneClusters
        {
            get { return true; }
        }

        protected override bool GetIsOrientatableNeighbor(QueryResult<IZoneInfo, RelativeZoneInfoQuery> consumptionQueryResult)
        {
            if (consumptionQueryResult.HasNoMatch) return false;
            return base.GetIsOrientatableNeighbor(consumptionQueryResult) || 
                consumptionQueryResult.MatchingObject.ConsumptionState.GetZoneConsumption() is ZoneClusterMemberConsumption;
        }
    }
}