using System.Drawing;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneConsumption.Base.Behaviours;

namespace Mirage.Urbanization.ZoneConsumption
{
    public class WoodlandZoneConsumption : BaseNetworkZoneConsumption
    {
        public WoodlandZoneConsumption(ZoneInfoFinder navigator) : base(navigator) { }

        public override string Name
        {
            get { return "Woodlands"; }
        }

        public override char KeyChar { get { return 'b'; } }

        private static readonly DynamicPollutionBehaviour WoodlandPollutionBehaviour = new DynamicPollutionBehaviour(() => -50);

        public IPollutionBehaviour PollutionBehaviour
        {
            get { return WoodlandPollutionBehaviour; }
        }

        public override Color Color
        {
            get { return System.Drawing.Color.DarkGreen; }
        }

        public override IGetCanOverrideWithResult GetCanOverrideWith(IAreaZoneConsumption consumption)
        {
            return new AreaZoneConsumptionOverrideInfoResult(consumption, consumption);
        }
    }
}