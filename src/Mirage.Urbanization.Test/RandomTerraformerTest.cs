using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;
using Moq;

namespace Mirage.Urbanization.Test
{
    [TestClass]
    public class RandomTerraformerTest
    {
        private ZoneInfoGrid InitializeZoneInfoGrid(bool withHorizontalRiver, bool withVerticalRiver, bool withWoodlands)
        {
            var r = new RandomTerraformer(
                () => new WaterZoneConsumption(new ZoneInfoFinder(x => QueryResult<IZoneInfo>.Create())),
                () => new WoodlandZoneConsumption(new ZoneInfoFinder(x => QueryResult<IZoneInfo>.Create())));
            var zoneInfoGrid = new ZoneInfoGrid(100, new Mock<ILandValueCalculator>().Object);

            var options = new TerraformingOptions()
            {
                HorizontalRiver = withHorizontalRiver,
                VerticalRiver = withVerticalRiver
            };

            options.SetWoodlands(withWoodlands ? 10 : 0);
            options.SetLakes(0);

            r.ApplyWith(zoneInfoGrid, options);

            return zoneInfoGrid;
        }

        [TestMethod]
        public void TestNoTerraforming()
        {
            var zoneInfoGrid = InitializeZoneInfoGrid(withHorizontalRiver: false, withVerticalRiver: false, withWoodlands: false);

            Assert.IsTrue(zoneInfoGrid.ZoneInfos.All(x => x.Value.ConsumptionState.GetZoneConsumption() is EmptyZoneConsumption));
            Assert.IsFalse(zoneInfoGrid.ZoneInfos.All(x => x.Value.ConsumptionState.GetZoneConsumption() is WoodlandZoneConsumption));
            Assert.IsFalse(zoneInfoGrid.ZoneInfos.All(x => x.Value.ConsumptionState.GetZoneConsumption() is WaterZoneConsumption));
        }

        [TestMethod]
        public void TestTerraformingWithWoods()
        {
            var zoneInfoGrid = InitializeZoneInfoGrid(withHorizontalRiver: false, withVerticalRiver: false, withWoodlands: true);

            Assert.IsFalse(zoneInfoGrid.ZoneInfos.All(x => x.Value.ConsumptionState.GetZoneConsumption() is EmptyZoneConsumption));
            Assert.IsTrue(zoneInfoGrid.ZoneInfos.Any(x => x.Value.ConsumptionState.GetZoneConsumption() is WoodlandZoneConsumption));
            Assert.IsFalse(zoneInfoGrid.ZoneInfos.All(x => x.Value.ConsumptionState.GetZoneConsumption() is WaterZoneConsumption));
        }

        [TestMethod]
        public void TestTerraformingWithRivers()
        {
            foreach (var boolPair in new[] { new { A = false, B = true }, new { A = true, B = false } })
            {
                var zoneInfoGrid = InitializeZoneInfoGrid(withHorizontalRiver: boolPair.A, withVerticalRiver: boolPair.B, withWoodlands: false);

                Assert.IsFalse(zoneInfoGrid.ZoneInfos.All(x => x.Value.ConsumptionState.GetZoneConsumption() is EmptyZoneConsumption));
                Assert.IsFalse(zoneInfoGrid.ZoneInfos.All(x => x.Value.ConsumptionState.GetZoneConsumption() is WoodlandZoneConsumption));
                Assert.IsTrue(zoneInfoGrid.ZoneInfos.Any(x => x.Value.ConsumptionState.GetZoneConsumption() is WaterZoneConsumption));
            }
        }
    }
}