using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Mirage.Urbanization.Simulation;
using Mirage.Urbanization.Simulation.Datameters;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization.Web
{
    public class SimulationHub : Hub
    {
        public void ConsumeZone(string name, string x, string y)
        {
            var session = GameServer.Instance.SimulationSession;

            var target = session
                .Area
                .EnumerateZoneInfos()
                .Single(zone => zone.Point == new ZonePoint() { X = int.Parse(x), Y = int.Parse(y) });

            var factory = session
                .Area
                .GetSupportedZoneConsumptionFactories()
                .Single(fact => fact.Invoke().Name == name);

            session.ConsumeZoneAt(target, factory());
        }

        public void RequestZonesFor(int dataMeterWebId)
        {
            Task.Run(async () =>
            {

                var initialState = GameServer
                    .Instance
                    .SimulationSession
                    .Area
                    .EnumerateZoneInfos()
                    .Select(x => ClientDataMeterZoneInfo.Create(x, DataMeterInstances.DataMeters.Single(y => y.WebId == dataMeterWebId)))
                    .Where(x => !string.IsNullOrWhiteSpace(x.colour));

                foreach (var batchState in initialState.GetBatched(100))
                {
                    Clients.Caller.submitDataMeterInfos(batchState);

                    await Task.Delay(200);
                }
            });
        }

        public void JoinDataMeterGroup(int dataMeterId)
        {
            foreach (var x in DataMeterInstances.DataMeters.Select(x => x.WebId))
            {
                Groups.Remove(Context.ConnectionId, GetDataMeterGroupName(x));
            }

            Groups.Add(Context.ConnectionId, GetDataMeterGroupName(dataMeterId));
        }

        public static string GetDataMeterGroupName(int dataMeterId) => "dataMeter" + dataMeterId;

        public void RequestMenuStructure()
        {
            Task.Run(async () =>
            {
                Clients.Caller.submitMenuStructure(
                    new
                    {
                        dataMeterInstances = DataMeterInstances
                            .DataMeters
                            .Select(x => new { name = x.Name, webId = x.WebId }),

                        buttonDefinitions = GameServer
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
                                keyChar = x.KeyChar,
                                isClickAndDrag = x.BuildStyle == BuildStyle.ClickAndDrag,
                                isClearButton = x.GetType() == typeof(EmptyZoneConsumption)
                            })
                    }
                    );
                await Task.Delay(1000);

                Clients.Caller
                    .submitZoneInfos(GameServer
                        .Instance
                        .SimulationSession
                        .Area
                        .EnumerateZoneInfos()
                        .Reverse()
                        .Take(5)
                        .Select(ClientZoneInfo.Create)
                    );

                await Task.Delay(1000);

                var initialState = GameServer
                    .Instance
                    .SimulationSession
                    .Area
                    .EnumerateZoneInfos()
                    .Where(x => x.ZoneConsumptionState.GetZoneConsumption().GetType() != typeof(EmptyZoneConsumption))
                    .ToList();

                foreach (var batchState in initialState.GetBatched(50))
                {
                    Clients.Caller
                        .submitZoneInfos(batchState.Select(ClientZoneInfo.Create));

                    await Task.Delay(200);
                }
            });
        }
    }
}
