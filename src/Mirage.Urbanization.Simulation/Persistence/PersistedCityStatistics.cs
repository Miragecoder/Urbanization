using System;
using System.Linq;
using Mirage.Urbanization.Statistics;

namespace Mirage.Urbanization.Simulation.Persistence
{
    public class PersistedCityStatistics
    {
        public PersistedCityStatistics() { }

        public PersistedCityStatistics(
            int timeCode,
            IPowerGridStatistics powerGridStatistics,
            IGrowthZoneStatistics growthZoneStatistics,
            IMiscCityStatistics miscCityStatistics
        )
        {
            if (timeCode == default(int)) throw new ArgumentOutOfRangeException("timeCode");
            if (powerGridStatistics == null) throw new ArgumentNullException("powerGridStatistics");
            if (growthZoneStatistics == null) throw new ArgumentNullException("growthZoneStatistics");
            if (miscCityStatistics == null) throw new ArgumentNullException("miscCityStatistics");

            TimeCode = timeCode;
            CrimeNumbers = new PersistedNumberSummary(miscCityStatistics.CrimeNumbers);
            FireHazardNumbers = new PersistedNumberSummary(miscCityStatistics.FireHazardNumbers);
            PollutionNumbers = new PersistedNumberSummary(miscCityStatistics.PollutionNumbers);
            LandValueNumbers = new PersistedNumberSummary(miscCityStatistics.LandValueNumbers);
            TrafficNumbers = new PersistedNumberSummary(growthZoneStatistics.RoadInfrastructureStatistics.TrafficNumbers);

            NumberOfRoadZones = growthZoneStatistics.RoadInfrastructureStatistics.NumberOfRoadZones;

            AverageTravelDistanceStatistics = new PersistedNumberSummary(miscCityStatistics.TravelDistanceNumbers);

            NumberOfRailRoadZones = growthZoneStatistics.RailroadInfrastructureStatistics.NumberOfRailRoadZones;
            NumberOfTrainStations = growthZoneStatistics.RailroadInfrastructureStatistics.NumberOfTrainStations;

            PowerAmountOfConsumers = powerGridStatistics.PowerGridNetworkStatistics.Sum(x => x.AmountOfConsumers);
            PowerAmountOfSuppliers = powerGridStatistics.PowerGridNetworkStatistics.Sum(x => x.AmountOfSuppliers);
            PowerConsumptionInUnits = powerGridStatistics.PowerGridNetworkStatistics.Sum(x => x.ConsumptionInUnits);
            PowerSupplyInUnits = powerGridStatistics.PowerGridNetworkStatistics.Sum(x => x.SupplyInUnits);

            CommercialZonePopulationStatistics = new PersistedNumberSummary(growthZoneStatistics.CommercialZonePopulationNumbers);
            IndustrialZonePopulationStatistics = new PersistedNumberSummary(growthZoneStatistics.IndustrialZonePopulationNumbers);
            ResidentialZonePopulationStatistics = new PersistedNumberSummary(growthZoneStatistics.ResidentialZonePopulationNumbers);
            GlobalZonePopulationStatistics = new PersistedNumberSummary(growthZoneStatistics.GlobalZonePopulationNumbers);

            NumberOfStadiums = growthZoneStatistics.CityServicesStatistics.NumberOfStadiums;
            NumberOfAirports = growthZoneStatistics.CityServicesStatistics.NumberOfAirports;
            NumberOfSeaPorts = growthZoneStatistics.CityServicesStatistics.NumberOfSeaports;
            NumberOfPoliceStations = growthZoneStatistics.CityServicesStatistics.NumberOfPoliceStations;
            NumberOfFireStations = growthZoneStatistics.CityServicesStatistics.NumberOfFireStations;

            TimeCode = TimeCode;

            _yearAndMonthLazy = new Lazy<IReadOnlyYearAndMonth>(() => new ReadOnlyYearAndMonth(TimeCode));
        }

        public int TimeCode { get; set; }

        private readonly Lazy<IReadOnlyYearAndMonth> _yearAndMonthLazy;

        public IReadOnlyYearAndMonth GetYearAndMonth()
        {
            return _yearAndMonthLazy.Value;
        }

        public bool SharesYearWith(PersistedCityStatistics statistics) { return GetYearAndMonth() == statistics.GetYearAndMonth(); }

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

        public int NumberOfStadiums { get; set; }
        public int NumberOfAirports { get; set; }
        public int NumberOfSeaPorts { get; set; }
        public int NumberOfPoliceStations { get; set; }
        public int NumberOfFireStations { get; set; }

        public PersistedNumberSummary CrimeNumbers { get; set; }
        public PersistedNumberSummary FireHazardNumbers { get; set; }
        public PersistedNumberSummary PollutionNumbers { get; set; }
        public PersistedNumberSummary LandValueNumbers { get; set; }
        public PersistedNumberSummary TrafficNumbers { get; set; }
        public PersistedNumberSummary AverageTravelDistanceStatistics { get; set; }
    }
}