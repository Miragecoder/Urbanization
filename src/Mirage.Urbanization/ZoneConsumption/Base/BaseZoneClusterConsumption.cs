using System;
using System.Collections.Generic;
using System.Linq;
using Mirage.Urbanization.ZoneConsumption.Base.Behaviours;

namespace Mirage.Urbanization.ZoneConsumption.Base
{
    public abstract class BaseZoneClusterConsumption : IAreaZoneClusterConsumption
    {
        public abstract char KeyChar { get; }

        public abstract int Cost { get; }

        public abstract IPollutionBehaviour PollutionBehaviour { get; }
        public abstract ICrimeBehaviour CrimeBehaviour { get; }
        public abstract IFireHazardBehaviour FireHazardBehaviour { get; }
        public IElectricityBehaviour ElectricityBehaviour { get; }

        protected BaseZoneClusterConsumption(IElectricityBehaviour electricityBehaviour)
        {
            ElectricityBehaviour = electricityBehaviour;
        }

        public bool HasPower => ElectricityBehaviour.IsPowered;

        public abstract string Name { get; }
        public abstract IReadOnlyCollection<ZoneClusterMemberConsumption> ZoneClusterMembers { get; }
        public bool ClusterMembersAreUnlocked { get; private set; }

        protected DateTime DateTimeCreated { get; } = DateTime.Now;

        private readonly object _clusterMemberLocker = new object();

        public void WithUnlockedClusterMembers(Action action)
        {
            lock (_clusterMemberLocker)
            {
                if (ClusterMembersAreUnlocked) throw new InvalidOperationException();
                ClusterMembersAreUnlocked = true;
                try
                {
                    action();
                }
                catch
                {
                    ClusterMembersAreUnlocked = false;
                    throw;
                }
            }
        }
    }
}