namespace Mirage.Urbanization
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

        public int NumberOfTrainStations
        {
            get { return _numberOfTrainStations; }
        }

        public int NumberOfRailRoadZones
        {
            get { return _numberOfRailRoadZones; }
        }
    }
}