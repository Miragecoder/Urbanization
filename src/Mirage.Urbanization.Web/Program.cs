using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Mirage.Urbanization.Simulation;

namespace Mirage.Urbanization.Web
{
    static class Program
    {
        static void Main(string[] args)
        {
            using (Microsoft.Owin.Hosting.WebApp.Start<Startup>("http://localhost:9000"))
            {
                SimulationHub.Instance.StartSimulation();

                Logger.Instance.OnLogMessage += (s, e) => Console.WriteLine(e.CreatedOn + " " + e.LogMessage);

                Console.WriteLine("Press CTRL + C to quit...");
                bool sentry = true;
                Console.CancelKeyPress += (s, e) =>
                {
                    sentry = false;
                    SimulationHub.Instance.Dispose();
                };

                while (sentry)
                {
                    System.Threading.Thread.Sleep(500);
                }
            }
        }
    }
}
