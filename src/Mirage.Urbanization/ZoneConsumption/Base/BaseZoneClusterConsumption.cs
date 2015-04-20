using System;
using System.Collections.Generic;
using System.Linq;
using Mirage.Urbanization.ZoneConsumption.Base.Behaviours;

namespace Mirage.Urbanization.ZoneConsumption.Base
{
    public abstract class BaseZoneClusterConsumption : IAreaZoneClusterConsumption
    {
        private readonly IElectricityBehaviour _electricityBehaviour;

        public abstract IPollutionBehaviour PollutionBehaviour { get; }
        public abstract ICrimeBehaviour CrimeBehaviour { get; }
        public IElectricityBehaviour ElectricityBehaviour { get { return _electricityBehaviour; } }

        protected BaseZoneClusterConsumption(IElectricityBehaviour electricityBehaviour)
        {
            _electricityBehaviour = electricityBehaviour;
        }

        public bool HasPower
        {
            get { return _electricityBehaviour.IsPowered; }
        }

        public abstract string Name { get; }
        public abstract IReadOnlyCollection<ZoneClusterMemberConsumption> ZoneClusterMembers { get; }
        public bool ClusterMembersAreUnlocked { get { return _clusterMembersAreUnlocked; } }

        private readonly DateTime _dateTimeCreated = DateTime.Now;

        protected DateTime DateTimeCreated { get { return _dateTimeCreated; } }

        private bool _clusterMembersAreUnlocked;
        private readonly object _clusterMemberLocker = new object();

        public void WithUnlockedClusterMembers(Action action)
        {
            lock (_clusterMemberLocker)
            {
                if (_clusterMembersAreUnlocked) throw new InvalidOperationException();
                _clusterMembersAreUnlocked = true;
                try
                {
                    action();
                }
                catch
                {
                    _clusterMembersAreUnlocked = false;
                    throw;
                }
            }
        }
    }
}