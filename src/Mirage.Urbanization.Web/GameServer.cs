using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Mirage.Urbanization.Simulation;

namespace Mirage.Urbanization.Web
{
    public class GameServer : IDisposable
    {
        private readonly ISimulationSession _simulationSession;
        private readonly string _url;
        private IDisposable _webServer;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly NeverEndingTask _looper;

        public GameServer(ISimulationSession simulationSession, string url)
        {
            if (simulationSession == null)
                throw new ArgumentNullException(nameof(simulationSession));
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("", nameof(url));

            _simulationSession = simulationSession;
            _url = url;
            _looper = new NeverEndingTask("asd", () =>
            {
                var zoneInfos = _simulationSession.Area.EnumerateZoneInfos()
                    .Select(zoneInfo => new ClientZoneInfo
                    {
                        key = $"{zoneInfo.Point.X}_{zoneInfo.Point.Y}",
                        bitmapLayerOne = TilesetProvider
                            .GetTilePathFor(zoneInfo.ZoneConsumptionState.GetZoneConsumption(), x => x.LayerOne),
                        bitmapLayerTwo = TilesetProvider
                            .GetTilePathFor(zoneInfo.ZoneConsumptionState.GetZoneConsumption(), x => x.LayerTwo),
                        point = new ClientZonePoint
                        {
                            x = zoneInfo.Point.X,
                            y = zoneInfo.Point.Y
                        },
                        color = zoneInfo.ZoneConsumptionState.GetZoneConsumption().ColorName,
                    }).ToList();

                GlobalHost
                    .ConnectionManager
                    .GetHubContext<SimulationHub>()
                    .Clients
                    .All
                    .submitZoneInfos(zoneInfos);
            }, _cancellationTokenSource.Token);
        }

        public void StartServer()
        {
            _webServer = Microsoft.Owin.Hosting.WebApp.Start<Startup>(_url);
            _looper.Start();
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _looper.Wait();
            _webServer?.Dispose();
        }
    }
}