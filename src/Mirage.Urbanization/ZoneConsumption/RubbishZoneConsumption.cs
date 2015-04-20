using System.Drawing;

namespace Mirage.Urbanization.ZoneConsumption
{
    public class RubbishZoneConsumption : EmptyZoneConsumption
    {
        public override string Name
        {
            get { return "Rubbish"; }
        }

        public override Color Color
        {
            get
            {
                return Color.SaddleBrown;
            }
        }
    }
}