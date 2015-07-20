using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mirage.Urbanization.Statistics
{
    internal class GrowthZoneNetworkStatistics : IGrowthZoneStatistics
    {
        private readonly IRoadInfrastructureStatistics _roadInfraStructureStatistics;
        private readonly IRailroadInfrastructureStatistics _railroadInfrastructureStatistics;
        private readonly ICityServicesStatistics _cityServicesStatistics;

        public IRoadInfrastructureStatistics RoadInfrastructureStatistics => _roadInfraStructureStatistics;

        public GrowthZoneNetworkStatistics(
            IRoadInfrastructureStatistics roadInfraStructureStatistics,
            IRailroadInfrastructureStatistics railroadInfrastructureStatistics, 
            ICollection<int> residentialZonePopulationNumbers,
            ICollection<int> commercialZonePopulationNumbers,
            ICollection<int> industrialZonePopulationNumbers, 
            ICityServicesStatistics cityServicesStatistics
        )
        {
            _roadInfraStructureStatistics = roadInfraStructureStatistics;
            _railroadInfrastructureStatistics = railroadInfrastructureStatistics;
            _cityServicesStatistics = cityServicesStatistics;

            _residentialZonePopulationNumbers = new NumberSummary(residentialZonePopulationNumbers);
            _commercialZonePopulationNumbers = new NumberSummary(commercialZonePopulationNumbers);
            _industrialZonePopulationNumbers = new NumberSummary(industrialZonePopulationNumbers);

            _globalZonePopulationNumbers = new NumberSummary(residentialZonePopulationNumbers.Concat(commercialZonePopulationNumbers).Concat(industrialZonePopulationNumbers));
        }

        public IRailroadInfrastructureStatistics RailroadInfrastructureStatistics => _railroadInfrastructureStatistics;

        private readonly INumberSummary _residentialZonePopulationNumbers,
            _commercialZonePopulationNumbers,
            _industrialZonePopulationNumbers,
            _globalZonePopulationNumbers;

        public INumberSummary ResidentialZonePopulationNumbers => _residentialZonePopulationNumbers;

        public INumberSummary CommercialZonePopulationNumbers => _commercialZonePopulationNumbers;

        public INumberSummary IndustrialZonePopulationNumbers => _industrialZonePopulationNumbers;

        public INumberSummary GlobalZonePopulationNumbers => _globalZonePopulationNumbers;

        public ICityServicesStatistics CityServicesStatistics => _cityServicesStatistics;
    }
}