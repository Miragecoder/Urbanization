using System.Collections.Generic;

namespace Mirage.Urbanization
{
    public interface ITrain : IMoveableVehicle
    {
        void SetTrainNetwork(ISet<IZoneInfo> trainNetwork);
    }
}