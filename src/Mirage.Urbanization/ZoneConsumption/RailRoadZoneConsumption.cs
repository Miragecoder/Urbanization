using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization.ZoneConsumption
{
    public class RailRoadZoneConsumption : BaseInfrastructureNetworkZoneConsumption
    {
        public RailRoadZoneConsumption(ZoneInfoFinder neighborNavigator) : base(neighborNavigator) { }

        public override string Name => "Railroad";

        public override char KeyChar => 't';

        public override bool CanBeOverriddenByZoneClusters => false;
        public override int Cost => 50;

        public override Color Color => System.Drawing.Color.SaddleBrown;
    }
}