namespace Mirage.Urbanization.Statistics
{
    public interface ICityServicesStatistics
    {
        int NumberOfPoliceStations { get; }
        int NumberOfFireStations { get; }

        int NumberOfStadiums { get; }
        int NumberOfHarbours { get; }
        int NumberOfAirports { get; }
    }
}