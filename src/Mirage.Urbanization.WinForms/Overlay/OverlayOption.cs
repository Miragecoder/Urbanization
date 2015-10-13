using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Mirage.Urbanization.Simulation.Datameters;
using Mirage.Urbanization.Tilesets;
using Mirage.Urbanization.WinForms.Rendering;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.WinForms.Overlay
{
    public class OverlayOption : IToolstripMenuOption
    {
        private readonly Func<bool> _toggleShowNumbersFunc;
        private readonly QueryResult<Func<ZoneInfoDataMeter>> _getDataMeterFunc;

        public static IEnumerable<OverlayOption> CreateOverlayOptionInstances(Func<bool> toggleShowNumbersFunc)
        {
            return new[]
            {
                new OverlayOption("None", null, toggleShowNumbersFunc)
            }
              .Concat(DataMeterInstances
                  .DataMeters
                  .Select(x =>
                  {
                      var localX = x;
                      return new OverlayOption(localX.Name, () => localX, toggleShowNumbersFunc);
                  })
              )
              .ToList();
        }

        public OverlayOption(string name, Func<ZoneInfoDataMeter> getDataMeterFunc, Func<bool> toggleShowNumbersFunc)
        {
            Name = name;
            _toggleShowNumbersFunc = toggleShowNumbersFunc;
            _getDataMeterFunc = QueryResult<Func<ZoneInfoDataMeter>>.Create(getDataMeterFunc);
        }

        public void Render(IReadOnlyZoneInfo zoneInfo, Rectangle rectangle, IGraphicsWrapper graphics)
        {
            _getDataMeterFunc.WithResultIfHasMatch(f =>
            {
                var dataMeterResult = f().GetDataMeterResult(zoneInfo);

                var brush = BrushManager.Instance.GetBrushFor(dataMeterResult.ValueCategory);
                if (brush.HasMatch)
                    graphics.FillRectangle(brush.MatchingObject, rectangle);

                if (_toggleShowNumbersFunc())
                {
                    var amount = dataMeterResult.Amount;
                    if (amount != 0)
                    {
                        graphics.FillRectangle(BrushManager.DarkNumberSurfaceBrush, rectangle);
                        graphics.DrawString(amount.ToString(),
                            BrushManager.ZoneInfoFont,
                            BrushManager.RedSolidBrush,
                            rectangle);
                    }
                }
            });
        }

        public string Name { get; }
    }
}