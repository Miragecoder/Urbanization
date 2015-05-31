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

            _trainRenderState = new TrainRenderState(() => _zoneRenderInfos);

            _graphicsManager = CreateGraphicsManagerWrapperWithFactory(renderZoneOptions.SelectedGraphicsManager.Factory);
        }

        private readonly object _locker = new object();

        private IGraphicsManagerWrapper CreateGraphicsManagerWrapperWithFactory(Func<Panel, Action, IGraphicsManagerWrapper> graphicsManagerWrapper)
        {
            return graphicsManagerWrapper(_canvasPanel, () =>
            {
                Action<IAreaConsumption> highlightAction = null;

                Point currentCursorPoint = new Point() { X = -100, Y = -100 };

                _canvasPanel.BeginInvoke(new MethodInvoker(() =>
                {
                    currentCursorPoint = _canvasPanel.PointToClient(Cursor.Position);
                }));

                foreach (var rect in GetToBeRenderedAreas())
                {
                    var result = rect.RenderZoneInto(_graphicsManager.GetGraphicsWrapper(), rect.GetRectangle().Contains(currentCursorPoint));
                    if (result != null) highlightAction = result;
                }

                _trainRenderState.Render(_graphicsManager.GetGraphicsWrapper());

                if (highlightAction != null)
                {
                    var consumption = _zoneSelectionPanelBehaviour.CreateNewCurrentZoneConsumption();
                    highlightAction(consumption);
                }
            });
        }

        private readonly TrainRenderState _trainRenderState;

        private class TrainRenderState
        {
            private readonly Func<IDictionary<IReadOnlyZoneInfo, ZoneRenderInfo>> _getZoneRenderInfosFunc;

            public TrainRenderState(Func<IDictionary<IReadOnlyZoneInfo, ZoneRenderInfo>> getZoneRenderInfosFunc)
            {
                _getZoneRenderInfosFunc = getZoneRenderInfosFunc;

                _cachedNetworks = new SimpleCache<ISet<ISet<ZoneRenderInfo>>>(GetRailwayNetworks, new TimeSpan(0, 0, 1));
            }

            internal void Render(IGraphicsWrapper graphicsWrapper)
            {
                var cachedNetworksEntry = _cachedNetworks.GetValue();

                if (!cachedNetworksEntry.SelectMany(x => x).Any())
                    return;

                foreach (var network in cachedNetworksEntry.Where(x => x.Count() > 20))
                {
                    var desiredAmountOfTrains = Math.Abs(network.Count() / 50) + 1;

                    List<Train> trainsInNetwork = null;

                    while (trainsInNetwork == null || trainsInNetwork.Count() < desiredAmountOfTrains)
                    {
                        trainsInNetwork = _trains
                            .Where(x => network.Contains(x.CurrentPosition))
                            .ToList();

                        int desiredAdditionaTrains = desiredAmountOfTrains - trainsInNetwork.Count;

                        if (desiredAdditionaTrains > 0)
                        {
                            foreach (var iteration in Enumerable.Range(0, desiredAmountOfTrains - trainsInNetwork.Count))
                            {
                                _trains.Add(new Train(_getZoneRenderInfosFunc, network
                                    .OrderBy(x => Random.Next())
                                    .First()
                                ));
                            }
                        }
                    }

                    foreach (var train in trainsInNetwork)
                    {
                        train.CrawlNetwork(network);
                    }
                }

                foreach (var orphanTrain in _trains.Where(x => x.CanBeRemoved).ToArray())
                    _trains.Remove(orphanTrain);

                foreach (var train in _trains)
                {
                    train.DrawInto(graphicsWrapper);
                }
            }

            private readonly HashSet<Train> _trains = new HashSet<Train>();

            private static readonly Random Random = new Random();

            private class Train
            {
                private ZoneRenderInfo _currentPosition;
                private ZoneRenderInfo _previousPosition;
                private ZoneRenderInfo _previousPreviousPosition;
                private ZoneRenderInfo _previousPreviousPreviousPosition;
                private ZoneRenderInfo _previousPreviousPreviousPreviousPosition;

                public ZoneRenderInfo CurrentPosition { get { return _currentPosition; } }

                public void DrawInto(IGraphicsWrapper graphicsWrapper)
                {
                    if (_previousPreviousPreviousPreviousPosition == null)
                        return;

                    foreach (var pair in new[]
                    {
                        new { Render = true, First = _currentPosition, Second = _previousPosition, Third = _previousPreviousPosition, Head = true},
                        new { Render = true, First = _previousPosition, Second = _previousPreviousPosition, Third = _previousPreviousPreviousPosition, Head = false},
                        new { Render = true, First = _previousPreviousPosition, Second = _previousPreviousPreviousPosition, Third = _previousPreviousPreviousPreviousPosition, Head = false}
                    })
                    {
                        if (pair.Third.ZoneInfo.Point == pair.First.ZoneInfo.Point)
                            continue;

                        var orientation = pair.Third.ZoneInfo.Point.OrientationTo(pair.First.ZoneInfo.Point);

                        var bitmap = MiscBitmaps.GetTrainBitmap(orientation);

                        if (pair.Render)
                        {
                            graphicsWrapper.DrawImage(
                                bitmap: bitmap,
                                rectangle: pair.Second
                                    .GetRectangle()
                                    .ChangeSize(bitmap.Size)
                                    .Relocate(currentLocation =>
                                    {
                                        if (orientation.HasFlag(Orientation.East))
                                            currentLocation.Y += 16;
                                        if (orientation.HasFlag(Orientation.West))
                                            currentLocation.Y += 3;
                                        if (orientation.HasFlag(Orientation.North))
                                            currentLocation.X += 16;
                                        if (orientation.HasFlag(Orientation.South))
                                            currentLocation.X += 3;

                                        bool multiple = !new[] { Orientation.North, Orientation.East, Orientation.South, Orientation.West }
                                            .Any(x => x == orientation);

                                        if (multiple)
                                        {
                                            if (orientation.HasFlag(Orientation.East))
                                                currentLocation.X -= 3;
                                            if (orientation.HasFlag(Orientation.West))
                                                currentLocation.X -= 6;
                                            //if (orientation.HasFlag(Orientation.North))
                                            //    currentLocation.Y += 3;
                                            if (orientation.HasFlag(Orientation.South))
                                                currentLocation.Y -= 3;
                                        }

                                        return currentLocation;
                                    }
                                )
                            );

                        }
                    }
                }

                public bool CanBeRemoved
                {
                    get
                    {
                        return _currentPosition == null || _lastChange < DateTime.Now.AddSeconds(-3);
                    }
                }

                private readonly Func<IDictionary<IReadOnlyZoneInfo, ZoneRenderInfo>> _getZoneRenderInfosFunc;

                public Train(Func<IDictionary<IReadOnlyZoneInfo, ZoneRenderInfo>> getZoneRenderInfosFunc, ZoneRenderInfo currentPosition)
                {
                    _getZoneRenderInfosFunc = getZoneRenderInfosFunc;
                    _currentPosition = currentPosition;
                }

                private DateTime _lastChange = DateTime.Now;

                public void CrawlNetwork(ISet<ZoneRenderInfo> trainNetwork)
                {
                    if (_lastChange > DateTime.Now.AddMilliseconds(-300))
                    {
                        return;
                    }
                    _lastChange = DateTime.Now;
                    if (!trainNetwork.Contains(_currentPosition))
                    {
                        _currentPosition = trainNetwork.First();
                    }
                    else
                    {
                        var queryNext = _currentPosition
                            .ZoneInfo
                            .GetNorthEastSouthWest()
                            .OrderBy(x => Random.Next())
                            .Where(x => x.HasMatch)
                            .Select(x => x.MatchingObject)
                            .Where(x => trainNetwork.Select(y => y.ZoneInfo).Contains(x))
                            .Select(x => _getZoneRenderInfosFunc()[x])
                            .AsQueryable();

                        var next = queryNext
                            .FirstOrDefault(x => x != _previousPosition && x != _currentPosition)
                            ?? queryNext.FirstOrDefault();

                        _previousPreviousPreviousPreviousPosition = _previousPreviousPreviousPosition;
                        _previousPreviousPreviousPosition = _previousPreviousPosition;
                        _previousPreviousPosition = _previousPosition;

                        _previousPosition = _currentPosition;

                        _currentPosition = next;
                    }
                }
            }

            private readonly SimpleCache<ISet<ISet<ZoneRenderInfo>>> _cachedNetworks;

            private ISet<ISet<ZoneRenderInfo>> GetRailwayNetworks()
            {
                var railwayNetworks = new HashSet<ISet<ZoneRenderInfo>>();
                foreach (var railroadZoneInfo in _getZoneRenderInfosFunc()
                    .Where(x => x.Key.ZoneConsumptionState.GetIsRailroadNetworkMember())
                    .Where(x => !railwayNetworks.SelectMany(y => y).Contains(x.Value)))
                {
                    var railwayNetwork = new HashSet<ZoneRenderInfo> { railroadZoneInfo.Value };

                    foreach (var member in railroadZoneInfo
                        .Key
                        .CrawlAllDirections(x => x.ConsumptionState.GetIsRailroadNetworkMember())
                        )
                    {
                        railwayNetwork.Add(_getZoneRenderInfosFunc().First(x => x.Key == member).Value);
                    }

                    railwayNetworks.Add(railwayNetwork);
                }
                return railwayNetworks;
            }
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
