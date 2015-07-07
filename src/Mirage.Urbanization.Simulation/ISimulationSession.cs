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

        QueryResult<PersistedCityStatistics> GetRecentStatistics();

        IReadOnlyCollection<PersistedCityStatistics> GetAllCityStatistics(); 

        event EventHandler<EventArgsWithData<IYearAndMonth>> OnYearAndOrMonthChanged;
        PersistedSimulation GeneratePersistedArea();

        event EventHandler<SimulationSessionHotMessageEventArgs> OnAreaHotMessage;
        event EventHandler<SimulationSessionMessageEventArgs> OnAreaMessage;
        event EventHandler<CityBudgetValueChangedEventArgs> OnCityBudgetValueChanged;
        event EventHandler<CityStatisticsUpdatedEventArgs> CityStatisticsUpdated;
    }

    public class CityStatisticsUpdatedEventArgs : EventArgs
    {
        private readonly ICityStatistics _cityStatistics;

        public CityStatisticsUpdatedEventArgs(ICityStatistics cityStatistics)
        {
            _cityStatistics = cityStatistics;
        }

        public ICityStatistics CityStatistics
        {
            get { return _cityStatistics; }
        }
    }
}