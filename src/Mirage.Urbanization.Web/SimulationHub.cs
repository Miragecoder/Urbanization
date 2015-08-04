using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Mirage.Urbanization.Simulation;

namespace Mirage.Urbanization.Web
{
    public class SimulationHub : Hub
    {
        public static readonly ISimulationSession Instance = new SimulationSession(
            new SimulationOptions(new Func<TerraformingOptions>(() =>
            {
                var t = new TerraformingOptions();
                t.HorizontalRiver = t.VerticalRiver = true;
                t.SetWoodlands(80);
                t.SetZoneWidthAndHeight(70);
                return t;
            })(), new ProcessOptions(() => false, () => false)));

        public void GetCurrentState()
        {
            var zoneInfos = Instance.Area.EnumerateZoneInfos()
                .Select(zoneInfo => new
                {
                    key = $"{zoneInfo.Point.X}_{zoneInfo.Point.Y}",
                    point = new
                    {
                        x = zoneInfo.Point.X,
                        y = zoneInfo.Point.Y
                    },
                    color = zoneInfo.ZoneConsumptionState.GetZoneConsumption().ColorName,
                    zoneConsumption = zoneInfo.ZoneConsumptionState.GetZoneConsumption().Name,
                    landValue = zoneInfo.GetLastLandValueResult().WithResultIfHasMatch(r => r.ValueInUnits),
                    lastTravelDistance = zoneInfo.GetLastAverageTravelDistance(),
                    crime = zoneInfo.GetLastQueryCrimeResult().WithResultIfHasMatch(r => r.ValueInUnits),
                    fireHazard = zoneInfo.GetLastQueryFireHazardResult().WithResultIfHasMatch(r => r.ValueInUnits),
                    pollution = zoneInfo.GetLastQueryPollutionResult().WithResultIfHasMatch(r => r.ValueInUnits)
                }).ToArray();

            Clients.Caller.submitZoneInfos(zoneInfos);
        }
    }
}
