using System;
using System.Collections.Generic;
using System.Linq;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Simulation.Datameters
{
    public class LandValueCalculator : ILandValueCalculator
    {
        private readonly ICityBudgetConfiguration _cityBudgetConfiguration;

        public LandValueCalculator(ICityBudgetConfiguration cityBudgetConfiguration)
        {
            _cityBudgetConfiguration = cityBudgetConfiguration;
        }

        public QueryResult<IQueryLandValueResult> GetFor(IReadOnlyZoneInfo zoneInfo)
        {
            var score = GetScoreFor(zoneInfo);

            if (!score.HasValue)
                return QueryResult<IQueryLandValueResult>.Create();

            var newScore = GetIssueResults(zoneInfo, x => x.RepresentsIssue)
                .Aggregate(
                    (decimal)score,
                    (currentScore, percentageScore) => currentScore / (1 + percentageScore.IssueAmount)
                );

            return QueryResult<IQueryLandValueResult>.Create(new QueryLandValueResult((int)Math.Round(newScore)));
        }

        private struct IssueResult
        {
            public IssueResult(string issue, decimal issueAmount)
            {
                Issue = issue;
                IssueAmount = issueAmount;
            }

            public string Issue { get; }
            public decimal IssueAmount { get; }
        }

        private static IEnumerable<IssueResult> GetIssueResults(IReadOnlyZoneInfo zoneInfo, Func<ZoneInfoDataMeter, bool> where)
        {
            return DataMeterInstances
                .DataMeters
                .Where(where)
                .Select(x => new IssueResult(x.Name, x.GetDataMeterResult(zoneInfo).PercentageScore))
                .Where(x => x.IssueAmount > 0);
        }

        public IEnumerable<string> GetUndesirabilityReasons(BaseGrowthZoneClusterConsumption baseGrowthZoneClusterConsumption)
        {
            if (baseGrowthZoneClusterConsumption is ResidentialZoneClusterConsumption)
                return GetUndesirabilityReasons(baseGrowthZoneClusterConsumption, x => x.ResidentialTaxRate);
            if (baseGrowthZoneClusterConsumption is CommercialZoneClusterConsumption)
                return GetUndesirabilityReasons(baseGrowthZoneClusterConsumption, x => x.CommercialTaxRate);
            if (baseGrowthZoneClusterConsumption is IndustrialZoneClusterConsumption)
                return GetUndesirabilityReasons(baseGrowthZoneClusterConsumption, x => x.IndustrialTaxRate);
            throw new InvalidOperationException($"The type '{baseGrowthZoneClusterConsumption.GetType().Name}' is currently not supported.");
        }

        private IEnumerable<string> GetUndesirabilityReasons<TBaseGrowthZoneClusterConsumption>(
            TBaseGrowthZoneClusterConsumption baseGrowthZoneClusterConsumption,
            Func<ICityBudgetConfiguration, decimal> getCityBudgetValue)
            where TBaseGrowthZoneClusterConsumption : BaseGrowthZoneClusterConsumption
        {
            if (baseGrowthZoneClusterConsumption.PopulationDensity < 9)
                return Enumerable.Empty<string>();

            if (!_taxRateMultiplier.HasValue)
                _taxRateMultiplier = 1M / TaxDefinition.GetSelectableTaxRatePercentages().Max();

            return baseGrowthZoneClusterConsumption
                .ZoneClusterMembers
                .Single(x => x.IsCentralClusterMember)
                .GetZoneInfo()
                .WithResultIfHasMatch(centralZone =>
                {
                    var issueThreshold = 1.5M - (getCityBudgetValue(_cityBudgetConfiguration) * _taxRateMultiplier);
                    return GetIssueResults(centralZone, x => x.RepresentsUndesirabilityFor(baseGrowthZoneClusterConsumption))
                        .Where(x => x.IssueAmount > issueThreshold)
                        .Select(x => x.Issue);
                });

        }

        private decimal? _taxRateMultiplier;

        private static int? GetScoreFor(IReadOnlyZoneInfo zoneInfo)
        {
            var consumption = zoneInfo.ZoneConsumptionState.GetZoneConsumption();

            if (consumption is ZoneClusterMemberConsumption)
            {
                var clusterMemberConsumption = consumption as ZoneClusterMemberConsumption;

                var parentAsGrowthZone = clusterMemberConsumption.ParentBaseZoneClusterConsumption as BaseGrowthZoneClusterConsumption;
                if (parentAsGrowthZone != null)
                {
                    return clusterMemberConsumption.SingleCellCost;
                }
            }
            return null;
        }
    }
}