using System;

namespace Mirage.Urbanization.Vehicles
{
    public interface IVehicleController<out TVehicle> where TVehicle : IVehicle
    {
        void ForEachActiveVehicle(Action<TVehicle> vehicleAction);
    }
}