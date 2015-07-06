using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mirage.Urbanization.WinForms.Rendering.BufferedGraphics;
using Mirage.Urbanization.WinForms.Rendering.SharpDx;
using Mirage.Urbanization.Simulation;
using Mirage.Urbanization.Tilesets;
using Mirage.Urbanization.WinForms.Rendering;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;
using Orientation = Mirage.Urbanization.ZoneConsumption.Base.Orientation;

namespace Mirage.Urbanization.WinForms
{
    public class SimulationRenderHelper
    {
        private readonly ISimulationSession _simulationSession;
        private readonly ITilesetAccessor _tilesetAccessor = new TilesetAccessor();
        private readonly ZoneSelectionPanelCreator _zoneSelectionPanelBehaviour;

        private readonly Panel _viewportPanel = new Panel();
        private readonly Panel _zoneSelectionPanel = new Panel();
        private readonly Panel _canvasPanel;
        private IGraphicsManagerWrapper _graphicsManager;
        private readonly IDictionary<IReadOnlyZoneInfo, ZoneRenderInfo> _zoneRenderInfos;

        private bool _zoomStateChanged;

        public void ToggleZoom(ZoomMode mode)
        {
            switch (mode)
            {
                case ZoomMode.Full:
                    _tilesetAccessor.TileWidthAndSizeInPixels = 25;
                    break;
                case ZoomMode.Half:
                    _tilesetAccessor.TileWidthAndSizeInPixels = 12;
                    break;
                default:
                    throw new ArgumentException(String.Format("The given '{0}' is currently not supported.", mode), "mode");
            }
            _canvasPanel.Size = _tilesetAccessor.GetAreaSize(_simulationSession.Area);
            _zoomStateChanged = true;
        }

        public bool HandleKeyChar(char @char)
        {
            return _zoneSelectionPanelBehaviour.HandleKeyCharAction(@char);
        }

        private static void MoveScroll(Panel panel, Func<Panel, ScrollProperties> scrollPropertiesSelector, int modifier)
        {
            var scrollProperties = scrollPropertiesSelector(panel);

            int newValue = scrollProperties.Value + (((int)modifier * 300));
            if (newValue > scrollProperties.Minimum && newValue < scrollProperties.Maximum)
            {
                scrollProperties.Value = newValue;
            }
            else if (newValue <= scrollProperties.Minimum)
            {
                scrollProperties.Value = scrollProperties.Minimum;
            }
            else if (newValue >= scrollProperties.Maximum)
            {
                scrollProperties.Value = scrollProperties.Maximum;
            }
            else
            {
                throw new InvalidOperationException();
            }
            panel.ScrollControlIntoView(panel);
        }

        public void MoveRight()
        {
            MoveScroll(_viewportPanel, x => x.HorizontalScroll, 1);
        }

        public void MoveLeft()
        {
            MoveScroll(_viewportPanel, x => x.HorizontalScroll, -1);
        }

        public void MoveUp()
        {
            MoveScroll(_viewportPanel, x => x.VerticalScroll, -1);
        }

        public void MoveDown()
        {
            MoveScroll(_viewportPanel, x => x.VerticalScroll, 1);
        }

        public SimulationRenderHelper(Panel gamePanel, RenderZoneOptions renderZoneOptions, SimulationOptions options)
        {
            if (gamePanel == null) throw new ArgumentNullException("gamePanel");
            gamePanel.Controls.Clear();

            _simulationSession = new SimulationSession(options);

            _zoneSelectionPanel.Width = 160;
            _zoneSelectionPanel.Dock = DockStyle.Left;

            _viewportPanel.Dock = DockStyle.Fill;
            _viewportPanel.AutoScroll = true;

            gamePanel.Controls.Add(_viewportPanel);
            gamePanel.Controls.Add(_zoneSelectionPanel);

            _zoneSelectionPanel.BringToFront();
            _viewportPanel.BringToFront();

            if (renderZoneOptions == null) throw new ArgumentNullException("renderZoneOptions");

            _canvasPanel = new Panel
            {
                BackColor = EmptyZoneConsumption.DefaultColor,
                Size = _tilesetAccessor.GetAreaSize(_simulationSession.Area),
                Dock = DockStyle.None
            };

            _viewportPanel.Controls.Add(_canvasPanel);

            _zoneSelectionPanelBehaviour = new ZoneSelectionPanelCreator(
                area: _simulationSession.Area,
                targetPanel: _zoneSelectionPanel
            );

            MouseEventHandler eventHandler = (sender, e) =>
            {
                var point = _canvasPanel.PointToClient(Cursor.Position);
                var zone = GetZoneStateFor(point);

                var zoneConsumption = (e.Button == MouseButtons.Right)
                    ? new EmptyZoneConsumption()
                    : _zoneSelectionPanelBehaviour.CreateNewCurrentZoneConsumption();

                var result = _simulationSession.ConsumeZoneAt(zone, zoneConsumption);
                if (result == null) throw new InvalidOperationException();
            };

            _canvasPanel.MouseDown += eventHandler;
            _canvasPanel.MouseMove += (sender, e) =>
            {
                if (e.Button != MouseButtons.None && _zoneSelectionPanelBehaviour.IsCurrentlyNetworkZoning)
                {
                    eventHandler(sender, e);
                }
            };

            _zoneRenderInfos = _simulationSession.Area
                    .EnumerateZoneInfos()
                    .ToDictionary(x => x,
                    zoneRenderInfo =>
                        new ZoneRenderInfo(
                            zoneInfo: zoneRenderInfo,
                            createRectangle: zonePoint => new Rectangle(
                                x: zonePoint.Point.X * _tilesetAccessor.TileWidthAndSizeInPixels,
                                y: zonePoint.Point.Y * _tilesetAccessor.TileWidthAndSizeInPixels,
                                width: _tilesetAccessor.TileWidthAndSizeInPixels,
                                height: _tilesetAccessor.TileWidthAndSizeInPixels
                                ),
                            tilesetAccessor: _tilesetAccessor,
                            renderZoneOptions: renderZoneOptions
                        ));

            _graphicsManager = CreateGraphicsManagerWrapperWithFactory(renderZoneOptions.SelectedGraphicsManager.Factory);
        }

        private readonly object _locker = new object();

        private IGraphicsManagerWrapper CreateGraphicsManagerWrapperWithFactory(Func<Panel, Action, IGraphicsManagerWrapper> graphicsManagerWrapper)
        {
            return graphicsManagerWrapper(_canvasPanel, () =>
            {
                Point currentCursorPoint = new Point() { X = -100, Y = -100 };

                _canvasPanel.BeginInvoke(new MethodInvoker(() =>
                {
                    currentCursorPoint = _canvasPanel.PointToClient(Cursor.Position);
                }));

                var continuations = GetToBeRenderedAreas()
                    .Select(rect => rect.RenderZoneInto(_graphicsManager.GetGraphicsWrapper(), rect.GetRectangle().Contains(currentCursorPoint)))
                    .ToArray();

                foreach (var controller in new[]
                {
                    _simulationSession.Area.ShipController,
                    _simulationSession.Area.TrainController,
                    _simulationSession.Area.AirplaneController as IVehicleController<IMoveableVehicle>
                })
                {
                    if (controller == _simulationSession.Area.TrainController)
                    {
                        foreach (var continuation in continuations.Where(x => x.HasDrawSecondLayerDelegate))
                            continuation.DrawSecondLayer();
                    }
                    
                    controller.ForEachActiveVehicle(airplane =>
                    {
                        if (airplane.PreviousPreviousPreviousPreviousPosition == null)
                            return;

                        foreach (var pair in new[]
                    {
                        new { Render = (airplane is ITrain), First = airplane.CurrentPosition, Second = airplane.PreviousPosition, Third = airplane.PreviousPreviousPosition, Head = true},
                        new { Render = true, First = airplane.PreviousPosition, Second = airplane.PreviousPreviousPosition, Third = airplane.PreviousPreviousPreviousPosition, Head = false},
                        new { Render = (airplane is ITrain), First = airplane.PreviousPreviousPosition, Second = airplane.PreviousPreviousPreviousPosition, Third = airplane.PreviousPreviousPreviousPreviousPosition, Head = false}
                    })
                        {
                            var orientation = (pair.Third.Point != pair.First.Point)
                                ? pair.Third.Point.OrientationTo(pair.First.Point)
                                : pair.Second.Point.OrientationTo(pair.First.Point);

                            Bitmap bitmap;

                            if (airplane is IAirplane)
                                bitmap = MiscBitmaps.Plane.GetBitmap(orientation);
                            else if (airplane is ITrain)
                                bitmap = MiscBitmaps.Train.GetBitmap(orientation);
                            else if (airplane is IShip)
                                bitmap = MiscBitmaps.GetShipBitmapFrame().GetBitmap(orientation);
                            else
                                throw new InvalidOperationException();

                            if (pair.Render)
                            {
                                _graphicsManager.GetGraphicsWrapper().DrawImage(
                                    bitmap: bitmap,
                                    rectangle: _zoneRenderInfos[pair.Second]
                                        .GetRectangle()
                                        .ChangeSize(_tilesetAccessor.ResizeToTileWidthAndSize(bitmap.Size))
                                    );
                            }
                        }
                    });
                }

                var highlightaction = continuations.FirstOrDefault(x => x.HasDrawHighlighterDelegate);

                if (highlightaction != null)
                {
                    var consumption = _zoneSelectionPanelBehaviour.CreateNewCurrentZoneConsumption();
                    highlightaction.DrawHighlighter(consumption);
                }
            });
        }

        public void ChangeRenderer(Func<Panel, Action, IGraphicsManagerWrapper> graphicsManagerWrapperFactory)
        {
            lock (_locker)
            {
                _graphicsManager.Dispose();
                _graphicsManager = CreateGraphicsManagerWrapperWithFactory(graphicsManagerWrapperFactory);
                _graphicsManager.StartRendering();
            }
        }

        public void Start()
        {
            lock (_locker)
            {
                _graphicsManager.StartRendering();
                _simulationSession.StartSimulation();
            }
        }

        public void Stop()
        {
            lock (_locker)
            {
                _simulationSession.Dispose();
                _graphicsManager.Dispose();
            }
        }

        private bool IsVisibleInViewPort(Rectangle rect)
        {
            var visibleRectangle = new Rectangle
            {
                Size = _viewportPanel.Size,
                Location = new Point(
                    x: -_viewportPanel.AutoScrollPosition.X,
                    y: -_viewportPanel.AutoScrollPosition.Y
                )
            };
            return rect.IntersectsWith(visibleRectangle);
        }

        public ISimulationSession SimulationSession { get { return _simulationSession; } }

        private Rectangle _lastViewportRectangle = default(Rectangle);

        private readonly HashSet<ZoneRenderInfo> _toBeRenderedZoneInfosCache = new HashSet<ZoneRenderInfo>();

        public IEnumerable<ZoneRenderInfo> GetToBeRenderedAreas()
        {
            var currentViewportRectangle = new Rectangle
            {
                Size = _viewportPanel.Size,
                Location = new Point(
                    x: -_viewportPanel.AutoScrollPosition.X,
                    y: -_viewportPanel.AutoScrollPosition.Y
                )
            };

            if (!_lastViewportRectangle.Equals(currentViewportRectangle) || _zoomStateChanged || _toBeRenderedZoneInfosCache.Count == 0)
            {
                _zoomStateChanged = false;
                _lastViewportRectangle = currentViewportRectangle;

                _toBeRenderedZoneInfosCache.Clear();

                foreach (var x in _zoneRenderInfos.Where(rect => IsVisibleInViewPort(rect.Value.GetRectangle())))
                {
                    _toBeRenderedZoneInfosCache.Add(x.Value);
                }
            }

            return _toBeRenderedZoneInfosCache;
        }

        public IReadOnlyZoneInfo GetZoneStateFor(Point point)
        {
            return _simulationSession.Area
                .EnumerateZoneInfos()
                .Single(zonePoint =>
                    zonePoint.Point.X == (point.X / _tilesetAccessor.TileWidthAndSizeInPixels)
                    && zonePoint.Point.Y == (point.Y / _tilesetAccessor.TileWidthAndSizeInPixels)
                );
        }
    }
}
