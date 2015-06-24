using System;
using System.Collections.Generic;
using System.Drawing;
using Mirage.Urbanization.Simulation.Datameters;
using Mirage.Urbanization.WinForms.Rendering;

namespace Mirage.Urbanization.WinForms
{
    public class OverlayOption : IToolstripMenuOption
    {
        private readonly string _name;
        private readonly QueryResult<Func<ZoneInfoDataMeter>> _getDataMeterFunc;

        private static readonly IReadOnlyCollection<OverlayOption> OverlayOptionInstances = new[]
        {
            new OverlayOption("None", null),
            new OverlayOption("Crime", () => DataMeterInstances.CrimeDataMeter),
            new OverlayOption("Pollution", () => DataMeterInstances.PollutionDataMeter)
        };

        public static IReadOnlyCollection<OverlayOption> OverlayOptions { get { return OverlayOptionInstances; } }

        public OverlayOption(string name, Func<ZoneInfoDataMeter> getDataMeterFunc)
        {
            _name = name;
            _getDataMeterFunc = new QueryResult<Func<ZoneInfoDataMeter>>(getDataMeterFunc);
        }

        public void Render(IReadOnlyZoneInfo zoneInfo, Rectangle rectangle, IGraphicsWrapper graphics)
        {
            _getDataMeterFunc.WithResultIfHasMatch(f =>
            {
                var brush = BrushManager.Instance.GetBrushFor(f().GetDataMeterResult(zoneInfo).ValueCategory);
                if (brush.HasMatch)
                    graphics.FillRectangle(brush.MatchingObject, rectangle);
            });
        }

        public string Name { get { return _name; } }
    }
}