using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Mirage.Urbanization.Simulation;
using Mirage.Urbanization.Simulation.Persistence;

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
                    .submitAndDraw(ClientZoneInfo.Create(e.ZoneInfo));
            };

            List<ClientZoneInfo> previous = null;
            
            _simulationSession.OnAreaMessage += SimulationSession_OnAreaMessage;
            _simulationSession.OnYearAndOrMonthChanged += SimulationSession_OnYearAndOrMonthChanged;
            _simulationSession.OnCityBudgetValueChanged += SimulationSession_OnCityBudgetValueChanged;
            _simulationSession.OnAreaHotMessage += SimulationSession_OnAreaHotMessage;

            _looper = new NeverEndingTask("SignalR Game state submission", () =>
            {
                var zoneInfos = _simulationSession.Area.EnumerateZoneInfos()
                    .Select(ClientZoneInfo.Create).ToList();

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

        private void SimulationSession_OnAreaHotMessage(object sender, SimulationSessionHotMessageEventArgs e)
        {
            GlobalHost
                .ConnectionManager
                .GetHubContext<SimulationHub>()
                .Clients
                .All
                .submitAreaHotMessage(new
                {
                    title = e.Title,
                    message = e.Message
                });
        }

        private readonly CityBudgetPanelPublisher _cityBudgetPanelPublisher = new CityBudgetPanelPublisher();

        private void SimulationSession_OnCityBudgetValueChanged(object sender, CityBudgetValueChangedEventArgs e)
        {
            GlobalHost
                .ConnectionManager
                .GetHubContext<SimulationHub>()
                .Clients
                .All
                .submitCityBudgetValue(new
                {
                    cityBudgetState = _cityBudgetPanelPublisher.GenerateCityBudgetState(_simulationSession),
                    currentAmount = e.EventData.CurrentAmountDescription,
                    projectedIncome = e.EventData.ProjectedIncomeDescription
                });
        }

        private void SimulationSession_OnYearAndOrMonthChanged(object sender, EventArgsWithData<IYearAndMonth> e)
        {
            GlobalHost
                .ConnectionManager
                .GetHubContext<SimulationHub>()
                .Clients
                .All
                .submitYearAndMonth(e.EventData.GetCurrentDescription());
        }

        private static void SimulationSession_OnAreaMessage(object sender, SimulationSessionMessageEventArgs e)
        {
            GlobalHost
                .ConnectionManager
                .GetHubContext<SimulationHub>()
                .Clients
                .All
                .submitAreaMessage(e.Message);
        }

        public void StartServer()
        {
            _webServer = Microsoft.Owin.Hosting.WebApp.Start<Startup>(_url);
            _looper.Start();
        }

        public void Dispose()
        {
            _simulationSession.OnAreaMessage -= SimulationSession_OnAreaMessage;
            _simulationSession.OnYearAndOrMonthChanged -= SimulationSession_OnYearAndOrMonthChanged;
            _simulationSession.OnCityBudgetValueChanged -= SimulationSession_OnCityBudgetValueChanged;
            _simulationSession.OnAreaHotMessage -= SimulationSession_OnAreaHotMessage;
            _cancellationTokenSource.Cancel();
            _looper.Wait();
            _webServer?.Dispose();
            Instance = null;
        }
    }
}