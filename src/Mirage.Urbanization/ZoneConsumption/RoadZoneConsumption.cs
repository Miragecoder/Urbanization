using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneConsumption.Base.Behaviours;

namespace Mirage.Urbanization.ZoneConsumption
{
    public interface IZoneConsumptionWithTraffic
    {
        TrafficDensity GetTrafficDensity();
        int GetTrafficDensityAsInt();
    }

    public class RoadZoneConsumption : BaseInfrastructureNetworkZoneConsumption, IZoneConsumptionWithTraffic
    {
        public RoadZoneConsumption(ZoneInfoFinder neighborNavigator)
            : base(neighborNavigator)
        {
            _pollutionBehaviour = new DynamicPollutionBehaviour(() =>
            {
                switch (GetTrafficDensity())
                {
                    case TrafficDensity.None:
                        return 0;
                    case TrafficDensity.Low:
                        return 3;
                    case TrafficDensity.High:
                        return 8;
                    default:
                        throw new InvalidOperationException();
                }
            });
        }

        public override string Name
        {
            get { return "Road"; }
        }

        public override bool CanBeOverriddenByZoneClusters
        {
            get { return false; }
        }

        public override Color Color
        {
            get { return Color.Gray; }
        }

        private readonly IPollutionBehaviour _pollutionBehaviour;

        public IPollutionBehaviour PollutionBehaviour
        {
            get { return _pollutionBehaviour; }
        }

        public TrafficDensity GetTrafficDensity()
        {
            const int noneThreshold = 200;
            const int highThreshold = 400;

            if (_trafficDensity <= noneThreshold) return TrafficDensity.None;
            else if (_trafficDensity > noneThreshold && _trafficDensity <= highThreshold)
                return TrafficDensity.Low;
            else
                return TrafficDensity.High;
        }

        public int GetTrafficDensityAsInt()
        {
            return _trafficDensity;
        }

        private int _trafficDensity = default(int);

        public void SetTrafficDensity(int density)
        {
            _trafficDensity = density;
        }
    }
}
