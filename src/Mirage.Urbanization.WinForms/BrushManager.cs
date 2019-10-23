using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using Mirage.Urbanization.Simulation.Datameters;
using Mirage.Urbanization.Tilesets;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.WinForms
{

    public class BrushManager
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
                _currentBrush = new SolidBrush(consumption.Color.ToSysDrawingColor());
                _currentAreaZoneConsumption = consumption;
            }
            return _currentBrush;
        }

        private static readonly ConcurrentDictionary<Color, SolidBrush> BrushCache = new ConcurrentDictionary<Color, SolidBrush>();

        public QueryResult<SolidBrush> GetBrushFor(DataMeterValueCategory dataMeterValueCategory)
        {
            var result = DatameterColourDefinitions.Instance.GetColorFor(dataMeterValueCategory);
            if (result.HasValue)
            {
                var color = result.Value.ToSysDrawingColor();
                return BrushCache.GetOrAdd(color, c => new SolidBrush(result.Value.ToSysDrawingColor())).ToQueryResult();
            }
            return QueryResult<SolidBrush>.Empty;
        }

        public static readonly SolidBrush DarkNumberSurfaceBrush = new SolidBrush(Color.FromArgb(120, 0, 0, 0));
    }
}