using System.Collections.Generic;
using System.Linq;
using Mirage.Urbanization.ZoneConsumption;

namespace Mirage.Urbanization.Statistics
{
    public class RoadInfrastructureStatistics : IRoadInfrastructureStatistics
    {
        public RoadInfrastructureStatistics(IEnumerable<RoadZoneConsumption> roadZoneConsumptions)
        {
            var capturedRoadZoneConsumptions = roadZoneConsumptions.ToList();

            NumberOfRoadZones = capturedRoadZoneConsumptions.Count();
            TrafficNumbers = new NumberSummary(capturedRoadZoneConsumptions.Select(x => x.GetTrafficDensityAsInt()));
        }

        public int NumberOfRoadZones { get; }

        public INumberSummary TrafficNumbers { get; }
    }
}