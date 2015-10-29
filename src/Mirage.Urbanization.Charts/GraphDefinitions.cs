using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Mirage.Urbanization.Simulation.Datameters;
using Mirage.Urbanization.Simulation.Persistence;

namespace Mirage.Urbanization.Charts
{
    public static class GraphDefinitions
    {
        public static IReadOnlyCollection<GraphDefinition> Instances = GenerateGraphDefinitions().ToList(); 

        private static IEnumerable<GraphDefinition> GenerateGraphDefinitions()
        {
            yield return new GraphDefinition("Amount of zones",
                new GraphSeries(
                    x => x.PersistedCityStatistics.ResidentialZonePopulationStatistics.Count,
                    "Residential",
                    Color.Green
                    ), new GraphSeries(
                        x => x.PersistedCityStatistics.CommercialZonePopulationStatistics.Count,
                        "Commercial",
                        Color.Blue
                        ), new GraphSeries(
                            x => x.PersistedCityStatistics.IndustrialZonePopulationStatistics.Count,
                            "Industrial",
                            Color.Goldenrod
                            ), new GraphSeries(
                                x => x.PersistedCityStatistics.GlobalZonePopulationStatistics.Count,
                                "Global",
                                Color.DarkRed
                                )
                );

            yield return new GraphDefinition("Amount of funds",
                new GraphSeries(x => x.CurrentAmountOfFunds, "Current amount of funds", Color.Red),
                new GraphSeries(x => x.CurrentProjectedAmountOfFunds, "Projected income", Color.Gray));

            yield return new GraphDefinition("Tax income",
                new GraphSeries(x => x.ResidentialTaxIncome, "Residential zones", Color.Green),
                new GraphSeries(x => x.CommercialTaxIncome, "Commercial zones", Color.Blue),
                new GraphSeries(x => x.IndustrialTaxIncome, "Industrial zones", Color.Goldenrod));

            yield return new GraphDefinition("Public sector expenses",
                new GraphSeries(x => x.PoliceServiceExpenses, "Police force", Color.Blue),
                new GraphSeries(x => x.FireServiceExpenses, "Fire fighters", Color.Red),
                new GraphSeries(x => x.RoadInfrastructureExpenses, "Infrastructure (Road)", Color.Gray),
                new GraphSeries(x => x.RailroadInfrastructureExpenses, "Infrastructure (Railroad)", Color.Yellow));

            yield return new GraphDefinition("Population",
                new GraphSeries(
                    x => x.PersistedCityStatistics.ResidentialZonePopulationStatistics.Sum,
                    "Residential",
                    Color.Green
                    ), new GraphSeries(
                        x => x.PersistedCityStatistics.CommercialZonePopulationStatistics.Sum,
                        "Commercial",
                        Color.Blue
                        ), new GraphSeries(
                            x => x.PersistedCityStatistics.IndustrialZonePopulationStatistics.Sum,
                            "Industrial",
                            Color.Goldenrod
                            ), new GraphSeries(
                                x => x.PersistedCityStatistics.GlobalZonePopulationStatistics.Sum,
                                "Global",
                                Color.DarkRed
                                )
                );

            foreach (var x in
                GetNumbarSummaryGraphs(x => x.PersistedCityStatistics.CrimeNumbers, DataMeterInstances.CrimeDataMeter, "Crime", Color.Red, Color.DarkRed)
                    .Concat(GetNumbarSummaryGraphs(x => x.PersistedCityStatistics.FireHazardNumbers, DataMeterInstances.FireHazardDataMeter, "Fire hazard", Color.Red, Color.DarkRed))
                    .Concat(GetNumbarSummaryGraphs(x => x.PersistedCityStatistics.PollutionNumbers, DataMeterInstances.PollutionDataMeter, "Pollution", Color.Green, Color.DarkOliveGreen))
                    .Concat(GetNumbarSummaryGraphs(x => x.PersistedCityStatistics.TrafficNumbers, DataMeterInstances.TrafficDataMeter, "Traffic", Color.Blue, Color.DarkBlue))
                    .Concat(GetNumbarSummaryGraphs(x => x.PersistedCityStatistics.LandValueNumbers, DataMeterInstances.LandValueDataMeter, "Land value", Color.Yellow, Color.GreenYellow))
                    .Concat(GetNumbarSummaryGraphs(x => x.PersistedCityStatistics.AverageTravelDistanceStatistics, DataMeterInstances.TravelDistanceDataMeter, "Travel distances", Color.Blue, Color.DarkBlue))
                )
                yield return x;

            yield return new GraphDefinition("Power grid",
                new GraphSeries(
                    x => x.PersistedCityStatistics.PowerSupplyInUnits,
                    "Power supply (Total)",
                    Color.Green
                    ), new GraphSeries(
                        x => x.PersistedCityStatistics.PowerConsumptionInUnits,
                        "Consumption",
                        Color.Yellow
                        ), new GraphSeries(
                            x => x.PersistedCityStatistics.PowerSupplyInUnits - x.PersistedCityStatistics.PowerConsumptionInUnits,
                            "Power supply (Remaining)",
                            Color.Red
                            )
                );

            yield return new GraphDefinition("Infastructure size",
                new GraphSeries(
                    x => x.PersistedCityStatistics.NumberOfRoadZones,
                    "Total amount of road zones",
                    Color.Blue
                    ), new GraphSeries(
                        x => x.PersistedCityStatistics.NumberOfRailRoadZones,
                        "Total amount of railroad zones",
                        Color.Goldenrod
                        )
                );
        }

        private static IEnumerable<GraphDefinition> GetNumbarSummaryGraphs(
            Func<PersistedCityStatisticsWithFinancialData, PersistedNumberSummary> getNumberSummary,
            DataMeter dataMeter,
            string title,
            Color primaryColor,
            Color secondaryColor)
        {
            Func<PersistedCityStatisticsWithFinancialData, PersistedNumberSummary> getNumberSummarySafeFunc =
                x => getNumberSummary(x) ?? PersistedNumberSummary.EmptyInstance;

            yield return new GraphDefinition(title,
                dataMeter,
                new GraphSeries(
                    x => getNumberSummarySafeFunc(x).Average,
                    "Average",
                    secondaryColor
                    ),
                new GraphSeries(
                    x => getNumberSummarySafeFunc(x).Max,
                    "Highest",
                    primaryColor
                    )
                );
        }
    }
}