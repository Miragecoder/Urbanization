using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;
using Rhino;
using Rhino.Mocks;

namespace Mirage.Urbanization.Test
{
    [TestClass]
    public class DistanceTrackerTest
    {
        private static IZoneInfo GenerateZoneInfoMockFor<T>(IReadOnlyArea area)
            where T : class, IAreaZoneConsumption
        {
            var zone = MockRepository.GenerateMock<IZoneInfo>();
            var consumptionState = MockRepository.GenerateMock<IZoneConsumptionState>();

            consumptionState
                .Expect(x => x.GetZoneConsumption())
                .Return(area
                    .GetSupportedZoneConsumptionFactories()
                    .Single(x => x() is T)() as T
            );

            zone
                .Expect(x => x.ConsumptionState)
                .Return(consumptionState);

            return zone;
        }

        private static IZoneInfoPathNode GenerateIZoneInfoPathNodeMock<T>(IReadOnlyArea area)
            where T : class, IAreaZoneConsumption
        {
            var pathNodeMock = MockRepository.GenerateMock<IZoneInfoPathNode>();

            pathNodeMock
                .Expect(x => x.ZoneInfo)
                .Return(GenerateZoneInfoMockFor<T>(area));

            return pathNodeMock;
        }

        [TestMethod]
        public void SimpleDistanceTrackerTest()
        {
            var area = new Area(new AreaOptions(FakeLandValueCalculator.Instance, new TerraformingOptions(), new ProcessOptions(() => true)));

            var tracker = new ZoneInfoDistanceTracker(x => x.IsGrowthZoneClusterOfType<ResidentialZoneClusterConsumption>());

            var zone = GenerateZoneInfoMockFor<RoadZoneConsumption>(area);
            
            var previous = GenerateIZoneInfoPathNodeMock<RoadZoneConsumption>(area);

            foreach (var testValueSet in new[]
            {
                new Tuple<int, Action<bool>>(12, Assert.IsTrue),
                new Tuple<int, Action<bool>>(12, Assert.IsFalse),
                new Tuple<int, Action<bool>>(13, Assert.IsFalse),
                new Tuple<int, Action<bool>>(10, Assert.IsTrue),
                new Tuple<int, Action<bool>>(12, Assert.IsFalse),
            })
            {
                testValueSet.Item2(tracker.IsPreviouslyNotSeenOrSeenAtLargerDistance(
                    node: zone,
                    distance: testValueSet.Item1,
                    previousPathNode: previous
                    ));
            }
        }
    }
}
