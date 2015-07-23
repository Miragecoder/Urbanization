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