namespace Mirage.Urbanization.ZoneConsumption.Base
{
    public interface IAreaObjectWithSeed
    {
        int Id { get; }
    }

    public interface IAreaObjectWithPopulationDensity : IAreaObjectWithSeed
    {
        int PopulationDensity { get; }
    }
}