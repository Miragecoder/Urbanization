using System.Collections.Generic;
using System.Drawing;
using Mirage.Urbanization.Simulation.Datameters;
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

        private static readonly Dictionary<DataMeterValueCategory, SolidBrush> DataMeterValueBrushes = new Dictionary<DataMeterValueCategory, SolidBrush>
        {
            { DataMeterValueCategory.Low, new SolidBrush(Color.FromArgb(190, 150, 200, 200))},
            { DataMeterValueCategory.Medium, new SolidBrush(Color.FromArgb(190, 70, 50, 180))},
            { DataMeterValueCategory.High, new SolidBrush(Color.FromArgb(190, 180, 0, 180))},
            { DataMeterValueCategory.VeryHigh, new SolidBrush(Color.FromArgb(190, 250, 0, 50))}
        };

        public static readonly SolidBrush DarkNumberSurfaceBrush = new SolidBrush(Color.FromArgb(120, 0,0,0));

        public QueryResult<SolidBrush> GetBrushFor(DataMeterValueCategory dataMeterValueCategory)
        {
            if (DataMeterValueBrushes.ContainsKey(dataMeterValueCategory))
                return new QueryResult<SolidBrush>(DataMeterValueBrushes[dataMeterValueCategory]);
            return QueryResult<SolidBrush>.Empty;
        }
    }
}