namespace Mirage.Urbanization.Statistics
{
    public class RailroadInfrastructureStatistics : IRailroadInfrastructureStatistics
    {
        public RailroadInfrastructureStatistics(int numberOfTrainStations, int numberOfRailRoadZones)
        {
            NumberOfTrainStations = numberOfTrainStations;
            NumberOfRailRoadZones = numberOfRailRoadZones;
        }

        public int NumberOfTrainStations { get; }

        public int NumberOfRailRoadZones { get; }
    }
}