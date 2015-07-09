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

        public int NumberOfPoliceStations { get; private set; }
        public int NumberOfFireStations { get; private set; }
        public int NumberOfStadiums { get; private set; }
        public int NumberOfSeaports { get; private set; }
        public int NumberOfAirports { get; private set; }
    }
}