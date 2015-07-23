namespace Mirage.Urbanization.Vehicles
{
    public interface IMoveableVehicle : IVehicle
    {
        void Move();

        decimal Progress { get; }
    }
}