using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mirage.Urbanization.Statistics
{
    internal class GrowthZoneNetworkStatistics : IGrowthZoneStatistics
    {
        private readonly IRoadInfrastructureStatistics _roadInfraStructureStatistics;
        private readonly IRailroadInfrastructureStatistics _railroadInfrastructureStatistics;

        public IRoadInfrastructureStatistics RoadInfrastructureStatistics
        {
            get
            {
                return _roadInfraStructureStatistics;
            }
        }

        public GrowthZoneNetworkStatistics(
            IRoadInfrastructureStatistics roadInfraStructureStatistics,
            IRailroadInfrastructureStatistics railroadInfrastructureStatistics, 
            ICollection<int> residentialZonePopulationNumbers,
            ICollection<int> commercialZonePopulationNumbers,
            ICollection<int> industrialZonePopulationNumbers)
        {
            _roadInfraStructureStatistics = roadInfraStructureStatistics;
            _railroadInfrastructureStatistics = railroadInfrastructureStatistics;

            _residentialZonePopulationNumbers = new NumberSummary(residentialZonePopulationNumbers);
            _commercialZonePopulationNumbers = new NumberSummary(commercialZonePopulationNumbers);
            _industrialZonePopulationNumbers = new NumberSummary(industrialZonePopulationNumbers);

            _globalZonePopulationNumbers = new NumberSummary(residentialZonePopulationNumbers.Concat(commercialZonePopulationNumbers).Concat(industrialZonePopulationNumbers));
        }

        public IRailroadInfrastructureStatistics RailroadInfrastructureStatistics
        {
            get { return _railroadInfrastructureStatistics; }
        }

        private readonly INumberSummary _residentialZonePopulationNumbers,
            _commercialZonePopulationNumbers,
            _industrialZonePopulationNumbers,
            _globalZonePopulationNumbers;

        public INumberSummary ResidentialZonePopulationNumbers
        {
            get { return _residentialZonePopulationNumbers; }
        }

        public INumberSummary CommercialZonePopulationNumbers
        {
            get { return _commercialZonePopulationNumbers; }
        }

        public INumberSummary IndustrialZonePopulationNumbers
        {
            get { return _industrialZonePopulationNumbers; }
        }

        public INumberSummary GlobalZonePopulationNumbers
        {
            get { return _globalZonePopulationNumbers; }
        }
    }
}