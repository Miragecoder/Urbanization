using System.Collections.Generic;

namespace Mirage.Urbanization.Vehicles
{
    public interface IVehicle
    {
        bool CanBeRemoved { get; }
        IEnumerable<IZoneInfo> TraversePath();
        IZoneInfo PreviousPreviousPreviousPreviousPosition { get; }
        IZoneInfo PreviousPreviousPreviousPosition { get; }
        IZoneInfo PreviousPreviousPosition { get; }
        IZoneInfo PreviousPosition { get; }
        IZoneInfo CurrentPosition { get; }
    }
}