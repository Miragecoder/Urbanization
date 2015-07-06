using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization.ZoneConsumption
{
    public class ResidentialZoneClusterConsumption : BaseGrowthZoneClusterConsumption
    {
        private readonly Stack<ZoneClusterMemberConsumption> _houseMembers = new Stack<ZoneClusterMemberConsumption>();

        public ResidentialZoneClusterConsumption(Func<ZoneInfoFinder> createZoneInfoFinderFunc)
            : base(createZoneInfoFinderFunc, Color.Green)
        {

        }

        public override char KeyChar { get { return 'g'; } }

        protected override decimal PopulationPollutionFactor
        {
            get { return 0.5M; }
        }

        protected override decimal PopulationCrimeFactor
        {
            get { return 0.5M; }
        }

        protected override decimal PopulationFireHazardFactor
        {
            get { return 0.5M; }
        }

        public override string Name { get { return "Residential zone"; } }

        public bool RenderAsHouse(ZoneClusterMemberConsumption zoneClusterMember)
        {
            if (!_zoneClusterMembers.Contains(zoneClusterMember))
                throw new InvalidOperationException();
            int localDensity = PopulationDensity;
            if (localDensity > _zoneClusterMembers.Count(x => !x.IsCentralClusterMember))
                return false;

            while (_houseMembers.Count > localDensity)
            {
                _houseMembers.Pop();
            };

            while (_houseMembers.Count < localDensity)
            {
                _houseMembers
                    .Push(_zoneClusterMembers
                        .Where(x => !x.IsCentralClusterMember)
                        .OrderBy(x => Guid.NewGuid()
                    )
                    .Except(_houseMembers)
                    .First()
                );
            };

            return _houseMembers.Contains(zoneClusterMember);
        }
    }
}