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
        private static readonly ZoneInfoDataMeter
            CrimeDataMeter = new ZoneInfoDataMeter(100,
                "Crime",
                x => x.PersistedCityStatistics.CrimeNumbers.Average,
                x => x.GetLastQueryCrimeResult().WithResultIfHasMatch(y => y.ValueInUnits),
                true
                ),
            FireHazardDataMeter = new ZoneInfoDataMeter(120,
                "Fire hazard",
                x => x.PersistedCityStatistics.FireHazardNumbers.Average,
                x => x.GetLastQueryFireHazardResult().WithResultIfHasMatch(y => y.ValueInUnits),
                true
                ),
            PollutionDataMeter = new ZoneInfoDataMeter(150,
                "Pollution",
                x => x.PersistedCityStatistics.PollutionNumbers.Average,
                x => x.GetLastQueryPollutionResult().WithResultIfHasMatch(y => y.ValueInUnits),
                true
                ),
            TrafficDataMeter = new ZoneInfoDataMeter(
                100,
                "Traffic",
                x => x.PersistedCityStatistics.TrafficNumbers.Average,
                x => new QueryResult<IZoneConsumptionWithTraffic>(x.ZoneConsumptionState.GetZoneConsumption() as IZoneConsumptionWithTraffic)
                    .WithResultIfHasMatch(y => y.GetTrafficDensityAsInt()),
                true
                ),
            TravelDistanceDataMeter = new ZoneInfoDataMeter(
                30,
                "Travel distance",
                x => x.PersistedCityStatistics.AverageTravelDistanceStatistics.Average,
                x => x.GetLastAverageTravelDistance() ?? 0,
                true
                ),
            PopulationDataMeter = new ZoneInfoDataMeter(
                10, "Population",
                x => x.PersistedCityStatistics.GlobalZonePopulationStatistics.Average,
                x => x.GetPopulationDensity(),
                false
                ),
            LandValueDataMeter = new ZoneInfoDataMeter(
                10, "Land value",
                x => x.PersistedCityStatistics.LandValueNumbers.Average,
                x => x.GetLastLandValueResult().WithResultIfHasMatch(y => y.ValueInUnits),
                false
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