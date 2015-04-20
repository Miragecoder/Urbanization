namespace Mirage.Urbanization.Persistence
{
    public class PersistedSingleZoneConsumption
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Name { get; set; }
    }

    public class PersistedIntersectingZoneConsumption
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string EastWestName { get; set; }
        public string NorthSouthName { get; set; }
    }

    public class PersistedStaticZoneClusterCentralMemberConsumption
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Name { get; set; }
    }

    public class PersistedGrowthZoneClusterCentralMemberConsumption
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public int Population { get; set; }
    }

    public class PersistedTrafficState
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Traffic { get; set; }
    }
}