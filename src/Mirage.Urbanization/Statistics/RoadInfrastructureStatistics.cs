using System.Collections.Generic;
using System.Linq;
using Mirage.Urbanization.ZoneConsumption;

namespace Mirage.Urbanization.Statistics
{
    public class RoadInfrastructureStatistics : IRoadInfrastructureStatistics
    {
        private readonly int _numberOfRoadZones;
        private readonly INumberSummary _trafficNumbers;

        public RoadInfrastructureStatistics(IEnumerable<RoadZoneConsumption> roadZoneConsumptions)
        {
            var capturedRoadZoneConsumptions = roadZoneConsumptions.ToList();

            _numberOfRoadZones = capturedRoadZoneConsumptions.Count();
            _trafficNumbers = new NumberSummary(capturedRoadZoneConsumptions.Select(x => x.GetTrafficDensityAsInt()));
        }

        public int NumberOfRoadZones
        {
            get { return _numberOfRoadZones; }
        }

        public INumberSummary TrafficNumbers
        {
            get { return _trafficNumbers; }
        }
    }
}