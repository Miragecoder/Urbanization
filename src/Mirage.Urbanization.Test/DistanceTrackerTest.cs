using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;
using Moq;

namespace Mirage.Urbanization.Test
{
    [TestClass]
    public class DistanceTrackerTest
    {
        private static IZoneInfo GenerateZoneInfoMockFor<T>(IReadOnlyArea area)
            where T : class, IAreaZoneConsumption
        {
            var zone = new Mock<IZoneInfo>();
            var consumptionState = new Mock<IZoneConsumptionState>();

            consumptionState
                .Setup(x => x.GetZoneConsumption())
                .Returns(area
                    .GetSupportedZoneConsumptionFactories()
                    .Single(x => x() is T)() as T
            );

            zone
                .Setup(x => x.ConsumptionState)
                .Returns(consumptionState.Object);

            return zone.Object;
        }

        private static IZoneInfoPathNode GenerateIZoneInfoPathNodeMock<T>(IReadOnlyArea area)
            where T : class, IAreaZoneConsumption
        {
            var pathNodeMock = new Mock<IZoneInfoPathNode>();

            pathNodeMock
                .Setup(x => x.ZoneInfo)
                .Returns(GenerateZoneInfoMockFor<T>(area));

            return pathNodeMock.Object;
        }

        [TestMethod]
        public void SimpleDistanceTrackerTest()
        {
            var area = new Area(new AreaOptions(() => FakeLandValueCalculator.Instance, new TerraformingOptions(), new ProcessOptions(() => true, () => false), () => new Mock<ICityServiceStrengthLevels>().Object));

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
