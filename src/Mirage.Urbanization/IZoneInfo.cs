using System;
using System.Collections.Generic;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneConsumption.Base.Behaviours;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization
{
    public interface IZoneInfo : IReadOnlyZoneInfo
    {
        IZoneConsumptionState ConsumptionState { get; }
        QueryResult<IZoneInfo, RelativeZoneInfoQuery> GetRelativeZoneInfo(RelativeZoneInfoQuery relativeZoneInfoQuery);
        int GetDistanceScoreBasedOnConsumption();
        QueryResult<IZoneInfo, RelativeZoneInfoQuery> GetNorth();
        QueryResult<IZoneInfo, RelativeZoneInfoQuery> GetSouth();
        QueryResult<IZoneInfo, RelativeZoneInfoQuery> GetEast();
        QueryResult<IZoneInfo, RelativeZoneInfoQuery> GetWest();
        QueryResult<IQueryPollutionResult> QueryPollution();
        QueryResult<IPollutionBehaviour> GetPollutionBehaviour();
        QueryResult<ICrimeBehaviour> GetCrimeBehaviour();
        QueryResult<IFireHazardBehaviour> GetFireHazardBehaviour();
        QueryResult<IQueryCrimeResult> QueryCrime();
        void WithZoneConsumptionIf<T>(Action<T> action) where T : BaseZoneConsumption;

        IEnumerable<Func<IZoneInfo, QueryResult<IZoneInfo, RelativeZoneInfoQuery>>> GetSteerDirections(
            Func<IZoneInfo, QueryResult<IZoneInfo, RelativeZoneInfoQuery>> expression
            );

        QueryResult<TNetworkZoneConsumption> GetNetworkZoneConsumption<TNetworkZoneConsumption>()
            where TNetworkZoneConsumption : BaseNetworkZoneConsumption;

        void WithNetworkConsumptionIf<TNetworkZoneConsumption>(Action<TNetworkZoneConsumption> action)
            where TNetworkZoneConsumption : BaseNetworkZoneConsumption;

        bool IsGrowthZoneClusterOfType<T>() where T : BaseGrowthZoneClusterConsumption;
        QueryResult<T> GetAsZoneCluster<T>() where T : BaseZoneClusterConsumption;
        void WithZoneClusterIf<T>(Action<T> action) where T : BaseZoneClusterConsumption;
        QueryResult<IQueryLandValueResult> QueryLandValue();
    }
}