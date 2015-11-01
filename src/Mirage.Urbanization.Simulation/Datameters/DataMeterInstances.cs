using System;
using System.Collections.Generic;
using System.Linq;
using Mirage.Urbanization.Simulation.Persistence;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Simulation.Datameters
{
    public static class DataMeterInstances
    {
        public static readonly ZoneInfoDataMeter
            CrimeDataMeter = new ZoneInfoDataMeter(100,
                "Crime",
                x => x.PersistedCityStatistics.CrimeNumbers.Average,
                x => x.GetLastQueryCrimeResult().WithResultIfHasMatch(y => y.ValueInUnits),
                true,
                new [] { typeof(ResidentialZoneClusterConsumption), typeof(CommercialZoneClusterConsumption), typeof(IndustrialZoneClusterConsumption)}
                ),
            FireHazardDataMeter = new ZoneInfoDataMeter(120,
                "Fire hazard",
                x => x.PersistedCityStatistics.FireHazardNumbers.Average,
                x => x.GetLastQueryFireHazardResult().WithResultIfHasMatch(y => y.ValueInUnits),
                true,
                new[] { typeof(ResidentialZoneClusterConsumption), typeof(CommercialZoneClusterConsumption), typeof(IndustrialZoneClusterConsumption) }
                ),
            PollutionDataMeter = new ZoneInfoDataMeter(150,
                "Pollution",
                x => x.PersistedCityStatistics.PollutionNumbers.Average,
                x => x.GetLastQueryPollutionResult().WithResultIfHasMatch(y => y.ValueInUnits),
                true,
                new[] { typeof(ResidentialZoneClusterConsumption) }
                ),
            TrafficDataMeter = new ZoneInfoDataMeter(
                100,
                "Traffic",
                x => x.PersistedCityStatistics.TrafficNumbers.Average,
                x => QueryResult<IZoneConsumptionWithTraffic>.Create(x.ZoneConsumptionState.GetZoneConsumption() as IZoneConsumptionWithTraffic)
                    .WithResultIfHasMatch(y => y.GetTrafficDensityAsInt()),
                true,
                Enumerable.Empty<Type>().ToArray()
                ),
            TravelDistanceDataMeter = new ZoneInfoDataMeter(
                60,
                "Travel distance",
                x => x.PersistedCityStatistics.AverageTravelDistanceStatistics.Average,
                x => x.GetLastAverageTravelDistance() ?? 0,
                true,
                Enumerable.Empty<Type>().ToArray()
                ),
            PopulationDataMeter = new ZoneInfoDataMeter(
                10, "Population",
                x => x.PersistedCityStatistics.GlobalZonePopulationStatistics.Average,
                x => x.GetPopulationDensity(),
                false,
                Enumerable.Empty<Type>().ToArray()
                ),
            LandValueDataMeter = new ZoneInfoDataMeter(
                3, "Land value",
                x => x.PersistedCityStatistics.LandValueNumbers.Average,
                x => x.GetLastLandValueResult().WithResultIfHasMatch(y => y.ValueInUnits),
                false,
                Enumerable.Empty<Type>().ToArray()
                );

        public static readonly IReadOnlyCollection<ZoneInfoDataMeter> DataMeters = new[]
        {
            CrimeDataMeter,
            FireHazardDataMeter,
            PollutionDataMeter,
            TrafficDataMeter,
            PopulationDataMeter,
            TravelDistanceDataMeter,
            LandValueDataMeter
        };

        public static IEnumerable<DataMeterResult> GetDataMeterResults(PersistedCityStatisticsWithFinancialData statistics, Func<DataMeter, bool> predicate)
        {
            return DataMeters.Where(predicate).Select(meter => meter.GetDataMeterResult(statistics));
        }
    }
}