using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Reflection;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.ZoneConsumption
{
    public class PowerLineConsumption : BaseInfrastructureNetworkZoneConsumption
    {
        public PowerLineConsumption(ZoneInfoFinder neighborNavigator) : base(neighborNavigator) { }

        public override string Name => "Power line";
        public override int Cost => 10;

        public override char KeyChar => 'l';

        public override Color Color => Color.Teal;

        public override bool CanBeOverriddenByZoneClusters => true;
        public override Image Tile => new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Mirage.Urbanization.Tiles.powerline.png"));

        protected override bool GetIsOrientatableNeighbor(QueryResult<IZoneInfo, RelativeZoneInfoQuery> consumptionQueryResult)
        {
            if (consumptionQueryResult.HasNoMatch) return false;
            return base.GetIsOrientatableNeighbor(consumptionQueryResult) || 
                consumptionQueryResult.MatchingObject.ConsumptionState.GetZoneConsumption() is ZoneClusterMemberConsumption;
        }
    }
}