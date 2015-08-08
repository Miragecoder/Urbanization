using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Mirage.Urbanization.Simulation;

namespace Mirage.Urbanization.Web
{
    public struct ClientZoneInfo
    {
        public string key { get; set; }
        public int bitmapLayerOne { get; set; }
        public int bitmapLayerTwo { get; set; }
        public ClientZonePoint point { get; set; }
        public string color { get; set; }
        public string GetIdentityString() => $"{key}_{bitmapLayerOne}_{bitmapLayerTwo}_{point.GetIdentityString()}_{color}";
    }

    public struct ClientZonePoint
    {
        public int x { get; set; }
        public int y { get; set; }
        public string GetIdentityString() => $"{x}_{y}";
    }

    public class SimulationHub : Hub
    {
        private static readonly IDictionary<dynamic, IDictionary<ClientZonePoint, ClientZoneInfo>> ResponseCache = new Dictionary<dynamic, IDictionary<ClientZonePoint, ClientZoneInfo>>(); 
        public void RequestZoneInfoSubmission()
        {
            var caller = Clients.Caller;
            Task.Run(() =>
            {
                CurrentSimulation.With(simulationSession =>
                {
                    var zoneInfos = simulationSession.Area.EnumerateZoneInfos()
                        .Select(zoneInfo => new ClientZoneInfo
                        {
                            key = $"{zoneInfo.Point.X}_{zoneInfo.Point.Y}",
                            bitmapLayerOne = TilesetProvider
                                .GetTilePathFor(zoneInfo.ZoneConsumptionState.GetZoneConsumption(), x => x.LayerOne),
                            bitmapLayerTwo = TilesetProvider
                                .GetTilePathFor(zoneInfo.ZoneConsumptionState.GetZoneConsumption(), x => x.LayerTwo),
                            point = new ClientZonePoint
                            {
                                x = zoneInfo.Point.X,
                                y = zoneInfo.Point.Y
                            },
                            color = zoneInfo.ZoneConsumptionState.GetZoneConsumption().ColorName
                        }).ToList();

                    if (!ResponseCache.ContainsKey(caller))
                    {
                        ResponseCache.Add(caller, zoneInfos.ToDictionary(x => x.point, x => x));
                        caller.submitZoneInfos(zoneInfos);
                    }
                    else
                    {
                        caller.submitZoneInfos(zoneInfos.Where(x => ResponseCache[caller][x.point].GetIdentityString() != x.GetIdentityString()));
                        ResponseCache[caller] = zoneInfos.ToDictionary(x => x.point, x => x);
                    }



                });
            });
        }

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
    }
}
