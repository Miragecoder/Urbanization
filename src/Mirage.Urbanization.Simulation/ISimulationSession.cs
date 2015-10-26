using System;
using System.Collections.Generic;
using Mirage.Urbanization.Persistence;
using Mirage.Urbanization.Simulation.Persistence;
using Mirage.Urbanization.Statistics;
using Mirage.Urbanization.ZoneConsumption.Base;
using Mirage.Urbanization.ZoneStatisticsQuerying;

namespace Mirage.Urbanization.Simulation
{
    public interface ISimulationSession : IDisposable
    {
        IReadOnlyArea Area { get; }
        IAreaConsumptionResult ConsumeZoneAt(IReadOnlyZoneInfo readOnlyZoneInfo, IAreaConsumption consumption);
        void StartSimulation();

        ICityBudgetConfiguration CityBudgetConfiguration { get; }

        QueryResult<PersistedCityStatisticsWithFinancialData> GetRecentStatistics();

        IEnumerable<PersistedCityStatisticsWithFinancialData> GetAllCityStatisticsForCurrentYear();

        IEnumerable<PersistedCityStatisticsWithFinancialData> GetAllCityStatistics(); 

        event EventHandler<EventArgsWithData<IYearAndMonth>> OnYearAndOrMonthChanged;
        PersistedSimulation GeneratePersistedArea();

        event EventHandler<SimulationSessionHotMessageEventArgs> OnAreaHotMessage;
        event EventHandler<SimulationSessionMessageEventArgs> OnAreaMessage;
        event EventHandler<CityBudgetValueChangedEventArgs> OnCityBudgetValueChanged;
        event EventHandler<CityStatisticsUpdatedEventArgs> CityStatisticsUpdated;
    }
}