using System;
using System.Collections.Generic;
using System.Threading;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization.GrowthPathFinding
{
    class GrowthZoneInfoPathNode : ZoneInfoPathNode
    {
        public GrowthZoneInfoPathNode(IZoneInfo zoneInfo, ZoneClusterMemberConsumption clusterMemberConsumption, ProcessOptions processOptions, HashSet<BaseGrowthZoneClusterConsumption> undesirableGrowthZones)
            : base(
            zoneInfo: zoneInfo, 
            canBeJoinedFunc: (previousPath, currentZoneInfo) =>
            {
                var matchingPath = previousPath;

                if (processOptions.GetStepByStepGrowthCyclingToggled()) Thread.Sleep(50);

                var evaluatingNonIntersection = false;

                while (!evaluatingNonIntersection)
                {
                    var intersection =
                        matchingPath.ZoneInfo.ConsumptionState.GetZoneConsumption() as
                            IntersectingZoneConsumption;

                    if (intersection != null)
                    {
                        matchingPath = matchingPath.PreviousPathNode;
                    }
                    else
                    {
                        evaluatingNonIntersection = true;
                    }
                }
                matchingPath.ZoneInfo.GrowthAlgorithmHighlightState.SetState(HighlightState.Examined);
                currentZoneInfo.MatchingObject.GrowthAlgorithmHighlightState.SetState(HighlightState.Examined);

                Func<bool> isSuccessFunc = null;
                string currentIsSuccessFuncDesc = String.Empty;

                Action<string, Func<bool>> assignIsSuccessOverrideFuncAction = (description, action) =>
                {
                    isSuccessFunc = action;
                    currentIsSuccessFuncDesc = description;
                };

                Action<string, Func<bool>> assignIsSuccessFuncAction = (description, action) =>
                {
                    if (isSuccessFunc != null) throw new InvalidOperationException($"Could not set {description} as the 'IsSuccessFunc'. {currentIsSuccessFuncDesc} Is currently set.");
                    assignIsSuccessOverrideFuncAction(description, action);
                };

                // If the current zone is a growth zone...
                currentZoneInfo
                    .MatchingObject
                    .WithZoneClusterIf<BaseGrowthZoneClusterConsumption>(growthZone =>
                    {
                        if (!growthZone.HasPower)
                            return;

                        // And the previous path member was also a growth zone, then 
                        // they must both be part of the zone cluster that originated this
                        // path...
                        matchingPath
                            .ZoneInfo
                            .WithZoneClusterIf<BaseGrowthZoneClusterConsumption>(res =>
                                assignIsSuccessFuncAction("BaseGrowthZoneClusterConsumption",
                                    () =>
                                        res == clusterMemberConsumption.ParentBaseZoneClusterConsumption
                                        && growthZone == clusterMemberConsumption.ParentBaseZoneClusterConsumption)
                            );

                        // And the previous zone was a road zone...
                        matchingPath
                            .ZoneInfo
                            .WithNetworkConsumptionIf<RoadZoneConsumption>(previousRoadZone => assignIsSuccessFuncAction("RoadZoneConsumption", () => true));
                    });

                // If the current zone is a road...
                currentZoneInfo
                    .MatchingObject
                    .WithNetworkConsumptionIf<RoadZoneConsumption>(currentRoadZone =>
                    {
                        // And the previous zone was a growth zone...
                        matchingPath
                            .ZoneInfo
                            .WithZoneClusterIf<BaseGrowthZoneClusterConsumption>(res => assignIsSuccessFuncAction(
                                "BaseGrowthZoneClusterConsumption", () =>
                                    (res ==
                                     clusterMemberConsumption
                                         .ParentBaseZoneClusterConsumption)));

                        // And the previous zone was a trainstation...
                        matchingPath
                            .ZoneInfo
                            .WithZoneClusterIf<TrainStationZoneClusterConsumption>(res => assignIsSuccessOverrideFuncAction("RoadZoneConsumption/TrainStationZoneClusterConsumption", () => true));

                        // And the previous one was also a road...
                        matchingPath
                            .ZoneInfo
                            .WithNetworkConsumptionIf<RoadZoneConsumption>(
                                previousRoadZone => assignIsSuccessFuncAction("RoadZoneConsumption", () => true));
                    });

                // If the current zone is a railroad...
                currentZoneInfo
                    .MatchingObject
                    .WithNetworkConsumptionIf<RailRoadZoneConsumption>(currentRailRoad =>
                    {
                        // And the previous one was also a railroad...
                        matchingPath
                            .ZoneInfo
                            .WithNetworkConsumptionIf<RailRoadZoneConsumption>(
                                previousRailRoadZone => assignIsSuccessFuncAction("RailRoadZoneConsumption", () => true));

                        // And the previous zone was a trainstation zone...
                        matchingPath
                            .ZoneInfo
                            .WithZoneClusterIf<TrainStationZoneClusterConsumption>(res =>
                            {
                                if (res.HasPower)
                                    assignIsSuccessOverrideFuncAction(
                                    "RailRoadZoneConsumption/TrainStationZoneClusterConsumption", 
                                    () => true
                                );
                            });
                    });

                // If the current zone is a trainstation zone...
                currentZoneInfo
                    .MatchingObject
                    .WithZoneClusterIf<TrainStationZoneClusterConsumption>(growthZone =>
                    {
                        if (!growthZone.HasPower)
                            return;

                        // And the previous zone was part of the same trainstation...
                        matchingPath
                            .ZoneInfo
                            .WithZoneClusterIf<TrainStationZoneClusterConsumption>(res => assignIsSuccessFuncAction("TrainStationZoneClusterConsumption/TrainStationZoneClusterConsumption", () => growthZone == res));

                        // And the previous one was a road...
                        matchingPath
                            .ZoneInfo
                            .WithNetworkConsumptionIf<RoadZoneConsumption>(
                                previousRoadZone => assignIsSuccessFuncAction("RoadZoneConsumption", () => true));

                        // And the previous one was a road...
                        matchingPath
                            .ZoneInfo
                            .WithNetworkConsumptionIf<RailRoadZoneConsumption>(
                                previousRoadZone => assignIsSuccessFuncAction("RailRoadZoneConsumption", () => true));
                    });

                return isSuccessFunc != null
                       && isSuccessFunc();
            }, 
            getDestinationHashCode: (match) =>
            {
                var destinationHashCode = default(int?);

                if (clusterMemberConsumption.ParentBaseZoneClusterConsumption is IndustrialZoneClusterConsumption)
                {
                    match.WithZoneClusterIf<ResidentialZoneClusterConsumption>(
                        cluster =>
                        {
                            if (undesirableGrowthZones.Contains(cluster))
                                return;
                            if (cluster.GetType() !=
                                clusterMemberConsumption.ParentBaseZoneClusterConsumption
                                    .GetType())
                                destinationHashCode = cluster.GetHashCode();
                        });
                }
                else
                {
                    match.WithZoneClusterIf<BaseGrowthZoneClusterConsumption>(
                        cluster =>
                        {
                            if (undesirableGrowthZones.Contains(cluster))
                                return;
                            if (cluster.GetType() !=
                                clusterMemberConsumption.ParentBaseZoneClusterConsumption
                                    .GetType())
                                destinationHashCode = cluster.GetHashCode();
                        });
                    
                }
                return destinationHashCode;
            },
            distanceTracker: new ZoneInfoDistanceTracker(new Func<Func<IZoneInfo, bool>[]>(() =>
            {
                if (clusterMemberConsumption.ParentBaseZoneClusterConsumption is ResidentialZoneClusterConsumption)
                {
                    return new Func<IZoneInfo, bool>[]
                    {
                        x => x.IsGrowthZoneClusterOfType<CommercialZoneClusterConsumption>(),
                        x => x.IsGrowthZoneClusterOfType<IndustrialZoneClusterConsumption>()
                    };
                }
                if (clusterMemberConsumption.ParentBaseZoneClusterConsumption is CommercialZoneClusterConsumption)
                {
                    return new Func<IZoneInfo, bool>[]
                    {
                        x => x.IsGrowthZoneClusterOfType<ResidentialZoneClusterConsumption>(),
                        x => x.IsGrowthZoneClusterOfType<IndustrialZoneClusterConsumption>()
                    };
                }
                if (clusterMemberConsumption.ParentBaseZoneClusterConsumption is IndustrialZoneClusterConsumption)
                {
                    return new Func<IZoneInfo, bool>[]
                    {
                        x => x.IsGrowthZoneClusterOfType<ResidentialZoneClusterConsumption>()
                    };
                }

                throw new InvalidOperationException();
            })()
                )
            )
        {
            OriginBaseZoneClusterConsumption = clusterMemberConsumption.ParentBaseZoneClusterConsumption;
        }

        public BaseZoneClusterConsumption OriginBaseZoneClusterConsumption { get; }
    }
}