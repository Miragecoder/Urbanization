using System;
using System.Collections.Generic;
using System.Linq;
using Mirage.Urbanization.Statistics;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneConsumption.Base.Behaviours;

namespace Mirage.Urbanization.Networks
{
    internal class PowerNetworkInfo : BaseZoneNetworkInfo
    {
        private readonly Action<string> _onBrownoutsMessageFunc;

        private PowerNetworkInfo(IEnumerable<IZoneInfo> memberZoneInfos, Action<string> onBrownoutsMessageFunc)
            : base(memberZoneInfos)
        {
            _onBrownoutsMessageFunc = onBrownoutsMessageFunc ?? throw new ArgumentNullException(nameof(onBrownoutsMessageFunc));
        }

        public static IEnumerable<PowerNetworkInfo> GenerateFrom(
            IReadOnlyDictionary<ZonePoint, ZoneInfo> zoneInfos,
            Action<string> onBrownoutsMessageFunc 
        )
        {
            var members = CollectZoneInfoNetworkSets(
                zoneInfos: zoneInfos,
                isNetworkMember: zoneInfo => zoneInfo
                    .ConsumptionState
                    .GetIsPowerGridMember()
                );

            return members
                .Select(network => new PowerNetworkInfo(network, onBrownoutsMessageFunc));
        }

        private IEnumerable<TElectricityBehaviour> GetElectricityBehaviours<TElectricityBehaviour>()
            where TElectricityBehaviour : IElectricityBehaviour
        {
            var seenZoneInfos = new HashSet<IZoneInfo>();
            return MemberZoneInfos
                .SelectMany(zoneInfo => zoneInfo
                    .GetNorthEastSouthWest()
                    .Where(queryResult => queryResult.HasMatch))
                    .Where(queryResult => seenZoneInfos.Add(queryResult.MatchingObject))
                    .Where(queryResult => queryResult
                        .MatchingObject
                        .ConsumptionState
                        .GetZoneConsumption() is ZoneClusterMemberConsumption
                    )
                    .Select(queryResult => queryResult
                        .MatchingObject
                        .ConsumptionState
                        .GetZoneConsumption()
                    )
                    .OfType<ZoneClusterMemberConsumption>()
                    .Where(x => x.ParentBaseZoneClusterConsumption.ElectricityBehaviour is TElectricityBehaviour)
                    .Select(x => x.ParentBaseZoneClusterConsumption.ElectricityBehaviour)
                    .OfType<TElectricityBehaviour>()
                    .Distinct();
        }

        public bool HasSuppliers => GetElectricitySuppliers().Any();

        public IEnumerable<IElectricitySupplier> GetElectricitySuppliers()
        {
            return GetElectricityBehaviours<IElectricitySupplier>();
        }

        public IEnumerable<IElectricityConsumer> GetElectricityConsumers()
        {
            return GetElectricityBehaviours<IElectricityConsumer>();
        }

        public IPowerGridNetworkStatistics PerformSurge()
        {
            var consumers = GetElectricityConsumers().ToList();
            var suppliers = GetElectricitySuppliers().ToList();

            var availableContributionUnits = suppliers.Sum(x => x.ContributionInUnits);

            foreach (var powerConsumer in consumers)
            {
                availableContributionUnits -= powerConsumer.ConsumptionInUnits;
                powerConsumer.TogglePowered((availableContributionUnits > 0));
                powerConsumer.ToggleConnected(suppliers.Any());

                if (availableContributionUnits <= 0 && suppliers.Any())
                    _onBrownoutsMessageFunc("Brownouts. Build another power plant.");
            }
            return new PowerGridNetworkStatistics(
                amountOfSuppliers: suppliers.Count,
                amountOfConsumers: consumers.Count(x => x.IsConnected),
                supplyInUnits: suppliers.Sum(x => x.ContributionInUnits),
                consumptionInUnits: consumers.Where(x => x.IsConnected).Sum(x => x.ConsumptionInUnits)
            );
        }
    }
}