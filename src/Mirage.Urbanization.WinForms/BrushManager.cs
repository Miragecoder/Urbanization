using System.Drawing;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization.WinForms
{
    internal class BrushManager
    {
        private BrushManager() { }

        public static readonly BrushManager Instance = new BrushManager();

        private IAreaZoneConsumption _currentAreaZoneConsumption;

        public static readonly Font ZoneInfoFont = new Font(FontFamily.GenericMonospace, 6);
        public static readonly Pen RedPen = new Pen(new SolidBrush(Color.Red)),
            BluePen = new Pen(new SolidBrush(Color.Blue)),
            YellowPen = new Pen(new SolidBrush(Color.Yellow), 3f),
            GreenPen = new Pen(new SolidBrush(Color.LawnGreen), 3f);

        public static readonly SolidBrush BlackSolidBrush = new SolidBrush(Color.Black),
            RedSolidBrush = new SolidBrush(Color.Red);

        private SolidBrush _currentBrush;

        public SolidBrush GetBrushFor(IAreaZoneConsumption consumption)
        {
            if (consumption != _currentAreaZoneConsumption)
            {
                _currentBrush = new SolidBrush(consumption.Color);
                _currentAreaZoneConsumption = consumption;
            }
            return _currentBrush;
        }
    }
}