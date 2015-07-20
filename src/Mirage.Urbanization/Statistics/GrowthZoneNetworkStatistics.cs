using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mirage.Urbanization.Statistics
{
    internal class GrowthZoneNetworkStatistics : IGrowthZoneStatistics
    {
        public IRoadInfrastructureStatistics RoadInfrastructureStatistics { get; }

        public GrowthZoneNetworkStatistics(
            IRoadInfrastructureStatistics roadInfraStructureStatistics,
            IRailroadInfrastructureStatistics railroadInfrastructureStatistics, 
            ICollection<int> residentialZonePopulationNumbers,
            ICollection<int> commercialZonePopulationNumbers,
            ICollection<int> industrialZonePopulationNumbers, 
            ICityServicesStatistics cityServicesStatistics
        )
        {
            RoadInfrastructureStatistics = roadInfraStructureStatistics;
            RailroadInfrastructureStatistics = railroadInfrastructureStatistics;
            CityServicesStatistics = cityServicesStatistics;

            ResidentialZonePopulationNumbers = new NumberSummary(residentialZonePopulationNumbers);
            CommercialZonePopulationNumbers = new NumberSummary(commercialZonePopulationNumbers);
            IndustrialZonePopulationNumbers = new NumberSummary(industrialZonePopulationNumbers);

            GlobalZonePopulationNumbers = new NumberSummary(residentialZonePopulationNumbers.Concat(commercialZonePopulationNumbers).Concat(industrialZonePopulationNumbers));
        }

        public IRailroadInfrastructureStatistics RailroadInfrastructureStatistics { get; }

        public INumberSummary ResidentialZonePopulationNumbers { get; }

        public INumberSummary CommercialZonePopulationNumbers { get; }

        public INumberSummary IndustrialZonePopulationNumbers { get; }

        public INumberSummary GlobalZonePopulationNumbers { get; }

        public ICityServicesStatistics CityServicesStatistics { get; }
    }
}