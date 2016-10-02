namespace Mirage.Urbanization.Vehicles
{
    public interface IShip : IMoveableVehicle
    {
        bool IsReadyAndMoving { get; }
    }
}