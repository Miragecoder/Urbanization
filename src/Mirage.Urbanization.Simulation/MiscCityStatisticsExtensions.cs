using System.Linq;
using Mirage.Urbanization.Statistics;

namespace Mirage.Urbanization.Simulation
{
    public static class MiscCityStatisticsExtensions
    {
        public static PersistedCityStatistics Convert(this ICityStatistics cityStatistics)
        {
            return new PersistedCityStatistics
            {
                CrimeNumbers = new PersistedNumberSummary(cityStatistics.MiscCityStatistics.CrimeNumbers),
                PollutionNumbers = new PersistedNumberSummary(cityStatistics.MiscCityStatistics.PollutionNumbers),
                LandValueNumbers = new PersistedNumberSummary(cityStatistics.MiscCityStatistics.LandValueNumbers),
                TrafficNumbers = new PersistedNumberSummary(cityStatistics.GrowthZoneStatistics.RoadInfrastructureStatistics.TrafficNumbers),

                NumberOfRoadZones = cityStatistics.GrowthZoneStatistics.RoadInfrastructureStatistics.NumberOfRoadZones,

                AverageTravelDistanceStatistics = new PersistedNumberSummary(cityStatistics.MiscCityStatistics.TravelDistanceNumbers),

                NumberOfRailRoadZones = cityStatistics.GrowthZoneStatistics.RailroadInfrastructureStatistics.NumberOfRailRoadZones,
                NumberOfTrainStations = cityStatistics.GrowthZoneStatistics.RailroadInfrastructureStatistics.NumberOfTrainStations,

                PowerAmountOfConsumers = cityStatistics.PowerGridStatistics.PowerGridNetworkStatistics.Sum(x => x.AmountOfConsumers),
                PowerAmountOfSuppliers = cityStatistics.PowerGridStatistics.PowerGridNetworkStatistics.Sum(x => x.AmountOfSuppliers),
                PowerConsumptionInUnits = cityStatistics.PowerGridStatistics.PowerGridNetworkStatistics.Sum(x => x.ConsumptionInUnits),
                PowerSupplyInUnits = cityStatistics.PowerGridStatistics.PowerGridNetworkStatistics.Sum(x => x.SupplyInUnits),

                CommercialZonePopulationStatistics = new PersistedNumberSummary(cityStatistics.GrowthZoneStatistics.CommercialZonePopulationNumbers),
                IndustrialZonePopulationStatistics = new PersistedNumberSummary(cityStatistics.GrowthZoneStatistics.IndustrialZonePopulationNumbers),
                ResidentialZonePopulationStatistics = new PersistedNumberSummary(cityStatistics.GrowthZoneStatistics.ResidentialZonePopulationNumbers),
                GlobalZonePopulationStatistics = new PersistedNumberSummary(cityStatistics.GrowthZoneStatistics.GlobalZonePopulationNumbers),

                TimeCode = cityStatistics.TimeCode,
            };
        }
    }
}