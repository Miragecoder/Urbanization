namespace Mirage.Urbanization.Simulation
{
    public class PersistedCityStatistics
    {
        public int TimeCode { get; set; }

        public int PowerAmountOfConsumers { get; set; }
        public int PowerAmountOfSuppliers { get; set; }
        public int PowerConsumptionInUnits { get; set; }
        public int PowerSupplyInUnits { get; set; }

        public PersistedNumberSummary GlobalZonePopulationStatistics { get; set; }
        public PersistedNumberSummary ResidentialZonePopulationStatistics { get; set; }
        public PersistedNumberSummary CommercialZonePopulationStatistics { get; set; }
        public PersistedNumberSummary IndustrialZonePopulationStatistics { get; set; }

        public int NumberOfRoadZones { get; set; }
        public int NumberOfRailRoadZones { get; set; }
        public int NumberOfTrainStations { get; set; }

        public PersistedNumberSummary CrimeNumbers { get; set; }
        public PersistedNumberSummary PollutionNumbers { get; set; }
        public PersistedNumberSummary LandValueNumbers { get; set; }
        public PersistedNumberSummary TrafficNumbers { get; set; }
        public PersistedNumberSummary AverageTravelDistanceStatistics { get; set; }
    }
}