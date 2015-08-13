using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Mirage.Urbanization.Simulation;

namespace Mirage.Urbanization.Web
{
    public class GameServer : IDisposable
    {
        public static GameServer Instance;

        private readonly ISimulationSession _simulationSession;
        private readonly string _url;
        private IDisposable _webServer;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly NeverEndingTask _looper;

        public ISimulationSession SimulationSession => _simulationSession;

        public GameServer(ISimulationSession simulationSession, string url)
        {
            if (simulationSession == null)
                throw new ArgumentNullException(nameof(simulationSession));
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("", nameof(url));

            _simulationSession = simulationSession;
            _url = url;

            simulationSession.Area.ZoneInfoUpdated += (sender, e) =>
            {
                GlobalHost
                    .ConnectionManager
                    .GetHubContext<SimulationHub>()
                    .Clients
                    .All
                    .submitAndDraw(e.ZoneInfo.ToClientZoneInfo());
            };

            List<ClientZoneInfo> previous = null;

            _looper = new NeverEndingTask("asd", () =>
            {
                var zoneInfos = _simulationSession.Area.EnumerateZoneInfos()
                    .Select(zoneInfo => zoneInfo.ToClientZoneInfo()).ToList();

                var toBeSubmitted = zoneInfos;

                if (previous != null)
                {
                    var previousUids = previous.Select(x => x.GetIdentityString()).ToHashSet();

                    toBeSubmitted = zoneInfos.Where(z => !previousUids.Contains(z.GetIdentityString())).ToList();

                }

                GlobalHost
                    .ConnectionManager
                    .GetHubContext<SimulationHub>()
                    .Clients
                    .All
                    .submitZoneInfos(toBeSubmitted);

                previous = zoneInfos;

            }, _cancellationTokenSource.Token, 250);

            if (Instance == null)
                Instance = this;
            else throw new InvalidOperationException();
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
            Instance = null;
        }
    }
}