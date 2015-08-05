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
        public static class CurrentSimulation
        {
            private static ISimulationSession _instance;

            public static void Set(ISimulationSession simulationSession)
            {
                if (_instance == null)
                    _instance = simulationSession;
                else
                    throw new InvalidOperationException();
            }

            public static void With(Action<ISimulationSession> action)
            {
                var instance = _instance;
                if (instance != null)
                    action(_instance);
                else
                    throw new InvalidOperationException();
            }
        }

        public void GetCurrentState()
        {
            CurrentSimulation.With(session =>
            {

                var zoneInfos = session.Area.EnumerateZoneInfos()
                    .Select(zoneInfo => new
                    {
                        key = $"{zoneInfo.Point.X}_{zoneInfo.Point.Y}",
                        bitmapLayerOne = TilesetProvider
                            .GetTilePathFor(zoneInfo.ZoneConsumptionState.GetZoneConsumption(), x => x.LayerOne),
                        bitmapLayerTwo = TilesetProvider
                            .GetTilePathFor(zoneInfo.ZoneConsumptionState.GetZoneConsumption(), x => x.LayerTwo),
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

            });
        }
    }
}
