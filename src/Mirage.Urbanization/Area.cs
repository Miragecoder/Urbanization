using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mirage.Urbanization.Networks;
using Mirage.Urbanization.Persistence;
using Mirage.Urbanization.Statistics;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization
{
    public class Area : IReadOnlyArea
    {
        private readonly ZoneInfoGrid _zoneInfoGrid;
        private readonly Func<ZoneInfoFinder> _createZoneInfoFinder;
        private readonly AreaOptions _areaOptions;
        private readonly TrainController _trainController;

        public ITrainController TrainController
        {
            get
            {
                return _trainController;
            }
        }

        public Area(AreaOptions options)
        {
            _createZoneInfoFinder = () => new ZoneInfoFinder(
                (queryObject) => _zoneInfoGrid
                    .GetZoneInfoFor(queryObject));
            _zoneInfoGrid = new ZoneInfoGrid(options.GetZoneWidthAndHeight());

            options.WithTerraformingOptions(terraFormingOptions =>
            {
                var randomTerraformer = new RandomTerraformer(CreateWaterZone, CreateWoodlandZone);
                randomTerraformer.ApplyWith(_zoneInfoGrid, terraFormingOptions);
            });

            options.WithPersistedArea(persistedArea =>
            {
                var factoriesAndNames =
                    GetSupportedZoneConsumptionFactoriesPrivate()
                    .ToDictionary(x => x().Name, x => x);

                if (persistedArea
                    .PersistedSingleZoneConsumptions
                    .Select(x => ConsumeZoneAt(
                        readOnlyZoneInfo: _zoneInfoGrid.ZoneInfos[new ZonePoint { X = x.X, Y = x.Y }],
                        consumption: factoriesAndNames[x.Name]()))
                    .Concat(persistedArea
                        .PersistedStaticZoneClusterCentralMemberConsumptions
                        .Select(x => ConsumeZoneAt(
                            readOnlyZoneInfo: _zoneInfoGrid.ZoneInfos[new ZonePoint { X = x.X, Y = x.Y }],
                            consumption: factoriesAndNames[x.Name]())
                        )
                    )
                    .Any(result => !result.Success)
                )
                {
                    throw new InvalidOperationException();
                }

                if (persistedArea
                    .PersistedIntersectingZoneConsumptions
                    .Select(x => ConsumeZoneAt(
                        readOnlyZoneInfo: _zoneInfoGrid.ZoneInfos[new ZonePoint { X = x.X, Y = x.Y }],
                        consumption: new IntersectingZoneConsumption(_createZoneInfoFinder(),
                            factoriesAndNames[x.EastWestName]() as BaseInfrastructureNetworkZoneConsumption,
                            factoriesAndNames[x.NorthSouthName]() as BaseInfrastructureNetworkZoneConsumption)
                    ))
                    .Any(result => !result.Success))
                {
                    throw new InvalidOperationException();
                }

                if (persistedArea
                    .PersistedGrowthZoneClusterCentralMemberConsumptions
                    .Select(x =>
                    {
                        var consumption = factoriesAndNames[x.Name]() as IBaseGrowthZoneClusterConsumptionInLoadContext;

                        consumption.Set(x.Id, x.Population);

                        return ConsumeZoneAt(
                            readOnlyZoneInfo: _zoneInfoGrid.ZoneInfos[new ZonePoint { X = x.X, Y = x.Y }],
                            consumption: consumption
                        );
                    })
                    .Any(result => !result.Success))
                {
                    throw new InvalidOperationException();
                }

                if (persistedArea.PersistedTrafficStates != null)
                {
                    foreach (var trafficState in persistedArea.PersistedTrafficStates)
                    {
                        var localTrafficState = trafficState;
                        _zoneInfoGrid.ZoneInfos[new ZonePoint() { X = trafficState.X, Y = trafficState.Y }]
                            .WithNetworkConsumptionIf((RoadZoneConsumption x) =>
                            {
                                x.SetTrafficDensity(trafficState.Traffic);
                            });
                    }
                    Console.WriteLine(String.Empty);
                }
            });

            _areaOptions = options;
            _trainController = new TrainController(() => _zoneInfoGrid.ZoneInfos.ToDictionary(x => x.Value as IReadOnlyZoneInfo, x => x.Value));
        }

        private WoodlandZoneConsumption CreateWoodlandZone() { return new WoodlandZoneConsumption(_createZoneInfoFinder()); }

        private WaterZoneConsumption CreateWaterZone() { return new WaterZoneConsumption(_createZoneInfoFinder()); }

        public IEnumerable<ZoneInfo> EnumerateZoneInfos()
        {
            return _zoneInfoGrid.ZoneInfos.Values;
        }

        public int AmountOfZonesX { get { return AmountOfZonesY; } }

        public int AmountOfZonesY
        {
            get
            {
                return new HashSet<int>(_zoneInfoGrid.ZoneInfos
                    .GroupBy(x => x.Value.Point.X)
                    .Select(x => x.Count())
                    .Concat(_zoneInfoGrid.ZoneInfos
                        .GroupBy(x => x.Value.Point.Y)
                        .Select(x => x.Count())
                    )
                )
                .Single();
            }
        }

        private readonly object _zoneConsumptionLock = new object();

        public IAreaConsumptionResult ConsumeZoneAt(IReadOnlyZoneInfo readOnlyZoneInfo, IAreaConsumption consumption)
        {
            var result = ConsumeZoneAtPrivate(readOnlyZoneInfo, consumption);

            var onAreaMessage = OnAreaMessage;
            if (onAreaMessage != null)
                onAreaMessage(this, new AreaMessageEventArgs(result));

            return result;
        }

        private IAreaConsumptionResult ConsumeZoneAtPrivate(IReadOnlyZoneInfo readOnlyZoneInfo, IAreaConsumption consumption)
        {
            var zoneInfo = readOnlyZoneInfo as ZoneInfo;
            if (zoneInfo == null) throw new ArgumentException("readOnlyZoneInfo");

            lock (_zoneConsumptionLock)
            {
                if (consumption is IAreaZoneConsumption)
                {
                    var zoneClusterMember = zoneInfo.ConsumptionState.GetZoneConsumption() as ZoneClusterMemberConsumption;
                    if (consumption is EmptyZoneConsumption && zoneClusterMember != null && zoneClusterMember.IsCentralClusterMember)
                    {
                        List<IConsumeAreaOperation> consumeAreaOperations = null;
                        zoneClusterMember.ParentBaseZoneClusterConsumption.WithUnlockedClusterMembers(() =>
                        {
                            consumeAreaOperations = zoneClusterMember
                                .ParentBaseZoneClusterConsumption
                                .ZoneClusterMembers
                                .Select(zoneMemberConsumption => _zoneInfoGrid
                                    .GetZoneInfoFor(zoneMemberConsumption)
                                )
                                .Where(x => x.HasMatch)
                                .Select(clusterMemberZoneInfo => clusterMemberZoneInfo
                                    .MatchingObject
                                    .ConsumptionState
                                    .TryConsumeWith(new RubbishZoneConsumption())
                                )
                                .ToList();
                            if (consumeAreaOperations.All(x => x.CanOverrideWithResult.WillSucceed))
                            {
                                foreach (var consumeAreaOperation in consumeAreaOperations)
                                    consumeAreaOperation.Apply();
                            }
                        });

                        if (consumeAreaOperations == null) throw new InvalidOperationException();
                        return new AreaConsumptionResult(consumeAreaOperations.First().Description, true);
                    }

                    var consumptionOperation = zoneInfo
                        .ConsumptionState
                        .TryConsumeWith(consumption as IAreaZoneConsumption);

                    if (consumptionOperation.CanOverrideWithResult.WillSucceed)
                        consumptionOperation.Apply();

                    return new AreaConsumptionResult(consumptionOperation.Description, true);
                }
                else if (consumption is IAreaZoneClusterConsumption)
                {
                    var clusterZoneConsumption = consumption as IAreaZoneClusterConsumption;

                    var queryResults = clusterZoneConsumption
                        .ZoneClusterMembers
                        .Select(member =>
                            new
                            {
                                QueryResult = zoneInfo
                                    .GetRelativeZoneInfo(new RelativeZoneInfoQuery(
                                    relativeX: member.RelativeToParentCenterX,
                                    relativeY: member.RelativeToParentCenterY)),
                                ConsumptionZoneMember = member
                            }
                        )
                        .ToArray();

                    if (queryResults.Any(x => x.QueryResult.HasNoMatch))
                        return new AreaConsumptionResult("Cannot build across map boundaries.", false);

                    var consumeAreaOperations = queryResults
                        .Select(x => x
                            .QueryResult
                            .MatchingObject
                            .ConsumptionState
                            .TryConsumeWith(x.ConsumptionZoneMember)
                        )
                        .ToArray();

                    if (consumeAreaOperations.All(x => x.CanOverrideWithResult.WillSucceed))
                    {
                        foreach (var consumeAreaOperation in consumeAreaOperations)
                            consumeAreaOperation.Apply();
                        return new AreaConsumptionResult(consumeAreaOperations.First().Description, true);
                    }
                    return new AreaConsumptionResult(String.Join(", ", consumeAreaOperations
                        .Where(x => !x.CanOverrideWithResult.WillSucceed)
                        .Select(x => x.Description)
                        .Distinct()), false
                    );
                }
                else throw new InvalidOperationException();
            }
        }

        public Task<IPowerGridStatistics> CalculatePowergridStatistics(CancellationToken cancellationToken)
        {
            var powerTask = new Task<IPowerGridStatistics>(() => new PowerGridStatistics(
                PowerNetworkInfo
                    .GenerateFrom(_zoneInfoGrid.ZoneInfos)
                    .Select(x => x.PerformSurge())
                ), cancellationToken
            );

            powerTask.Start();

            return powerTask;
        }

        private readonly Random _random = new Random();

        public Task<IGrowthZoneStatistics> PerformGrowthSimulationCycle(CancellationToken cancellationToken)
        {
            if (cancellationToken == null) throw new ArgumentNullException("cancellationToken");

            return Task<IGrowthZoneStatistics>.Run(() =>
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var growthZones = new HashSet<BaseGrowthZoneClusterConsumption>(_zoneInfoGrid
                    .ZoneInfos
                    .Values
                    .Select(x => x.ConsumptionState.GetZoneConsumption())
                    .OfType<ZoneClusterMemberConsumption>()
                    .Where(x => x.IsCentralClusterMember)
                    .Select(x => x.ParentBaseZoneClusterConsumption)
                    .OfType<BaseGrowthZoneClusterConsumption>());

                var connector = new GrowthZoneConnector(_zoneInfoGrid, cancellationToken);

                connector.DecreasePopulation(growthZones);

                // Outside influence which powers initial growth...
                {
                    var industrialZones = growthZones.OfType<IndustrialZoneClusterConsumption>().ToList();

                    if (industrialZones.All(x => x.PopulationDensity == 0))
                    {
                        foreach (var z in industrialZones.Where(x => x.HasPower).OrderBy(x => _random.Next()).Take(2))
                        {
                            z.IncreasePopulation();
                            z.IncreasePopulation();
                            z.IncreasePopulation();
                        }
                    }
                }

                var poweredClusters = growthZones
                    .SelectMany(x => x.ZoneClusterMembers)
                    .Where(
                        x =>
                            x.IsCentralClusterMember &&
                            x.ParentBaseZoneClusterConsumption.ElectricityBehaviour.IsPowered);

                Action<ZoneClusterMemberConsumption> body = clusterMemberConsumption =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    clusterMemberConsumption.GetZoneInfo()
                        .WithResultIfHasMatch(zoneInfo => connector.Process(
                            growthZoneInfoPathNode: new GrowthZoneInfoPathNode(
                                zoneInfo: zoneInfo,
                                clusterMemberConsumption: clusterMemberConsumption,
                                processOptions: _areaOptions.ProcessOptions
                                )
                            )
                        );
                };

                foreach (var poweredCluster in poweredClusters)
                {
                    if (_areaOptions.ProcessOptions.GetStepByStepGrowthCyclingToggled())
                        Thread.Sleep(500);
                    body(poweredCluster);
                }

                var roadInfraStructureStatistics = connector.ApplyTraffic();
                cancellationToken.ThrowIfCancellationRequested();

                connector.ApplyAverageTravelDistances();
                cancellationToken.ThrowIfCancellationRequested();

                Console.WriteLine("Growth cycle completed. (Took: " + stopwatch.Elapsed + ")");
                return new GrowthZoneNetworkStatistics(
                    roadInfraStructureStatistics: roadInfraStructureStatistics,
                    railroadInfrastructureStatistics: new RailroadInfrastructureStatistics(
                        numberOfTrainStations: _zoneInfoGrid
                            .ZoneInfos
                            .Values
                            .Select(x => x.GetAsZoneCluster<TrainStationZoneClusterConsumption>())
                            .Where(x => x.HasMatch)
                            .Select(x => x.MatchingObject)
                            .Distinct()
                            .Count(),
                        numberOfRailRoadZones: _zoneInfoGrid
                            .ZoneInfos
                            .Values
                            .Select(x => x.GetNetworkZoneConsumption<RailRoadZoneConsumption>())
                            .Where(x => x.HasMatch)
                            .Select(x => x.MatchingObject)
                            .Count()
                            ),
                    residentialZonePopulationNumbers: growthZones.OfType<ResidentialZoneClusterConsumption>().Distinct().Select(x => x.PopulationStatistics).ToList(),
                    commercialZonePopulationNumbers: growthZones.OfType<CommercialZoneClusterConsumption>().Distinct().Select(x => x.PopulationStatistics).ToList(),
                    industrialZonePopulationNumbers: growthZones.OfType<IndustrialZoneClusterConsumption>().Distinct().Select(x => x.PopulationStatistics).ToList()
                ) as IGrowthZoneStatistics;
            }, cancellationToken: cancellationToken);
        }

        internal IEnumerable<Func<IAreaConsumption>> GetSupportedZoneConsumptionFactoriesPrivate()
        {
            foreach (var x in GetSupportedZoneConsumptionFactories())
                yield return x;
            yield return () => new RubbishZoneConsumption();
        }

        public IEnumerable<Func<IAreaConsumption>> GetSupportedZoneConsumptionFactories()
        {
            yield return () => new EmptyZoneConsumption();
            yield return () => new WoodlandZoneConsumption(_createZoneInfoFinder());
            yield return () => new WaterZoneConsumption(_createZoneInfoFinder());
            yield return () => new PowerLineConsumption(_createZoneInfoFinder());
            yield return () => new RoadZoneConsumption(_createZoneInfoFinder());
            yield return () => new RailRoadZoneConsumption(_createZoneInfoFinder());

            yield return () => new TrainStationZoneClusterConsumption(_createZoneInfoFinder);

            yield return () => new ResidentialZoneClusterConsumption(_createZoneInfoFinder);
            yield return () => new CommercialZoneClusterConsumption(_createZoneInfoFinder);
            yield return () => new IndustrialZoneClusterConsumption(_createZoneInfoFinder);
            yield return () => new PoliceStationZoneClusterConsumption(_createZoneInfoFinder);
            yield return () => new CoalPowerPlantZoneClusterConsumption(_createZoneInfoFinder);
            yield return () => new AirportZoneClusterConsumption(_createZoneInfoFinder);
        }

        IEnumerable<IReadOnlyZoneInfo> IReadOnlyArea.EnumerateZoneInfos() { return EnumerateZoneInfos(); }

        public event EventHandler<AreaMessageEventArgs> OnAreaMessage;

        public PersistedArea GeneratePersistenceSnapshot()
        {
            lock (_zoneConsumptionLock)
            {
                var snapshot = _zoneInfoGrid
                    .ZoneInfos
                    .ToDictionary(x => x.Key, x => x.Value.ConsumptionState);

                var clusterCenters = snapshot
                    .Where(x => x.Value.GetZoneConsumption() is ZoneClusterMemberConsumption &&
                                ((ZoneClusterMemberConsumption)x.Value.GetZoneConsumption()).IsCentralClusterMember);

                return new PersistedArea
                {
                    PersistedSingleZoneConsumptions = snapshot
                        .Where(x => x.Value.GetZoneConsumption() is BaseSingleZoneConsumption)
                        .Select(x => new PersistedSingleZoneConsumption
                        {
                            X = x.Key.X,
                            Y = x.Key.Y,
                            Name = x.Value.GetZoneConsumption().Name
                        }).ToArray(),
                    PersistedIntersectingZoneConsumptions = snapshot
                        .Where(x => x.Value.GetZoneConsumption() is IntersectingZoneConsumption)
                        .Select(x => new PersistedIntersectingZoneConsumption
                        {
                            X = x.Key.X,
                            Y = x.Key.Y,
                            EastWestName = ((IntersectingZoneConsumption)x.Value.GetZoneConsumption()).EastWestZoneConsumption.Name,
                            NorthSouthName = ((IntersectingZoneConsumption)x.Value.GetZoneConsumption()).NorthSouthZoneConsumption.Name
                        }).ToArray(),
                    PersistedGrowthZoneClusterCentralMemberConsumptions = clusterCenters
                        .Where(
                            x =>
                                ((ZoneClusterMemberConsumption)x.Value.GetZoneConsumption()).ParentBaseZoneClusterConsumption is
                                    BaseGrowthZoneClusterConsumption)
                        .Select(x => new PersistedGrowthZoneClusterCentralMemberConsumption
                        {
                            X = x.Key.X,
                            Y = x.Key.Y,
                            Id =
                                (((ZoneClusterMemberConsumption)x.Value.GetZoneConsumption()).ParentBaseZoneClusterConsumption as
                                    BaseGrowthZoneClusterConsumption).Id,
                            Name =
                                (((ZoneClusterMemberConsumption)x.Value.GetZoneConsumption()).ParentBaseZoneClusterConsumption as
                                    BaseGrowthZoneClusterConsumption).Name,
                            Population =
                                (((ZoneClusterMemberConsumption)x.Value.GetZoneConsumption()).ParentBaseZoneClusterConsumption as
                                    BaseGrowthZoneClusterConsumption).PopulationDensity
                        }).ToArray(),
                    PersistedStaticZoneClusterCentralMemberConsumptions = clusterCenters
                        .Where(
                            x =>
                                ((ZoneClusterMemberConsumption)x.Value.GetZoneConsumption()).ParentBaseZoneClusterConsumption is
                                    StaticZoneClusterConsumption)
                        .Select(x => new PersistedStaticZoneClusterCentralMemberConsumption()
                        {
                            X = x.Key.X,
                            Y = x.Key.Y,
                            Name =
                                (((ZoneClusterMemberConsumption)x.Value.GetZoneConsumption()).ParentBaseZoneClusterConsumption as
                                    StaticZoneClusterConsumption).Name,
                        }).ToArray(),
                    PersistedTrafficStates = snapshot
                        .Where(x => x.Value.GetIsRoadNetworkMember())
                        .Select(x => new PersistedTrafficState()
                        {
                            X = x.Key.X,
                            Y = x.Key.Y,
                            Traffic = new Func<int>(() =>
                            {
                                var traffic = default(int);
                                x.Value.WithNetworkMember<RoadZoneConsumption>(y =>
                                {
                                    traffic = y.GetTrafficDensityAsInt();
                                });
                                return traffic;
                            })()
                        }).ToArray()
                };
            }
        }
    }

    public interface ITrain
    {
        bool CanBeRemoved { get; }
        ZoneInfo CurrentPosition { get; }
        void CrawlNetwork(ISet<ZoneInfo> network);
        ZoneInfo PreviousPreviousPreviousPreviousPosition { get; }
        ZoneInfo PreviousPreviousPreviousPosition { get; }
        ZoneInfo PreviousPreviousPosition { get; }
        ZoneInfo PreviousPosition { get; }
    }

    public interface ITrainController
    {
        void ForEachActiveTrain(Action<ITrain> trainAction);
    }

    internal class TrainController : ITrainController
    {
        private readonly Func<IDictionary<IReadOnlyZoneInfo, ZoneInfo>> _getZoneInfosFunc;

        public TrainController(Func<IDictionary<IReadOnlyZoneInfo, ZoneInfo>> getZoneInfosFunc)
        {
            _getZoneInfosFunc = getZoneInfosFunc;

            _cachedNetworks = new SimpleCache<ISet<ISet<ZoneInfo>>>(GetRailwayNetworks, new TimeSpan(0, 0, 1));
        }

        public void ForEachActiveTrain(Action<ITrain> trainAction)
        {
            PerformTrainMovementCycle();

            foreach (var train in _trains)
            {
                trainAction(train);
            }
        }

        private void PerformTrainMovementCycle()
        {
            var cachedNetworksEntry = _cachedNetworks.GetValue();

            if (!cachedNetworksEntry.SelectMany(x => x).Any())
                return;

            foreach (var network in cachedNetworksEntry.Where(x => x.Count() > 20))
            {
                var desiredAmountOfTrains = Math.Abs(network.Count() / 50) + 1;

                List<ITrain> trainsInNetwork = null;

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
                            _trains.Add(new Train(_getZoneInfosFunc, network
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

        }

        private readonly HashSet<ITrain> _trains = new HashSet<ITrain>();

        private static readonly Random Random = new Random();

        private class Train : ITrain
        {
            private ZoneInfo _currentPosition;
            private ZoneInfo _previousPosition;
            private ZoneInfo _previousPreviousPosition;
            private ZoneInfo _previousPreviousPreviousPosition;
            private ZoneInfo _previousPreviousPreviousPreviousPosition;

            public ZoneInfo CurrentPosition { get { return _currentPosition; } }

            public bool CanBeRemoved
            {
                get
                {
                    return _currentPosition == null || _lastChange < DateTime.Now.AddSeconds(-3);
                }
            }

            public ZoneInfo PreviousPreviousPreviousPreviousPosition { get { return _previousPreviousPreviousPreviousPosition; } }
            public ZoneInfo PreviousPreviousPreviousPosition { get { return _previousPreviousPreviousPosition; } }
            public ZoneInfo PreviousPreviousPosition { get { return _previousPreviousPosition; } }
            public ZoneInfo PreviousPosition { get { return _previousPosition; } }

            private readonly Func<IDictionary<IReadOnlyZoneInfo, ZoneInfo>> _getZoneInfosFunc;

            public Train(Func<IDictionary<IReadOnlyZoneInfo, ZoneInfo>> getZoneInfosFunc, ZoneInfo currentPosition)
            {
                _getZoneInfosFunc = getZoneInfosFunc;
                _currentPosition = currentPosition;
            }

            private DateTime _lastChange = DateTime.Now;

            public void CrawlNetwork(ISet<ZoneInfo> trainNetwork)
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
                        .GetNorthEastSouthWest()
                        .OrderBy(x => Random.Next())
                        .Where(x => x.HasMatch)
                        .Select(x => x.MatchingObject)
                        .Where(trainNetwork.Contains)
                        .Select(x => _getZoneInfosFunc()[x])
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

        private readonly SimpleCache<ISet<ISet<ZoneInfo>>> _cachedNetworks;

        private ISet<ISet<ZoneInfo>> GetRailwayNetworks()
        {
            var railwayNetworks = new HashSet<ISet<ZoneInfo>>();
            foreach (var railroadZoneInfo in _getZoneInfosFunc()
                .Where(x => x.Key.ZoneConsumptionState.GetIsRailroadNetworkMember())
                .Where(x => !railwayNetworks.SelectMany(y => y).Contains(x.Value)))
            {
                var railwayNetwork = new HashSet<ZoneInfo> { railroadZoneInfo.Value };

                foreach (var member in railroadZoneInfo
                    .Key
                    .CrawlAllDirections(x => x.ConsumptionState.GetIsRailroadNetworkMember())
                    )
                {
                    railwayNetwork.Add(_getZoneInfosFunc().First(x => x.Key == member).Value);
                }

                railwayNetworks.Add(railwayNetwork);
            }
            return railwayNetworks;
        }
    }
}
