namespace Mirage.Urbanization
{
    public interface IVehicle
    {
        bool CanBeRemoved { get; }
        IZoneInfo CurrentPosition { get; }
        IZoneInfo PreviousPreviousPreviousPreviousPosition { get; }
        IZoneInfo PreviousPreviousPreviousPosition { get; }
        IZoneInfo PreviousPreviousPosition { get; }
        IZoneInfo PreviousPosition { get; }
    }
}