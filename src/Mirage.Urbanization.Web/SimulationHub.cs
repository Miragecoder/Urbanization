using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Mirage.Urbanization.Charts;
using Mirage.Urbanization.Simulation;
using Mirage.Urbanization.Simulation.Datameters;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Web
{
    public class SimulationHub : Hub
    {
        public class ConsumeZoneInput
        {
            public string Name { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
        }

        public Task ConsumeZone(ConsumeZoneInput input)
        {
            var session = GameServer.Instance.SimulationSession;

            var target = session
                .Area
                .EnumerateZoneInfos()
                .Single(zone => zone.Point == new ZonePoint() { X = input.X, Y = input.Y });

            var factory = session
                .Area
                .GetSupportedZoneConsumptionFactories()
                .Single(fact => fact.Invoke().Name == input.Name);

            session.ConsumeZoneAt(target, factory());
            return Task.CompletedTask;
        }

        public async Task RequestZonesFor(int dataMeterWebId)
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
                await Clients.Caller.SendAsync("submitDataMeterInfos", batchState);
            }
        }

        public async Task JoinDataMeterGroup(int dataMeterId)
        {
            foreach (var x in DataMeterInstances.DataMeters.Select(x => x.WebId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetDataMeterGroupName(x));
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, GetDataMeterGroupName(dataMeterId));
        }

        public static string GetDataMeterGroupName(int dataMeterId) => "dataMeter" + dataMeterId;

        public void RaiseCityServiceFunding(string cityService)
            => RaiseBudgetComponent(CityServiceDefinition.CityServiceDefinitions, cityService);
        public void LowerCityServiceFunding(string cityService)
            => LowerBudgetComponent(CityServiceDefinition.CityServiceDefinitions, cityService);

        public void RaiseTax(string taxName) => RaiseBudgetComponent(TaxDefinition.TaxDefinitions, taxName);
        public void LowerTax(string taxName) => LowerBudgetComponent(TaxDefinition.TaxDefinitions, taxName);

        private void RaiseBudgetComponent(IEnumerable<BudgetComponentDefinition> budgetComponentDefinitions, string taxName)
            => MutateBudgetComponent(budgetComponentDefinitions, taxName, (currentRate, selectableRate) => currentRate < selectableRate, x => x.First());

        private void LowerBudgetComponent(IEnumerable<BudgetComponentDefinition> budgetComponentDefinitions, string taxName)
            => MutateBudgetComponent(budgetComponentDefinitions, taxName, (currentRate, selectableRate) => currentRate > selectableRate, x => x.Last());

        private void MutateBudgetComponent(
            IEnumerable<BudgetComponentDefinition> budgetComponentDefinitions,
            string taxName,
            Func<decimal, decimal, bool> comparison,
            Func<IEnumerable<decimal>, decimal> selector)
        {
            var budget = GameServer.Instance.SimulationSession.CityBudgetConfiguration;

            budgetComponentDefinitions
                .Single(x => x.Name == taxName)
                .Pipe(taxdefinition =>
                {
                    taxdefinition
                        .GetSelectableRatePercentages()
                        .Where(selectableRate => comparison(taxdefinition.CurrentRate(budget), selectableRate))
                        .Pipe(results =>
                        {
                            if (results.Any())
                            {
                                taxdefinition.SetCurrentRate(budget, selector(results));
                            }
                        });
                });
        }

        public async Task RequestMenuStructure()
        {
            await Clients.Caller.SendAsync("submitMenuStructure",
                new
                {
                    graphDefinitions = GraphDefinitions
                        .Instances
                        .Select(x => new { title = x.Title, webId = x.WebId }),

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
                            isClearButton = x.GetType() == typeof(EmptyZoneConsumption),
                            widthInCells = (x as IAreaZoneClusterConsumption)?.WidthInCells ?? 1,
                            heightInCells = (x as IAreaZoneClusterConsumption)?.HeightInCells ?? 1,
                            horizontalCellOffset = (x as IAreaZoneClusterConsumption)?.HorizontalCellOffset ?? 0,
                            verticalCellOffset = (x as IAreaZoneClusterConsumption)?.VerticalCellOffset ?? 0,
                        })
                });

            await Clients.Caller
                .SendAsync("submitZoneInfos", GameServer
                    .Instance
                    .SimulationSession
                    .Area
                    .EnumerateZoneInfos()
                    .Reverse()
                    .Take(5)
                    .Select(ClientZoneInfo.Create)
                );


            var initialState = GameServer
                .Instance
                .SimulationSession
                .Area
                .EnumerateZoneInfos()
                .Where(x => x.ZoneConsumptionState.GetZoneConsumption().GetType() != typeof(EmptyZoneConsumption))
                .ToList();

            foreach (var batchState in initialState.GetBatched(50))
            {
                await Clients.Caller
                    .SendAsync("submitZoneInfos", batchState.Select(ClientZoneInfo.Create));
            }
        }
    }
}
