using System.Collections.Generic;

namespace Mirage.Urbanization.Vehicles
{
    public interface ITrain : IMoveableVehicle
    {
        void SetTrainNetwork(ISet<IZoneInfo> trainNetwork);
    }
}