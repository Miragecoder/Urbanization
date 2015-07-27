using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mirage.Urbanization.GrowthPathFinding;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization.Test
{
    [TestClass]
    public class PathfindingTest
    {
        [TestMethod]
        public void TestSimplePath()
        {
            TestSimplePath(buildRoad: true, saveAndReload: false, buildCommercial: false);
        }

        [TestMethod]
        public void TestAbsenceOfPath()
        {
            TestSimplePath(buildRoad: false, saveAndReload: false, buildCommercial: false);
        }

        [TestMethod]
        public void TestSimplePathSaveAndReload()
        {
            TestSimplePath(buildRoad: true, saveAndReload: true, buildCommercial: false);
        }

        [TestMethod]
        public void TestAbsenceOfPathSaveAndReload()
        {
            TestSimplePath(buildRoad: false, saveAndReload: true, buildCommercial: false);
        }

        [TestMethod]
        public void TestSimplePathAndRailRoad()
        {
            TestSimplePath(buildRoad: true, saveAndReload: false, buildCommercial: true);
        }

        [TestMethod]
        public void TestAbsenceOfPathAndRailRoad()
        {
            TestSimplePath(buildRoad: false, saveAndReload: false, buildCommercial: true);
        }

        [TestMethod]
        public void TestSimplePathSaveAndReloadAndRailRoad()
        {
            TestSimplePath(buildRoad: true, saveAndReload: true, buildCommercial: true);
        }

        [TestMethod]
        public void TestAbsenceOfPathSaveAndReloadAndRailRoad()
        {
            TestSimplePath(buildRoad: false, saveAndReload: true, buildCommercial: true);
        }

        private void TestSimplePath(bool buildRoad, bool saveAndReload, bool buildCommercial)
        {
            var testCity = new TestCity();

            testCity.BuildIndustrialZone();
            testCity.BuildResidentialZone();

            if (buildRoad)
            {
                testCity.BuildRoadBetweenResidentialAndIndustrial();
            }

            if (buildCommercial)
            {
                testCity.BuildCommercialZoneConnectedWithRailRoad();
            }

            if (saveAndReload)
            {
                var oldTestCity = testCity.Area;
                var persistedArea = testCity.Area.GeneratePersistenceSnapshot();

                testCity = new TestCity(persistedArea);

                Assert.AreNotSame(oldTestCity, testCity);
            }

            testCity.BuildPowerplantAndGrid();

            var pathFinderForIndustrial = new GrowthZoneInfoPathNode(
                zoneInfo: testCity.GetZoneInfoFor(x => x.IndustrialZonePoint), 
                clusterMemberConsumption: testCity
                    .GetZoneInfoFor(x => x.IndustrialZonePoint)
                    .GetAsZoneCluster<IndustrialZoneClusterConsumption>()
                    .MatchingObject
                    .ZoneClusterMembers
                    .Single(x => x.IsCentralClusterMember), 
                processOptions: TestCity.ProcessOptions,
                undesirableGrowthZones: new HashSet<BaseGrowthZoneClusterConsumption>()
            );

            var destinationPathNodesForIndustrial = pathFinderForIndustrial
                .EnumerateAllChildPathNodes()
                .Where(x => x.IsDestination)
                .ToList();

            Assert.AreEqual(destinationPathNodesForIndustrial.Any() && destinationPathNodesForIndustrial.All(destinationNode => 
                destinationNode
                    .ZoneInfo
                    .IsGrowthZoneClusterOfType<ResidentialZoneClusterConsumption>() 
                && destinationNode
                   .ZoneInfo
                   .GetAsZoneCluster<ResidentialZoneClusterConsumption>()
                   .MatchingObject
                   .ZoneClusterMembers
                   .Single(x => x.IsCentralClusterMember)
                   .GetZoneInfo()
                   .MatchingObject == testCity
                        .GetZoneInfoFor(x => x.ResidentialZonePoint)
            ), buildRoad);

            foreach (var destinationPathNode in destinationPathNodesForIndustrial)
            {
                destinationPathNode.WithPathMembers(x =>
                {
                    var consumption = x.ZoneInfo.ConsumptionState.GetZoneConsumption();
                    Assert.IsTrue(GetValidPathMemberTests().Count(func => func(consumption)) == 1);
                });
            }

            if (buildCommercial)
            {
                var commercialPathFinder = new GrowthZoneInfoPathNode(
                    zoneInfo: testCity
                        .GetZoneInfoFor(y => y.CommercialZonePoint), 
                    clusterMemberConsumption: testCity
                        .GetZoneInfoFor(y => y.CommercialZonePoint)
                        .GetAsZoneCluster<CommercialZoneClusterConsumption>()
                        .MatchingObject
                        .ZoneClusterMembers
                        .Single(x => x.IsCentralClusterMember), 
                        processOptions: TestCity.ProcessOptions,
                    undesirableGrowthZones: new HashSet<BaseGrowthZoneClusterConsumption>()
                );

                var commercialDestinationNodes = commercialPathFinder
                    .EnumerateAllChildPathNodes()
                    .Where(x => x.IsDestination)
                    .ToList();

                Assert.AreEqual(buildRoad, commercialDestinationNodes.Any());
                bool foundResidential = false;
                bool foundIndustrial = false;
                foreach (var destinationPathNode in commercialDestinationNodes)
                {
                    destinationPathNode.WithDestination(
                        x =>
                        {
                            x.WithZoneClusterIf<IndustrialZoneClusterConsumption>(
                                zone =>
                                    foundIndustrial =
                                        zone ==
                                        testCity.GetZoneInfoFor(y => y.IndustrialZonePoint).GetAsZoneCluster<IndustrialZoneClusterConsumption>().MatchingObject);
                            x.WithZoneClusterIf<ResidentialZoneClusterConsumption>(
                                zone =>
                                    foundResidential =
                                        zone ==
                                        testCity.GetZoneInfoFor(y => y.ResidentialZonePoint).GetAsZoneCluster<ResidentialZoneClusterConsumption>().MatchingObject);
                        }
                    );
                    destinationPathNode.WithPathMembers(x =>
                    {
                        var consumption = x.ZoneInfo.ConsumptionState.GetZoneConsumption();
                        Assert.IsTrue(GetValidPathMemberTests().Count(func => func(consumption)) == 1);
                    });
                }
                Assert.AreEqual(buildRoad, foundIndustrial);
                Assert.AreEqual(buildRoad, foundResidential);
            }
        }

        private static IEnumerable<Func<IAreaZoneConsumption, bool>> GetValidPathMemberTests()
        {
            yield return x => x is RoadZoneConsumption;
            yield return x => x is RailRoadZoneConsumption;
            yield return
                x =>
                {
                    var y = x as ZoneClusterMemberConsumption;
                    if (y == null) return false;

                    return y.ParentBaseZoneClusterConsumption is TrainStationZoneClusterConsumption
                        || y.ParentBaseZoneClusterConsumption is BaseGrowthZoneClusterConsumption;
                };
            yield return x =>
            {
                var y = x as IntersectingZoneConsumption;
                if (y == null) return false;

                return
                    y.GetIntersectingTypes()
                        .Any(t => new[] { typeof(RoadZoneConsumption), typeof(RailRoadZoneConsumption) }.Contains(t));
            };
        }
    }
}
