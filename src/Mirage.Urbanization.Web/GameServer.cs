using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Mirage.Urbanization.Simulation;
using Mirage.Urbanization.Simulation.Datameters;
using Mirage.Urbanization.Simulation.Persistence;
using Mirage.Urbanization.Tilesets;
using Mirage.Urbanization.Web.ClientMessages;

namespace Mirage.Urbanization.Web
{
    public struct ClientDataMeterZoneInfo
    {
        public static ClientDataMeterZoneInfo Create(IReadOnlyZoneInfo zoneInfo, ZoneInfoDataMeter zoneInfoDataMeter)
        {
            return new ClientDataMeterZoneInfo
            {
                colour = BrushManager
                            .Instance
                            .GetBrushFor(zoneInfoDataMeter.GetDataMeterResult(zoneInfo).ValueCategory)
                            .WithResultIfHasMatch(brush => System.Drawing.ColorTranslator.ToHtml(brush.Color), string.Empty),
                x = zoneInfo.Point.X,
                y = zoneInfo.Point.Y
            };
        }

        public int x { get; set; }
        public int y { get; set; }
        public string colour { get; set; }
    }

    public class DataMeterPublishState
    {
        public ZoneInfoDataMeter DataMeter { get; }
        private readonly Func<IEnumerable<IReadOnlyZoneInfo>> _getZoneInfos;

        public DataMeterPublishState(ZoneInfoDataMeter dataMeter, Func<IEnumerable<IReadOnlyZoneInfo>> getZoneInfos)
        {
            DataMeter = dataMeter;
            _getZoneInfos = getZoneInfos;
        }

        public IEnumerable<ClientDataMeterZoneInfo> GetChanged()
        {
            var currentClientDataMeterStates = _getZoneInfos()
                .Select(x => ClientDataMeterZoneInfo.Create(x, DataMeter))
                .ToHashSet();

            foreach (var x in currentClientDataMeterStates
                .Except(_previousClientDataMeterStates))
                yield return x;

            _previousClientDataMeterStates = currentClientDataMeterStates;
        }

        private ISet<ClientDataMeterZoneInfo> _previousClientDataMeterStates = new HashSet<ClientDataMeterZoneInfo>(); 
    }

    public class DataMeterPublishStateManager
    {
        public IList<DataMeterPublishState> DataMeterPublishStates { get; }

        public DataMeterPublishStateManager(ISimulationSession session)
        {
            DataMeterPublishStates = DataMeterInstances
                .DataMeters
                .Select(x => new DataMeterPublishState(x, session.Area.EnumerateZoneInfos))
                .ToList();
        }
    }

    public class GameServer : IDisposable
    {
        public static GameServer Instance;

        private readonly ISimulationSession _simulationSession;
        private readonly string _url;
        private readonly bool _controlVehicles;
        private IDisposable _webServer;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly NeverEndingTask _looper, _dataMeterStateLooper;

        public ISimulationSession SimulationSession => _simulationSession;

        public GameServer(ISimulationSession simulationSession, string url, bool controlVehicles)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("", nameof(url));

            _simulationSession = simulationSession ?? throw new ArgumentNullException(nameof(simulationSession));
            _url = url;
            _controlVehicles = controlVehicles;

            List<ClientZoneInfo> previous = null;

            _simulationSession.OnAreaMessage += SimulationSession_OnAreaMessage;
            _simulationSession.OnYearAndOrMonthChanged += SimulationSession_OnYearAndOrMonthChanged;
            _simulationSession.OnCityBudgetValueChanged += SimulationSession_OnCityBudgetValueChanged;
            _simulationSession.OnAreaHotMessage += SimulationSession_OnAreaHotMessage;

            var zoneInfoBatchLooper = new LoopBatchEnumerator<IReadOnlyZoneInfo>(_simulationSession.Area.EnumerateZoneInfos().ToList());

            var dataMeterStateManager = new DataMeterPublishStateManager(simulationSession);

            _dataMeterStateLooper = new NeverEndingTask("Data meter state submission", async () =>
            {
                foreach (var x in dataMeterStateManager.DataMeterPublishStates)
                {
                    foreach (var batch in x.GetChanged().GetBatched(100))
                    {
                        GlobalHost
                            .ConnectionManager.GetHubContext<SimulationHub>()
                            .Clients
                            .Group(SimulationHub.GetDataMeterGroupName(x.DataMeter.WebId))
                            .submitDataMeterInfos(batch);
                        await Task.Delay(30);
                    }
                }

                await Task.Delay(30);
            }, _cancellationTokenSource.Token, 10);

            _looper = new NeverEndingTask("SignalR Game state submission", async () =>
            {
                GlobalHost
                    .ConnectionManager
                    .GetHubContext<SimulationHub>()
                    .Clients
                    .All
                    .submitZoneInfos(zoneInfoBatchLooper.GetBatch().Select(ClientZoneInfo.Create));

                await Task.Delay(40);

                var zoneInfos = _simulationSession.Area.EnumerateZoneInfos()
                    .Select(ClientZoneInfo.Create)
                    .ToList();

                var toBeSubmitted = zoneInfos;

                if (previous != null)
                {
                    var previousUids = previous.Select(x => x.GetIdentityString()).ToHashSet();

                    toBeSubmitted = zoneInfos.Where(z => !previousUids.Contains(z.GetIdentityString())).ToList();
                }

                foreach (var toBeSubmittedBatch in toBeSubmitted.GetBatched(20))
                {
                    GlobalHost
                        .ConnectionManager
                        .GetHubContext<SimulationHub>()
                        .Clients
                        .All
                        .submitZoneInfos(toBeSubmittedBatch);
                    await Task.Delay(20);
                }

                previous = zoneInfos;

                await Task.Delay(40);

                try
                {
                    var list = new List<ClientVehiclePositionInfo>();
                    foreach (var vehicleController in _simulationSession.Area.EnumerateVehicleControllers())
                    {
                        vehicleController
                            .ForEachActiveVehicle(_controlVehicles,
                                vehicle =>
                                {
                                    list.AddRange(TilesetProvider
                                        .GetBitmapsAndPointsFor(vehicle)
                                        .Select(ClientVehiclePositionInfo.Create)
                                    );
                                });
                    }

                    GlobalHost
                        .ConnectionManager
                        .GetHubContext<SimulationHub>()
                        .Clients
                        .All
                        .submitVehicleStates(list);

                    await Task.Delay(6);
                }
                catch (Exception ex)
                {
                    Logger.Instance.WriteLine("Possible race condition-based exception:: " + ex);
                }

            }, _cancellationTokenSource.Token, 10);

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
                    currentAmount = e.EventData.CurrentAmount,
                    projectedIncome = e.EventData.ProjectedIncome
                });
        }

        private void SimulationSession_OnYearAndOrMonthChanged(object sender, EventArgsWithData<IYearAndMonth> e)
        {
            SimulationSession.GetRecentStatistics().WithResultIfHasMatch(cityStatistics =>
            {
                var cityStatisticsView = new CityStatisticsView(cityStatistics);

                GlobalHost
                    .ConnectionManager
                    .GetHubContext<SimulationHub>()
                    .Clients
                    .All
                    .onYearAndMonthChanged(new YearAndMonthChangedState
                    {
                        yearAndMonthDescription = e.EventData.GetCurrentDescription(),
                        overallLabelsAndValues = new[]
                        {
                            new LabelAndValue { label = "Population", value = cityStatisticsView.Population.ToString("N0") },
                            new LabelAndValue { label = "Assessed value", value = cityStatisticsView.AssessedValue.ToString("C0") },
                            new LabelAndValue { label = "Category", value = cityStatisticsView.CityCategory }
                        },
                        generalOpinion = new[]
                        {
                            new { Opinion = cityStatisticsView.GetPositiveOpinion(), Label = "Positive" },
                            new { Opinion = cityStatisticsView.GetNegativeOpinion(), Label = "Negative" }
                        }
                        .OrderByDescending(y => y.Opinion)
                        .Select(y => new LabelAndValue { label = $"{y.Label}", value = $"{y.Opinion.ToString("P1")}" })
                        .ToArray(),
                        cityBudgetLabelsAndValues = new[]
                        {
                            new LabelAndValue { label = "Current funds", value = cityStatisticsView.CurrentAmountOfFunds.ToString()},
                            new LabelAndValue { label = "Projected income", value = cityStatisticsView.CurrentProjectedAmountOfFunds.ToString()},
                        },
                        issueLabelAndValues = cityStatisticsView
                            .GetIssueDataMeterResults()
                            .Select(x => new LabelAndValue()
                            {
                                label = x.Name,
                                value = $"{x.ValueCategory} ({x.PercentageScoreString}%)"
                            })
                            .ToArray()
                    });
            });
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
            _dataMeterStateLooper.Start();
        }

        public void Dispose()
        {
            _simulationSession.OnAreaMessage -= SimulationSession_OnAreaMessage;
            _simulationSession.OnYearAndOrMonthChanged -= SimulationSession_OnYearAndOrMonthChanged;
            _simulationSession.OnCityBudgetValueChanged -= SimulationSession_OnCityBudgetValueChanged;
            _simulationSession.OnAreaHotMessage -= SimulationSession_OnAreaHotMessage;
            _cancellationTokenSource.Cancel();
            _looper.Wait();
            _dataMeterStateLooper.Wait();
            _webServer?.Dispose();
            Instance = null;
        }
    }
}