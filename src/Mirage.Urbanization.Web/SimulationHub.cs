using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Mirage.Urbanization.Simulation;

namespace Mirage.Urbanization.Web
{
    public class SimulationHub : Hub
    {
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
