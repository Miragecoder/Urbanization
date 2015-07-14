using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirage.Urbanization.Persistence;

namespace Mirage.Urbanization.Simulation.Persistence
{
    public class PersistedSimulation
    {
        public PersistedCityBudgetConfiguration PersistedCityBudgetConfiguration { get; set; }
        public PersistedArea PersistedArea { get; set; }
        public PersistedCityStatisticsWithFinancialData[] PersistedCityStatistics { get; set; }
    }
}
