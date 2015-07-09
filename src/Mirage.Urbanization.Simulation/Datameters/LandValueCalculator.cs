using System;
using System.Linq;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Simulation.Datameters
{
    public class LandValueCalculator : ILandValueCalculator
    {
        public static readonly LandValueCalculator Instance = new LandValueCalculator();

        public QueryResult<IQueryLandValueResult> GetFor(IReadOnlyZoneInfo zoneInfo)
        {
            var score = GetScoreFor(zoneInfo);

            var newScore = DataMeterInstances
                .DataMeters
                .Where(x => x.RepresentsIssue)
                .Select(x => x.GetDataMeterResult(zoneInfo))
                .Where(x => x.PercentageScore > 0)
                .Select(x => x.PercentageScore)
                .Aggregate(
                    (decimal)score,
                    (currentScore, percentageScore) => currentScore / (1 + percentageScore)
                );

            return new QueryResult<IQueryLandValueResult>(new QueryLandValueResult((int)Math.Round(newScore)));
        }

        private int GetScoreFor(IReadOnlyZoneInfo zoneInfo)
        {
            var consumption = zoneInfo.ZoneConsumptionState.GetZoneConsumption();

            if (consumption is ZoneClusterMemberConsumption)
            {
                var clusterMemberConsumption = consumption as ZoneClusterMemberConsumption;

                int multiplier = 1;

                var parentAsGrowthZone = clusterMemberConsumption.ParentBaseZoneClusterConsumption as BaseGrowthZoneClusterConsumption;
                if (parentAsGrowthZone != null)
                {
                    multiplier = parentAsGrowthZone.PopulationDensity;
                }

                return clusterMemberConsumption.SingleCellCost * multiplier;
            }

            if (consumption is EmptyZoneConsumption || consumption is WoodlandZoneConsumption || consumption is WaterZoneConsumption)
                return 0;

            return consumption.Cost;
        }
    }
}