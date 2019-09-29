using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Mirage.Urbanization.ZoneConsumption.Base.Behaviours;

namespace Mirage.Urbanization.ZoneConsumption.Base
{
    public abstract class BaseImplementedZoneClusterConsumption : BaseZoneClusterConsumption
    {
        protected readonly IReadOnlyCollection<ZoneClusterMemberConsumption> _zoneClusterMembers;
        public override IReadOnlyCollection<ZoneClusterMemberConsumption> ZoneClusterMembers => _zoneClusterMembers;

        protected BaseImplementedZoneClusterConsumption(
            Func<ZoneInfoFinder> createZoneInfoFinderFunc,
            IElectricityBehaviour electricityBehaviour,
            Color color,
            int widthInZones,
            int heightInZones)
            : base(electricityBehaviour)
        {
            _zoneClusterMembers = ZoneClusterMemberConsumption
                .Generate(this, createZoneInfoFinderFunc, widthInZones, heightInZones, color)
                .ToList();
        }
    }
}