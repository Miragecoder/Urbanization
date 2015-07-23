namespace Mirage.Urbanization
{
    public interface ICityServiceStrengthLevels
    {
        decimal PoliceStrength { get; }
        decimal FireSquadStrength { get; }

        decimal RoadInfrastructureStrength { get; }

        decimal RailroadInfrastructureStrength { get; }
    }
}