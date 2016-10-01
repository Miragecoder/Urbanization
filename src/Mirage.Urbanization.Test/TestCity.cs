using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mirage.Urbanization.Persistence;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;
using Rhino.Mocks;

namespace Mirage.Urbanization.Test
{
    internal class TestCity
    {
        private static Area CreateTestArea(PersistedArea persistedArea = null)
        {
            var terraFormingOptions = new TerraformingOptions
            {
                HorizontalRiver = false,
                VerticalRiver = false
            };

            terraFormingOptions.SetWoodlands(0);
            terraFormingOptions.SetLakes(0);

            return persistedArea != null ?
                new Area(new AreaOptions(() => FakeLandValueCalculator.Instance, persistedArea, ProcessOptions, () => MockRepository.GenerateMock<ICityServiceStrengthLevels>()))
                : new Area(
                    options: new AreaOptions(
                        getLandValueCalculator: () => FakeLandValueCalculator.Instance, 
                        terraformingOptions: terraFormingOptions,
                        processOptions: ProcessOptions,
                        getCityServiceStrengthLevels: () => MockRepository.GenerateMock<ICityServiceStrengthLevels>()
                        )
                    );
        }

        private readonly ZonePoint _powerPlantZonePoint = new ZonePoint {X = 3, Y = 30};

        private readonly Dictionary<Type, Func<IAreaConsumption>> _factories;

        public TestCity(PersistedArea persistedArea = null)
        {
            Area = CreateTestArea(persistedArea);

            _factories = Area
                .GetSupportedZoneConsumptionFactoriesPrivate()
                .ToDictionary(x => x().GetType(), x => x);
        }

        public Area Area { get; }

        public ZonePoint IndustrialZonePoint { get; } = new ZonePoint { X = 3, Y = 3 };

        public ZonePoint ResidentialZonePoint { get; } = new ZonePoint { X = 3, Y = 20 };

        public ZonePoint CommercialZonePoint { get; } = new ZonePoint { X = 9, Y = 10 };

        public ZonePoint TrainStationZonePointB { get; } = new ZonePoint { X = 8, Y = 6 };

        public ZonePoint TrainStationZonePointA { get; } = new ZonePoint { X = 2, Y = 6 };

        public ZoneInfo GetZoneInfoFor(Func<TestCity, ZonePoint> expression)
        {
            return expression(this).GetZoneInfoOn(Area).MatchingObject;
        }

        public bool ConsumeZonePointWith<T>(ZonePoint point)
            where T : IAreaConsumption
        {
            var result = Area
                .ConsumeZoneAt(point.GetZoneInfoOn(Area).MatchingObject,
                    _factories[typeof(T)]());

            return result.Success;
        }

        public bool ConsumeZonePointWithNetwork<T>(ZonePoint point)
            where T : BaseInfrastructureNetworkZoneConsumption
        {
            var zoneInfo = point.GetZoneInfoOn(Area).MatchingObject;

            if (zoneInfo.ConsumptionState.GetIsNetworkMember<T>())
            {
                return true;
            }

            return ConsumeZonePointWith<T>(point);
        }

        public void BuildIndustrialZone()
        {
            Assert.IsTrue(ConsumeZonePointWith<IndustrialZoneClusterConsumption>(IndustrialZonePoint));
        }

        public void BuildResidentialZone()
        {
            Assert.IsTrue(ConsumeZonePointWith<ResidentialZoneClusterConsumption>(ResidentialZonePoint));
        }

        public void BuildPowerplantAndGrid()
        {
            Assert.IsTrue(ConsumeZonePointWith<CoalPowerPlantZoneClusterConsumption>(_powerPlantZonePoint));

            foreach (var point in Enumerable.Range(1, 30).Select(x => new ZonePoint() {X = 1, Y = x}))
            {
                Assert.IsTrue(ConsumeZonePointWithNetwork<PowerLineConsumption>(point));
            }
            foreach (var point in Enumerable.Range(0, 8).Select(x => new ZonePoint() {X = x, Y = 11}))
            {
                Assert.IsTrue(ConsumeZonePointWithNetwork<PowerLineConsumption>(point));
            }
            foreach (var point in Enumerable.Range(0, 8).Select(x => new ZonePoint() { X = x, Y = 6 }))
            {
                point.GetZoneInfoOn(Area).WithResultIfHasMatch(x =>
                {
                    if (x.ConsumptionState.GetZoneConsumption() is ZoneClusterMemberConsumption)
                        return;
                    Assert.IsTrue(ConsumeZonePointWithNetwork<PowerLineConsumption>(point));
                });
            }

            Assert.IsTrue(Area.CalculatePowergridStatistics().PowerGridNetworkStatistics.Any());

            Assert.IsTrue(GetZoneInfoFor(x => ResidentialZonePoint).GetAsZoneCluster<ResidentialZoneClusterConsumption>().MatchingObject.HasPower);
            Assert.IsTrue(GetZoneInfoFor(x => IndustrialZonePoint).GetAsZoneCluster<IndustrialZoneClusterConsumption>().MatchingObject.HasPower);

            if (_commercialZoneBuilt)
            {
                Assert.IsTrue(GetZoneInfoFor(x => CommercialZonePoint).GetAsZoneCluster<CommercialZoneClusterConsumption>().MatchingObject.HasPower);
                Assert.IsTrue(GetZoneInfoFor(x => TrainStationZonePointA).GetAsZoneCluster<TrainStationZoneClusterConsumption>().MatchingObject.HasPower);
                Assert.IsTrue(GetZoneInfoFor(x => TrainStationZonePointB).GetAsZoneCluster<TrainStationZoneClusterConsumption>().MatchingObject.HasPower);
                
            }
        }

        public void BuildRoadBetweenResidentialAndIndustrial()
        {
            foreach (var point in Enumerable.Range(3, 20).Select(x => new ZonePoint() { X = 5, Y = x }))
            {
                Assert.IsTrue(ConsumeZonePointWithNetwork<RoadZoneConsumption>(point));
            }
        }

        private bool _commercialZoneBuilt;

        public void BuildCommercialZoneConnectedWithRailRoad()
        {
            Assert.IsTrue(ConsumeZonePointWith<CommercialZoneClusterConsumption>(CommercialZonePoint));
            Assert.IsTrue(ConsumeZonePointWith<TrainStationZoneClusterConsumption>(TrainStationZonePointA));
            Assert.IsTrue(ConsumeZonePointWith<TrainStationZoneClusterConsumption>(TrainStationZonePointB));

            foreach (var i in Enumerable.Range(9, 11))
            {
                Assert.IsTrue(ConsumeZonePointWith<RoadZoneConsumption>(new ZonePoint { X = i, Y = 8 }));
            }

            foreach (var i in Enumerable.Range(2, 12))
            {
                Assert.IsTrue(ConsumeZonePointWith<RailRoadZoneConsumption>(new ZonePoint { X = i, Y = 5 }));
            }

            foreach (var i in Enumerable.Range(2, 3))
            {
                Assert.IsTrue(ConsumeZonePointWith<RoadZoneConsumption>(new ZonePoint { X = i, Y = 8 }));
            }
            _commercialZoneBuilt = true;
        }

        public static readonly ProcessOptions ProcessOptions = new ProcessOptions(() => false, () => false);
    }
}