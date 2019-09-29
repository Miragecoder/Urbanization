using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;
using Rhino.Mocks;

namespace Mirage.Urbanization.Test
{
    public class FakeLandValueCalculator : ILandValueCalculator
    {
        public static readonly FakeLandValueCalculator Instance = new FakeLandValueCalculator();

        public QueryResult<IQueryLandValueResult> GetFor(IReadOnlyZoneInfo zoneInfo)
        {
            return QueryResult<IQueryLandValueResult>.Empty;
        }

        public IEnumerable<string> GetUndesirabilityReasons(BaseGrowthZoneClusterConsumption baseGrowthZoneClusterConsumption)
        {
            return Enumerable.Empty<string>();
        }
    }

    [TestClass]
    public class ZonesAndAreasTests
    {
        [TestMethod]
        public void VerifyAreaHasHundredZones()
        {
            var terraFormingOptions = new TerraformingOptions();
            terraFormingOptions.SetZoneWidthAndHeight(100);
            var options = new AreaOptions(() => FakeLandValueCalculator.Instance, terraFormingOptions, TestCity.ProcessOptions, () => MockRepository.GenerateMock<ICityServiceStrengthLevels>());

            var area = new Area(options);

            options.WithTerraformingOptions(x =>
            {
                Assert.AreEqual(x.ZoneWidthAndHeight, area.AmountOfZonesX);
                Assert.AreEqual(x.ZoneWidthAndHeight, area.AmountOfZonesY);
            });
        }

        [TestMethod]
        public void VerifyAreaInitiallyHasNoZoneConsumption()
        {
            var terraFormingOptions = new TerraformingOptions();
            terraFormingOptions.SetZoneWidthAndHeight(100);
            var options = new AreaOptions(() => FakeLandValueCalculator.Instance, terraFormingOptions, TestCity.ProcessOptions, () =>MockRepository.GenerateMock<ICityServiceStrengthLevels>());

            var area = new Area(options);

            options.WithTerraformingOptions(x =>
            {
                Assert.AreEqual(
                    expected: x.ZoneWidthAndHeight * x.ZoneWidthAndHeight,
                    actual: area
                        .EnumerateZoneInfos()
                        .Count(y => y.ConsumptionState.GetZoneConsumption() is EmptyZoneConsumption)
                );
            });
        }

        [TestMethod]
        public void VerifySingleConsumption()
        {
            var terraFormingOptions = new TerraformingOptions();
            terraFormingOptions.SetZoneWidthAndHeight(100);
            var options = new AreaOptions(() => FakeLandValueCalculator.Instance, terraFormingOptions, TestCity.ProcessOptions, () => MockRepository.GenerateMock<ICityServiceStrengthLevels>());
            var area = new Area(options);

            var predicate = new Func<IZoneInfo, bool>(x => x.Point.X == 2 && x.Point.Y == 4);
            var firstZone = area.EnumerateZoneInfos().Single(predicate);

            Assert.IsTrue(area.EnumerateZoneInfos().Single(predicate).ConsumptionState.GetZoneConsumption() is EmptyZoneConsumption);
            area.ConsumeZoneAt(firstZone, new RoadZoneConsumption(new ZoneInfoFinder(null)));

            Assert.IsTrue(area.EnumerateZoneInfos().Single(predicate).ConsumptionState.GetZoneConsumption() is RoadZoneConsumption);
        }

        [TestMethod]
        public void VerifyClusterConsumption()
        {
            var terraFormingOptions = new TerraformingOptions();
            terraFormingOptions.SetZoneWidthAndHeight(100);
            var options = new AreaOptions(() => FakeLandValueCalculator.Instance, terraFormingOptions, TestCity.ProcessOptions, () => MockRepository.GenerateMock<ICityServiceStrengthLevels>());
            var area = new Area(options);

            var predicate = new Func<IZoneInfo, bool>(x => x.Point.X == 2 && x.Point.Y == 4);
            var firstZone = area.EnumerateZoneInfos().Single(predicate);

            Assert.IsTrue(area.EnumerateZoneInfos().Single(predicate).ConsumptionState.GetZoneConsumption() is EmptyZoneConsumption);
            area.ConsumeZoneAt(firstZone, new ResidentialZoneClusterConsumption(() => new ZoneInfoFinder(null)));

            Assert.IsTrue(area.EnumerateZoneInfos().Single(predicate).ConsumptionState.GetZoneConsumption() is ZoneClusterMemberConsumption);
        }

        [TestMethod]
        public void EnsurePointEquality()
        {
            foreach (var generateZonePointCurrent in
                from currentX in Enumerable.Range(0, 100)
                from currentY in Enumerable.Range(0, 100)
                select new Func<ZonePoint>(() => new ZonePoint { X = currentX, Y = currentY }))
            {
                Assert.AreEqual(generateZonePointCurrent(), generateZonePointCurrent());

                var dict = new Dictionary<ZonePoint, string> { { generateZonePointCurrent(), "a" } };

                Assert.IsTrue(dict.ContainsKey(generateZonePointCurrent()));
                Assert.IsNotNull(dict[generateZonePointCurrent()]);
            }
        }
    }
}
