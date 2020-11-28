using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Mirage.Urbanization.Simulation;
using Mirage.Urbanization.Simulation.Datameters;
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
                colour = DatameterColourDefinitions
                            .Instance
                            .GetColorFor(zoneInfoDataMeter.GetDataMeterResult(zoneInfo).ValueCategory)
                            .Pipe(x => x.HasValue ? x.Value.ToHex() : string.Empty),
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
                    await Startup.WithSimulationHub(async hub =>
                    {
                        foreach (var batch in x.GetChanged().GetBatched(100))
                        {
                            await hub
                                .Clients
                                .Group(SimulationHub.GetDataMeterGroupName(x.DataMeter.WebId))
                                .SendAsync("submitDataMeterInfos", batch);
                        }
                    });
                }
            }, _cancellationTokenSource.Token, 10);

            _looper = new NeverEndingTask("SignalR Game state submission", async () =>
            {
                await Startup.WithSimulationHub(async simulationHub =>
                {
                    await simulationHub
                        .Clients
                        .All
                        .SendAsync("submitZoneInfos", zoneInfoBatchLooper.GetBatch().Select(ClientZoneInfo.Create), _cancellationTokenSource.Token);

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
                        await simulationHub
                            .Clients
                            .All
                            .SendAsync("submitZoneInfos", toBeSubmittedBatch);
                    }

                    previous = zoneInfos;

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

                        await simulationHub
                            .Clients
                            .All
                            .SendAsync("submitVehicleStates", list);
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.WriteLine("Possible race condition-based exception:: " + ex);
                    }
                });
            }, _cancellationTokenSource.Token, 10);

            if (Instance == null)
                Instance = this;
            else throw new InvalidOperationException();
        }

        private async void SimulationSession_OnAreaHotMessage(object sender, SimulationSessionHotMessageEventArgs e)
        {
            try
            {
                await Startup.WithSimulationHub(async simulationHub =>
                {
                    await simulationHub
                        .Clients
                        .All
                        .SendAsync("submitAreaHotMessage", new
                        {
                            title = e.Title,
                            message = e.Message
                        });
                });

            }
            catch (Exception ex)
            {
                Logger.Instance.WriteLine(ex);
            }
        }

        private readonly CityBudgetPanelPublisher _cityBudgetPanelPublisher = new CityBudgetPanelPublisher();

        private async void SimulationSession_OnCityBudgetValueChanged(object sender, CityBudgetValueChangedEventArgs e)
        {
            try
            {

                await Startup.WithSimulationHub(async simulationHub =>
                {
                    await simulationHub
                        .Clients
                        .All
                        .SendAsync("submitCityBudgetValue", new
                        {
                            cityBudgetState = _cityBudgetPanelPublisher.GenerateCityBudgetState(_simulationSession),
                            currentAmount = e.EventData.CurrentAmount,
                            projectedIncome = e.EventData.ProjectedIncome
                        });
                });
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteLine(ex);
            }
        }

        private async void SimulationSession_OnYearAndOrMonthChanged(object sender, EventArgsWithData<IYearAndMonth> e)
        {
            try
            {
                await Startup.WithSimulationHub(async simulationHub =>
                {
                    await SimulationSession.GetRecentStatistics().WithResultIfHasMatch(cityStatistics =>
                    {
                        var cityStatisticsView = new CityStatisticsView(cityStatistics);

                        return simulationHub
                            .Clients
                            .All
                            .SendAsync("onYearAndMonthChanged", new YearAndMonthChangedState
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
                                .Select(y => new LabelAndValue { label = $"{y.Label}", value = $"{y.Opinion:P1}" })
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
                });
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteLine(ex);
            }
        }

        private static async void SimulationSession_OnAreaMessage(object sender, SimulationSessionMessageEventArgs e)
        {
            try
            {

                await Startup.WithSimulationHub(async simulationHub =>
                {
                    await simulationHub
                        .Clients
                        .All
                        .SendAsync("submitAreaMessage", e.Message);
                });
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteLine(ex);
            }
        }

        public void StartServer()
        {
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
            Instance = null;
        }
    }
}