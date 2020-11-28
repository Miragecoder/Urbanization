using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mirage.Urbanization.Vehicles;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneConsumption.Base.Behaviours;
using Mirage.Urbanization.ZoneStatisticsQuerying;
using Moq;

namespace Mirage.Urbanization.Test
{
    [TestClass]
    public class ShipControllerTests
    {
        private void TestWithAmountOfHarbors(int numberOfHarbours)
        {
            var terraFormingOptions = new TerraformingOptions()
            {
                VerticalRiver = true,
                HorizontalRiver = true
            };

            terraFormingOptions.SetZoneWidthAndHeight(TerraformingOptions.MinWidthAndHeight);
            var area = new Area(new AreaOptions(() => new Mock<ILandValueCalculator>().Object, terraFormingOptions, new ProcessOptions(() => false, () => false), () => new Mock<ICityServiceStrengthLevels>().Object));
            var shipController = new ShipController(() => area.EnumerateZoneInfos().OfType<IZoneInfo>().ToHashSet(), TimeSpan.FromMilliseconds(1), 1);

            var harbourFactory = new Func<SeaPortZoneClusterConsumption>(
                () => new SeaPortZoneClusterConsumption(
                    () => new ZoneInfoFinder(x => QueryResult<IZoneInfo>.Create(area.EnumerateZoneInfos().SingleOrDefault(y => y.ConsumptionState.GetZoneConsumption() == x))),
                    new ElectricitySupplierBehaviour(10)));

            foreach (var iteration in Enumerable.Range(0, numberOfHarbours))
            {
                var result = area.EnumerateZoneInfos().FirstOrDefault(x => area.ConsumeZoneAt(x, harbourFactory()).Success);

                Assert.IsNotNull(result);
            }
            Assert.AreNotEqual(0, area.CalculatePowergridStatistics());

            var shipDict = new Dictionary<IShip, IZoneInfo>();

            IZoneInfo previousPosition = null;
            foreach (var attempt in Enumerable.Range(0, 10))
            {
                shipController.ForEachActiveVehicle(true, ship =>
                {
                    if (!shipDict.ContainsKey(ship))
                        shipDict.Add(ship, null);
                    previousPosition = ship.CurrentPosition;
                    shipDict[ship] = previousPosition;
                });
                if ((numberOfHarbours * ShipController.AmountOfShipsPerHarbour == shipDict.Count) && shipDict.Values.All(x => x != null))
                    break;
                System.Threading.Thread.Sleep(100);
            }
            Assert.AreEqual(numberOfHarbours * ShipController.AmountOfShipsPerHarbour, shipDict.Count);

            foreach (var iteration in Enumerable.Range(0, 3))
            {
                System.Threading.Thread.Sleep(10);

                shipController.ForEachActiveVehicle(true, ship =>
                {
                    Assert.AreNotSame(previousPosition, ship.CurrentPosition);
                    previousPosition = ship.CurrentPosition;
                });
            }
        }

        [TestMethod]
        public void ShipControllerWithoutHarbours()
        {
            TestWithAmountOfHarbors(0);
        }
        [TestMethod]
        public void ShipControllerWithHarbour()
        {
            TestWithAmountOfHarbors(1);
        }

        [TestMethod]
        [Ignore]
        public void ShipControllerWithMultipleHarbours()
        {
            foreach (var i in Enumerable.Range(1, 15))
                TestWithAmountOfHarbors(i);
        }
    }
}
