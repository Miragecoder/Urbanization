using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mirage.Urbanization.GrowthPathFinding;
using Mirage.Urbanization.Networks;
using Mirage.Urbanization.Persistence;
using Mirage.Urbanization.Statistics;
using Mirage.Urbanization.Vehicles;
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
        private readonly AirplaneController _airplaneController;
        private readonly ShipController _shipController;

        public IVehicleController<ITrain> TrainController
        {
            get
            {
                return _trainController;
            }
        }

        public IVehicleController<IAirplane> AirplaneController
        {
            get { return _airplaneController; }
        }

        public IVehicleController<IShip> ShipController
        {
            get { return _shipController; }
        }

        public Area(AreaOptions options)
        {
            _createZoneInfoFinder = () => new ZoneInfoFinder(
                (queryObject) => _zoneInfoGrid
                    .GetZoneInfoFor(queryObject));
            _zoneInfoGrid = new ZoneInfoGrid(options.GetZoneWidthAndHeight(), options.LandValueCalculator);

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
            var zoneInfos = new HashSet<IZoneInfo>(_zoneInfoGrid.ZoneInfos.Values);

            _trainController = new TrainController(() => zoneInfos);
            _airplaneController = new AirplaneController(() => zoneInfos);
            _shipController = new ShipController(() => zoneInfos);
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

            var onAreaMessage = OnAreaConsumptionResult;
            if (onAreaMessage != null)
                onAreaMessage(this, new AreaConsumptionResultEventArgs(result));

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
                        return new AreaConsumptionResult(
                            areaConsumption: consumption,
                            success: true,
                            message: consumeAreaOperations.First().Description
                        );
                    }

                    var consumptionOperation = zoneInfo
                        .ConsumptionState
                        .TryConsumeWith(consumption as IAreaZoneConsumption);

                    if (consumptionOperation.CanOverrideWithResult.WillSucceed)
                        consumptionOperation.Apply();

                    return new AreaConsumptionResult(
                        areaConsumption: consumption,
                        success: consumptionOperation.CanOverrideWithResult.WillSucceed,
                        message: consumptionOperation.Description
                    );
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
                        return new AreaConsumptionResult(
                            areaConsumption: consumption,
                            success: true,
                            message: "Cannot build across map boundaries."
                        );

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
                        return new AreaConsumptionResult(
                            areaConsumption: consumption,
                            success: true,
                            message: consumeAreaOperations.First().Description
                        );
                    }
                    return new AreaConsumptionResult(
                        areaConsumption: consumption,
                        success: false,
                        message: string.Join(", ", consumeAreaOperations
                            .Where(x => !x.CanOverrideWithResult.WillSucceed)
                            .Select(x => x.Description)
                            .Distinct()
                        )
                    );
                }
                else throw new InvalidOperationException();
            }
        }

        public Task<IPowerGridStatistics> CalculatePowergridStatistics(CancellationToken cancellationToken)
        {
            var powerTask = new Task<IPowerGridStatistics>(() => new PowerGridStatistics(
                PowerNetworkInfo
                    .GenerateFrom(_zoneInfoGrid.ZoneInfos, RaiseAreaMessageEvent)
                    .Select(x => x.PerformSurge())
                ), cancellationToken
            );

            powerTask.Start();

            return powerTask;
        }

        private readonly Random _random = new Random();

        private ISet<TBaseZoneClusterConsumption> GetZoneClusterConsumptions<TBaseZoneClusterConsumption>()
            where TBaseZoneClusterConsumption : BaseZoneClusterConsumption
        {
            return new HashSet<TBaseZoneClusterConsumption>(_zoneInfoGrid
                .ZoneInfos
                .Values
                .Select(x => x.ConsumptionState.GetZoneConsumption())
                .OfType<ZoneClusterMemberConsumption>()
                .Where(x => x.IsCentralClusterMember)
                .Select(x => x.ParentBaseZoneClusterConsumption)
                .OfType<TBaseZoneClusterConsumption>()
            );
        }

        public Task<IGrowthZoneStatistics> PerformGrowthSimulationCycle(CancellationToken cancellationToken)
        {
            if (cancellationToken == null) throw new ArgumentNullException("cancellationToken");

            return Task<IGrowthZoneStatistics>.Run(() =>
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var zoneClusters = GetZoneClusterConsumptions<BaseZoneClusterConsumption>();

                var growthZones = new HashSet<BaseGrowthZoneClusterConsumption>(zoneClusters.OfType<BaseGrowthZoneClusterConsumption>());

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

                var growthZoneDemandThresholds = new IGrowthZoneDemandThreshold[]
                {
                    new GrowthZoneDemandThreshold<IndustrialZoneClusterConsumption, SeaPortZoneClusterConsumption>(
                        currentlyOffered: GetZoneClusterConsumptions<SeaPortZoneClusterConsumption>(), 
                        onExceededMessage: "Industry requires seaport",
                        growthFactor: 40
                    ),
                    new GrowthZoneDemandThreshold<CommercialZoneClusterConsumption, AirportZoneClusterConsumption>(
                        currentlyOffered: GetZoneClusterConsumptions<AirportZoneClusterConsumption>(),
                        onExceededMessage: "Commerce requires airport", 
                        growthFactor: 50
                    ),
                    new GrowthZoneDemandThreshold<ResidentialZoneClusterConsumption, StadiumZoneClusterConsumption>(
                        currentlyOffered: GetZoneClusterConsumptions<StadiumZoneClusterConsumption>(),
                        onExceededMessage: "Citizens demand stadium",
                        growthFactor: 30
                    )
                };

                foreach (var poweredCluster in growthZones
                    .SelectMany(x => x.ZoneClusterMembers)
                    .Where(x => x.IsCentralClusterMember && x.ParentBaseZoneClusterConsumption.ElectricityBehaviour.IsPowered)
                )
                {
                    var violatedThreshold = growthZoneDemandThresholds
                        .SingleOrDefault(x => x.DecrementAvailableConsumption(poweredCluster.ParentBaseZoneClusterConsumption));

                    if (violatedThreshold != null && violatedThreshold.AvailableConsumptionsExceeded)
                    {
                        RaiseAreaMessageEvent(violatedThreshold.OnExceededMessage);
                        continue;
                    }
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
                    industrialZonePopulationNumbers: growthZones.OfType<IndustrialZoneClusterConsumption>().Distinct().Select(x => x.PopulationStatistics).ToList(),
                    cityServicesStatistics: new CityServiceStatistics(
                        numberOfPoliceStations: zoneClusters.OfType<PoliceStationZoneClusterConsumption>().Count(),
                        numberOfFireStations: zoneClusters.OfType<FireStationZoneclusterConsumption>().Count(),
                        numberOfStadiums: zoneClusters.OfType<StadiumZoneClusterConsumption>().Count(),
                        numberOfSeaports: zoneClusters.OfType<SeaPortZoneClusterConsumption>().Count(),
                        numberOfAirports: zoneClusters.OfType<AirportZoneClusterConsumption>().Count()
                    )
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
            yield return () => new FireStationZoneclusterConsumption(_createZoneInfoFinder);

            yield return () => new CoalPowerPlantZoneClusterConsumption(_createZoneInfoFinder);
            yield return () => new NuclearPowerPlantZoneClusterConsumption(_createZoneInfoFinder);

            yield return () => new StadiumZoneClusterConsumption(_createZoneInfoFinder);
            yield return () => new SeaPortZoneClusterConsumption(_createZoneInfoFinder);
            yield return () => new AirportZoneClusterConsumption(_createZoneInfoFinder);
        }

        IEnumerable<IReadOnlyZoneInfo> IReadOnlyArea.EnumerateZoneInfos() { return EnumerateZoneInfos(); }

        public event EventHandler<AreaConsumptionResultEventArgs> OnAreaConsumptionResult;

        public event EventHandler<SimulationSessionMessageEventArgs> OnAreaMessage;
        private void RaiseAreaMessageEvent(string message)
        {
            var onOnAreaMessage = OnAreaMessage;
            if (onOnAreaMessage == null)
                return;
            onOnAreaMessage(this, new SimulationSessionMessageEventArgs(message));
        }

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
}
