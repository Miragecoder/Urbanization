namespace Mirage.Urbanization.Statistics
{
    internal class CityServiceStatistics : ICityServicesStatistics
    {
        public CityServiceStatistics(
            int numberOfPoliceStations, 
            int numberOfFireStations, 
            int numberOfStadiums, 
            int numberOfSeaports, 
            int numberOfAirports
            )
        {
            NumberOfPoliceStations = numberOfPoliceStations;
            NumberOfFireStations = numberOfFireStations;
            NumberOfStadiums = numberOfStadiums;
            NumberOfSeaports = numberOfSeaports;
            NumberOfAirports = numberOfAirports;
        }

        public int NumberOfPoliceStations { get; }
        public int NumberOfFireStations { get; }
        public int NumberOfStadiums { get; }
        public int NumberOfSeaports { get; }
        public int NumberOfAirports { get; }
    }
}