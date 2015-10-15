using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mirage.Urbanization.Simulation.Persistence;

namespace Mirage.Urbanization.WinForms
{
    public class FormWithCityStatisticsEvent : Form
    {
        [Obsolete("For designer only!", true)]
        public FormWithCityStatisticsEvent()
        {
        }

        protected FormWithCityStatisticsEvent(SimulationRenderHelper helper)
        {
            this.Shown += (x, y) => UpdatePrivate(helper.SimulationSession.GetAllCityStatistics(), helper.SimulationSession.GetRecentStatistics().MatchingObject);
            helper.SimulationSession.CityStatisticsUpdated += (x, y) => UpdatePrivate(helper.SimulationSession.GetAllCityStatistics(), y.EventData);
        }

        private void UpdatePrivate(IReadOnlyCollection<PersistedCityStatisticsWithFinancialData> statistics, 
            PersistedCityStatisticsWithFinancialData current)
        {
            if (!IsHandleCreated) return;

            Update(statistics, current);
        }

        public virtual void Update(IReadOnlyCollection<PersistedCityStatisticsWithFinancialData> statistics,
            PersistedCityStatisticsWithFinancialData current)
        {
            throw new NotImplementedException();
        }
    }
}