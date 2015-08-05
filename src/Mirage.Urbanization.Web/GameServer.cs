using System;
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
        }

        public void Dispose() => _webServer?.Dispose();
    }
}