using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using System.Windows.Forms;
using Mirage.Urbanization.GrowthPathFinding;
using Mirage.Urbanization.Simulation.Datameters;
using Mirage.Urbanization.Tilesets;
using Mirage.Urbanization.WinForms.Rendering;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.WinForms
{
    public class RenderZoneContinuation
    {
        private readonly Action _drawSecondLayerAction;
        private readonly Action<IAreaConsumption> _drawHighligterAction;

        public void DrawSecondLayer()=>_drawSecondLayerAction?.Invoke();

        public void DrawHighlighter(IAreaConsumption consumption)=>_drawHighligterAction?.Invoke(consumption);

        public bool HasDrawHighlighterDelegate => _drawHighligterAction != null;

        public bool HasDrawSecondLayerDelegate => _drawSecondLayerAction != null;

        public RenderZoneContinuation(Action drawSecondLayerAction, Action<IAreaConsumption> drawHighligterAction)
        {
            _drawSecondLayerAction = drawSecondLayerAction;
            _drawHighligterAction = drawHighligterAction;
        }
    }

    public class ZoneRenderInfo
    {
        private readonly Func<IReadOnlyZoneInfo, Rectangle> _createRectangle;
        private readonly ITilesetAccessor _tilesetAccessor;
        private readonly RenderZoneOptions _renderZoneOptions;

        public IReadOnlyZoneInfo ZoneInfo { get; }

        public Rectangle GetRectangle()
        {
            return _createRectangle(ZoneInfo);
        }

        public ZoneRenderInfo(IReadOnlyZoneInfo zoneInfo, Func<IReadOnlyZoneInfo, Rectangle> createRectangle, ITilesetAccessor tilesetAccessor, RenderZoneOptions renderZoneOptions)
        {
            ZoneInfo = zoneInfo;
            _createRectangle = createRectangle;
            _tilesetAccessor = tilesetAccessor;
            _renderZoneOptions = renderZoneOptions;
        }

        public RenderZoneContinuation RenderZoneInto(IGraphicsWrapper graphics, bool isHighlighted)
        {
            if (graphics == null) throw new ArgumentNullException(nameof(graphics));

            var rectangle = GetRectangle();

            var consumption = ZoneInfo.ZoneConsumptionState.GetZoneConsumption();

            Action drawSecondLayerAction = null;

            QueryResult<AnimatedCellBitmapSetLayers> bitmapLayer = _tilesetAccessor.TryGetBitmapFor(ZoneInfo.TakeSnapshot());

            if (bitmapLayer.HasMatch)
            {
                graphics.DrawImage(bitmapLayer.MatchingObject.LayerOne.Current, rectangle);

                if (bitmapLayer.MatchingObject.LayerTwo.HasMatch)
                    drawSecondLayerAction = () => { graphics.DrawImage(bitmapLayer.MatchingObject.LayerTwo.MatchingObject.Current, rectangle); };

                if (_renderZoneOptions.ShowDebugGrowthPathFinding)
                {
                    switch (ZoneInfo.GrowthAlgorithmHighlightState.Current)
                    {
                        case HighlightState.UsedAsPath:
                            graphics.DrawRectangle(BrushManager.GreenPen, rectangle);
                            break;
                        case HighlightState.Examined:
                            graphics.DrawRectangle(BrushManager.YellowPen, rectangle);
                            break;
                    }
                }
            }
            else
            {
                graphics.FillRectangle(BrushManager.Instance.GetBrushFor(consumption), rectangle);
            }

            var overlayOption = _renderZoneOptions.CurrentOverlayOption;
            if (overlayOption != null)
                overlayOption.Render(ZoneInfo, rectangle, graphics);

            if (isHighlighted)
            {
                return new RenderZoneContinuation(
                    drawSecondLayerAction,

                (areaConsumption) =>
                {
                    if (areaConsumption is IAreaZoneClusterConsumption)
                    {
                        var sampleZones = (areaConsumption as IAreaZoneClusterConsumption)
                            .ZoneClusterMembers;

                        var width = sampleZones.GroupBy(x => x.RelativeToParentCenterX).Count() * _tilesetAccessor.TileWidthAndSizeInPixels;
                        var height = sampleZones.GroupBy(x => x.RelativeToParentCenterY).Count() * _tilesetAccessor.TileWidthAndSizeInPixels;

                        var xOffset = sampleZones.Min(x => x.RelativeToParentCenterX) * _tilesetAccessor.TileWidthAndSizeInPixels;
                        var yOffset = sampleZones.Min(x => x.RelativeToParentCenterY) * _tilesetAccessor.TileWidthAndSizeInPixels;

                        rectangle.Size = new Size(
                            width: width,
                            height: height
                        );

                        rectangle.Location = new Point(
                            x: rectangle.Location.X + xOffset, y: rectangle.Location.Y + yOffset);
                    }

                    var pen = (DateTime.Now.Millisecond % 400) > 200 ? BrushManager.BluePen : BrushManager.RedPen;
                    graphics.DrawRectangle(pen, rectangle);
                });
            }
            return new RenderZoneContinuation(drawSecondLayerAction, null);
        }
    }
}