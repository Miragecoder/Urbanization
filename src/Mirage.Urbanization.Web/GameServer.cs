using System;
using System.Linq;
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

        public GameServer(ISimulationSession simulationSession, string url)
        {
            if (simulationSession == null)
                throw new ArgumentNullException(nameof(simulationSession));
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("", nameof(url));

            _simulationSession = simulationSession;
            _url = url;
        }

        public void StartServer()
        {
            _webServer = Microsoft.Owin.Hosting.WebApp.Start<Startup>(_url);
            SimulationHub.CurrentSimulation.Set(_simulationSession);

            Task.Run(() =>
            {
                while (!_cancelled)
                {
                    var hubContext = GlobalHost.ConnectionManager.GetHubContext<SimulationHub>();
                    var zoneInfos = _simulationSession.Area.EnumerateZoneInfos()
                        .Select(zoneInfo => new
                        {
                            key = $"{zoneInfo.Point.X}_{zoneInfo.Point.Y}",
                            bitmapLayerOne = TilesetProvider
                                .GetTilePathFor(zoneInfo.ZoneConsumptionState.GetZoneConsumption(), x => x.LayerOne),
                            bitmapLayerTwo = TilesetProvider
                                .GetTilePathFor(zoneInfo.ZoneConsumptionState.GetZoneConsumption(), x => x.LayerTwo),
                            point = new
                            {
                                x = zoneInfo.Point.X,
                                y = zoneInfo.Point.Y
                            },
                            color = zoneInfo.ZoneConsumptionState.GetZoneConsumption().ColorName,
                            zoneConsumption = zoneInfo.ZoneConsumptionState.GetZoneConsumption().Name,
                            landValue = zoneInfo.GetLastLandValueResult().WithResultIfHasMatch(r => r.ValueInUnits),
                            lastTravelDistance = zoneInfo.GetLastAverageTravelDistance(),
                            crime = zoneInfo.GetLastQueryCrimeResult().WithResultIfHasMatch(r => r.ValueInUnits),
                            fireHazard = zoneInfo.GetLastQueryFireHazardResult().WithResultIfHasMatch(r => r.ValueInUnits),
                            pollution = zoneInfo.GetLastQueryPollutionResult().WithResultIfHasMatch(r => r.ValueInUnits)
                        }).ToArray();

                    hubContext.Clients.All.submitZoneInfos(zoneInfos);
                }
            });
        }

        private bool _cancelled;

        public void Dispose()
        {
            _cancelled = true;
            _webServer?.Dispose();
        }
    }
}