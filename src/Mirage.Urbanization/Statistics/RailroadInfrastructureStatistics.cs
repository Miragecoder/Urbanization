namespace Mirage.Urbanization.Statistics
{
    public class RailroadInfrastructureStatistics : IRailroadInfrastructureStatistics
    {
        private readonly int _numberOfTrainStations;
        private readonly int _numberOfRailRoadZones;

        public RailroadInfrastructureStatistics(int numberOfTrainStations, int numberOfRailRoadZones)
        {
            _numberOfTrainStations = numberOfTrainStations;
            _numberOfRailRoadZones = numberOfRailRoadZones;
        }

        public int NumberOfTrainStations => _numberOfTrainStations;

        public int NumberOfRailRoadZones => _numberOfRailRoadZones;
    }
}