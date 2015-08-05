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
            var simulationSession = new SimulationSession(
                new SimulationOptions(new Func<TerraformingOptions>(() =>
                {
                    var t = new TerraformingOptions();
                    t.HorizontalRiver = t.VerticalRiver = false;
                    t.SetWoodlands(80);
                    t.SetZoneWidthAndHeight(120);
                    return t;
                })(), new ProcessOptions(() => false, () => false)));
            using (var server = new GameServer(simulationSession, "http://localhost:9000/"))
            {
                server.StartServer();
                simulationSession.StartSimulation();

                Logger.Instance.OnLogMessage += (s, e) => Console.WriteLine(e.CreatedOn + " " + e.LogMessage);

                Console.WriteLine("Press CTRL + C to quit...");
                var sentry = true;
                Console.CancelKeyPress += (s, e) =>
                {
                    sentry = false;
                };

                while (sentry)
                {
                    System.Threading.Thread.Sleep(500);
                }
            }
        }
    }
}