using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mirage.Urbanization.Simulation.Persistence;
using Mirage.Urbanization.Statistics;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Simulation
{
    public class SimulationSession : ISimulationSession
    {
        private readonly SimulationOptions _simulationOptions;
        private readonly PersistedCityStatisticsCollection _persistedCityStatisticsCollection = new PersistedCityStatisticsCollection();

        private readonly PersistedCityBudgetConfiguration _cityBudgetConfiguration;

        private readonly Area _area;
        private readonly Task _growthSimulationTask, _crimeAndPollutionTask, _powerTask;
        private readonly YearAndMonth _yearAndMonth = new YearAndMonth();
        private IPowerGridStatistics _lastPowerGridStatistics;
        private IMiscCityStatistics _lastMiscCityStatistics;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public IReadOnlyArea Area => _area;

        private class InsufficientFundsAreaConsumptionResult : IAreaConsumptionResult
        {
            public InsufficientFundsAreaConsumptionResult(IAreaConsumption consumption)
            {
                if (consumption == null) throw new ArgumentNullException(nameof(consumption));
                AreaConsumption = consumption;
            }

            public bool Success => false;
            public IAreaConsumption AreaConsumption { get; }

            public string Message => "You currently do not have enough funds to build a " + AreaConsumption.Name;
        }

        public IAreaConsumptionResult ConsumeZoneAt(IReadOnlyZoneInfo readOnlyZoneInfo, IAreaConsumption consumption)
        {
            if (_cityBudget.CurrentAmount < consumption.Cost && !_simulationOptions.ProcessOptions.GetIsMoneyCheatEnabled())
            {
                var insufficientFundsResult = new InsufficientFundsAreaConsumptionResult(consumption);
                RaiseAreaHotMessageEvent("Insufficient funds", insufficientFundsResult.Message);
                return insufficientFundsResult;
            }

            var result = _area.ConsumeZoneAt(readOnlyZoneInfo, consumption);
            if (result.Success)
                _cityBudget.Handle(result);
            return result;
        }

        private bool PowerAndMiscStatisticsLoaded => (_lastPowerGridStatistics != null && _lastMiscCityStatistics != null);

        private readonly HashSet<CityCategoryDefinition> _seenCityStates = new HashSet<CityCategoryDefinition>
        {
            CityCategoryDefinition.Village
        };

        private void OnWeekPass(object sender, YearAndMonthWeekElapsedEventArgs e)
        {
            var recent = GetRecentStatistics();
            if (recent.HasMatch)
            {
                var category = CityCategoryDefinition
                    .GetForPopulation(recent.MatchingObject.PersistedCityStatistics.GlobalZonePopulationStatistics.Sum);
                if (_seenCityStates.Add(category)
                    && _seenCityStates.OrderByDescending(x => x.MinimumPopulation).First() == category)
                {
                    RaiseAreaHotMessageEvent("Your city has grown!", "Congratulations! Your city has grown into a " + category.Name + "!");
                }
            }
        }

        public SimulationSession(SimulationOptions simulationOptions)
        {
            _simulationOptions = simulationOptions;
            _area = new Area(simulationOptions.GetAreaOptions());

            _area.OnAreaConsumptionResult += HandleAreaConsumptionResult;
            _area.OnAreaMessage += (s, e) => RaiseAreaMessageEvent(e.Message);

            PersistedCityBudgetConfiguration persistedCityBudgetConfiguration = null;

            simulationOptions.WithPersistedSimulation(persistedSimulation =>
            {
                if (persistedSimulation.PersistedCityStatistics == null)
                    return;
                foreach (var x in persistedSimulation.PersistedCityStatistics)
                    _persistedCityStatisticsCollection.Add(x);

                var last =
                    persistedSimulation.PersistedCityStatistics.OrderByDescending(
                        x => x.PersistedCityStatistics.TimeCode).First();

                _yearAndMonth.LoadTimeCode(last.PersistedCityStatistics.TimeCode);

                _yearAndMonth.AddWeek();

                _cityBudget.RestoreFrom(last);

                persistedCityBudgetConfiguration = persistedSimulation.PersistedCityBudgetConfiguration;
            });

            _cityBudgetConfiguration = persistedCityBudgetConfiguration ?? new PersistedCityBudgetConfiguration();

            _growthSimulationTask = new Task(() =>
            {
                while (true)
                {
                    OnYearAndOrMonthChanged?.Invoke(this, new EventArgsWithData<IYearAndMonth>(_yearAndMonth));
                    foreach (var iteration in Enumerable.Range(0, 4))
                    {
                        if (PowerAndMiscStatisticsLoaded)
                        {
                            var growthZoneStatistics = _area.PerformGrowthSimulationCycle(_cancellationTokenSource.Token).Result;
                            var onYearAndOrMonthChanged = OnYearAndOrMonthChanged;
                            onYearAndOrMonthChanged?.Invoke(this, new EventArgsWithData<IYearAndMonth>(_yearAndMonth));

                            _persistedCityStatisticsCollection.Add(
                                   _cityBudget.ProcessFinances(new PersistedCityStatistics(
                                        timeCode: _yearAndMonth.TimeCode,
                                        powerGridStatistics: _lastPowerGridStatistics,
                                        growthZoneStatistics: growthZoneStatistics,
                                        miscCityStatistics: _lastMiscCityStatistics
                                    ),
                                    _cityBudgetConfiguration
                                )
                            );

                            CityStatisticsUpdated?.Invoke(this, new CityStatisticsUpdatedEventArgs(_persistedCityStatisticsCollection.GetMostRecentPersistedCityStatistics().MatchingObject));
                            _yearAndMonth.AddWeek();
                            if (_yearAndMonth.IsAtBeginningOfNewYear)
                                _cityBudget.AddProjectedIncomeToCurrentAmount();
                        }
                        _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        Task.Delay(2000).Wait();
                    }
                }
            }, _cancellationTokenSource.Token);
            _powerTask = new Task(() =>
            {
                while (true)
                {
                    var stopwatch = new Stopwatch();

                    stopwatch.Start();
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    _lastPowerGridStatistics = _area.CalculatePowergridStatistics(_cancellationTokenSource.Token).Result;
                    Console.WriteLine("Power grid scan completed. (Took: '{0}')", stopwatch.Elapsed);

                    Task.Delay(2000).Wait();
                }
            }, _cancellationTokenSource.Token);
            _crimeAndPollutionTask = new Task(() =>
            {
                while (true)
                {
                    var stopwatch = new Stopwatch();

                    stopwatch.Start();

                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var queryCrimeResults = new HashSet<IQueryCrimeResult>(_area.EnumerateZoneInfos().Select(x => x.QueryCrime()).Where(x => x.HasMatch).Select(x => x.MatchingObject));

                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var queryLandValueResults = new HashSet<IQueryLandValueResult>(_area.EnumerateZoneInfos().Select(x => x.QueryLandValue()).Where(x => x.HasMatch).Select(x => x.MatchingObject));

                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var queryPollutionResults = new HashSet<IQueryPollutionResult>(_area.EnumerateZoneInfos().Select(x => x.QueryPollution()).Where(x => x.HasMatch).Select(x => x.MatchingObject));

                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var queryFireHazardResults = new HashSet<IQueryFireHazardResult>(_area.EnumerateZoneInfos().Select(x => x.QueryFireHazard()).Where(x => x.HasMatch).Select(x => x.MatchingObject));

                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    var travelDistances = new HashSet<BaseGrowthZoneClusterConsumption>(
                            _area
                            .EnumerateZoneInfos()
                            .Select(x => x.ConsumptionState.GetZoneConsumption())
                            .OfType<ZoneClusterMemberConsumption>()
                            .Where(x => x.IsCentralClusterMember)
                            .Select(x => x.ParentBaseZoneClusterConsumption)
                            .OfType<BaseGrowthZoneClusterConsumption>()
                        )
                        .Select(x => x.ZoneClusterMembers.First(y => y.IsCentralClusterMember))
                        .Select(x => x.GetZoneInfo())
                        .Select(x => x.WithResultIfHasMatch(y => y.GetLastAverageTravelDistance()))
                        .Where(x => x.HasValue)
                        .Select(x => x.Value);

                    _lastMiscCityStatistics = new MiscCityStatistics(
                        queryCrimeResults: queryCrimeResults,
                        queryFireHazardResults: queryFireHazardResults,
                        queryLandValueResults: queryLandValueResults,
                        queryPollutionResults: queryPollutionResults,
                        queryTravelDistanceResults: travelDistances
                    );

                    Console.WriteLine("Crime and pollution scan completed. (Took: '{0}')", stopwatch.Elapsed);

                    Task.Delay(2000).Wait();
                }
            }, _cancellationTokenSource.Token);


            _cityBudget.OnCityBudgetValueChanged += (sender, e) =>
            {
                OnCityBudgetValueChanged?.Invoke(this, e);
            };

            _yearAndMonth.OnWeekElapsed += OnWeekPass;
        }

        public void StartSimulation()
        {
            _cityBudget.RaiseCityBudgetValueChangedEvent();
            _powerTask.Start();
            _growthSimulationTask.Start();
            _crimeAndPollutionTask.Start();

            RaiseAreaMessageEvent("Ready");
        }

        public void Dispose()
        {
            Console.WriteLine("Killing simulation...");

            _cancellationTokenSource.Cancel();

            foreach (var task in new[] { _powerTask, _growthSimulationTask, _crimeAndPollutionTask })
            {
                try
                {
                    task.Wait();
                }
                catch (AggregateException aggEx)
                {
                    if (!aggEx.IsCancelled())
                        throw;
                }
            }
            Console.WriteLine("Simulation killed.");
        }

        public QueryResult<PersistedCityStatisticsWithFinancialData> GetRecentStatistics()
        {
            return new QueryResult<PersistedCityStatisticsWithFinancialData>(
                _persistedCityStatisticsCollection
                .GetAll()
                .OrderByDescending(x => x.PersistedCityStatistics.TimeCode)
                .FirstOrDefault()
            );
        }

        public event EventHandler<EventArgsWithData<IYearAndMonth>> OnYearAndOrMonthChanged;

        public PersistedSimulation GeneratePersistedArea()
        {
            return new PersistedSimulation
            {
                PersistedArea = _area.GeneratePersistenceSnapshot(),
                PersistedCityStatistics = _persistedCityStatisticsCollection.GetAll().ToArray(),
                PersistedCityBudgetConfiguration = _cityBudgetConfiguration
            };
        }

        public event EventHandler<CityStatisticsUpdatedEventArgs> CityStatisticsUpdated;

        public IReadOnlyCollection<PersistedCityStatisticsWithFinancialData> GetAllCityStatistics()
        {
            return _persistedCityStatisticsCollection.GetAll();
        }

        public event EventHandler<CityBudgetValueChangedEventArgs> OnCityBudgetValueChanged;

        private void HandleAreaConsumptionResult(object sender, AreaConsumptionResultEventArgs e)
        {
            RaiseAreaMessageEvent(e.AreaConsumptionResult.Message);
        }

        private void RaiseAreaHotMessageEvent(string title, string message)
        {
            OnAreaHotMessage?.Invoke(this, new SimulationSessionHotMessageEventArgs(title, message));
        }

        public event EventHandler<SimulationSessionMessageEventArgs> OnAreaMessage;
        private void RaiseAreaMessageEvent(string message)
        {
            OnAreaMessage?.Invoke(this, new SimulationSessionMessageEventArgs(message));
        }

        public ICityBudgetConfiguration CityBudgetConfiguration => _cityBudgetConfiguration;

        private readonly ICityBudget _cityBudget = new CityBudget();

        public event EventHandler<SimulationSessionHotMessageEventArgs> OnAreaHotMessage;
    }
}
