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

            var newScore = GetIssueResults(zoneInfo, x => x.RepresentsIssue)
                .Aggregate(
                    (decimal)score,
                    (currentScore, percentageScore) => currentScore / (1 + percentageScore)
                );

            return new QueryResult<IQueryLandValueResult>(new QueryLandValueResult((int)Math.Round(newScore)));
        }

        private static IEnumerable<decimal> GetIssueResults(IReadOnlyZoneInfo zoneInfo, Func<DataMeter, bool> where)
        {
            return DataMeterInstances
                .DataMeters
                .Where(x => x.RepresentsIssue)
                .Select(x => x.GetDataMeterResult(zoneInfo))
                .Where(x => x.PercentageScore > 0)
                .Select(x => x.PercentageScore);
        }

        public bool AllowsForGrowth(BaseGrowthZoneClusterConsumption baseGrowthZoneClusterConsumption)
        {
            if (baseGrowthZoneClusterConsumption is ResidentialZoneClusterConsumption)
                return AllowsForGrowth((ResidentialZoneClusterConsumption)baseGrowthZoneClusterConsumption, x => x.ResidentialTaxRate);
            else if (baseGrowthZoneClusterConsumption is CommercialZoneClusterConsumption)
                return AllowsForGrowth((CommercialZoneClusterConsumption)baseGrowthZoneClusterConsumption, x => x.CommercialTaxRate);
            else if (baseGrowthZoneClusterConsumption is IndustrialZoneClusterConsumption)
                return AllowsForGrowth((IndustrialZoneClusterConsumption)baseGrowthZoneClusterConsumption, x => x.IndustrialTaxRate);
            throw new InvalidOperationException($"The type '{baseGrowthZoneClusterConsumption.GetType().Name}' is currently not supported.");
        }

        private bool AllowsForGrowth<TBaseGrowthZoneClusterConsumption>(
            TBaseGrowthZoneClusterConsumption baseGrowthZoneClusterConsumption,
            Func<ICityBudgetConfiguration, decimal> getCityBudgetValue)
            where TBaseGrowthZoneClusterConsumption : BaseGrowthZoneClusterConsumption
        {
            if (baseGrowthZoneClusterConsumption.PopulationDensity < 9)
                return true;

            if (!_taxRateMultiplier.HasValue)
                _taxRateMultiplier = 1M / TaxDefinition.GetSelectableTaxRatePercentages().Max();

            return baseGrowthZoneClusterConsumption
                .ZoneClusterMembers
                .Single(x => x.IsCentralClusterMember)
                .GetZoneInfo()
                .WithResultIfHasMatch(centralZone =>
                {
                    var issueThreshold = getCityBudgetValue(_cityBudgetConfiguration) * _taxRateMultiplier;
                    return GetIssueResults(centralZone, x => x.RepresentsIssue 
                        && x != DataMeterInstances.PollutionDataMeter 
                        && x != DataMeterInstances.TravelDistanceDataMeter)
                        .All(x => x < issueThreshold);
                });
        }

        private decimal? _taxRateMultiplier;

        private static int GetScoreFor(IReadOnlyZoneInfo zoneInfo)
        {
            var consumption = zoneInfo.ZoneConsumptionState.GetZoneConsumption();

            if (consumption is ZoneClusterMemberConsumption)
            {
                var clusterMemberConsumption = consumption as ZoneClusterMemberConsumption;

                var parentAsGrowthZone = clusterMemberConsumption.ParentBaseZoneClusterConsumption as BaseGrowthZoneClusterConsumption;
                if (parentAsGrowthZone != null)
                {
                    return clusterMemberConsumption.SingleCellCost * parentAsGrowthZone.PopulationDensity;
                }
            }
            return 0;
        }
    }
}