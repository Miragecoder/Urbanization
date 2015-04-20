namespace Mirage.Urbanization.ZoneConsumption.Base.Behaviours
{
    public interface IElectricityBehaviour : IBehaviour
    {
        bool IsPowered { get; }
    }
}