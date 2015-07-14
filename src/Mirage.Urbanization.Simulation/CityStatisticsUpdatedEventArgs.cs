using System;
using Mirage.Urbanization.Simulation.Persistence;

namespace Mirage.Urbanization.Simulation
{
    public class CityStatisticsUpdatedEventArgs : EventArgsWithData<PersistedCityStatisticsWithFinancialData>
    {
        public CityStatisticsUpdatedEventArgs(PersistedCityStatisticsWithFinancialData data) : base(data) { }
    }
}