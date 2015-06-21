using System.Drawing;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization.ZoneConsumption
{
    public class EmptyZoneConsumption : BaseSingleZoneConsumption
    {
        public override string Name
        {
            get { return "Clear"; }
        }

        public override char KeyChar { get { return 'd'; } }

        public override Color Color
        {
            get { return DefaultColor; }
        }

        public override int Cost { get { return 10; } }

        public static Color DefaultColor = System.Drawing.Color.BurlyWood;

        public override IGetCanOverrideWithResult GetCanOverrideWith(IAreaZoneConsumption consumption)
        {
            return new AreaZoneConsumptionOverrideInfoResult(
                resultingAreaConsumption: consumption, 
                toBeDeployedAreaConsumption: consumption
            );
        }
    }
}