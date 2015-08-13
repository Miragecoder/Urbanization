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
        public void ConsumeZone(string name, string x, string y)
        {
            var session = GameServer.Instance.SimulationSession;

            var target = session
                .Area
                .EnumerateZoneInfos()
                .Single(zone => zone.Point == new ZonePoint() { X = int.Parse(x), Y = int.Parse(y) });

            var factory = session.Area.GetSupportedZoneConsumptionFactories()
                    .Single(fact => fact.Invoke().Name == name);

            session.ConsumeZoneAt(target, factory());
        }

        public void RequestMenuStructure()
        {
            Task.Run(() =>
            {
                Clients.Caller.submitMenuStructure(
                    GameServer
                        .Instance
                        .SimulationSession
                        .Area
                        .GetSupportedZoneConsumptionFactories()
                        .Select(x => x.Invoke())
                        .Select(x =>
                            new
                            {
                                name = x.Name,
                                cost = x.Cost,
                                keyChar = x.KeyChar
                            })
                    );

                GlobalHost
                    .ConnectionManager
                    .GetHubContext<SimulationHub>()
                    .Clients
                    .All
                    .submitZoneInfos(GameServer.Instance.SimulationSession.Area.EnumerateZoneInfos()
                            .Select(x => x.ToClientZoneInfo()).ToList()
                        );

            });
        }
    }
}
