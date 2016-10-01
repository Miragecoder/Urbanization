using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mirage.Urbanization.Statistics;
using Mirage.Urbanization.Vehicles;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization
{
    public interface IReadOnlyArea
    {
        int AmountOfZonesX { get; }
        int AmountOfZonesY { get; }

        IEnumerable<IReadOnlyZoneInfo> EnumerateZoneInfos();
        IEnumerable<Func<IAreaConsumption>> GetSupportedZoneConsumptionFactories();

        Task<IGrowthZoneStatistics> PerformGrowthSimulationCycle(CancellationToken cancellationToken);
        IPowerGridStatistics CalculatePowergridStatistics();

        IVehicleController<ITrain> TrainController { get; }
        IVehicleController<IAirplane> AirplaneController { get; }
        IVehicleController<IShip> ShipController { get; }

        IEnumerable<IVehicleController<IMoveableVehicle>> EnumerateVehicleControllers();
    }
}