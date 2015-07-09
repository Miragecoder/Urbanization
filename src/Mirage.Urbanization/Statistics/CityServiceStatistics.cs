namespace Mirage.Urbanization.Statistics
{
    internal class CityServiceStatistics : ICityServicesStatistics
    {
        public CityServiceStatistics(
            int numberOfPoliceStations, 
            int numberOfFireStations, 
            int numberOfStadiums, 
            int numberOfHarbours, 
            int numberOfAirports
            )
        {
            NumberOfPoliceStations = numberOfPoliceStations;
            NumberOfFireStations = numberOfFireStations;
            NumberOfStadiums = numberOfStadiums;
            NumberOfHarbours = numberOfHarbours;
            NumberOfAirports = numberOfAirports;
        }

        public int NumberOfPoliceStations { get; private set; }
        public int NumberOfFireStations { get; private set; }
        public int NumberOfStadiums { get; private set; }
        public int NumberOfHarbours { get; private set; }
        public int NumberOfAirports { get; private set; }
    }
}